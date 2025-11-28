import { UMB_DOCUMENT_ENTITY_TYPE } from "@umbraco-cms/backoffice/document";
import { FLIP_USER_PERMISSION_CHANGE_DOCUMENT_TYPE } from "./constants";

export const manifests = [
  {
    type: "entityUserPermission",
    alias: `Flip.EntityUserPermission.ChangeDocumentType`,
    name: `Flip Change Document Type User Permission`,
    forEntityTypes: [UMB_DOCUMENT_ENTITY_TYPE],
    meta: {
      verbs: [FLIP_USER_PERMISSION_CHANGE_DOCUMENT_TYPE],
      label: "#flip_changeDocumentType",
      description: "#flip_allowChangeDocumentType",
    },
  },
];
