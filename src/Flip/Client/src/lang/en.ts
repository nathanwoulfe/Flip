import type { UmbLocalizationDictionary } from "@umbraco-cms/backoffice/localization-api";

export default {
  flip: {
    changeDocumentType: "Change Document Type",
    noPermittedTypes:
      "The document type cannot be changed, as there are no other types permitted for this location.",
    newType: "New Document Type",
    newTemplate: "New template",
    allowChangeDocumentType: "Allow access to change document type",
    confirmChangeDocumentType: "Confirm Change Document Type",
    confirmChangeDocumentTypeDetail:
      "Are you sure you want to change the document type? This action may result in data loss depending on the selected mapping. After confirming, the backoffice will reload.",
    selectDocumentType: "Select a document type",
    currentProperty: "Current property",
    newProperty: "New property",
    mapProperties: "Map properties",
    mapType: "Map properties by",
    mapTypeInstruction: `Mapping by Data Type restricts mapping to properties using the exact Data Type (ie any property using the Textarea Data Type),
			mapping by Property Editor restricts mapping to properties using the same editor type (ie any property using the Umbraco.Textbox editor).
			Mapping by Data Type prevents config mismatches when mapping between properties using different instances of the same editor.`,
    mapPropertiesInstruction:
      "Confirm and/or amend the mapping of properties from the current type to the new type. Blank or unmapped properties will be set to empty. Properties with no compatible mapping will be ignored.",
  },
} as UmbLocalizationDictionary;
