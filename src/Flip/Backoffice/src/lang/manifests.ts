import { ManifestLocalization } from "@umbraco-cms/backoffice/extension-registry";

const localizationManifests: Array<ManifestLocalization> = [
    {
      type: "localization",
      alias: "Flip.Localization.En",
      weight: -100,
      name: "English",
      meta: {
        culture: "en",
      },
      js: () => import("./en.js"),
    },
]

export const manifests = [...localizationManifests];