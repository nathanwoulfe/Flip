import { UmbModalToken } from "@umbraco-cms/backoffice/modal";
import { CHANGE_DOCUMENT_TYPE_MODAL_ALIAS } from "./constants";
import type { UmbEntityUnique } from "@umbraco-cms/backoffice/entity";
import type { DocumentTypePropertyModel } from "../../generated";

export interface ChangeDocumentTypeModalData {
  document: {
    unique: UmbEntityUnique;
    entityType: string;
  };
}

export interface ChangeDocumentTypeModalValue {
  contentTypeUnique: string;
  templateId?: number;
  properties: DocumentTypePropertyModel[];
}

export const CHANGE_DOCUMENT_TYPE_MODAL = new UmbModalToken<
  ChangeDocumentTypeModalData,
  ChangeDocumentTypeModalValue
>(CHANGE_DOCUMENT_TYPE_MODAL_ALIAS, {
  modal: {
    type: "sidebar",
    size: "small",
  },
});
