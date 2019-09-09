# I18NFormatter release notes

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
