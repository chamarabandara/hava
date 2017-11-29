
var site = angular.module('Sites',
    ['ui.router', 
    'LocalStorageModule',
    'ipCookie',
       // 'BookingCreateCtrl',
       // 'BookingCtrl',
        ///'BookingHistoryCtrl',
        'ngSanitize', //'mgcrea.ngStrap',
        'siteService',
        'ngCookies']
    );
site.constant('_START_REQUEST_', '_START_REQUEST_');
site.constant('_END_REQUEST_', '_END_REQUEST_');
site.config(['$stateProvider', '$httpProvider', '$urlRouterProvider', '_START_REQUEST_', '_END_REQUEST_', function ($stateProvider, $httpProvider, $urlRouterProvider, _START_REQUEST_, _END_REQUEST_) {
    //  $httpProvider.defaults.headers.post['Content-Type'] = 'application/x-www-form-urlencoded;charset=utf-8';
    console.log('test');
    var $http,
    interceptor = ['$q', '$injector', function ($q, $injector) {
        $urlRouterProvider.otherwise('/error404');

        var rootScope;

        var stateParams = $injector.get('$stateParams');
        return {
            request: function (config) {
                // get $http via $injector because of circular dependency problem
                $http = $http || $injector.get('$http');
                var reqUrl = config.url.split('/');
                var lastString = reqUrl[reqUrl.length - 1];
                //// don't send notification until all requests are complete
                if ($http.pendingRequests.length < 1 && lastString != 'Validate') {
                    // && apiControllerName != 'common' && apiControllerName != 'GetPeopleForNavBar'
                    // get $rootScope via $injector because of circular dependency problem
                    rootScope = rootScope || $injector.get('$rootScope');
                    // send a notification requests are complete
                    if (!rootScope.eventStartd) {
                        rootScope.$broadcast(_START_REQUEST_);

                    }
                    //rootScope.test = true;
                }
                return config;
            },
            response: function (response) {
                // get $http via $injector because of circular dependency problem
                $http = $http || $injector.get('$http');
                // don't send notification until all requests are complete

                // get $rootScope via $injector because of circular dependency problem
                rootScope = rootScope || $injector.get('$rootScope');

                if ($http.pendingRequests.length < 1) {

                    rootScope.$broadcast(_END_REQUEST_);

                } else {
                    rootScope.$broadcast(_START_REQUEST_);
                }

                return response;
            }
        }
    }];
}]);
site.run(['$http', '$cookies', '$state', '$location', '$window', '$rootScope', '$timeout','localStorageService', function ($http, $cookies, $state, $location, $window, $rootScope, $timeout, localStorageService) {
    console.log("site app");
    //$rootScope.hideTiles = true;
    //$rootScope.hideTilesLink = false;
    //$rootScope.hideStats = false;
    //$rootScope.locationChanged = true;
    ////  console.log(loginUrl);
    var loginURL = loginUrl;
    ////  console.log(loginURL);
    //// var loginURL = $cookies.appUrl+'Home/Login';
    cookieToken = localStorageService.get('accessToken');
    if (cookieToken) {
        $http.defaults.headers.common['Authorization'] = 'Bearer ' + cookieToken;
        //$http.get(apiUrl + '/Account/IsExistingUser?userName=" "').
        //              success(function (data, status, headers, config) {
        //                  //  console.log('success');
        //              }).
        //              error(function (data, status, headers, config) {
        //                  if (status == 405) {
        //                      $window.location.href = loginURL + '?ref=' + window.location.href;
        //                  }
        //              });
    } else {
       // $window.location.href = loginURL + '?ref=' + window.location.href;
    }
}]);

(function (module) {
    var fileReader = function ($q, $log) {
        var onLoad = function (reader, deferred, scope) {
            return function () {
                scope.$apply(function () {
                    deferred.resolve(reader.result);
                });
            };
        };

        var onError = function (reader, deferred, scope) {
            return function () {
                scope.$apply(function () {
                    deferred.reject(reader.result);
                });
            };
        };

        var onProgress = function (reader, scope) {
            return function (event) {
                scope.$broadcast("fileProgress",
                    {
                        total: event.total,
                        loaded: event.loaded
                    });
            };
        };

        var getReader = function (deferred, scope) {
            var reader = new FileReader();
            reader.onload = onLoad(reader, deferred, scope);
            reader.onerror = onError(reader, deferred, scope);
            reader.onprogress = onProgress(reader, scope);
            return reader;
        };

        var readAsDataURL = function (file, scope) {
            var deferred = $q.defer();

            var reader = getReader(deferred, scope);
            reader.readAsDataURL(file);

            return deferred.promise;
        };

        return {
            readAsDataUrl: readAsDataURL
        };
    };

    module.factory("fileReader",
                   ["$q", "$log", fileReader]);

}(angular.module("Sites")));

