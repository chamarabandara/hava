var partnerService = angular.module('siteService', ['ngResource']);

partnerService.factory('HavaSiteService', ['$resource', function ($resource) {
    return $resource(apiUrl + '/Sites', {}, {

        createSites: { method: 'POST', url: appUrl + 'Sites/Post' },
        create: { method: 'POST', url: appUrl + 'Partner/Post' },
        getProduct: { method: 'GET', url: appUrl + 'Partner/GetProductList' },
        getLocations: { method: 'GET', url: appUrl + 'locationDetails/GetAllByPartnerId', params: { id: '@id' },isArray: false  },
        getProductDetails: { method: 'GET', url: appUrl + 'booking/GetProducts', params: { partnerId: '@partnerId', locationId: '@locationId' } },
    });
}]);

partnerService.factory('SiteServiceLocal', function () {
    return {};
});
