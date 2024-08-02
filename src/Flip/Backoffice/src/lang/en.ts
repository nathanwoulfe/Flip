import type { UmbLocalizationDictionary } from "@umbraco-cms/backoffice/localization-api";

export default {
  flip: {
    userPermissionDescription: "Allow access to change a document type",
    changeDocumentType: "Change Document Type",
    propertyEditor: "Property Editor UI",
    dataType: "Data Type",
    newType: "New Document Type",
    newTemplate: "New template",
    currentType: "Current Document Type",
    currentProperty: "Current property",
    newProperty: "New property",
    changeDocTypeInstruction:
      "To change the document type for the selected content, first select from the list of valid types for this location.",
    noPermittedTypes:
      "The document type cannot be changed, as there are no other types permitted for this location.",
    mapProperties: "Map properties",
    mapType: "Mapping type",
    mapPropertiesInstruction:
      "Confirm and/or amend the mapping of properties from the current type to the new type. Blank or unmapped properties will be ignored.",
    mapTypeInstruction: `Mapping by Data Type restricts mapping to properties using the exact Data Type (ie any property using the Textarea Data Type), 
		mapping by Property Editor UI restricts mapping to properties using the same editor type (ie any property using the Umb.PropertyEditorUi.Textbox editor). 
		Mapping by Data Type prevents config mismatches when mapping between properties using different instances of the same editor.`,
  },
} as UmbLocalizationDictionary;
