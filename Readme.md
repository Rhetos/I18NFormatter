I18NFormatter is a DSL package for [Rhetos development platform](https://github.com/Rhetos/Rhetos).

It enables localization of Rhetos applications, using message format compatible with https://github.com/turquoiseowl/i18n.

## Rhetos.I18NFormatter

This plugin does not translate the message to the end user's language.
It formats the messages, keeping the message parameters separate from the main message structure,
to allow later localization by adding [i18n ASP.NET plugin](https://github.com/turquoiseowl/i18n) to the web server.

## GetTranslatableStrings

A command-line utility for extracting *translatable* strings from the source files.
It can be used on generated object model (ServerDom.cs) to extract error messages from miscellaneous business validation.
