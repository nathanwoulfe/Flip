import { FlipResource } from './js/flip.resource';
import { FlipDialogController } from './js/flip.controller';

const ServicesModule = angular.module('flip.services', [])
    .service(FlipResource.serviceName, FlipResource).name;

const ControllersModule = angular.module('flip.controllers', [])
    .controller(FlipDialogController.controllerName, FlipDialogController).name;

const flip = 'flip';

angular.module(flip, [
    ServicesModule,
    ControllersModule,
]);

angular.module('umbraco').requires.push(flip);