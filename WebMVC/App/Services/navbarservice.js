var navBarService = angular.module('navBarService', ['ngResource']);

navBarService.factory('HavaNavBarService', ['$resource', function ($resource) {
    return $resource(appUrl + 'api/Home', {}, {
         GetMenues: { method: 'GET', url: appUrl + 'Home/Menues' },

    });
}]);

navBarService.factory('PartnerServiceLocal', function () {
    return {};
});
