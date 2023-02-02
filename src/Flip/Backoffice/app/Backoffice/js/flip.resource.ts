import { IHttpService } from "angular";

export class FlipResource implements IFlipResource {

    public static serviceName = 'flipResource';

    private _apiBaseUrl = Umbraco.Sys.ServerVariables.Flip.apiBaseUrl;

    constructor(private $http: IHttpService, private umbRequestHelper) { }

    private _request = (method: 'POST' | 'GET', url: string, errorMessage: string = 'Something broke', data?: object) =>
        this.umbRequestHelper.resourcePromise(
                method === 'POST' ? this.$http.post(this._apiBaseUrl + url, data) : this.$http.get(this._apiBaseUrl + url),
            errorMessage);

    getPermittedTypes = nodeId => this._request('GET', `GetPermittedTypes?nodeId=${nodeId}`, 'Failed to retrieve permitted types for node id' + nodeId);

    changeContentType = (nodeId: number, contentTypeId: number, templateId: number, properties: Array<IDocumentTypeProperty>) =>
        this._request('POST', 'ChangeContentType', 'Failed to change document type for node id ' + nodeId, { nodeId, contentTypeId, templateId, properties });
   
    getContentModel = (nodeId: number) =>
        this._request('GET', `GetContentModel?nodeId=${nodeId}`, 'Failed to get content type for node id ' + nodeId);
}