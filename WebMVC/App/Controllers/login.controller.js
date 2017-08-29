/**
*   Source File:		login.controller.js
*   Property of Autoconcept All rights reserved.
*
*   Description:        login controller functionality
*
*   Date		  Author/(Reviewer)		Description
*   -------------------------------------------------------	
*   5 May 2015    Chamara Bandara	    Creation 
*
*/


var loginControllers = angular.module('loginControllers', ['ipCookie', 'ngCookies', 'LocalStorageModule']);

loginControllers.controller('LoginCtrl', ['$scope', '$http', '$cookies', '$cookieStore', '$location',  'ipCookie', 'localStorageService', function ($scope, $http, $cookies, $cookieStore, $location, ipCookie, localStorageService) {

    $scope.Login = function (login) {
        $scope.submitted = true;

        if ($scope.loginForm.$invalid == false) {

            var loginData = {
                grant_type: 'password',
                username: login.loginUsername,
                password: login.loginPassword,
                client_id: 'HavaApp'
            };
            //window.location.href = appUrl;

            $http.post(appUrl + '/Token', $.param(loginData)).
            success(function (data, status, headers, config) {
                console.log(data);
                var expireTime = Date.now() + parseInt(data.refreshToken_timeout) * 60000;
                localStorageService.set('accessToken', data.access_token);
                localStorageService.set('refreshToken', data.refresh_token);
                localStorageService.set('refreshTokenTimeOut', parseInt(data.refreshToken_timeout));
                localStorageService.set('refreshOn', expireTime);
                var redirectUrl = location.href;
                var a = redirectUrl.indexOf("ref=");
                if (redirectUrl.indexOf("ref=") < 0) {
                    window.location.href = appUrl;
                } else {
                    window.location.href = redirectUrl.substring(redirectUrl.indexOf("ref=") + 4);
                }

                
            }).
            error(function (data, status, headers, config) {
                $scope.invalidUserNamePassword = true;
            });

        } else {
        }
    }
}]);