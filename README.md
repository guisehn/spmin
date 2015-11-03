# SPMin
Automatic JavaScript and CSS minification and combination for SharePoint's Style Library.

## Usage
SPMin is a farm solution that automatizes the minification and combination of JavaScript and CSS files inside the Style Library. The [release page](https://github.com/ghsehn/SPMin/releases/latest) contains the installation instructions.

To start using it, create a file with the name ending with `spm.js` or `spm.css` in any folder inside the Style Library. As it will trigger the minification only for files with this name pattern, it is possible to apply this solution into your existing project gradually only for specific assets.

For the file `/Style Library/acme/js/app.spm.js`, for example, it will create a correspondent one `/Style Library/spmin/acme-js-app.js` with the minified code.

Operations such as editing, checking-out/checking-in and deleting are synchronized for the minified files.

### Assets combination
SPMin provides a way to concatenate multiple assets into one file, which can reduce the number of HTTP requests that your user's browser makes to render the web page. Web browsers are limited in the number of requests that they can make in parallel, so fewer requests can mean faster loading for your application. It works similar to [Ruby on Rails's asset pipeline](https://github.com/rails/sprockets).

You can tell SPMin to concatenate assets by adding a comment at the top of the `.spm.js` or `.spm.css` code declaring which files to include. For the following sample files:

##### /Style Library/acme/js/app.spm.js
```javascript
/*
 *= file1.js
 *= file2.js
 */
alert('Hello from app.spm.js');
```

##### /Style Library/acme/js/file1.js
```javascript
alert('Hello from file1.js');
```

##### /Style Library/acme/js/file2.js
```javascript
alert('Hello from file2.js');
```

It will automatically generate this when `app.spm.js` is saved:

##### /Style Library/spmin/acme-js-app.js
```javascript
alert('Hello from file1.js');alert('Hello from file2.js');alert('Hello from app.spm.js')
```

There are a few points that are worth attention:

- The comment that specifies the files to be included must be at the very top of the code.
- The path of the files included in the comment must be relative to the current file location. You can use `../file.js` or `subfolder/file.js` to include a file from another folder.
- You must re-save or check-in the file with the inclusion directives to re-generate the minified/combined file. Updating only the included files will not trigger it.

### Fingerprinting
SPMin helps to invalidate the cache of CSS and JavaScript files in your users' web browsers whenever you modify them by using fingerprinting, which is a technique that makes the name of a file dependent on its content. Every time you check-in a `.spm.js/css` file, SPMin will rename the minified correspondent file to include the MD5 hash of its contents on its name.

Once `/Style Library/acme/js/app.spm.js` is checked-in with the content `alert('test');`, for example, the minified file path will be `/Style Library/spmin/acme-js-app-0ef17d2cda19e1b18a4110b93f0bb9e8.js`. Whenever you change the code and check-in `app.spm.js` again, it will change the hash on the file name again in order to invalidate your users' web browsers cache.

You don't need to care about updating the JavaScript/CSS inclusion tags throughout your site to handle the filename changes because SPMin includes some controls to automatically do this and some other things for you. This will be explained in the [next section](#including-assets-in-your-page).

### Including assets in your page
SPMin includes controls to help with the inclusion of minified assets in the site pages.

To use the controls first you need to include the following directive at the top of your web page file:

```asp
<%@ Register TagPrefix="SPMin" Namespace="SPMin.Controls"
Assembly="SPMin, Version=1.0.0.0, Culture=neutral, PublicKeyToken=a86ef32346edfcab" %>
```

Then you can add the `JsRegistration` control into your page to include a JavaScript file or `CssRegistration` to include a CSS file.

```asp
<SPMin:JsRegistration FilePath="acme/js/app.spm.js" runat="server" />
```

These are the properties available for the controls:

| Property    | Description  |
| :---------- | :----------- |
| FilePath    | The path to the `.spm.css` or `.spm.js` file relative to the Style Library. |
| IncludeOnce | Indicates if the file should be included only once in the web page, no matter how many times it is included. By default it is `True`, and should be explicity set to `False` if you need to include the same script many times in the page. |
| AddToHead   | Indicates if the file should be rendered inside `<head>` instead of the place where the control is defined. By default, it is `True` for `CssRegistration` and `False` for `JsRegistration`. |

The generated HTML will change based on the [environment mode](#setting-the-environment-mode).

For the production environment, SPMin will print the inclusion tag pointing directly to the minified/combined file.

```html
<script src='/Style Library/spmin/acme-js-app-c1e71ca30e01f8b39b5ebc1dc7030578.js' type='text/javascript'></script>
```

For the development environment, it will print inclusion tags for the original files to keep debugging the same way as you would do normally.

```html
<script src='/Style Library/acme/js/file1.js' type='text/javascript'></script>
<script src='/Style Library/acme/js/file2.js' type='text/javascript'></script>
<script src='/Style Library/acme/js/app.spm.js' type='text/javascript'></script>
```

### Setting the environment mode

The environment mode can be set either by using SharePoint Designer or the SharePoint Management Shell.

#### Using SharePoint Designer

1. Open the root site of your site collection in SharePoint Designer.
2. Click in the *Site Options* icon on the ribbon.
3. Select the parameter named "SPMinEnvironment" and click in *Modify*.
4. Change its value to "Development" or "Production" and click *OK*.
5. Click *OK* in the site options window to apply the changes.

#### Using SharePoint Management Shell

Open it and run the following commands:

```powershell
$site = Get-SPSite http://site-collection-url/
$web = $site.RootWeb
$web.AllProperties["SPMinEnvironment"] = "Production" # or "Development"
$web.Update()
$site.Dispose()
```

#### Previewing production mode
When the environment mode is set to development, you can add the `?spmin=production` query string to your URL in order to preview how SPMin will render the controls in production mode.

## Contributing
1. Fork it
2. Create your feature branch (`git checkout -b my-new-feature`)
3. Commit your changes (`git commit -am 'Add some feature'`)
4. Push to the branch (`git push origin my-new-feature`)
5. Create new Pull Request
