export const manifests = [
  {
    type: "localization",
    alias: "Flip.Localization.En",
    weight: -100,
    name: "Flip Localization - English",
    meta: {
      culture: "en",
    },
    js: () => import("./en.js"),
  },
  {
    type: "localization",
    alias: "Flip.Localization.Da",
    weight: -100,
    name: "Flip Localization - Danish",
    meta: {
      culture: "da",
    },
    js: () => import("./da.js"),
  },
  {
    type: "localization",
    alias: "Flip.Localization.Nl",
    weight: -100,
    name: "Flip Localization - Dutch",
    meta: {
      culture: "nl",
    },
    js: () => import("./nl.js"),
  },
  {
    type: "localization",
    alias: "Flip.Localization.Sv",
    weight: -100,
    name: "Flip Localization - Swedish",
    meta: {
      culture: "sv",
    },
    js: () => import("./sv.js"),
  },
  {
    type: "localization",
    alias: "Flip.Localization.It",
    weight: -100,
    name: "Flip Localization - Italian",
    meta: {
      culture: "it",
    },
    js: () => import("./it.js"),
  },
  {
    type: "localization",
    alias: "Flip.Localization.Fr",
    weight: -100,
    name: "Flip Localization - French",
    meta: {
      culture: "fr",
    },
    js: () => import("./fr.js"),
  },
];
