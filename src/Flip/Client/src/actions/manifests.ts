import {
  UMB_DOCUMENT_ENTITY_TYPE,
  UMB_DOCUMENT_USER_PERMISSION_CONDITION_ALIAS,
} from "@umbraco-cms/backoffice/document";
import { FLIP_USER_PERMISSION_CHANGE_DOCUMENT_TYPE, FLIP_USER_PERMISSION_CHANGE_ELEMENT_TYPE } from "../permissions/constants.js";
import { FlipChangeEntityTypeEntityAction } from "./change-entity-type.action.js";
import { UMB_ELEMENT_ENTITY_TYPE, UMB_ELEMENT_USER_PERMISSION_CONDITION_ALIAS } from "@umbraco-cms/backoffice/element";

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
    api: FlipChangeEntityTypeEntityAction,
    conditions: [
      {
        alias: UMB_DOCUMENT_USER_PERMISSION_CONDITION_ALIAS,
        allOf: [FLIP_USER_PERMISSION_CHANGE_DOCUMENT_TYPE],
      },
    ],
  },
  {
    type: "entityAction",
    kind: "default",
    name: `Flip Change Element Type Action`,
    alias: `Flip.EntityAction.ChangeElementType`,
    forEntityTypes: [UMB_ELEMENT_ENTITY_TYPE],
    meta: {
      label: "#flip_changeElementType",
      icon: "icon-axis-rotation",
    },
    api: FlipChangeEntityTypeEntityAction,
    conditions: [
      {
        alias: UMB_ELEMENT_USER_PERMISSION_CONDITION_ALIAS,
        allOf: [FLIP_USER_PERMISSION_CHANGE_ELEMENT_TYPE],
      },
    ],
  },
];
