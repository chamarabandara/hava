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

var sitesControllers = angular.module('Sites', ['siteService', 'ui.router']);

sitesControllers.controller('BookingCreateCtrl', ['$scope', '$http', 'HavaSiteService', '$state', '$stateParams', function ($scope, $http, HavaSiteService, $state, $stateParams) {
  
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
    $scope.locations = [{ 'id': 1, 'name': "Nuwara Eliya", 'PartnerId': 1003, 'IsActive': true }, { 'id': 2, "name": "Jaffna includes 2 Days & 1 Night for returnrn", 'PartnerId': 1003, 'IsActive': true}];

    //HavaSiteService.getLocations({ 'id': 1003 }).$promise.then(
    //         function (result) {
    //             // angular.forEach(result);
    //           //  console.log(JSON.parse(result.data));
    //           //  var dt = JSON.parse(result.data);
    //             $scope.locations = [{ "Id": 1, "name": "Nuwara Eliya rn", "PartnerId": 1003, "IsActive": true, "FromLocation": "Colombo", "ToLocation": "Nuwara Eliya rn" }, { "Id": 2, "name": "Jaffna includes 2 Days & 1 Night for returnrn", "PartnerId": 1003, "IsActive": true, "FromLocation": "Colombo ", "ToLocation": "Jaffna" }];
    //         });
     // $scope.locations = [{ 'id': 1, 'name': 'test' },{ 'id': 1, 'name': 'test' }];
    $scope.isMain = true;

    $scope.programeList = [{ 'name': "Asia Miles", id: 1 }, { 'name': "Air France Flying Blue" }];

    $scope.titleList = [{ 'name': "Mr", id: 1 }, { 'name': "Mrs.",id:2 }];
    $scope.searchBooking = function (model) {
        $scope.submitted = true;
        if ($('#searchTextField').val() != "" && $scope.dropLocation != undefined && $scope.dropLocation)
        {
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
            console.log(urlparms);

            HavaSiteService.getProductDetails({ 'partnerId': parseInt(urlparms.P), 'locationId': $scope.search.dropLocation.id }).$promise.then(
                     function (result) {
                         // angular.forEach(result);
                         console.log(JSON.parse(result.data.replace(/'/g, '"')));

                         //  var dt = JSON.parse(result.data);
                         $scope.locations = [{ "Id": 1, "name": "Nuwara Eliya rn", "PartnerId": 1003, "IsActive": true, "FromLocation": "Colombo", "ToLocation": "Nuwara Eliya rn" }, { "Id": 2, "name": "Jaffna includes 2 Days & 1 Night for returnrn", "PartnerId": 1003, "IsActive": true, "FromLocation": "Colombo ", "ToLocation": "Jaffna" }];
                     });
        }else
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

    $scope.selectDate = function (id) {
        setTimeout(function () {
            $('#' + id).trigger('focus');
        }, 50);
    }

    $scope.ValidateFlight = function (users) {

    }

    angular.element(document).ready(function () {
        //$(function () {
        //    $("#datepicker").datepicker();
        //});
        $('#timepicker').timepicker({});
        $('#timepicker2').timepicker({});

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