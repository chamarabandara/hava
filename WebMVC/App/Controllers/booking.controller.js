﻿/**
*   Source File:		            dealer.controller.js

*   Property of Hava All rights reserved.

*   Description:                    Dealer functionalities
*
*   Date		    Author/(Reviewer)		Description
*   -------------------------------------------------------	
*   17 Aug 2017    Chamara Bandara	        Creation 
*
*/

var sitesControllers = angular.module('Sites', ['siteService', 'ui.router']);

sitesControllers.controller('BookingCreateCtrl', ['$scope', '$http', 'HavaSiteService', '$state', '$stateParams', '$timeout', function ($scope, $http, HavaSiteService, $state, $stateParams, $timeout) {

    $scope.search = {};

    $scope.parseQueryString = function (url) {
        var urlParams = {};
        url.replace(
          new RegExp("([^?=&]+)(=([^&]*))?", "g"),
          function ($0, $1, $2, $3) {
              urlParams[$1] = $3;
          }
        );

        return urlParams;
    }

    $scope.urlparms = $scope.parseQueryString(window.location.href);
    $scope.paramData = $scope.urlparms.P.split('S');
    $scope.PartnerIdTemp = $scope.paramData[0];
    $scope.siteIdTemp = $scope.paramData[1];

    HavaSiteService.getLocations({ id: parseInt($scope.PartnerIdTemp) }).$promise.then(
                 function (result) {
                     $scope.locations = result.data;
                 });

    HavaSiteService.getPartnerIstest({ partnerId: parseInt($scope.PartnerIdTemp), siteId: parseInt($scope.siteIdTemp) }).$promise.then(
             function (result) {
                 $scope.siteDetails = result.data;
             });
    $scope.isMain = true;
    $scope.durantions = [
        { 'name': "1 Day", id: 1 },
        { 'name': "2 Days", id: 2 },
        { 'name': "3 Days", id: 3 }, 
        { 'name': "4 Days", id: 4 },
        { 'name': "5 Days", id: 5 }
    ];
    $scope.programeList = [{ 'name': "Asia Miles", id: 1 }, { 'name': "Air France Flying Blue" }];

    $scope.titleList = [{ 'name': "Mr", id: 1 }, { 'name': "Mrs.", id: 2 }];
    $scope.searchBooking = function (model) {
        $scope.submitted = true;
        if ($('#searchTextField').val() != "" && (($scope.dropLocation != undefined && $scope.dropLocation) || $scope.duration != undefined)) {
            $scope.isMain = false;

            $scope.search.pickupLocation = $('#searchTextField').val();
            $scope.search.dropLocation = $scope.dropLocation;

            if ($('#inputGroupSuccessDate').val() != "")
                $scope.search.pickupDate = $('#inputGroupSuccessDate').val();
            else
                $scope.search.pickupDate = $('#inputGroupSuccessDate2').val();
            if ($('#timepicker').val() != "")
                $scope.search.pickupTime = $('#timepicker').val();
            else
                $scope.search.pickupTime = $('#timepicker2').val();

            var urlparms = $scope.parseQueryString(window.location.href);
          
            HavaSiteService.getProductDetails({ partnerId: parseInt(PartnerIdTemp), locationId: ($scope.search.dropLocation != undefined) ? $scope.search.dropLocation.Id : 0, PromotionCode: ($scope.promotionCode != undefined) ? $scope.promotionCode : 0 }).$promise.then(
                     function (result) {
                        $scope.Products = result.data;
                         console.log(result.data);
                     });
        } else
            return true;

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

    $scope.setBookingProduct = function (product) {
        $scope.selectedProduct = product;
        $scope.totalSellingPrice = angular.copy($scope.selectedProduct.PartnerSellingPrice);

    }
    //calculate price
    $scope.calculateTotal = function (hours,sets) {
        var hrs = (hours != undefined && hours != "")?parseInt(hours):0;
        var st = (sets != undefined && sets != "") ? parseInt(sets) : 0;
        var hrsTotal= 0, stTotal=0, partPrice = 0;
        var hrsTotal = (hrs * parseInt($scope.selectedProduct.AdditionaHourRate));
        var partPrice = parseInt(angular.copy($scope.selectedProduct.PartnerSellingPrice));
        var stTotal = ($scope.selectedProduct.ChildSeatRate != null)?(st * parseInt($scope.selectedProduct.ChildSeatRate)):0;
        $scope.totalSellingPrice = partPrice + hrsTotal + stTotal;
    }


    $scope.selectDate = function (id) {
        setTimeout(function () {
            $('#' + id).trigger('focus');
        }, 50);
    }

    $scope.ValidateFlight = function (users) {

    }

  

    $scope.disabledButton = true;

    $scope.isTermsAcepted = function (status) {
        if (status == true) {
            $scope.disabledButton = false;
        } else {
            $scope.disabledButton = true;
        }
    }

    $scope.createUser = function (user) {
        $scope.userNameRequired = false;
        $scope.firstNameRequired = false;
        $scope.submitted = true;
        if (user.UserName != undefined && user.UserName != "" && user.FirstName != undefined && user.FirstName != "" && 
            user.Password != undefined && user.Password != "") {
            var data = angular.copy(user);
           delete data.isTermsAcepted;
            HavaSiteService.createUser(data, function (data) {
                if (data.data > 0) {
                    $scope.UserId = data.data;
                    $scope.navigateSteps(4);
                } else {
                   
                }
            })
        } else {
            if (user.UserName == undefined || user.UserName == "")
                $scope.userNameRequired = true;
            if (user.FirstName == undefined || user.FirstName == "")
                $scope.firstNameRequired = true;
            if (user.Password != undefined || user.Password != "")
                $scope.PasswordRequired = true;
        }
       

    }
    $scope.saveBookingOptions = function (optionData) {
        $scope.bookingOptionData = optionData;
        $scope.navigateSteps(3);
    }

    $scope.creditCardDetails = function (booking) {
        $scope.CardHolderNameRequired = false;
        $scope.CardNoRequired = false;
        if (booking.CardHolderName != undefined && booking.CardHolderName != "" && booking.CardNo != undefined && booking.CardNo != "") {
            $scope.selectedProduct.Partner.Id = parseInt($scope.urlparms.P);
            var data = {
                "Id":0,
                "BookingType": {
                    "id":1
                },
                "UserId": $scope.UserId,
                'BookingProducts': $scope.selectedProduct,
                "BookingStatu": {
                    "Id": 1,
                    "Name": "Pending",
                    "IsActive": true
                },
                "PickupLocation": $scope.search.pickupLocation,
                "PickupDate": $scope.search.pickupDate,
                "PickupTime": $scope.search.pickupTime,
                "DropLocation": $scope.dropLocation.Id,
                "BookingOptions": $scope.bookingOptionData,
                "BookingPayments": {
                    "CardHolderName": booking.CardHolderName,
                    "BookingPayments": booking.CardNo,
                    "ExpireDate": booking.ExpireDateMM + "/" + booking.ExpireDateYY
                },
                "UserConfirmed": true,
                "IsAirportTransfer": $scope.selectedProduct.LocationDetail.IsAirPortTour,
                "Partner": $scope.selectedProduct.Partner,
                "BookingType": {
                    "Id": 1,
                    "type": "Online",
                    "IsActive": true
                },
                "IsReturn":false
            }
            HavaSiteService.createBooking(data, function (data) {
                if (data.success == true) {
                    $scope.infoMsg = "Booking sucessfully created.";

                    $timeout(function () {
                        location.reload();
                    }, 2000);
                } else {

                }
            })
        } else {
            if (booking.CardHolderName == undefined || booking.CardHolderName == "")
                $scope.CardHolderNameRequired = true;
            if (booking.CardNo == undefined || booking.CardNo == "")
                $scope.CardNoRequired = true;
            
        }
    }


    angular.element(document).ready(function () {
        //$(function () {
        //    $("#datepicker").datepicker();
        //});
        $('#timepicker').timepicker({
            timeFormat: 'HH:mm:ss',
            showSecond: true,
            ampm: false
        });
        $('#timepicker2').timepicker({
            timeFormat: 'HH:mm:ss',
            showSecond: true,
            ampm: false
        });

        var dateToday = new Date();

        $('#inputGroupSuccessDate').datepicker({
            format: 'yyyy-mm-dd',
            endDate: dateToday,
            todayBtn: 'linked',
            autoclose: true,
        });
        $('#inputGroupSuccessDate2').datepicker({
            format: 'yyyy-mm-dd',
            endDate: dateToday,
            todayBtn: 'linked',
            autoclose: true,
        });
    });
}]);

sitesControllers.controller('BookingCtrl', ['$scope', '$http', 'HavaSiteService', '$stateParams', '$state', '$sce', '$window', '$timeout', function ($scope, $http, HavaSiteService, $stateParams, $state, $sce, $window, $timeout) {

    $scope.bindActionButtons = function (o) {
        var actionButtons = "";
        actionButtons = '<div class="ngCellText" ng-cell-text ng-class="col.colIndex()">';
        var viewButton = '<a data-dataId="' + o.id + '" class="link-action action-button" data-view="view" title="View"><i class="fa fa-eye"></i></a>';
        var editButton = '<a data-dataId="' + o.id + '" class="link-action action-button"  data-view="edit" title="Edit"><i class="access-link fa fa-pencil-square-o"></i></a>';
        var deleteButton = '<a data-dataId="' + o.id + '" class="link-action action-button" data-view="delete" title="Delete"><i class="fa fa-trash"></i></a>';
        actionButtons += viewButton + editButton;
        if (o.status != "Closed")
            actionButtons += deleteButton;
        actionButtons += '</div>';
        return actionButtons;
    }

    $('#datatable-booking').DataTable({
        //  "processing": true,
        //  "serverSide": true,
        'ajax': {
            'url': appUrl + 'Booking/GetBookingList',
            'type': 'GET',
            //'beforeSend': function (request) {
            //    //  $("#loadingWidget").css({ display: 'block' });
            //  //  headers.append('Access-Control-Allow-Origin', apiUrl);
            //  //  headers.append('Access-Control-Allow-Credentials', 'true');
            //    request.setRequestHeader('Access-Control-Allow-Origin', apiUrl);
            //    request.setRequestHeader('Access-Control-Allow-Credentials', true);
            //    //request.header('Access-Control-Allow-Origin', '*');
            //},
            contentType: 'application/json',

            "data": function (d) {
                return JSON.stringify(d);
            },
        },

        "aoColumns": [
             {
                 "data": "refNo", sWidth: "25%", "render": function (data, type, row, meta) {
                     return '<a data-view="view" data-dataId="' + row.rId + '">' + ((data != null) ? data : '<center>-</center>') + '</a>';
                 }
             },
             {
                 "data": "partner", sWidth: "25%", "render": function (data, type, row, meta) {
                     return '<a data-view="view" data-dataId="' + row.rId + '">' + ((data != null) ? data : '<center>-</center>') + '</a>';
                 }
             },
             {
                 "data": "bookingType", sWidth: "20%", "render": function (data, type, row, meta) {
                     return '<a data-view="view" data-dataId="' + row.rId + '">' + ((data != null) ? data : '<center>-</center>') + '</a>';
                 }
             },
             {
                 "data": "pickupDate", sWidth: "20%", "render": function (data, type, row, meta) {
                     return '<a data-view="view" data-dataId="' + row.rId + '">' + ((data != null) ? data : '<center>-</center>') + '</a>';
                 }
             },
             {
                 "data": "pickupTime", sWidth: "20%", "render": function (data, type, row, meta) {
                     return '<a data-view="view" data-dataId="' + row.rId + '">' + ((data != null) ? data : '<center>-</center>') + '</a>';
                 }
             },
             {
                 "data": "pickupLocation", sWidth: "20%", "render": function (data, type, row, meta) {
                     return '<a data-view="view" data-dataId="' + row.rId + '">' + ((data != null) ? data : '<center>-</center>') + '</a>';
                 }
             },
             {
                 "data": "returnDate", sWidth: "20%", "render": function (data, type, row, meta) {
                     return '<a data-view="view" data-dataId="' + row.rId + '">' + ((data != null) ? data : '<center>-</center>') + '</a>';
                 }
             },
             {
                 "data": "returnTime", sWidth: "20%", "render": function (data, type, row, meta) {
                     return '<a data-view="view" data-dataId="' + row.rId + '">' + ((data != null) ? data : '<center>-</center>') + '</a>';
                 }
             },
             {
                 "data": "dropLocation", sWidth: "20%", "render": function (data, type, row, meta) {
                     return '<a data-view="view" data-dataId="' + row.rId + '">' + ((data != null) ? data : '<center>-</center>') + '</a>';
                 }
             },
             {
                 "data": "bookingStatus", sWidth: "20%", "render": function (data, type, row, meta) {
                     return '<a data-view="view" data-dataId="' + row.rId + '">' + ((data != null) ? data : '<center>-</center>') + '</a>';
                 }
             },
            {
                "data": null,
                "bSortable": false,
                "mRender": function (o) {
                    return $scope.bindActionButtons(o);
                }
            }
        ]
    });

    $scope.action = function (row, task) {
        if (task == 'edit') {
            $state.go('^.update', { 'id': row.id });

        } else if (task == 'view') {
            //var url = $state.href('app.tsp', { 'id': row.id });
            //$window.open(url, '_blank');
            $state.go('^.view', { 'id': row.id });


        } else if (task == 'copy') {

        }
        else if (task == 'delete') {
            //$scope.claimRow = row;
            //console.log($scope.claimRow);
            //$scope.viewTask = 'confirmDelete';
            window.scrollTo(0, 0);

        }
    }


    angular.element(document).ready(function () {
        $('#claim-grid').on('click', '.search-cleardata', function (e) {
            e.preventDefault();
            $('#claim-grid .searchInputs:input').val('');
            $("#claim-grid").dataTable().fnDestroy();
            $scope.claimFilterFunction();
        });

        $('#datatable-partner').on('click', 'tbody tr td a', function () {
            var id = $(this).attr('data-dataId');
            var view = $(this).attr('data-view');
            $timeout(function () {
                $scope.action({ 'id': parseInt(id) }, view);
            }, 100);
        });
        //var dateToday = new Date();
        //$('.date .endDate').datepicker({
        //    format: 'yyyy-mm-dd',
        //    startDate: dateToday,
        //    todayBtn: 'linked',
        //    autoclose: true,
        //});

        //$('.date .startDate').datepicker({
        //    format: 'yyyy-mm-dd',
        //    todayBtn: 'linked',
        //    autoclose: true,
        //});
    });
}]);

sitesControllers.directive('onlyDigits', function ($filter) {
    return {
        require: 'ngModel',
        link: function (scope, element, attrs, modelCtrl) {
            modelCtrl.$parsers.push(function (inputValue) {

                if (inputValue == undefined) return ''
                var transformedInput = inputValue.replace(/[^0-9]/g, '');
                if (transformedInput != inputValue) {
                    modelCtrl.$setViewValue(transformedInput);
                    modelCtrl.$render();
                }

                return transformedInput;
            });
        }
    };
});