var partnerService = angular.module('siteService', ['ngResource']);

partnerService.factory('HavaSiteService', ['$resource', function ($resource) {
    return $resource(apiUrl + '/Sites', {}, {
      
        createSites: { method: 'POST', url: appUrl + 'Sites/Post' },
        create: { method: 'POST', url: appUrl + 'Partner/Post' },
        getProduct: { method: 'GET', url: appUrl + 'Partner/GetProductList' },
        getLocations: { method: 'GET', url: appUrl + 'api/locationDetails/GetAllByPartnerId', params: { id: '@id' }, isArray: true },
    });
}]);

partnerService.factory('SiteServiceLocal', function () {
    return {};
});
