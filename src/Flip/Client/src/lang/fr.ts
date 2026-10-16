import type { UmbLocalizationDictionary } from "@umbraco-cms/backoffice/localization-api";

export default {
  flip: {
    changeDocumentType: "Changer le type de document",
    changeElementType: "Changer le type d'élément",
    changeEntityType: "Changer le type",
    noPermittedTypes:
      "Le type ne peut pas être changé, car aucun autre type n'est autorisé à cet emplacement.",
    newType: "Nouveau type",
    newTemplate: "Nouveau modèle",
    allowChangeDocumentType: "Permettre de changer le type de document",
    allowChangeElementType: "Permettre de changer le type d'élément",
    confirmChangeEntityType: "Confirmer le changement de type",
    confirmChangeEntityTypeDetail:
      "Êtes-vous sûr de vouloir changer le type ? Selon le mappage sélectionné, cette action peut entraîner une perte de données.",
    currentProperty: "Propriété actuelle",
    newProperty: "Nouvelle propriété",
    mapProperties: "Mapper les propriétés",
    mapType: "Mapper les propriétés par",
    mapTypeInstruction: `<ul>
      <li>Le mappage par type de données limite le mappage aux propriétés utilisant exactement le même type de données (par ex. toutes les propriétés utilisant le type de données Textarea)</li>
      <li>Le mappage par property editor limite le mappage aux propriétés utilisant le même type d'éditeur (par ex. toutes les propriétés utilisant l'éditeur Umbraco.Textbox).</li>
      <li>Le mappage par type de données évite les incohérences de configuration lors du mappage entre des propriétés utilisant différentes instances du même éditeur.</li></ul>`,
    mapPropertiesInstruction:
      "Confirmez et/ou modifiez le mappage des propriétés du type actuel vers le nouveau type. Les propriétés vides ou non mappées seront vidées. Les propriétés sans mappage compatible seront ignorées.",
  },
} as UmbLocalizationDictionary;
