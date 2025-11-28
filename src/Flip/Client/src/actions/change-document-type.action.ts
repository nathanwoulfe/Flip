import { UmbEntityActionBase } from "@umbraco-cms/backoffice/entity-action";
import { umbConfirmModal, umbOpenModal } from "@umbraco-cms/backoffice/modal";
import { CHANGE_DOCUMENT_TYPE_MODAL_ALIAS } from "../modal/constants";
import type {
  ChangeDocumentTypeModalData,
  ChangeDocumentTypeModalValue,
} from "../modal";
import { FlipService } from "../../generated";
import { tryExecute } from "@umbraco-cms/backoffice/resources";
import { UmbLocalizationController } from "@umbraco-cms/backoffice/localization-api";

export class FlipChangeDocumentTypeEntityAction extends UmbEntityActionBase<never> {
  override async execute() {
    if (!this.args.unique || !this.args.entityType) return;

    const result = await umbOpenModal<
      ChangeDocumentTypeModalData,
      ChangeDocumentTypeModalValue
    >(this, CHANGE_DOCUMENT_TYPE_MODAL_ALIAS, {
      data: {
        document: {
          unique: this.args.unique,
          entityType: this.args.entityType,
        },
      },
      modal: {
        size: "medium",
        type: "sidebar",
      },
    }).catch(() => {});

    if (!result) return;

    const localize = new UmbLocalizationController(this);

    await umbConfirmModal(this, {
      headline: localize.term("flip_confirmChangeDocumentType"),
      content: localize.term("flip_confirmChangeDocumentTypeDetail"),
    });

    await tryExecute(
      this,
      FlipService.postChangeType({
        body: {
          ...result,
          unique: this.args.unique.toString(),
        },
      })
    );

    location.reload();
  }
}

export { FlipChangeDocumentTypeEntityAction as api };
