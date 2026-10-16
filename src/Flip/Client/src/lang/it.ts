import type { UmbLocalizationDictionary } from "@umbraco-cms/backoffice/localization-api";

export default {
  flip: {
    changeDocumentType: "Cambia tipo di documento",
    noPermittedTypes:
      "Il tipo di documento non può essere cambiato, perché non ci sono altri tipi consentiti in questa posizione.",
    newType: "Nuovo tipo di documento",
    newTemplate: "Nuovo template",
    allowChangeDocumentType: "Consenti l'accesso per cambiare tipo di documento",
    confirmChangeDocumentType: "Conferma cambio del tipo di documento",
    confirmChangeDocumentTypeDetail:
      "Sei sicuro di voler cambiare il tipo di documento? A seconda della mappatura selezionata, questa azione potrebbe causare la perdita di dati.",
    selectDocumentType: "Seleziona un tipo di documento",
    currentProperty: "Proprietà attuale",
    newProperty: "Nuova proprietà",
    mapProperties: "Mappa proprietà",
    mapType: "Mappa proprietà per",
    mapTypeInstruction: `La mappatura per tipo di dato limita la mappatura alle proprietà che usano esattamente lo stesso tipo di dato (es. tutte le proprietà che usano il tipo di dato Textarea),
			la mappatura per property editor limita la mappatura alle proprietà che usano lo stesso tipo di editor (es. tutte le proprietà che usano l'editor Umbraco.Textbox).
			La mappatura per tipo di dato evita incongruenze di configurazione quando si mappano proprietà che usano istanze diverse dello stesso editor.`,
    mapPropertiesInstruction:
      "Conferma e/o modifica la mappatura delle proprietà dal tipo attuale al nuovo tipo. Le proprietà vuote o non mappate verranno svuotate. Le proprietà senza una mappatura compatibile verranno ignorate.",
  },
} as UmbLocalizationDictionary;
