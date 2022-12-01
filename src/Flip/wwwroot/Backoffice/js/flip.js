(function(){function r(e,n,t){function o(i,f){if(!n[i]){if(!e[i]){var c="function"==typeof require&&require;if(!f&&c)return c(i,!0);if(u)return u(i,!0);var a=new Error("Cannot find module '"+i+"'");throw a.code="MODULE_NOT_FOUND",a}var p=n[i]={exports:{}};e[i][0].call(p.exports,function(r){var n=e[i][1][r];return o(n||r)},p,p.exports,r,e,n,t)}return n[i].exports}for(var u="function"==typeof require&&require,i=0;i<t.length;i++)o(t[i]);return o}return r})()({1:[function(require,module,exports){
"use strict";
Object.defineProperty(exports, "__esModule", { value: true });
const flip_resource_1 = require("./js/flip.resource");
const flip_controller_1 = require("./js/flip.controller");
const ServicesModule = angular.module('flip.services', [])
    .service(flip_resource_1.FlipResource.serviceName, flip_resource_1.FlipResource).name;
const ControllersModule = angular.module('flip.controllers', [])
    .controller(flip_controller_1.FlipDialogController.controllerName, flip_controller_1.FlipDialogController).name;
const flip = 'flip';
angular.module(flip, [
    ServicesModule,
    ControllersModule,
]);
angular.module('umbraco').requires.push(flip);
},{"./js/flip.controller":2,"./js/flip.resource":3}],2:[function(require,module,exports){
"use strict";
Object.defineProperty(exports, "__esModule", { value: true });
exports.FlipDialogController = void 0;
class FlipDialogController {
    constructor(flipResource, $scope, navigationService, $window) {
        this.flipResource = flipResource;
        this.$scope = $scope;
        this.navigationService = navigationService;
        this.$window = $window;
        this.loaded = false;
        this.permittedTypes = [];
        this.mapType = 'DATATYPE';
        this.newProperties = {};
        this.mapTypeChange = $scope.$watch(() => this.mapType, () => {
            this.newType ? this.getNewTypePropertyCollection() : {};
        });
    }
    $onInit() {
        this.flipResource.getContentModel(this.$scope.currentNode.id)
            .then(node => {
            this.node = node;
            this.getPermittedTypes();
        });
    }
    $onDestroy() {
        this.mapTypeChange();
    }
    /**
     *
     * */
    getPermittedTypes() {
        this.flipResource.getPermittedTypes(this.$scope.currentNode.id)
            .then(result => {
            this.permittedTypes = result.permittedTypes;
            this.loaded = true;
            if (this.permittedTypes.length === 1) {
                this.newType = this.permittedTypes[0];
                this.getNewTypePropertyCollection(true);
            }
        });
    }
    /**
     *
     * */
    getNewTypePropertyCollection(setsNewType = false) {
        this.newProperties = {};
        if (setsNewType) {
            this.newTemplateId = this.newType.DefaultTemplateId;
        }
        // clear to ensure no stale data after changing type or mapType
        this.node.properties.forEach(prop => {
            prop.newAlias = '';
        });
        // iterates over explict and composed properties
        const propertyTypes = [
            ...this.newType.ContentTypeComposition.map(x => x.PropertyGroups.map(y => y.PropertyTypes).flat()).flat(),
            ...this.newType.PropertyGroups.map(x => x.PropertyTypes).flat()
        ];
        propertyTypes.forEach(type => {
            const propertyKey = this.mapType === 'DATATYPE' ? type.DataTypeKey : type.PropertyEditorAlias;
            if (!this.newProperties[propertyKey]) {
                this.newProperties[propertyKey] = [{ dataTypeKey: '', editor: '', alias: '', label: '' }];
            }
            this.newProperties[propertyKey].push({
                dataTypeKey: type.DataTypeKey,
                editor: type.PropertyEditorAlias,
                alias: type.Alias,
                label: type.Name,
            });
            // also check that the current type has a matching property - match on datatype key
            // to only allow matches on the exact data type, not the property editor as config may differ
            // if so, set newAlias on the current type to ensure the value is mapped on save
            // look for exact match by alias first, then check for broader match
            let existingProperty = this.node.properties.find(p => p.alias === type.Alias &&
                (this.mapType === 'DATATYPE' ? p.dataTypeKey === type.DataTypeKey : p.editor === type.PropertyEditorAlias));
            existingProperty = existingProperty !== null && existingProperty !== void 0 ? existingProperty : this.node.properties.find(p => this.mapType === 'DATATYPE' ? p.dataTypeKey === type.DataTypeKey : p.editor === type.PropertyEditorAlias);
            if (existingProperty) {
                existingProperty.newAlias = type.Alias;
            }
        });
    }
    /**
     *
     * */
    close() {
        this.navigationService.hideDialog();
    }
    /**
     *
     * */
    save() {
        this.flipResource.changeContentType(this.$scope.currentNode.id, this.newType.Id, this.newTemplateId || this.newType.DefaultTemplateId, this.node.properties)
            .then(result => {
            this.$window.location.reload();
        });
    }
}
exports.FlipDialogController = FlipDialogController;
FlipDialogController.controllerName = 'Flip.Dialog.Controller';
},{}],3:[function(require,module,exports){
"use strict";
Object.defineProperty(exports, "__esModule", { value: true });
exports.FlipResource = void 0;
class FlipResource {
    constructor($http, umbRequestHelper) {
        this.$http = $http;
        this.umbRequestHelper = umbRequestHelper;
        this._apiBaseUrl = Umbraco.Sys.ServerVariables.Flip.apiBaseUrl;
        this._request = (method, url, errorMessage = 'Something broke', data) => this.umbRequestHelper.resourcePromise(method === 'POST' ? this.$http.post(this._apiBaseUrl + url, data) : this.$http.get(this._apiBaseUrl + url), errorMessage);
        this.getPermittedTypes = nodeId => this._request('GET', `GetPermittedTypes?nodeId=${nodeId}`, 'Failed to retrieve permitted types for node id' + nodeId);
        this.changeContentType = (nodeId, contentTypeId, templateId, properties) => this._request('POST', 'ChangeContentType', 'Failed to change document type for node id ' + nodeId, { nodeId, contentTypeId, templateId, properties });
        this.getContentModel = (nodeId) => this._request('GET', `GetContentModel?nodeId=${nodeId}`, 'Failed to get content type for node id ' + nodeId);
    }
}
exports.FlipResource = FlipResource;
FlipResource.serviceName = 'flipResource';
},{}]},{},[1,2,3])

//# sourceMappingURL=flip.js.map