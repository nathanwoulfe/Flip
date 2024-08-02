import {
  DocumentTypeResponseModel,
  PermittedTypeResponseModel,
  TypeService,
} from "@flip/generated";
import { UMB_DOCUMENT_TYPE_PICKER_MODAL } from "@umbraco-cms/backoffice/document-type";
import {
  css,
  customElement,
  html,
  property,
  state,
  when,
} from "@umbraco-cms/backoffice/external/lit";
import { UmbLitElement } from "@umbraco-cms/backoffice/lit-element";
import { UMB_MODAL_MANAGER_CONTEXT } from "@umbraco-cms/backoffice/modal";

const elementName = "flip-target-type-selector";

@customElement(elementName)
export class FlipTargetTypeSelectorElement extends UmbLitElement {
  @property()
  unique?: string | null;

  @property({ type: Object })
  current?: DocumentTypeResponseModel;

  @state()
  private _permittedTypes?: Array<PermittedTypeResponseModel>;

  @state()
  value?: PermittedTypeResponseModel;

  async connectedCallback() {
    super.connectedCallback();

    await this.#getPermittedTypes();
  }

  async #getPermittedTypes() {
    if (!this.unique) return;

    this._permittedTypes = await TypeService.getTypePermitted({
      unique: this.unique,
    });

    if (this._permittedTypes.length === 1) {
      this.value = this._permittedTypes.at(0);
      this.dispatchEvent(new CustomEvent("change"));
    }
  }

  async #chooseType() {
    const modalContext = await this.getContext(UMB_MODAL_MANAGER_CONTEXT);
    const modalRef = modalContext.open(this, UMB_DOCUMENT_TYPE_PICKER_MODAL, {
      data: {
        pickableFilter: (item) =>
          this._permittedTypes?.some((x) => x.unique === item.unique) ?? false,
      },
    });

    await modalRef.onSubmit().catch(() => undefined);
    const { selection } = modalRef.getValue();

    this.value = this._permittedTypes?.find((x) => x.unique === selection[0]);
    this.dispatchEvent(new CustomEvent("change"));
  }

  #removeType() {
    this.value = undefined;
    this.dispatchEvent(new CustomEvent("clear"));
  }

  render() {
    return html`<div id="current-to-new">
        <uui-ref-node-document-type
          standalone
          .name=${this.current?.name ?? ""}
          .alias=${this.current?.alias ?? ""}
        >
          <umb-icon slot="icon" .name=${this.current?.icon}></umb-icon>
        </uui-ref-node-document-type>
        <umb-icon name="icon-arrow-right"></umb-icon>
        ${when(
          this.value,
          () => html` <uui-ref-node-document-type
            standalone
            .name=${this.value?.name ?? ""}
            .alias=${this.value?.alias ?? ""}
          >
            <umb-icon slot="icon" .name=${this.value?.icon}></umb-icon>
            <uui-action-bar slot="actions">
              <uui-button
                label=${this.localize.term("general_remove")}
                @click=${this.#removeType}
              ></uui-button>
            </uui-action-bar>
          </uui-ref-node-document-type>`,
          () =>
            html`<uui-button
              @click=${this.#chooseType}
              look="primary"
              ?disabled=${!this._permittedTypes?.length}
              .label=${"Select new document type"}
            ></uui-button>`
        )}
      </div>
      ${when(
        !this._permittedTypes?.length,
        () => html`<div id="alert">
          <umb-icon name="alert"></umb-icon>
          ${this.localize.term("flip_noPermittedTypes")}
        </div>`
      )}`;
  }

  static styles = css`
    #current-to-new {
      display: flex;
      column-gap: var(--uui-size-5);
    }

    #current-to-new uui-button,
    #current-to-new uui-ref-node-document-type {
      flex: 1;
    }

    #alert {
      display: flex;
      align-items: center;
      padding: var(--uui-size-3) var(--uui-size-4);
      margin-top: var(--uui-size-5);
      border-radius: var(--uui-border-radius);
      background: var(--uui-color-current);
    }

    #alert umb-icon {
      margin-right: var(--uui-size-3);
    }

    .flex {
      display: flex;
    }

    umb-icon {
      font-size: var(--uui-size-8);
    }
  `;
}

export default FlipTargetTypeSelectorElement;

declare global {
  interface HTMLElementTagMap {
    [elementName]: FlipTargetTypeSelectorElement;
  }
}
