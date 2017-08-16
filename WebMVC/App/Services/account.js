var roleService = angular.module('roleService', ['ngResource']);

roleService.factory('AutoConceptAdministration', ['$resource', function ($resource) {
    return $resource(apiUrl + '/api/Account/', {}, {
        query: { method: 'GET', params: { pageNumber: '@pageNumber', sortOrder: '@sortOrder', sortColumn: '@sortColumn', searchData: '@searchData', isDeleted: '@isDeleted', pageSize: '@pageSize', institutionId: '@institutionId' }, isArray: false },
        getUser: { method: 'GET', url: apiUrl + '/api/Account/User', params: { id: '@id' }, isArray: false },
        isExistingUser: { method: 'GET', url: apiUrl + '/api/Account/IsExistingUser', params: { userName: '@userName' }, isArray: false },
        isExistingSignature: { method: 'GET', url: apiUrl + '/api/Account/IsExistingSignature', params: { userName: '@userName', id: '@id' }, isArray: false },
        editUser: { method: 'POST', url: apiUrl + '/api/Account/EditUser', params: {}, isArray: false },
        getUsers: { method: 'GET', url: apiUrl + '/api/Account/Users', isArray: true },
        getRoles: { method: 'GET', url: apiUrl + '/api/Account/ActiveRoles', isArray: true },
        getUserRoles: { method: 'GET', url: apiUrl + '/api/Account/UserRoles', params: { user: '@user' }, isArray: false },
        // TODO remove this
        getPermissions: { method: 'GET', url: apiUrl + '/api/Account/Permissions', isArray: true },
        getModulePermissions: { method: 'GET', url: apiUrl + '/api/Account/ModulePermissions', isArray: true },
        roleDataQuery: { method: 'GET', url: apiUrl + '/api/Account/RoleList', params: { pageNumber: '@pageNumber', sortOrder: '@sortOrder', sortColumn: '@sortColumn', searchData: '@searchData', isDeleted: '@isDeleted', pageSize: '@pageSize', institutionId: '@institutionId' }, isArray: false },
        getRoleModulePermissions: { method: 'GET', url: apiUrl + '/api/Account/RoleModulePermissions', params: { role: '@role' }, isArray: false },
        getRolePermissions: { method: 'GET', url: apiUrl + '/api/Account/RolePermissions', params: { role: '@role' }, isArray: true },
        deleteRole: { method: 'DELETE', url: apiUrl + '/api/Account/DeleteRole/', params: { roleID: '@roleID' }, isArray: false },
        update: { method: 'PUT', params: {} },

    });
}]);
roleService.factory('userLocal', function () {
    return {};
});