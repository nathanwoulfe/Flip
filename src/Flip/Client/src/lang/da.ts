import type { UmbLocalizationDictionary } from "@umbraco-cms/backoffice/localization-api";

export default {
  flip: {
    changeDocumentType: "Skift dokumenttype",
    changeElementType: "Skift elementtype",
    changeEntityType: "Skift type",
    noPermittedTypes:
      "Typen kan ikke skiftes, da der ikke er andre tilladte typer på denne placering.",
    newType: "Ny type",
    newTemplate: "Ny skabelon",
    allowChangeDocumentType: "Tillad adgang til at skifte dokumenttype",
    allowChangeElementType: "Tillad adgang til at skifte elementtype",
    confirmChangeEntityType: "Bekræft skift af type",
    confirmChangeEntityTypeDetail:
      "Er du sikker på, at du vil skifte typen? Handlingen kan medføre tab af data afhængigt af den valgte tilknytning.",
    currentProperty: "Nuværende egenskab",
    newProperty: "Ny egenskab",
    mapProperties: "Tilknyt egenskaber",
    mapType: "Tilknyt egenskaber efter",
    mapTypeInstruction: `<ul>
      <li>Tilknytning efter datatype begrænser tilknytningen til egenskaber, der bruger præcis samme datatype (fx alle egenskaber, der bruger datatypen Textarea)</li>
      <li>Tilknytning efter egenskabseditor begrænser tilknytningen til egenskaber, der bruger samme editortype (fx alle egenskaber, der bruger editoren Umbraco.Textbox).</li>
      <li>Tilknytning efter datatype forhindrer uoverensstemmende konfiguration, når der tilknyttes mellem egenskaber, der bruger forskellige instanser af samme editor.</li></ul>`,
    mapPropertiesInstruction:
      "Bekræft og/eller ret tilknytningen af egenskaber fra den nuværende type til den nye type. Tomme eller ikke-tilknyttede egenskaber bliver tømt. Egenskaber uden en kompatibel tilknytning ignoreres.",
  },
} as UmbLocalizationDictionary;
