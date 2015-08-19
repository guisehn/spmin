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

## Including assets in your page
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
Of course you can also include the assets in your page using HTML script/link tags or ScriptLink/CssRegistration controls just like you would do with normal assets. However, by using this you can't easily include different versions of the file for each environment.

## Combining assets

Coming soon. :)

## How to install
SPMin is a farm solution so it will work only in on-premises SharePoint environments. The install instructions are detailed in the [release page](https://github.com/ghsehn/SPMin/releases/latest).
