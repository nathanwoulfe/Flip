import {
  css,
  customElement,
  html,
  state,
  when,
} from "@umbraco-cms/backoffice/external/lit";
import {
  UMB_MODAL_MANAGER_CONTEXT,
  UmbModalBaseElement,
} from "@umbraco-cms/backoffice/modal";
import {
  FlipChangeDocumentTypeModalData,
  FlipChangeDocumentTypeModalValue,
} from "./flip-modal.token.js";
import {
  ContentModelResponseModel,
  ContentService,
  DocumentTypePropertyResponseModel,
  PermittedTypeResponseModel,
  TypeService,
} from "@flip/generated";
import { UmbPropertyValueData } from "@umbraco-cms/backoffice/property";
import {
  UUIRadioGroupElement,
  UUIRadioGroupEvent,
  UUISelectEvent,
} from "@umbraco-cms/backoffice/external/uui";
import { UMB_DOCUMENT_TYPE_PICKER_MODAL } from "@umbraco-cms/backoffice/document-type";

interface PropertyModel {
  label?: string | null;
  alias?: string | null;
  dataTypeKey: string;
  editor?: string | null;
}

const elementName = "flip-change-document-type-modal";

@customElement(elementName)
export class FlipChangeDocumentTypeModalElement extends UmbModalBaseElement<
  FlipChangeDocumentTypeModalData,
  FlipChangeDocumentTypeModalValue
> {
  @state()
  private _contentModel?: ContentModelResponseModel;

  @state()
  private _permittedTypes?: Array<PermittedTypeResponseModel>;

  @state()
  private _newType?: PermittedTypeResponseModel;

  @state()
  private _newProperties: Record<string, Array<PropertyModel>> = {};

  @state()
  private _newTemplateId: UmbPropertyValueData = {
    alias: "newTemplateId",
  };

  @state()
  private _newDocumentType: UmbPropertyValueData = {
    alias: "newDocumentType",
  };

  @state()
  private _mapType: "DATATYPE" | "EDITOR" = "DATATYPE";

  async connectedCallback() {
    super.connectedCallback();

    await this.#getContentModel();
    await this.#getPermittedTypes();
  }

  #submitModal() {
    if (!this._contentModel) return;

    this.modalContext?.setValue({
      unique: this._contentModel.unique,
      documentTypeUnique: this._newType?.unique ?? "",
      templateId:
        Number(this._newTemplateId.value) ?? this._newType?.defaultTemplateId,
      properties: this._contentModel.properties ?? [],
    });

    this._submitModal();
  }

  async #getContentModel() {
    if (!this.data?.document.unique) return;
    this._contentModel = await ContentService.getContent({
      unique: this.data.document.unique,
    });
  }

  async #getPermittedTypes() {
    if (!this.data?.document.unique) return;
    this._permittedTypes = await TypeService.getTypePermitted({
      unique: this.data.document.unique,
    });
    if (this._permittedTypes.length === 1) {
      this._newType = this._permittedTypes.at(0);
      this._newDocumentType.value = this._newType?.unique;
      this.#getNewTypePropertyCollection(true);
    }
  }

  #getNewTypePropertyCollection(setsNewType: boolean) {
    this._newProperties = {};

    if (setsNewType) {
      this._newTemplateId.value = this._newType?.defaultTemplateId;
    }

    // clear to ensure no stale data after changing type or mapType
    this._contentModel?.properties?.forEach((prop) => {
      prop.newAlias = "";
    });

    this._newType?.propertyTypes.forEach((type) => {
      const propertyKey =
        this._mapType === "DATATYPE"
          ? type.dataTypeKey
          : type.propertyEditorAlias;

      if (!propertyKey) return;

      if (!this._newProperties[propertyKey]) {
        this._newProperties[propertyKey] = [
          { dataTypeKey: "", editor: "", alias: "", label: "" },
        ];
      }

      this._newProperties[propertyKey].push({
        dataTypeKey: type.dataTypeKey,
        editor: type.propertyEditorAlias,
        alias: type.alias,
        label: type.name,
      });

      // also check that the current type has a matching property - match on datatype key
      // to only allow matches on the exact data type, not the property editor as config may differ
      // if so, set newAlias on the current type to ensure the value is mapped on save

      let existingProperty = this._contentModel?.properties?.find(
        (p) =>
          p.alias === type.alias &&
          (this._mapType === "DATATYPE"
            ? p.dataTypeKey === type.dataTypeKey
            : p.editor === type.propertyEditorAlias)
      );

      if (existingProperty) {
        existingProperty.newAlias = type.alias;
      }
    });
  }

  #onMapTypeChange(event: UUIRadioGroupEvent) {
    const value = (event.target as UUIRadioGroupElement).value.toString();
    if (!value) return;

    this._mapType = value as any;
    if (this._newType) this.#getNewTypePropertyCollection(false);
  }

  #onPropertyMapChange(
    event: UUISelectEvent,
    prop: DocumentTypePropertyResponseModel
  ) {
    const value = event.target.value.toString();
    prop.newAlias = value;
  }

  async #chooseType() {
    const permittedTypes = this._permittedTypes ?? [];
    const modalContext = await this.getContext(UMB_MODAL_MANAGER_CONTEXT);
    const modalRef = modalContext.open(this, UMB_DOCUMENT_TYPE_PICKER_MODAL, {
      data: {
        multiple: false,
        createAction: undefined,
        pickableFilter(item) {
          return permittedTypes.some((x) => x.unique === item.unique);
        },
      },
    });

    await modalRef.onSubmit().catch(() => undefined);
    const { selection } = modalRef.getValue();

    this._newType = this._permittedTypes?.find(
      (x) => x.unique === selection[0]
    );

    this.#getNewTypePropertyCollection(true);
  }

  #removeNewType() {
    this._newType = undefined;
  }

  render() {
    if (!this._contentModel) return;
    return html`<umb-body-layout
      .headline=${this.localize.term("flip_changeDocumentType")}
    >
      <div id="main">
        <uui-box>
          <div id="current-to-new">
            <uui-ref-node-document-type
              standalone
              .name=${this._contentModel.documentType.name!}
              .alias=${this._contentModel.documentType.alias!}
            >
              <umb-icon
                slot="icon"
                .name=${this._contentModel.documentType.icon}
              ></umb-icon>
            </uui-ref-node-document-type>
            <umb-icon name="icon-arrow-right"></umb-icon>
            ${when(
              this._newType,
              () => html` <uui-ref-node-document-type
                standalone
                .name=${this._newType?.name ?? ""}
                .alias=${this._newType?.alias ?? ""}
              >
                <umb-icon slot="icon" .name=${this._newType?.icon}></umb-icon>
                <uui-action-bar slot="actions">
                  <uui-button
                    label=${this.localize.term("general_remove")}
                    @click=${this.#removeNewType}
                  ></uui-button>
                </uui-action-bar>
              </uui-ref-node-document-type>`,
              () =>
                html`<uui-button
                  @click=${this.#chooseType}
                  look="primary"
                  .label=${"Select new document type"}
                ></uui-button>`
            )}
          </div>
          ${when(
            !this._permittedTypes?.length,
            () => html`<div class="alert alert-info">
              ${this.localize.term("flip_noPermittedTypes")}
            </div>`
          )}
          ${when(
            (this._newType?.allowedTemplates.length ?? 0) > 1,
            () => html`<umb-property-dataset .value=${[this._newTemplateId]}>
              <umb-property
                property-editor-ui-alias="Umb.PropertyEditorUi.Select"
                label="New template"
                alias=${this._newTemplateId.alias}
                .config=${[
                  {
                    alias: "items",
                    value:
                      this._newType?.allowedTemplates?.map((x) => ({
                        name: x.name,
                        value: x.id,
                      })) ?? [],
                  },
                ]}
              ></umb-property>
            </umb-property-dataset>`
          )}
          ${when(
            this._newType && this._newProperties,
            () => html`<div>
              <h4>${this.localize.term("flip_mapType")}</h4>
              <p>${this.localize.term("flip_mapTypeInstruction")}</p>

              <uui-radio-group
                name="mapType"
                @change=${this.#onMapTypeChange}
                .value=${this._mapType}
              >
                <uui-radio value="DATATYPE">Data Type</uui-radio>
                <uui-radio value="EDITOR">Property Editor</uui-radio>
              </uui-radio-group>

              <h4>${this.localize.term("flip_mapProperties")}</h4>
              <p>${this.localize.term("flip_mapPropertiesInstruction")}</p>

              <uui-table>
                <uui-table-head>
                  <uui-table-head-cell>
                    ${this.localize.term("flip_currentProperty")}
                  </uui-table-head-cell>
                  <uui-table-head-cell>
                    ${this.localize.term("flip_newProperty")}
                  </uui-table-head-cell>
                </uui-table-head>
                ${this._contentModel?.properties?.map((prop) => {
                  const properties =
                    this._newProperties[
                      this._mapType === "DATATYPE"
                        ? prop.dataTypeKey!
                        : prop.editor!
                    ];

                  const options =
                    properties?.map((p) => ({
                      name: p.label!,
                      value: p.alias!,
                      selected: p.alias === prop.newAlias,
                    })) ?? [];

                  return html`<uui-table-row>
                    <uui-table-cell>${prop.label}</uui-table-cell>
                    <uui-table-cell>
                      <uui-select
                        @change=${(e: UUISelectEvent) =>
                          this.#onPropertyMapChange(e, prop)}
                        ?disabled=${!properties}
                        .options=${options}
                      ></uui-select>
                    </uui-table-cell>
                  </uui-table-row>`;
                })}
              </uui-table>
            </div>`
          )}
        </uui-box>
      </div>
      <div slot="actions">
        <uui-button
          label=${this.localize.term("general_close")}
          @click=${this._rejectModal}
        ></uui-button>
        <uui-button
          color="positive"
          look="primary"
          label=${this.localize.term("general_submit")}
          ?disabled=${!this._newType}
          @click=${this.#submitModal}
        ></uui-button>
      </div>
    </umb-body-layout>`;
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

    .flex {
      display: flex;
    }

    umb-icon {
      font-size: var(--uui-size-8);
    }
  `;
}

export default FlipChangeDocumentTypeModalElement;

declare global {
  interface HTMLElementTagMap {
    [elementName]: FlipChangeDocumentTypeModalElement;
  }
}
