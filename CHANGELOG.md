# Road map

- [x] Generate T-SQL table(s) schema and/or data
- [ ] Support switches
  - [x] IncludeIfNotExists - An existence check is added to script.
  - [x] IncludeHeaders - A header containing information about the object being scripted is included.
  - [x] NoBatchTerminator - Disables GO terminator in script.
  - [x] AppendToFile
  - [x] Indexes
  - [ ] BatchSize
  - [x] Encoding
  - [ ] NoSchemaQualify - Object names in script are not schema qualified.
  - [ ] Triggers
  - [ ] IncludeScriptingParametersHeader - A header containing information about the scripting parameters is included.
 - [ ] Move to netstandard (core, net47)
 - [ ] Localization 
 - [ ] Command line examples in help
 - [ ] Create wiki page
   - [ ] Move road map to wiki
 - [ ] Any others ?

# Change log

These are the changes to each version that has been released.

## 1.0.1
- [x] Support more switches

## 1.0.0

- [x] Initial release
- [x] Generate T-SQL table(s) schema and/or data
