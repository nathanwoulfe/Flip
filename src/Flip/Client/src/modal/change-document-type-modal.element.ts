import {
  css,
  customElement,
  html,
  state,
  when,
} from "@umbraco-cms/backoffice/external/lit";
import { UmbModalBaseElement } from "@umbraco-cms/backoffice/modal";
import type {
  ChangeDocumentTypeModalData,
  ChangeDocumentTypeModalValue,
} from "./change-document-type-modal.token";
import { tryExecute } from "@umbraco-cms/backoffice/resources";
import {
  FlipService,
  type ChangeDocumentTypeModel,
  type ContentTypeModel,
  type DocumentTypePropertyModel,
} from "../../generated";

type MapType = "DATATYPE" | "EDITOR";

const elementName = "change-document-type-modal";

@customElement(elementName)
export class ChangeDocumentTypeModalElement extends UmbModalBaseElement<
  ChangeDocumentTypeModalData,
  ChangeDocumentTypeModalValue
> {
  @state()
  private _isLoading = false;

  @state()
  private _permittedTypes: ContentTypeModel[] = [];

  @state()
  private _targetType?: ContentTypeModel;

  @state()
  private _entity: ChangeDocumentTypeModel | null = null;

  @state()
  private _newProperties: Record<string, DocumentTypePropertyModel[]> = {};

  @state()
  private _mapType: MapType = "DATATYPE";

  #newTemplateId?: number;

  async connectedCallback() {
    super.connectedCallback();

    this._isLoading = true;

    await this.#getContentModel();
    await this.#getPermitted();

    this._isLoading = false;
  }

  async #getContentModel() {
    const { data } = await tryExecute(
      this,
      FlipService.getContentModel({
        query: { unique: this.data?.document.unique?.toString() },
      })
    );

    this._entity = data ?? null;
  }

  async #getPermitted() {
    const { data } = await tryExecute(
      this,
      FlipService.getPermitted({
        query: { unique: this.data?.document.unique?.toString() },
      })
    );

    this._permittedTypes = data ?? [];
    this._isLoading = false;

    if (this._permittedTypes.length === 1) {
      this._targetType = this._permittedTypes[0];
      this.#getPropertyCollection(true);
    }
  }

  #onSelectedTypeChange(event: Event) {
    this._targetType = this._permittedTypes.find(
      (type) => type.unique === (event.target as HTMLSelectElement).value
    );

    this.#getPropertyCollection(true);
  }

  #onMapTypeChange(event: Event) {
    this._mapType = (event.target as HTMLInputElement).value as MapType;

    if (this._targetType) {
      this.#getPropertyCollection();
    }
  }

  #getPropertyCollection(setsNewType = false) {
    if (!this._entity) return;
    this._newProperties = {};

    if (setsNewType) {
      this.#newTemplateId = this._targetType?.defaultTemplateId;
    }

    this._entity.properties?.forEach((prop) => {
      prop.newAlias = "";
    });

    this._targetType?.propertyTypes.forEach((propType) => {
      const key =
        this._mapType === "DATATYPE"
          ? propType.dataTypeKey
          : propType.propertyEditorAlias;
      if (!key) return;

      if (!this._newProperties[key]) {
        this._newProperties[key] = [
          { dataTypeKey: "", editor: "", alias: "", label: "" },
        ];
      }

      this._newProperties[key]?.push({
        dataTypeKey: propType.dataTypeKey,
        editor: propType.propertyEditorAlias,
        alias: propType.alias,
        label: propType.name,
      });

      // also check that the current type has a matching property - match on datatype key
      // to only allow matches on the exact data type, not the property editor as config may differ
      // if so, set newAlias on the current type to ensure the value is mapped on save

      // look for exact match by alias first, then check for broader match
      const mapMatch = (p: DocumentTypePropertyModel) =>
        this._mapType === "DATATYPE"
          ? p.dataTypeKey === propType.dataTypeKey
          : p.editor === propType.propertyEditorAlias;

      const existingProperty =
        this._entity!.properties?.find(
          (p) => p.alias === propType.alias && mapMatch(p)
        ) ?? this._entity!.properties?.find((p) => mapMatch(p));

      if (existingProperty) {
        existingProperty.newAlias = propType.alias;
      }
    });
  }

  #getPropertiesByType(p: DocumentTypePropertyModel) {
    return (
      this._newProperties[
        this._mapType === "DATATYPE" ? p.dataTypeKey! : p.editor!
      ] ?? []
    );
  }

  #handleSubmit() {
    this.updateValue({
      contentTypeUnique: this._targetType!.unique!,
      templateId: this.#newTemplateId,
      properties: this._entity?.properties ?? [],
    });

    this._submitModal();
  }

  #renderTable() {
    return html`<umb-table
      .config=${{ allowSelection: false, hideIcon: true }}
      .columns=${[
        {
          alias: "current",
          name: this.localize.term("flip_currentProperty"),
        },
        {
          alias: "new",
          name: this.localize.term("flip_newProperty"),
        },
      ]}
      .items=${this._entity?.properties?.map((p) => {
        const props = this.#getPropertiesByType(p);

        return {
          id: p.alias!,
          data: [
            { columnAlias: "current", value: p.label },
            {
              columnAlias: "new",
              value: html`<uui-select
                .options=${props.map((o) => ({
                  name: o.label!,
                  value: o.alias!,
                  selected: o.alias === p.newAlias,
                })) ?? []}
                @change=${(event: Event) => {
                  p.newAlias = (event.target as HTMLSelectElement).value;
                }}
                ?disabled=${!props.length}
              ></uui-select>`,
            },
          ],
        };
      }) ?? []}
    >
    </umb-table>`;
  }

  #render() {
    return html` ${when(
      !this._permittedTypes.length,
      () =>
        html` <uui-box
          >${this.localize.term("flip_noPermittedTypes")}</uui-box
        >`,
      () => html`<uui-box .headline=${this.localize.term("general_settings")}
          ><umb-property-layout .label=${this.localize.term("flip_newType")}>
            <uui-select
              slot="editor"
              ?disabled=${this._permittedTypes.length === 1}
              .options=${this._permittedTypes.map((type) => ({
                name: type.name!,
                value: type.unique!,
                selected: type.unique === this._targetType?.unique,
              }))}
              @change=${this.#onSelectedTypeChange}
            ></uui-select>
          </umb-property-layout>
          ${when(
            this._targetType,
            (targetType) => html` <umb-property-layout
                .label=${this.localize.term("flip_newTemplate")}
              >
                <uui-select
                  slot="editor"
                  ?disabled=${targetType.allowedTemplates.length === 1}
                  .options=${targetType.allowedTemplates.map((type) => ({
                    name: type.name!,
                    value: type.id.toString(),
                    selected: type.id === this.#newTemplateId,
                  }))}
                ></uui-select>
              </umb-property-layout>
              <umb-property-layout .label=${this.localize.term("flip_mapType")}>
                <div slot="editor">
                  <uui-radio-group
                    @change=${this.#onMapTypeChange}
                    .value=${this._mapType}
                  >
                    <uui-radio
                      label="Data Type"
                      value=${"DATATYPE"}
                    ></uui-radio>
                    <uui-radio
                      label="Property Editor"
                      value=${"EDITOR"}
                    ></uui-radio>
                  </uui-radio-group>
                </div>
              </umb-property-layout>
              <small>${this.localize.term("flip_mapTypeInstruction")}</small>`
          )}
        </uui-box>

        ${when(
          this._targetType,
          () => html`<uui-box
            .headline=${this.localize.term("flip_mapProperties")}
          >
            <small
              >${this.localize.term("flip_mapPropertiesInstruction")}</small
            >
            ${this.#renderTable()}</uui-box
          >`
        )} `
    )}`;
  }

  render() {
    return html`<umb-body-layout
      .headline=${this.localize.term("flip_changeDocumentType")}
    >
      ${when(
        this._isLoading,
        () => html`<umb-loader></umb-loader>`,
        () => this.#render()
      )}
      <div slot="actions">
        <uui-button
          label=${this.localize.term("general_close")}
          @click=${this._rejectModal}
        ></uui-button>
        <uui-button
          color="positive"
          look="primary"
          label=${this.localize.term("general_submit")}
          @click=${this.#handleSubmit}
        ></uui-button>
      </div>
    </umb-body-layout>`;
  }

  static styles = css`
    uui-box + uui-box {
      margin-top: var(--uui-size-5);
    }

    umb-table {
      display: block;
      margin-top: var(--uui-size-5);
    }
  `;
}

export default ChangeDocumentTypeModalElement;

declare global {
  interface HTMLElementTagNameMap {
    [elementName]: ChangeDocumentTypeModalElement;
  }
}
