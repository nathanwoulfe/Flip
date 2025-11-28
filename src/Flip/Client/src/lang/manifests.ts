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
];
