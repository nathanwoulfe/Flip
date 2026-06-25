import type { UmbLocalizationDictionary } from "@umbraco-cms/backoffice/localization-api";

export default {
  flip: {
    changeDocumentType: "Change Document Type",
    changeElementType: "Change Element Type",
    changeEntityType: "Change %0% Type",
    noPermittedTypes:
      "The %0% type cannot be changed, as there are no other %0% types permitted for this location.",
    newType: "New type",
    newTemplate: "New template",
    allowChangeDocumentType: "Allow access to change document type",
    allowChangeElementType: "Allow access to change element type",
    confirmChangeEntityType: "Confirm Change Type",
    confirmChangeEntityTypeDetail:
      "Are you sure you want to change the entity type? This action may result in data loss depending on the selected mapping. After confirming, the backoffice will reload.",
    currentProperty: "Current property",
    newProperty: "New property",
    mapProperties: "Map properties",
    mapType: "Map properties by",
    mapTypeInstruction: `<ul>
      <li>Mapping by Data Type restricts mapping to properties using the exact Data Type (ie any property using the Textarea Data Type)</li>
      <li>Mapping by Property Editor restricts mapping to properties using the same editor type (ie any property using the Umbraco.Textbox editor).</li>
      <li>Mapping by Data Type prevents config mismatches when mapping between properties using different instances of the same editor.</li></ul>`,
    mapPropertiesInstruction:
      "Confirm and/or amend the mapping of properties from the current type to the new type. Blank or unmapped properties will be set to empty. Properties with no compatible mapping will be ignored.",
  },
} as UmbLocalizationDictionary;
