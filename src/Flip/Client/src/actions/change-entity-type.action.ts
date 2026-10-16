import {
  UmbEntityActionBase,
  UmbEntityUpdatedEvent,
  UmbRequestReloadStructureForEntityEvent,
} from "@umbraco-cms/backoffice/entity-action";
import { UMB_ACTION_EVENT_CONTEXT } from "@umbraco-cms/backoffice/action";
import { umbConfirmModal, umbOpenModal } from "@umbraco-cms/backoffice/modal";
import { CHANGE_ENTITY_TYPE_MODAL_ALIAS } from "../modal/constants.js";
import type {
  ChangeEntityTypeModalData,
  ChangeEntityTypeModalValue,
} from "../modal/change-entity-type-modal.token.js";
import { FlipService } from "../../generated/index.js";
import { tryExecute } from "@umbraco-cms/backoffice/resources";
import { UmbLocalizationController } from "@umbraco-cms/backoffice/localization-api";

export class FlipChangeEntityTypeEntityAction extends UmbEntityActionBase<never> {
  override async execute() {
    if (!this.args.unique || !this.args.entityType) return;

    const result = await umbOpenModal<
      ChangeEntityTypeModalData,
      ChangeEntityTypeModalValue
    >(this, CHANGE_ENTITY_TYPE_MODAL_ALIAS, {
      data: {
        document: {
          unique: this.args.unique,
          entityType: this.args.entityType,
        },
      },
      modal: {
        size: "large",
        type: "sidebar",
      },
    }).catch(() => {});

    if (!result) return;

    const localize = new UmbLocalizationController(this);

    const confirmed = await umbConfirmModal(this, {
      headline: localize.term("flip_confirmChangeEntityType"),
      content: localize.term("flip_confirmChangeEntityTypeDetail"),
    }).then(() => true).catch(() => false);

    if (!confirmed) return;

    const { error } = await tryExecute(
      this,
      FlipService.postChangeType({
        body: {
          ...result,
          unique: this.args.unique.toString(),
        },
        query: {
          entityType: this.args.entityType,
        },
      }),
    );

    if (error) return;

    await this.#dispatchReloadEvents(this.args.unique, this.args.entityType);
  }

  async #dispatchReloadEvents(unique: string, entityType: string) {
    const actionEventContext = await this.getContext(UMB_ACTION_EVENT_CONTEXT);

    // reloads the tree item, so it shows the new type's icon
    actionEventContext?.dispatchEvent(
      new UmbRequestReloadStructureForEntityEvent({ unique, entityType })
    );

    // reloads an open document or element workspace, so it renders the new type's properties
    actionEventContext?.dispatchEvent(
      new UmbEntityUpdatedEvent({ unique, entityType })
    );
  }
}

export { FlipChangeEntityTypeEntityAction as api };
