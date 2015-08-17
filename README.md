# SPMin
Automatic JavaScript and CSS minification for SharePoint's Style Library.

## How it works
This solution contains an event receiver that automatizes the minification of JavaScript and CSS files inside the
Style Library. It will create a minified version only for the files whose names end with `.spm.js` or `.spm.css`,
so you can apply this solution to your project without any impact to existing assets.

For each of these files, it will create a `.spm.min.js` or `.spm.min.css` correspondent with the minified code. Operations
such as editing, checking-out/checking-in and deleting are synchronized for the minified file.

## How to install
SPMin is a farm solution so it will work only in on-premises SharePoint environments. There are plans to transform it into
a sandbox solution in the future.

The install instructions are detailed in the [release page](https://github.com/ghsehn/SPMin/releases/latest).
