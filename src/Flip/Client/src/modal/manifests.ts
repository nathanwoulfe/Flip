import { CHANGE_DOCUMENT_TYPE_MODAL_ALIAS } from "./constants.js";

export const manifests: Array<UmbExtensionManifest> = [
  {
    type: "modal",
    alias: CHANGE_DOCUMENT_TYPE_MODAL_ALIAS,
    name: "Change Document Type Modal",
    js: () => import("./change-document-type-modal.element.js"),
  },
];
