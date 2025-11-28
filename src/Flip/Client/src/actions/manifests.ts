import { UMB_DOCUMENT_ENTITY_TYPE } from "@umbraco-cms/backoffice/document";
import { FLIP_USER_PERMISSION_CHANGE_DOCUMENT_TYPE } from "../permissions/constants.js";

export const manifests: Array<UmbExtensionManifest> = [
  {
    type: "entityAction",
    kind: "default",
    name: `Flip Change Document Type Action`,
    alias: `Flip.EntityAction.ChangeDocumentType`,
    forEntityTypes: [UMB_DOCUMENT_ENTITY_TYPE],
    meta: {
      label: "#flip_changeDocumentType",
      icon: "icon-axis-rotation",
    },
    api: () => import("./change-document-type.action.js"),
    conditions: [
      {
        alias: "Umb.Condition.UserPermission.Document",
        allOf: [FLIP_USER_PERMISSION_CHANGE_DOCUMENT_TYPE],
      },
    ],
  },
];
