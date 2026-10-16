import {
  UmbEntityActionBase,
  UmbEntityUpdatedEvent,
  UmbRequestReloadStructureForEntityEvent,
} from "@umbraco-cms/backoffice/entity-action";
import { UMB_ACTION_EVENT_CONTEXT } from "@umbraco-cms/backoffice/action";
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

    const { error } = await tryExecute(
      this,
      FlipService.postChangeType({
        body: {
          ...result,
          unique: this.args.unique.toString(),
        },
      })
    );

    if (error) return;

    await this.#dispatchReloadEvents(this.args.unique, this.args.entityType);
  }

  async #dispatchReloadEvents(unique: string, entityType: string) {
    const actionEventContext = await this.getContext(UMB_ACTION_EVENT_CONTEXT);

    // reloads the tree item, so it shows the new document type's icon
    actionEventContext?.dispatchEvent(
      new UmbRequestReloadStructureForEntityEvent({ unique, entityType })
    );

    // reloads an open document workspace, so it renders the new document type's properties
    actionEventContext?.dispatchEvent(
      new UmbEntityUpdatedEvent({ unique, entityType })
    );
  }
}

export { FlipChangeDocumentTypeEntityAction as api };
