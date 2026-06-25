import { UmbModalToken } from "@umbraco-cms/backoffice/modal";
import { CHANGE_ENTITY_TYPE_MODAL_ALIAS } from "./constants.js";
import type { UmbEntityUnique } from "@umbraco-cms/backoffice/entity";
import type { EntityTypePropertyModel } from "../../generated/index.js";

export interface ChangeEntityTypeModalData {
  document: {
    unique: UmbEntityUnique;
    entityType: string;
  };
}

export interface ChangeEntityTypeModalValue {
  contentTypeUnique: string;
  templateUnique?: string | null;
  properties: EntityTypePropertyModel[];
}

export const CHANGE_ENTITY_TYPE_MODAL = new UmbModalToken<
  ChangeEntityTypeModalData,
  ChangeEntityTypeModalValue
>(CHANGE_ENTITY_TYPE_MODAL_ALIAS, {
  modal: {
    type: "sidebar",
    size: "small",
  },
});
