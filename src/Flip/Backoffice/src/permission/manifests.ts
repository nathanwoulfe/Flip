import { UMB_DOCUMENT_ENTITY_TYPE } from "@umbraco-cms/backoffice/document";
import { ManifestEntityUserPermission } from "@umbraco-cms/backoffice/extension-registry";

export const FLIP_USER_PERMISSION_CHANGE_DOCUMENT_TYPE = "Flip.Permission.ChangeDocumentType";

const permissions: Array<ManifestEntityUserPermission> = [{
    type: "entityUserPermission",
    alias: FLIP_USER_PERMISSION_CHANGE_DOCUMENT_TYPE,
    name: "Flip Change Document Type User Permission",
    forEntityTypes: [UMB_DOCUMENT_ENTITY_TYPE],
    meta: {
        verbs:[FLIP_USER_PERMISSION_CHANGE_DOCUMENT_TYPE],
        label: `#flip_changeDocumentType`,
        description: `#flip_userPermissionDescription`,
    }
}];

export const manifests = [...permissions];