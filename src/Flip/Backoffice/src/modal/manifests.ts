import { ManifestModal } from "@umbraco-cms/backoffice/extension-registry";
import { FLIP_CHANGE_DOCUMENT_TYPE_MODAL_ALIAS } from "./flip-modal.token.js";

const modals: Array<ManifestModal> = [
  {
    type: "modal",
    alias: FLIP_CHANGE_DOCUMENT_TYPE_MODAL_ALIAS,
    name: "Flip Change Document Type Modal",
    js: () => import("./flip-modal.element.js"),
  },
];

export const manifests = [...modals];
