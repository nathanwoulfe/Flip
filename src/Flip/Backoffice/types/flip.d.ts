interface IFlipResource {
    getPermittedTypes: (nodeId: number) => Promise<{permittedTypes: Array<any>}>;
    changeContentType: (nodeId: number, contentTypeId: number, templateId: number, properties: Array<IDocumentTypeProperty>) => Promise<any>;
    getContentModel: (nodeId: number) => Promise<IContentType>;
}

interface IContentType {

}

interface IDocumentTypeProperty {
    label: string;
    alias: string;
    editor: string;
    dataTypeKey: string;
    value: any;
    newAlias?: string;
}