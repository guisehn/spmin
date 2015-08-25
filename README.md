# SPMin
Automatic JavaScript and CSS minification for SharePoint's Style Library.

## How it works
This solution contains an event receiver that automatizes the minification of JavaScript and CSS files inside the
Style Library. It can also combine multiple files into one to minimize the number of HTTP requests in the page.

## How to use
SPMin will create a minified version for the files whose names end with `.spm.js` or `.spm.css` in the Style Library.
This means that you can apply this solution into your project without any impact to existing assets.

For each of these files, it will create a `.spm.min.js` or `.spm.min.css` correspondent one with the minified code. Operations
such as editing, checking-out/checking-in and deleting are synchronized for the minified file.

### Including assets in your page
SPMin includes special controls that allow you to include the original assets in your development environment and use the minified version in the production environment, without having to write different code for each environment.

In order to use them, first you need to include the following directive at the top of your file:

```asp
<%@ Register TagPrefix="SPMin" Namespace="SPMin.Controls"
Assembly="SPMin, Version=1.0.0.0, Culture=neutral, PublicKeyToken=a86ef32346edfcab" %>
```

Then, you can add the `CssRegistration` control to include a CSS file and the `JsRegistration` control to include a JavaScript file.

```asp
<SPMin:CssRegistration FilePath="path/to/stylesheet.spm.css" runat="server" />
<SPMin:JsRegistration FilePath="path/to/javascript.spm.js" runat="server" />
```

The path must be always relative to the `Style Library` library of the current site collection. So, in the development environment, the following HTML would have been generated for the above example:

```html
<link rel='stylesheet' href='/sites/site-collection/Style Library/path/to/stylesheet.spm.css' type='text/css' />
<script src='/sites/site-collection/Style Library/path/to/javascript.spm.js' type='text/javascript'></script>
```

For the production environment, the minified version would have been printed:

```html
<link rel='stylesheet' href='/sites/site-collection/Style Library/path/to/stylesheet.spm.min.css' type='text/css' />
<script src='/sites/site-collection/Style Library/path/to/javascript.spm.min.js' type='text/javascript'></script>
```

These controls will not print duplicated inclusion tags for the same file in the page (it will render only the first one). If you need to include the same JavaScript or CSS file multiple times, add the `IncludeOnce="false"` attribute to the subsequent control definition.

### Combining assets

You can tell SPMin to combine different assets inside one file in order to reduce the number of HTTP requests in the prodution environment and thus improve the client-side loading performance. It works similarly to [Ruby on Rails's asset pipeline](https://github.com/rails/sprockets).

To do this, create a file in the Style Library that ends with `.spm.js` (for example `app.spm.js`) and add a comment at the top of it that specifices the files to be included in the following format:

```javascript
/*
 *= file1.js
 *= file2.js
 *= file3.js
 */
```

You can also add more code into this file, but the comment with the included files must be at the top of it.

This will tell SPMin to include the contents of `file1.js`, `file2.js` and `file3.js` at the beginning of `app.spm.js`. The included files must be in the same folder of your main file.

Everytime `app.spm.js` is updated, it will regenerate `app.spm.min.js` with the minified combined files. **Important:** if you update `file1.js` alone, it will not regenerate `app.spm.min.js`. You need to manually re-save `app.spm.js` in order to trigger the combination and minification again.

When the environment mode is set to development and you add a `JsRegistration` control pointing to your combined JavaScript file, it will generate one script tag for each included file. So this example:

```asp
<SPMin:JsRegistration FilePath="path/to/app.spm.js" runat="server" />
```

Generates this HTML:

```html
<script src='/sites/site-collection/Style Library/path/to/file1.js' type='text/javascript'></script>
<script src='/sites/site-collection/Style Library/path/to/file2.js' type='text/javascript'></script>
<script src='/sites/site-collection/Style Library/path/to/file3.js' type='text/javascript'></script>
<script src='/sites/site-collection/Style Library/path/to/app.spm.js' type='text/javascript'></script>
```

In the production environment, it will generate the following HTML:

```html
<script src='/sites/site-collection/Style Library/path/to/app.spm.min.js' type='text/javascript'></script>
```

This also works for CSS files.

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
```

#### Previewing production mode
When the environment mode is set to development, you can add the `?spmin=production` query string to your URL in order to preview how SPMin will render the controls in production mode.

## How to install
SPMin is a farm solution so it will work only in on-premises SharePoint environments. The install instructions are detailed in the [release page](https://github.com/ghsehn/SPMin/releases/latest).
