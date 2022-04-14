export class FlipDialogController {

    public static controllerName = 'Flip.Dialog.Controller';

    loaded: boolean = false;
    permittedTypes: Array<any> = [];
    node: any;
    nodeName!: string;
    newType: any;
    mapType: 'EDITOR' | 'DATATYPE' = 'DATATYPE';
    newTemplateId!: number;

    currentProperties: Array<IDocumentTypeProperty> = [];
    newProperties = {};

    mapTypeChange: Function;

    constructor(private flipResource: IFlipResource, public $scope, private navigationService, private contentResource, private $window) {
        this.mapTypeChange = $scope.$watch(() => this.mapType, () => {
            this.newType ? this.getNewTypePropertyCollection() : {};
        });
    }

    $onInit() {
        this.contentResource.getById(this.$scope.currentNode.id)
            .then(node => {
                this.node = node;
                this.nodeName = (this.node.variants.find(x => x.active) ?? this.node.variants[0]).name;

                this.getNodePropertyCollection();
                this.getPermittedTypes();
            });
    }

    $onDestroy() {
        this.mapTypeChange ? this.mapTypeChange() : {};
    }

    /**
     * 
     * */
    getPermittedTypes() {
        this.flipResource.getPermittedTypes(this.node.id)
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
    getNodePropertyCollection() {
        this.node.variants.forEach(variant => {
            variant.tabs.forEach(tab => {
                tab.properties.forEach(prop => {
                    this.currentProperties.push({
                        alias: prop.alias,
                        label: prop.label,
                        editor: prop.editor,
                        value: prop.value,
                        dataTypeKey: prop.dataTypeKey,
                    });
                });
            });
        });
    }

    /**
     * 
     * */
    getNewTypePropertyCollection(setsNewType: boolean = false) {
        this.newProperties = {};        

        if (setsNewType) {
            this.newTemplateId = this.newType.DefaultTemplateId;
        }

        // clear to ensure no stale data after changing type or mapType
        this.currentProperties.forEach(prop => {
            prop.newAlias = '';
        });

        this.newType.PropertyGroups.forEach(group => {
            group.PropertyTypes.forEach(type => {
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
                const existingProperty = this.currentProperties.find(p => this.mapType === 'DATATYPE' ? p.dataTypeKey === type.DataTypeKey : p.editor === type.PropertyEditorAlias);
                if (existingProperty != null) {
                    existingProperty.newAlias = type.Alias;
                }
            });
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
        this.flipResource.changeContentType(this.node.id, this.newType.Id, this.newTemplateId || this.newType.DefaultTemplateId, this.currentProperties)
            .then(result => {
                this.$window.location.reload();
            });
    }
}