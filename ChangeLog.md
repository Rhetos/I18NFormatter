# I18NFormatter release notes

## 5.0.0 (TO BE RELEASED)

* Migrated from .NET Framework to .NET 5 and Rhetos 5.
* Localization with `turquoiseowl/i18n` can no longer be used directly in a Rhetos app, since it does not support .NET Core.
  * I18NFormatter can still be useful in a Rhetos app, if a separate ASP.NET MVC application localizes messages with turquoiseowl/i18n.
  * If the localization is needed directly in the Rhetos app, use any ASP.NET Core localization utility instead of this plugin. See [Readme.md](Readme.md) for more info.
* GetTranslatableStrings reads source files with UTF-8 encoding by default.

## 1.1.0 (2019-09-09)

### Internal improvements

* Bugfix: Null message parameter value should be formatted as empty string
  (same behavior as default implementation "NoLocalizer" and string.Format).
* Bugfix: GetTranslatableStrings.exe did not detect customized error messages with *InvalidDataMessage*.

## 1.0.0 (2016-04-08)

Features:

* Formats user messages from Rhetos web API to be compatible with <https://github.com/turquoiseowl/i18n> internationalization plugin.
* GetTranslatableStrings.exe, a command-line utility for extracting template strings from the source files.

See [Readme.md](Readme.md) for more info.
