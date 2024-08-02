import {
  customElement,
  html,
  property,
} from "@umbraco-cms/backoffice/external/lit";
import {
  UUIRadioGroupElement,
  UUIRadioGroupEvent,
} from "@umbraco-cms/backoffice/external/uui";
import { UmbLitElement } from "@umbraco-cms/backoffice/lit-element";
import type { MapType } from "@flip/entities";

const elementName = "flip-map-type-selector";


@customElement(elementName)
export class FlipMapTypeSelectorElement extends UmbLitElement {
  @property()
  value: MapType = "DATATYPE";

  #onValueChange(event: UUIRadioGroupEvent) {
    const value = (event.target as UUIRadioGroupElement).value.toString();
    if (!value) return;

    this.value = value as MapType;
    this.dispatchEvent(new CustomEvent("change"));
  }

  render() {
    return html`<h4>${this.localize.term("flip_mapType")}</h4>
      <p>${this.localize.term("flip_mapTypeInstruction")}</p>

      <uui-radio-group
        name="mapType"
        @change=${this.#onValueChange}
        .value=${this.value}
      >
        <uui-radio value="DATATYPE"
          >${this.localize.term("flip_dataType")}</uui-radio
        >
        <uui-radio value="EDITOR"
          >${this.localize.term("flip_propertyEditor")}</uui-radio
        >
      </uui-radio-group>`;
  }
}

export default FlipMapTypeSelectorElement;

declare global {
  interface HTMLElementTagMap {
    [elementName]: FlipMapTypeSelectorElement;
  }
}
