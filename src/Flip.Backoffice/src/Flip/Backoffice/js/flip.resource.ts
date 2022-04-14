import { IHttpService, IQService } from "angular";

export class FlipResource implements IFlipResource {

    public static serviceName = 'flipResource';

    constructor(private $http: IHttpService, private $q: IQService, private umbRequestHelper) { }

    getPermittedTypes = nodeId =>
        this.umbRequestHelper.resourcePromise(
            this.$http.get(`${Umbraco.Sys.ServerVariables.Flip.apiBaseUrl}GetPermittedTypes?nodeId=${nodeId}`),
            'Failed to retrieve permitted types for content item id' + nodeId)
            .then(result => this.$q.when(result));

    changeContentType = (nodeId: number, contentTypeId: number, templateId: number, properties: Array<IDocumentTypeProperty>) =>
        this.umbRequestHelper.resourcePromise(
            this.$http.post(`${Umbraco.Sys.ServerVariables.Flip.apiBaseUrl}ChangeContentType`, { nodeId, contentTypeId, templateId, properties }),
            'Failed to change document type for content item id' + nodeId)
            .then(result => this.$q.when(result));

}