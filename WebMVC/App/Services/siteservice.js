var partnerService = angular.module('siteService', ['ngResource']);

partnerService.factory('HavaSiteService', ['$resource', function ($resource) {
    return $resource(apiUrl + '/Sites', {}, {

        createSites: { method: 'POST', url: appUrl + 'Sites/Post' },
        create: { method: 'POST', url: appUrl + 'Partner/Post' },
        createBooking: { method: 'POST', url: appUrl + '/Booking/Insert' },
        createUser: { method: 'POST', url: appUrl + 'api/Account/CreateAppUser' },
        getProduct: { method: 'GET', url: appUrl + 'Partner/GetProductList' },
        getCardTypes: { method: 'GET', url: appUrl + 'booking/CardTypes', isArray: false },
        getLocations: { method: 'GET', url: appUrl + 'locationDetails/GetAllByPartnerId', params: { id: '@id' },isArray: false  },
        getProductDetails: { method: 'GET', url: appUrl + 'booking/GetProducts', params: { partnerId: '@partnerId', locationId: '@locationId', PromotionCode: '@PromotionCode' } },
        getPartnerIstest: { method: 'GET', url: appUrl + 'Partner/GetPartnerSite', params: { partnerId: '@partnerId', siteId: '@siteId' }, }
        

    });
}]);

partnerService.factory('SiteServiceLocal', function () {
    return {};
});
