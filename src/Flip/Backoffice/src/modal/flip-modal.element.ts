import {
  customElement,
  html,
  state,
  when,
} from "@umbraco-cms/backoffice/external/lit";
import { UmbModalBaseElement } from "@umbraco-cms/backoffice/modal";
import {
  FlipChangeDocumentTypeModalData,
  FlipChangeDocumentTypeModalValue,
} from "./flip-modal.token.js";
import {
  ContentModelResponseModel,
  ContentService,
  PermittedTypeResponseModel,
} from "@flip/generated";
import {
  FlipTargetTypeSelectorElement,
  FlipMapTypeSelectorElement,
  FlipPropertyMapperElement,
} from "@flip/components";
import type { PropertyModel, MapType } from "@flip/entities";

const elementName = "flip-change-document-type-modal";

@customElement(elementName)
export class FlipChangeDocumentTypeModalElement extends UmbModalBaseElement<
  FlipChangeDocumentTypeModalData,
  FlipChangeDocumentTypeModalValue
> {
  @state()
  private _contentModel?: ContentModelResponseModel;

  @state()
  private _newType?: PermittedTypeResponseModel;

  @state()
  private _newProperties: Record<string, Array<PropertyModel>> = {};

  @state()
  private _mapType: MapType = "DATATYPE";

  async connectedCallback() {
    super.connectedCallback();

    await this.#getContentModel();
  }

  async #getContentModel() {
    if (!this.data?.document.unique) return;

    this._contentModel = await ContentService.getContent({
      unique: this.data.document.unique,
    });
  }

  #submitModal() {
    if (!this._contentModel) return;

    this.modalContext?.setValue({
      unique: this._contentModel.unique,
      documentTypeUnique: this._newType?.unique ?? "",
      templateId: this._newType?.defaultTemplateId,
      properties: this._contentModel.properties ?? [],
    });

    this._submitModal();
  }

  #getNewTypePropertyCollection() {
    this._newProperties = {};

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
        this._newProperties[propertyKey] = [];
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

  #onPropertyMapChange(event: CustomEvent) {
    const value = (event.target as FlipPropertyMapperElement).value;
    console.log(value);
  }

  #onTargetTypeClear() {
    this._newType = undefined;
    this.#getNewTypePropertyCollection();
  }

  #onTargetTypeChange(event: CustomEvent) {
    const value = (event.target as FlipTargetTypeSelectorElement).value;
    this._newType = value;
    this.#getNewTypePropertyCollection();
  }

  #onMapTypeChange(event: CustomEvent) {
    const value = (event.target as FlipMapTypeSelectorElement).value;
    this._mapType = value;
    this.#getNewTypePropertyCollection();
  }

  render() {
    if (!this._contentModel) return;

    return html`<umb-body-layout
      .headline=${this.localize.term("flip_changeDocumentType")}
    >
      <div id="main">
        <uui-box>
          <flip-target-type-selector
            .unique=${this.data?.document.unique}
            .current=${this._contentModel.documentType}
            @change=${this.#onTargetTypeChange}
            @clear=${this.#onTargetTypeClear}
          ></flip-target-type-selector>

          ${when(
            this._newType && this._newProperties,
            () => html`
              <flip-map-type-selector
                .value=${this._mapType}
                @change=${this.#onMapTypeChange}
              ></flip-map-type-selector>
              <flip-property-mapper
                .current=${this._contentModel?.properties}
                .new=${this._newProperties}
                .mapType=${this._mapType}
                @change=${this.#onPropertyMapChange}
              ></flip-property-mapper>
            `
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
}

export default FlipChangeDocumentTypeModalElement;

declare global {
  interface HTMLElementTagMap {
    [elementName]: FlipChangeDocumentTypeModalElement;
  }
}
