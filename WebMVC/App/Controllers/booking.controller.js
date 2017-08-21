/**
*   Source File:		            dealer.controller.js

*   Property of Hava All rights reserved.

*   Description:                    Dealer functionalities
*
*   Date		    Author/(Reviewer)		Description
*   -------------------------------------------------------	
*   17 Aug 2017    Chamara Bandara	        Creation 
*
*/

var sitesControllers = angular.module('Sites', ['siteService']);

sitesControllers.controller('BookingCreateCtrl', ['$scope', '$http', 'HavaSiteService', function ($scope, $http, HavaSiteService) {
  
    $scope.search = {};
    HavaSiteService.getLocations({ 'id': 1003 }).$promise.then(
             function (result) {
                // angular.forEach(result);
                 $scope.locations= JSON.parse(result);
             });
  //  $scope.locations = [{ 'id': 1, 'name': 'test' }];
    $scope.isMain = true;
    $scope.searchBooking = function (model) {
        $scope.isMain = false;
        $scope.pickupLocation = $('#searchTextField').val();
    }

    $scope.step = 1;
    $scope.navigateSteps = function (stp) {
        $scope.step = stp;

    }

    $scope.whatClassIsIt = function (st) {
        if (st == $scope.step)
            return "active"
        else
            return "";
    }

    $scope.selectDate = function (id) {
        setTimeout(function () {
            $('#' + id).trigger('focus');
        }, 50);
    }

    angular.element(document).ready(function () {
        //$(function () {
        //    $("#datepicker").datepicker();
        //});
        $('#timepicker').timepicker({});
        var dateToday = new Date();
        $('#inputGroupSuccessDate').datepicker({
        format: 'yyyy-mm-dd',
        endDate: dateToday,
        todayBtn: 'linked',
        autoclose: true,
    });
    });
}]);

