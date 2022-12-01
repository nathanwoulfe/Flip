export class FlipDialogController {

    public static controllerName = 'Flip.Dialog.Controller';

    loaded: boolean = false;
    permittedTypes: Array<any> = [];
    node: any;
    newType: any;
    mapType: 'EDITOR' | 'DATATYPE' = 'DATATYPE';
    newTemplateId!: number;

    newProperties = {};

    mapTypeChange: Function;

    constructor(
        private flipResource: IFlipResource,
        public $scope,
        private navigationService,
        private $window) {
        this.mapTypeChange = $scope.$watch(() => this.mapType, () => {
            this.newType ? this.getNewTypePropertyCollection() : {};
        });
    }

    $onInit() {
        this.flipResource.getContentModel(this.$scope.currentNode.id)
            .then(node => {
                this.node = node;
                this.getPermittedTypes();
            })
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
    getNewTypePropertyCollection(setsNewType: boolean = false) {
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
            ...this.newType.PropertyGroups.map(x => x.PropertyTypes).flat()];

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

            existingProperty = existingProperty ?? this.node.properties.find(p => this.mapType === 'DATATYPE' ? p.dataTypeKey === type.DataTypeKey : p.editor === type.PropertyEditorAlias);

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