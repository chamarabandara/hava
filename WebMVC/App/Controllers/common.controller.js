app.controller('CommonController', ['$scope', '$http', '$cookies', '$cookieStore', '$interval',  'ipCookie', 'localStorageService', '$window', function ($scope, $http, $cookies, $cookieStore, $interval,  ipCookie, localStorageService, $window) {
    //login URL
    $scope.loginURL = loginUrl;

    //refresh token
    //$scope.refreshToken = function () {
    //    localStorageService.set('requestSent', false);
    //    var accessToken = localStorageService.get('accessToken');
    //    var refreshToken = localStorageService.get('refreshToken');
    //    var refreshTokenTimeOut = localStorageService.get('refreshTokenTimeOut');
    //    var refresh_time = localStorageService.get('refreshOn') - Date.now();
    //    var requestSent = localStorageService.get('requestSent');
    //    if (refresh_time > 0) {

    //        $interval(function () {
    //            if (accessToken) {
    //                if (requestSent === "false") {
    //                    localStorageService.set('requestSent', true);
    //                    if (localStorageService.get('acc_token'))
    //                        $http.defaults.headers.common['Authorization'] = localStorageService.get('acc_token');
    //                    AutoConceptCommonService.refreshToken($.param({ client_secret: 'secret', grant_type: 'refresh_token', client_id: 'AutoConcept', refresh_token: ((localStorageService.get('refre_token')) ? localStorageService.get('refre_token') : localStorageService.get('refreshToken')) })).$promise.then(function (data) {
    //                        //remove 
    //                        localStorageService.remove('accessToken');
    //                        localStorageService.remove('refreshToken');
    //                        localStorageService.remove('refreshTokenTimeOut');
    //                        localStorageService.remove('refreshOn');
                           
    //                        localStorageService.remove('acc_token');
    //                        localStorageService.remove('refre_token');
    //                        //add
    //                        localStorageService.set('accessToken', data.access_token);
    //                        localStorageService.set('refreshToken', data.refresh_token);
    //                       // ipCookie('accessToken', data.access_token, { path: '/' });
    //                       // ipCookie('refreshToken', data.refresh_token, { path: '/' });
    //                        localStorageService.set('refre_token', data.refresh_token);

    //                        ipCookie('refreshTokenTimeOut', parseInt(data.refreshToken_timeout), { path: '/' });
    //                        var expireTime = Date.now() + parseInt(data.refreshToken_timeout) * 60000;
    //                        ipCookie('refreshOn', expireTime, { path: '/' });

    //                        localStorageService.set('requestSent', false);

    //                        var capitaliseFirstLetter = function (string) {
    //                            return string.charAt(0).toUpperCase() + string.slice(1);
    //                        }
    //                        localStorageService.set('acc_token', capitaliseFirstLetter(data.token_type) + ' ' + data.access_token);
    //                        $http.defaults.headers.common['Authorization'] = capitaliseFirstLetter(data.token_type) + ' ' + data.access_token;
    //                    });
    //                }
    //            }
    //        }, refresh_time);

    //    }
    //}

    //$scope.refreshToken();

    //logout function
    $scope.LogOut = function () {
        localStorageService.remove('accessToken');
        localStorageService.remove('refreshToken');
        localStorageService.remove('refreshToken');
        localStorageService.remove('refreshTokenTimeOut');
        localStorageService.remove('refreshOn');

        localStorageService.remove('queries');
        window.location.href = $scope.loginURL;
    }

    //$scope.onExit = function () {
    //    AutoConceptCommonService.cancelInvoice().$promise.then(function (result) {

    //    });
    //};

    //$window.onbeforeunload = $scope.onExit();
}]);