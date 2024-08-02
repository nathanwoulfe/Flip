export type MapType = "DATATYPE" | "EDITOR";

export interface PropertyModel {
    label?: string | null;
    alias?: string | null;
    dataTypeKey: string;
    editor?: string | null;
  }
  