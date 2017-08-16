/// <summary>
/// Source File:		app.js
/// Property ofAutoConcept All rights reserved.
///
/// Description:		Angular Application 
/// </summary>
/// <remarks>
/// Modification History:	
/// Date		Author/(Reviewer)		Description
/// -------------------------------------------------------	
/// 29 April 2015	Chamara	            Modify
/// </remarks>

//login module
//login module
var login = angular.module('login',
    ['ui.router',
        'loginControllers',
    ]
    );
//var site = angular.module('site',
//    ['ui.router',
//        'loginControllers',
//    ]
//    );
//other application modules

var app = angular.module('app',
    [
        'ui.router',
      //  'ui.sortable',
        'ipCookie',
       // 'ngGrid',
        'ngCookies',
        //'LocalStorageModule',
       // 'angular-datepicker',
       // 'commonService',
        'oc.lazyLoad',
       // 'ngRateIt',
    ]
    );
//site.controller('LoginController', ['$scope', function ($scope) {
//    $scope.getGooglePlaceAddress = function () {

//        var colomboBounds = new google.maps.LatLngBounds(
//            //6.927079, 79.861244.
//                new google.maps.LatLng(6.84000, 79.86000),
//                   new google.maps.LatLng(6.95194, 79.93778));

//        //var kalutharaBounds = new google.maps.LatLngBounds(

//        //       new google.maps.LatLng(6.5854, 79.9607),
//        //          new google.maps.LatLng(6.5854, 79.9607));

//        //var optionskal = {
//        //    bounds: kalutharaBounds,
//        //    strictBounds: true,
//        //    componentRestrictions: { country: 'LK' }
//        //};

//        var optionscol = {
//            bounds: colomboBounds,
//            strictBounds: true,
//            componentRestrictions: { country: 'LK' }
//        };

       
//        //var map;
//        //map = new google.maps.Map(document.getElementById('map'), {
//        //    center: pyrmont,
//        //    zoom: 15
//        //});

//        //var request = {
//        //    location: pyrmont,
//        //    radius: '500',
//        //    types: ['store']
//        //};

//        places = new google.maps.places.Autocomplete(document.getElementById('main_page_transfer_pickup'), optionscol);
//        google.maps.event.addListener(places, 'place_changed', function () {
//            var place = places.getPlace();
//            var address = place.formatted_address;
//            var latitude = place.geometry.location.lat();
//            var longitude = place.geometry.location.lng();
//            var mesg = "Address: " + address;
//            mesg += "\nLatitude: " + latitude;
//            mesg += "\nLongitude: " + longitude;
//            console.log(mesg);
//           // alert(mesg);
//        });
//    //    places.bindTo
//        console.log(places);
//      //  $scope.gPlace = new google.maps.places.Autocomplete(element[0], options);
//    }
//    $scope.getGooglePlaceAddress();
//}]);


app.run(['$http', '$cookies', '$state', '$location', '$window', '$rootScope', '$timeout', function ($http, $cookies, $state, $location, $window, $rootScope, $timeout) {

    //$rootScope.hideTiles = true;
    //$rootScope.hideTilesLink = false;
    //$rootScope.hideStats = false;
    //$rootScope.locationChanged = true;
    ////  console.log(loginUrl);
    //var loginURL = loginUrl;
    ////  console.log(loginURL);
    //// var loginURL = $cookies.appUrl+'Home/Login';
    //cookieToken = localStorageService.get('accessToken');
    // cookieToken = $cookies.accessToken;
    //console.log('test');
    //if (cookieToken) {
    //    $http.defaults.headers.common['Authorization'] = 'Bearer ' + cookieToken;
    //    $http.get(apiUrl + '/api/Account/IsExistingUser?userName=" "').
    //                  success(function (data, status, headers, config) {
    //                      //  console.log('success');
    //                  }).
    //                  error(function (data, status, headers, config) {
    //                      if (status == 405) {
    //                          $window.location.href = loginURL + '?ref=' + window.location.href;
    //                      }
    //                  });
    //} else {
    //    $window.location.href = loginURL + '?ref=' + window.location.href;
    //}



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

}(angular.module("app")));