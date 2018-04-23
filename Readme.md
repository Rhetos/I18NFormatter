# I18NFormatter

I18NFormatter is a plugin package for [Rhetos development platform](https://github.com/Rhetos/Rhetos).
It enables localization of Rhetos applications, using message format compatible with <https://github.com/turquoiseowl/i18n>.

## Features

This plugin formats the Rhetos response messages for end users (for example, a data validation error message)
in a specific format to allow the translation of the message by a [GetText / PO](http://en.wikipedia.org/wiki/Gettext)
localization plugin for ASP.NET applications: <https://github.com/turquoiseowl/i18n>.

For example, the message "Required property {0} is not set", with a parameter value "Name",
is reformatted as "[[[Required property %0 is not set|||Name]]]".

## GetTranslatableStrings.exe

A command-line utility for extracting *translatable* strings from the source files.
It can be used on generated object model (ServerDom.cs) to extract error messages from miscellaneous business validation.
