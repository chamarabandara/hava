﻿var partnerService = angular.module('partnerService', ['ngResource']);

partnerService.factory('HavaPartnerService', ['$resource', function ($resource) {
    return $resource(apiUrl + '/Product', {}, {
        query: { method: 'GET', url: apiUrl + 'api/Invoice/GetAllAdminInvoices', params: { pageNumber: '@pageNumber', sortOrder: '@sortOrder', sortColumn: '@sortColumn', searchData: '@searchData', isActive: '@isActive', pageSize: '@pageSize', isInvoiced: '@isInvoiced', invoiceDate: '@invoiceDate', period: '@period' }, isArray: false },
        queryToBeInvoice: { method: 'GET', url: apiUrl + 'api/Invoice/GetAllAdminInvoices', params: { pageNumber: '@pageNumber', sortOrder: '@sortOrder', sortColumn: '@sortColumn', searchData: '@searchData', isActive: '@isActive', pageSize: '@pageSize', isInvoiced: '@isInvoiced', invoiceDate: '@invoiceDate', period: '@period' }, isArray: false },
        changeInvoicedAdminInvoice: { method: 'POST', url: apiUrl + 'api/Invoice/ChangeInvoicedAdminInvoice' },
        cancelInvoice: { method: 'GET', url: apiUrl + 'api/Invoice/CancelAdminInvoice', params: { id: '@id' } },
        getAdminCancelPopupData: { method: 'GET', url: apiUrl + 'api/Invoice/GetAdminCancelPopupData', params: { id: '@id' } },
        cancelAdminInvoiceByModel: { method: 'POST', url: apiUrl + 'api/Invoice/CancelAdminInvoiceByModel' },
        GetInvoiceInitial: { method: 'GET', url: apiUrl + 'api/Invoice/GetInvoiceInitial' },
        createProduct: { method: 'POST', url: appUrl + 'Product/Post' },
        create: { method: 'POST', url: appUrl + 'Partner/Post' },
        getProduct: { method: 'GET', url: appUrl + 'Partner/GetProductList' },
        getSites: { method: 'GET', url: appUrl + 'Partner/GetSites' },
        getPartner: { method: 'GET', url: appUrl + 'Partner/GetPartnerById', params: { id: '@id' } },
        updatePartner: { method: 'POST', url: appUrl + 'Partner/EditPartner' },
        delete: { method: 'POST', url: appUrl + 'Partner/DeletePartner', params: { id: '@id' }, },
        getMainProducts: { method: 'GET', url: appUrl + 'booking/GetMainProducts' },
        getSubProducts: { method: 'GET', url: appUrl + 'booking/GetSubProducts' },
        getAllLocations: { method: 'GET', url: appUrl + 'LocationDetails/GetAll' },
        getProduct: { method: 'GET', url: appUrl + 'Product/GetProductById', params: { id: '@id' } },
    });
}]);

partnerService.factory('PartnerServiceLocal', function () {
    return {};
});
