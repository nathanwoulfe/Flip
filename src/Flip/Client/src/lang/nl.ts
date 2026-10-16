import type { UmbLocalizationDictionary } from "@umbraco-cms/backoffice/localization-api";

export default {
  flip: {
    changeDocumentType: "Documenttype wijzigen",
    noPermittedTypes:
      "Het documenttype kan niet worden gewijzigd, omdat er voor deze locatie geen andere types zijn toegestaan.",
    newType: "Nieuw documenttype",
    newTemplate: "Nieuw sjabloon",
    allowChangeDocumentType: "Toegang toestaan om het documenttype te wijzigen",
    confirmChangeDocumentType: "Wijzigen van documenttype bevestigen",
    confirmChangeDocumentTypeDetail:
      "Weet je zeker dat je het documenttype wilt wijzigen? Afhankelijk van de gekozen koppeling kan deze actie tot gegevensverlies leiden.",
    selectDocumentType: "Selecteer een documenttype",
    currentProperty: "Huidige eigenschap",
    newProperty: "Nieuwe eigenschap",
    mapProperties: "Eigenschappen koppelen",
    mapType: "Eigenschappen koppelen op",
    mapTypeInstruction: `Koppelen op datatype beperkt de koppeling tot eigenschappen die exact hetzelfde datatype gebruiken (bijv. alle eigenschappen die het datatype Textarea gebruiken),
			koppelen op property editor beperkt de koppeling tot eigenschappen die hetzelfde type editor gebruiken (bijv. alle eigenschappen die de editor Umbraco.Textbox gebruiken).
			Koppelen op datatype voorkomt verschillen in configuratie bij het koppelen tussen eigenschappen die verschillende instanties van dezelfde editor gebruiken.`,
    mapPropertiesInstruction:
      "Bevestig en/of wijzig de koppeling van eigenschappen van het huidige type naar het nieuwe type. Lege of niet-gekoppelde eigenschappen worden leeggemaakt. Eigenschappen zonder compatibele koppeling worden genegeerd.",
  },
} as UmbLocalizationDictionary;
