﻿var partnerService = angular.module('tspService', ['ngResource']);

partnerService.factory('HavaTSPService', ['$resource', function ($resource) {
    return $resource(apiUrl + '/TSP', {}, {
        query: { method: 'GET', url: apiUrl + 'api/Invoice/GetAllAdminInvoices', params: { pageNumber: '@pageNumber', sortOrder: '@sortOrder', sortColumn: '@sortColumn', searchData: '@searchData', isActive: '@isActive', pageSize: '@pageSize', isInvoiced: '@isInvoiced', invoiceDate: '@invoiceDate', period: '@period' }, isArray: false },
        queryToBeInvoice: { method: 'GET', url: apiUrl + 'api/Invoice/GetAllAdminInvoices', params: { pageNumber: '@pageNumber', sortOrder: '@sortOrder', sortColumn: '@sortColumn', searchData: '@searchData', isActive: '@isActive', pageSize: '@pageSize', isInvoiced: '@isInvoiced', invoiceDate: '@invoiceDate', period: '@period' }, isArray: false },
        changeInvoicedAdminInvoice: { method: 'POST', url: apiUrl + 'api/Invoice/ChangeInvoicedAdminInvoice' },
        cancelInvoice: { method: 'GET', url: apiUrl + 'api/Invoice/CancelAdminInvoice', params: { id: '@id' } },
        getAdminCancelPopupData: { method: 'GET', url: apiUrl + 'api/Invoice/GetAdminCancelPopupData', params: { id: '@id' } },
        cancelAdminInvoiceByModel: { method: 'POST', url: apiUrl + 'api/Invoice/CancelAdminInvoiceByModel' },
        GetInvoiceInitial: { method: 'GET', url: apiUrl + 'api/Invoice/GetInvoiceInitial' },
        createProduct: { method: 'POST', url: appUrl + 'Product/Post' },
        create: { method: 'POST', url: appUrl + 'TSP/AddTSP' },
        getProduct: { method: 'GET', url: appUrl + 'TSP/GetProducts' },
        getTSP: { method: 'GET', url: appUrl + 'TSP/GetTSPById', params: { id: '@id' } },
        updateTSP:{ method: 'POST', url: appUrl + 'TSP/EditTSP' },
        delete:{ method: 'POST', url: appUrl + 'TSP/DeleteTSP' ,params: { id: '@id' },}
    });
}]);

partnerService.factory('TSPServiceLocal', function () {
    return {};
});
