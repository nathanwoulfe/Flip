import type { UmbLocalizationDictionary } from "@umbraco-cms/backoffice/localization-api";

export default {
  flip: {
    changeDocumentType: "Skift dokumenttype",
    noPermittedTypes:
      "Dokumenttypen kan ikke skiftes, da der ikke er andre tilladte typer på denne placering.",
    newType: "Ny dokumenttype",
    newTemplate: "Ny skabelon",
    allowChangeDocumentType: "Tillad adgang til at skifte dokumenttype",
    confirmChangeDocumentType: "Bekræft skift af dokumenttype",
    confirmChangeDocumentTypeDetail:
      "Er du sikker på, at du vil skifte dokumenttype? Handlingen kan medføre tab af data afhængigt af den valgte tilknytning.",
    selectDocumentType: "Vælg en dokumenttype",
    currentProperty: "Nuværende egenskab",
    newProperty: "Ny egenskab",
    mapProperties: "Tilknyt egenskaber",
    mapType: "Tilknyt egenskaber efter",
    mapTypeInstruction: `Tilknytning efter datatype begrænser tilknytningen til egenskaber, der bruger præcis samme datatype (fx alle egenskaber, der bruger datatypen Textarea),
			tilknytning efter egenskabseditor begrænser tilknytningen til egenskaber, der bruger samme editortype (fx alle egenskaber, der bruger editoren Umbraco.Textbox).
			Tilknytning efter datatype forhindrer uoverensstemmende konfiguration, når der tilknyttes mellem egenskaber, der bruger forskellige instanser af samme editor.`,
    mapPropertiesInstruction:
      "Bekræft og/eller ret tilknytningen af egenskaber fra den nuværende type til den nye type. Tomme eller ikke-tilknyttede egenskaber bliver tømt. Egenskaber uden en kompatibel tilknytning ignoreres.",
  },
} as UmbLocalizationDictionary;
