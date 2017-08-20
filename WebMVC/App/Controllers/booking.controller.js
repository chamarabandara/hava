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

var sitesControllers = angular.module('Sites', []);

sitesControllers.controller('BookingCreateCtrl', ['$scope', '$http', function ($scope, $http) {
  
    $scope.search = {};
    $scope.locations = [{ 'id': 1, 'name': 'test' }];
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

}]);

