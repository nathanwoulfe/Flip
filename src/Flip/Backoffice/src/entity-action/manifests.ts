import type { ManifestEntityAction } from "@umbraco-cms/backoffice/extension-registry";
import { FLIP_USER_PERMISSION_CHANGE_DOCUMENT_TYPE } from "../permission/manifests.js";
import { UMB_ENTITY_IS_NOT_TRASHED_CONDITION_ALIAS } from "@umbraco-cms/backoffice/recycle-bin";

const entityAction: ManifestEntityAction = {
  type: "entityAction",
  kind: "default",
  alias: "Flip.EntityAction.ChangeDocumentType",
  name: "Flip Change Document Type Entity Action",
  weight: 60,
  api: () => import("./change-document-type.action.js"),
  forEntityTypes: ["document"],
  meta: {
    icon: "icon-axis-rotation",
    label: "Change document type",
  },
  conditions: [
    {
      alias: "Umb.Condition.UserPermission.Document",
      allOf: [FLIP_USER_PERMISSION_CHANGE_DOCUMENT_TYPE],
    },
    {
      alias: UMB_ENTITY_IS_NOT_TRASHED_CONDITION_ALIAS,
    },
  ],
};

export const manifests = [entityAction];
