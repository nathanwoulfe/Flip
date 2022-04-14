interface IFlipResource {
    getPermittedTypes: (nodeId: number) => Promise<any>;
    changeContentType: (nodeId: number, contentTypeId: number, templateId: number, properties: Array<IDocumentTypeProperty>) => Promise<any>;
}

interface IDocumentTypeProperty {
    label: string;
    alias: string;
    editor: string;
    dataTypeKey: string;
    value: any;
    newAlias?: string;
}