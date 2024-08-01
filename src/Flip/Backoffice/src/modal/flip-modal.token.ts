import { DocumentTypePropertyResponseModel } from "@flip/generated";
import { UmbModalToken } from "@umbraco-cms/backoffice/modal";

export const FLIP_CHANGE_DOCUMENT_TYPE_MODAL_ALIAS =
  "Flip.Modal.ChangeDocumentType";

export interface FlipChangeDocumentTypeModalData {
  document: {
    unique: string | null;
    name?: string;
  };
}

export interface FlipChangeDocumentTypeModalValue {
  unique: string;
  properties: Array<DocumentTypePropertyResponseModel>;
  documentTypeUnique: string;
  templateId?: number;
}

export const FLIP_CHANGE_DOCUMENT_TYPE_MODAL = new UmbModalToken<
  FlipChangeDocumentTypeModalData,
  FlipChangeDocumentTypeModalValue
>(FLIP_CHANGE_DOCUMENT_TYPE_MODAL_ALIAS, {
  modal: {
    type: "sidebar",
    size: "medium",
  },
});
