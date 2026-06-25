import { UMB_DOCUMENT_ENTITY_TYPE } from "@umbraco-cms/backoffice/document";
import {
  FLIP_USER_PERMISSION_CHANGE_DOCUMENT_TYPE,
  FLIP_USER_PERMISSION_CHANGE_ELEMENT_TYPE,
} from "./constants.js";
import { UMB_ELEMENT_ENTITY_TYPE } from "@umbraco-cms/backoffice/element";

const mapManifest = (key: "Document" | "Element") => ({
  type: "entityUserPermission",
  alias: `Flip.EntityUserPermission.Change${key}Type`,
  name: `Flip Change ${key} Type User Permission`,
  forEntityTypes: [
    key === "Document" ? UMB_DOCUMENT_ENTITY_TYPE : UMB_ELEMENT_ENTITY_TYPE,
  ],
  meta: {
    verbs: [
      key === "Document"
        ? FLIP_USER_PERMISSION_CHANGE_DOCUMENT_TYPE
        : FLIP_USER_PERMISSION_CHANGE_ELEMENT_TYPE,
    ],
    label: `#flip_change${key}Type`,
    description: `#flip_allowChange${key}Type`,
  },
});

export const manifests = [mapManifest("Document"), mapManifest("Element")];
