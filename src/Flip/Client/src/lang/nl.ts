import type { UmbLocalizationDictionary } from "@umbraco-cms/backoffice/localization-api";

export default {
  flip: {
    changeDocumentType: "Documenttype wijzigen",
    changeElementType: "Elementtype wijzigen",
    changeEntityType: "Type wijzigen",
    noPermittedTypes:
      "Het type kan niet worden gewijzigd, omdat er voor deze locatie geen andere types zijn toegestaan.",
    newType: "Nieuw type",
    newTemplate: "Nieuw sjabloon",
    allowChangeDocumentType: "Toegang toestaan om het documenttype te wijzigen",
    allowChangeElementType: "Toegang toestaan om het elementtype te wijzigen",
    confirmChangeEntityType: "Wijzigen van type bevestigen",
    confirmChangeEntityTypeDetail:
      "Weet je zeker dat je het type wilt wijzigen? Afhankelijk van de gekozen koppeling kan deze actie tot gegevensverlies leiden.",
    currentProperty: "Huidige eigenschap",
    newProperty: "Nieuwe eigenschap",
    mapProperties: "Eigenschappen koppelen",
    mapType: "Eigenschappen koppelen op",
    mapTypeInstruction: `<ul>
      <li>Koppelen op datatype beperkt de koppeling tot eigenschappen die exact hetzelfde datatype gebruiken (bijv. alle eigenschappen die het datatype Textarea gebruiken)</li>
      <li>Koppelen op property editor beperkt de koppeling tot eigenschappen die hetzelfde type editor gebruiken (bijv. alle eigenschappen die de editor Umbraco.Textbox gebruiken).</li>
      <li>Koppelen op datatype voorkomt verschillen in configuratie bij het koppelen tussen eigenschappen die verschillende instanties van dezelfde editor gebruiken.</li></ul>`,
    mapPropertiesInstruction:
      "Bevestig en/of wijzig de koppeling van eigenschappen van het huidige type naar het nieuwe type. Lege of niet-gekoppelde eigenschappen worden leeggemaakt. Eigenschappen zonder compatibele koppeling worden genegeerd.",
  },
} as UmbLocalizationDictionary;
