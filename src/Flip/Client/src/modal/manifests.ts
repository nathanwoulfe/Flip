import ChangeEntityTypeModalElement from "./change-entity-type-modal.element.js";
import { CHANGE_ENTITY_TYPE_MODAL_ALIAS } from "./constants.js";

export const manifests: Array<UmbExtensionManifest> = [
  {
    type: "modal",
    alias: CHANGE_ENTITY_TYPE_MODAL_ALIAS,
    name: "Change Entity Type Modal",
    js: ChangeEntityTypeModalElement,
  },
];
