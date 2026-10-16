import type { UmbLocalizationDictionary } from "@umbraco-cms/backoffice/localization-api";

export default {
  flip: {
    changeDocumentType: "Ändra dokumenttyp",
    changeElementType: "Ändra elementtyp",
    changeEntityType: "Ändra typ",
    noPermittedTypes:
      "Typen kan inte ändras eftersom inga andra typer är tillåtna på den här platsen.",
    newType: "Ny typ",
    newTemplate: "Ny sidmall",
    allowChangeDocumentType: "Ger behörighet att ändra dokumenttyp",
    allowChangeElementType: "Ger behörighet att ändra elementtyp",
    confirmChangeEntityType: "Bekräfta ändring av typ",
    confirmChangeEntityTypeDetail:
      "Är du säker på att du vill ändra typen? Beroende på vald mappning kan åtgärden leda till dataförlust.",
    currentProperty: "Nuvarande egenskap",
    newProperty: "Ny egenskap",
    mapProperties: "Mappa egenskaper",
    mapType: "Mappa egenskaper efter",
    mapTypeInstruction: `<ul>
      <li>Mappning efter datatyp begränsar mappningen till egenskaper som använder exakt samma datatyp (t.ex. alla egenskaper som använder datatypen Textarea)</li>
      <li>Mappning efter egenskapsredigerare begränsar mappningen till egenskaper som använder samma typ av redigerare (t.ex. alla egenskaper som använder redigeraren Umbraco.Textbox).</li>
      <li>Mappning efter datatyp förhindrar avvikande konfiguration vid mappning mellan egenskaper som använder olika instanser av samma redigerare.</li></ul>`,
    mapPropertiesInstruction:
      "Bekräfta och/eller justera mappningen av egenskaper från den nuvarande typen till den nya typen. Tomma eller omappade egenskaper töms. Egenskaper utan kompatibel mappning ignoreras.",
  },
} as UmbLocalizationDictionary;
