import { UmbEntityActionBase } from "@umbraco-cms/backoffice/entity-action";
import { UMB_MODAL_MANAGER_CONTEXT } from "@umbraco-cms/backoffice/modal";
import { FLIP_CHANGE_DOCUMENT_TYPE_MODAL } from "@flip/modal";
import { TypeService } from "@flip/generated";
import { tryExecuteAndNotify } from "@umbraco-cms/backoffice/resources";
import { UMB_EDIT_DOCUMENT_WORKSPACE_PATH_PATTERN } from "@umbraco-cms/backoffice/document";

export class FlipChangeDocumentTypeEntityAction extends UmbEntityActionBase<never> {
  async execute() {
    const modalManager = await this.getContext(UMB_MODAL_MANAGER_CONTEXT);
    const sidebarContext = modalManager.open(
      this,
      FLIP_CHANGE_DOCUMENT_TYPE_MODAL,
      {
        data: {
          document: {
            unique: this.args.unique,
          },
        },
      }
    );

    await sidebarContext.onSubmit().catch(() => {});
    const value = sidebarContext.getValue();

    if (!value) return;

    const { data, error } = await tryExecuteAndNotify(
      this,
      TypeService.postTypeChange({
        requestBody: {
          ...value,
        },
      })
    );

    if (!data || error) return;

    if (data.success) {
      // hard reload - calling load on the workspace context doesn't work (yet)
      location.href = UMB_EDIT_DOCUMENT_WORKSPACE_PATH_PATTERN.generateAbsolute(
        { unique: this.args.unique! }
      );
    }
  }
}

export default FlipChangeDocumentTypeEntityAction;
