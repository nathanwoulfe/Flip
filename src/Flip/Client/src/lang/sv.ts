import type { UmbLocalizationDictionary } from "@umbraco-cms/backoffice/localization-api";

export default {
  flip: {
    changeDocumentType: "Ändra dokumenttyp",
    noPermittedTypes:
      "Dokumenttypen kan inte ändras eftersom inga andra typer är tillåtna på den här platsen.",
    newType: "Ny dokumenttyp",
    newTemplate: "Ny sidmall",
    allowChangeDocumentType: "Ger behörighet att ändra dokumenttyp",
    confirmChangeDocumentType: "Bekräfta ändring av dokumenttyp",
    confirmChangeDocumentTypeDetail:
      "Är du säker på att du vill ändra dokumenttyp? Beroende på vald mappning kan åtgärden leda till dataförlust.",
    selectDocumentType: "Välj en dokumenttyp",
    currentProperty: "Nuvarande egenskap",
    newProperty: "Ny egenskap",
    mapProperties: "Mappa egenskaper",
    mapType: "Mappa egenskaper efter",
    mapTypeInstruction: `Mappning efter datatyp begränsar mappningen till egenskaper som använder exakt samma datatyp (t.ex. alla egenskaper som använder datatypen Textarea),
			mappning efter egenskapsredigerare begränsar mappningen till egenskaper som använder samma typ av redigerare (t.ex. alla egenskaper som använder redigeraren Umbraco.Textbox).
			Mappning efter datatyp förhindrar avvikande konfiguration vid mappning mellan egenskaper som använder olika instanser av samma redigerare.`,
    mapPropertiesInstruction:
      "Bekräfta och/eller justera mappningen av egenskaper från den nuvarande typen till den nya typen. Tomma eller omappade egenskaper töms. Egenskaper utan kompatibel mappning ignoreras.",
  },
} as UmbLocalizationDictionary;
