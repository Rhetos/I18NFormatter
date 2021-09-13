# I18NFormatter

I18NFormatter is a plugin package for [Rhetos development platform](https://github.com/Rhetos/Rhetos).
It enables localization of Rhetos applications, using message format compatible with <https://github.com/turquoiseowl/i18n>.

**Note that [turquoiseowl/i18n](https://github.com/turquoiseowl/i18n) currently supports only .NET Framework, and cannot be used directly in a Rhetos v5 app:**

1. You can still use turquoiseowl/i18n if you have the localization implemented in a **separate "frontend" app, for example on ASP.NET MVC**, that acts as a proxy for Rhetos REST services. In that case, add Rhetos.I18NFormatter package to the Rhetos app in order to make its messages formatted for localization that occurs in the frontend app.
2. If you want **localization implemented directly in the Rhetos app**, instead of I18NFormatter package and turquoiseowl/i18n, use any ASP.NET Core localization utility, by configuring Rhetos host with `.AddHostLocalization()`.
   * To use localization with "PO files" (compatible with turquoiseowl/i18n), see [Microsoft's recommendations for OrchardCore](https://docs.microsoft.com/en-us/aspnet/core/fundamentals/portable-object-localization?view=aspnetcore-5.0), and the usage [example](https://github.com/Rhetos/Bookstore/pull/17/files) in Bookstore Rhetos app.

## Features

This plugin formats the Rhetos response messages for end users (for example, a data validation error message)
in a specific format to allow the translation of the message by a [GetText / PO](http://en.wikipedia.org/wiki/Gettext)
localization plugin for ASP.NET applications: <https://github.com/turquoiseowl/i18n>.

For example, the message "Required property {0} is not set", with a parameter value "Name",
is reformatted as "[[[Required property %0 is not set|||Name]]]".

## GetTranslatableStrings.exe

A command-line utility for extracting *translatable* strings from the source files.
It can be used on generated object model (ServerDom.cs) to extract error messages from miscellaneous business validation.

## Build

**Note:** This package is already available at the [NuGet.org](https://www.nuget.org/) online gallery.
You don't need to build it from source in order to use it in your application.

To build the package from source, run `Build.bat`.
The script will pause in case of an error.
The build output is a NuGet package in the "Install" subfolder.

## Installation

To install this package to a Rhetos server, add it to the Rhetos server's *RhetosPackages.config* file
and make sure the NuGet package location is listed in the *RhetosPackageSources.config* file.

* The package ID is "**Rhetos.I18NFormatter**".
  This package is available at the [NuGet.org](https://www.nuget.org/) online gallery.
  It can be downloaded or installed directly from there.
* For more information, see [Installing plugin packages](https://github.com/Rhetos/Rhetos/wiki/Installing-plugin-packages).
