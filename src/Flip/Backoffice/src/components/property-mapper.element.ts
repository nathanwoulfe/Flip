import { DocumentTypePropertyResponseModel } from "@flip/generated";
import {
  customElement,
  html,
  property,
  state,
} from "@umbraco-cms/backoffice/external/lit";
import { UmbLitElement } from "@umbraco-cms/backoffice/lit-element";
import { UUISelectEvent } from "@umbraco-cms/backoffice/external/uui";
import type { PropertyModel, MapType } from "@flip/entities";

const elementName = "flip-property-mapper";

@customElement(elementName)
export class FlipPropertyMapperElement extends UmbLitElement {
  @property({ type: Array })
  current?: Array<DocumentTypePropertyResponseModel>;

  @property({ type: Object })
  new: Record<string, Array<PropertyModel>> = {};

  @property()
  mapType?: MapType;

  @state()
  value?: DocumentTypePropertyResponseModel;

  #onPropertyMapChange(
    event: UUISelectEvent,
    prop: DocumentTypePropertyResponseModel
  ) {
    prop.newAlias = event.target.value.toString();
    this.value = prop;
    this.dispatchEvent(new CustomEvent("change"));
  }

  render() {
    return html`<h4>${this.localize.term("flip_mapProperties")}</h4>
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
        ${this.current?.map((prop) => {
          const properties =
            this.new[
              this.mapType === "DATATYPE" ? prop.dataTypeKey! : prop.editor!
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
      </uui-table>`;
  }
}

export default FlipPropertyMapperElement;

declare global {
  interface HTMLElementTagMap {
    [elementName]: FlipPropertyMapperElement;
  }
}
