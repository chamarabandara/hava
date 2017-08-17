/**
*   Source File:		            tsp.controller.js

*   Property of Autoconcept All rights reserved.

*   Description:                    Dealer functionalities
*
*   Date		    Author/(Reviewer)		Description
*   -------------------------------------------------------	
*   10 July 2017    Chamara Bandara	        Creation 
*
*/

var tspControllers = angular.module('TSPControllers', ['ngCookies', 'tspService']);

tspControllers.controller('TSPCtrl', ['$scope', '$http', 'HavaTSPService', '$stateParams', '$state', '$sce', '$window', '$timeout', function ($scope, $http, HavaTSPService, $stateParams, $state, $sce, $window, $timeout) {
    $scope.create = function () {
        $scope.submitted = true;
        if ($scope.tspForm.$invalid == false) {
            $http.post(apiUrl + '/api/TSP/AddTSP', $scope.tsp).
            success(function (data, status, headers, config) {
                $scope.searchFunction();
                $scope.submitted = false;
                if (data.status) {
                    CustomerLocal.infoMsg = "Dealer " + $scope.dealer.companyDetails.orgName + " has been saved successfully.";
                    $state.go('app.customer.list');
                } else {
                    //dealerNotifications.infoMsg = "Dealer " + $scope.dealer.companyDetails.orgName + " has been saved successfully.";
                }

            });
        }
    }
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

    $('#datatable-partner').DataTable({
        //  "processing": true,
        //  "serverSide": true,
        'ajax': {
            'url': appUrl + 'TSP/GetList',
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
                 "data": "name", sWidth: "25%", "render": function (data, type, row, meta) {
                     return '<a data-view="view" data-dataId="' + row.rId + '">' + ((data != null) ? data : '<center>-</center>') + '</a>';
                 }
             },
             {
                 "data": "email", sWidth: "25%", "render": function (data, type, row, meta) {
                     return '<a data-view="view" data-dataId="' + row.rId + '">' + ((data != null) ? data : '<center>-</center>') + '</a>';
                 }
             },
             {
                 "data": "telephone", sWidth: "20%", "render": function (data, type, row, meta) {
                     return '<a data-view="view" data-dataId="' + row.rId + '">' + ((data != null) ? data : '<center>-</center>') + '</a>';
                 }
             },
             {
                 "data": "address", sWidth: "20%", "render": function (data, type, row, meta) {
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
            var url = $state.href('app.site', { 'id': row.id });
            $window.open(url, '_blank');


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

tspControllers.controller('TSPCreateCtrl', ['$scope', '$http', 'HavaTSPService', '$stateParams', '$state', '$sce', '$window', '$timeout', 'filterFilter', function ($scope, $http, HavaTSPService, $stateParams, $state, $sce, $window, $timeout, filterFilter) {

    $scope.tsp = {};
    $scope.tsp.vehicle = {};
    $scope.tsp.product = {};
    $scope.submitted = false;
    $scope.tsp.vehiclesGridData = [];

    // intial data
         HavaTSPService.getProduct().$promise.then(
              function (result) {
                  $scope.masterProductList = result.masterProducts;
                  $scope.nonMasterProductList = result.nonMasterProducts;

                  if ($stateParams.id) {
                      HavaTSPService.getTSP({ id: $stateParams.id }).$promise.then(
                       function (res) {
                           $scope.tsp.details = res.details;
                           $scope.tsp.vehiclesGridData = res.vehiclesGridData;
                           $scope.tsp.productGridData = res.productGridData;
                       });
                  }
              });

    $scope.userStatus = [
     { 'id': 1, 'name': 'Active' }, { 'id': 0, 'name': 'Inactive' }
    ];
    $scope.tsp.vehicle.status = { 'id': 1, 'name': 'Active' }

    $scope.addVehicle = function (vehicleData) {
        if (vehicleData.vehicleNo != "" && vehicleData.vehicleNo != undefined)
            $scope.vehicleNoRequired = false;
        else
            $scope.vehicleNoRequired = true;
        if (!$scope.vehicleNoRequired && vehicleData.regNo) {
            $scope.tsp.vehiclesGridData.push({
                'vehicleNo': (vehicleData.vehicleNo) ? vehicleData.vehicleNo : null,
                'regNo': (vehicleData.regNo) ? vehicleData.regNo : null,
                'driverName': vehicleData.driverName,
                'driverIDDLNo': (vehicleData.driverIDDLNo) ? vehicleData.driverIDDLNo : null,
                'maxPassengers': vehicleData.maxPassengers,
                'maxLuggages': vehicleData.maxLuggages,
                'product': (vehicleData.product) ? vehicleData.product.name : null,
                'status': (vehicleData.status) ? vehicleData.status.name : null,
                'productId': (vehicleData.product) ? vehicleData.product.id : null,
                'id': 0,
                'rowId': Math.random(),
                'isActive': vehicleData.status.name == "Active" ? true : false,
            });
            $scope.tsp.vehicle = {};
            $scope.tsp.vehicle.status = { 'id': 1, 'name': 'Active' };
        }
    }

    $scope.vehicleGridAction = function (row, task) {
        $scope.updateVehicle = false;
        $scope.rowIndex = row.rowIndex;
        if (task == "edit") {
            $scope.updateVehicle = true;
            var rData = angular.copy(row);
            $scope.tsp.vehicle = rData;
            $scope.tsp.vehicle.status = ($scope.tsp.vehicle.status == "Active") ? { 'id': 1, 'name': 'Active' } : { 'id': 0, 'name': 'Inactive' };
            angular.forEach($scope.masterProductList, function (prod) {
                if (prod.name == $scope.tsp.vehicle.product) {
                    $scope.tsp.vehicle.product = prod;
                }
            });
        } else if (task == "delete") {
            $scope.vehicleRow = row;
            $scope.viewTask = 'confirmDeleteVehicle';

        }
    }

    $scope.updateVehicleFunc = function (vehicleData) {
        if (vehicleData.vehicleNo != "" && vehicleData.vehicleNo != undefined)
            $scope.vehicleNoRequired = false;
        else
            $scope.vehicleNoRequired = true;

        var gridData = angular.copy($scope.tsp.vehiclesGridData);
        var rowIndex = null;
        angular.forEach(gridData, function (v, k) {
            if (v.rowId == vehicleData.rowId) {
                vehicleData.status = vehicleData.status.name;
                vehicleData.product = vehicleData.product.name;
                vehicleData.productId = (vehicleData.product) ? vehicleData.product.id : null;
                gridData[k] = vehicleData;
                $scope.tsp.vehicle = {};
                $scope.tsp.vehicle.status = { 'id': 1, 'name': 'Active' };
                $scope.updateVehicle = false;
                $scope.tsp.vehiclesGridData = gridData;
            }
        });
    }


    $scope.confirmDeleteVehicle = function (row) {
        var ky = null;
        angular.forEach($scope.tsp.vehiclesGridData, function (v, k) {
            if (row.rowId == v.rowId) {
                ky = k;
            }
        });
        $scope.tsp.vehiclesGridData.splice(ky, 1);

        $scope.viewTask = '';
        $scope.infoMsgDeleteVehicle = "'" + row.vehicleNo + "' has been deleted successfully.";
        $timeout(function () { $scope.infoMsgDeleteVehicle = ''; }, 1000);

        if ($scope.tsp.vehiclesGridData.length <= 0) {
            $scope.repRequired = false;
        }
    }

    $scope.tsp.productGridData = [];
    $scope.addProducts = function (product) {
        var alreadyAdded = false;
        if (product != null || product != undefined || product.productPrice != null || product.productPrice != undefined) {
            angular.forEach($scope.tsp.productGridData, function (v, k) {
                if (product.product.id == v.productId)
                    alreadyAdded = true;
            });
            if (!alreadyAdded) {
                $scope.tsp.productGridData.push({
                    'productPrice': product.productPrice,
                    'productName': product.product.name,
                    'productId': product.product.id,
                    'isActive':true
                });
            }
            alreadyAdded = false;
            $scope.tsp.product = {};
        }
    }

    $scope.priductViewAction = function (row, task) {
        $scope.updateProduct = false;
        $scope.rowIndex = row.rowIndex;
        if (task == "edit") {
            $scope.updateProduct = true;
            var rData = angular.copy(row);
            $scope.tsp.product = rData;
            angular.forEach($scope.nonMasterProductList, function (prod) {
                if (prod.name == $scope.tsp.product.productName) {
                    $scope.tsp.product.product = prod;
                }
            });
        } else if (task == "delete") {
            $scope.productRow = row;
            $scope.viewTask = 'confirmDeleteProduct';

        }
    }

    $scope.updateProductFunc = function (prodData) {
        if (prodData.product != null || prodData.product != undefined) {
            var gridData = angular.copy($scope.tsp.productGridData);
            var rowIndex = null;
            angular.forEach(gridData, function (v, k) {
                if (v.productId == prodData.productId) {
                    prodData.productName = prodData.product.name;
                    prodData.productId = prodData.product.id;
                    gridData[k] = prodData;
                    $scope.tsp.product = {};
                    $scope.updateProduct = false;
                    $scope.tsp.productGridData = gridData;
                }
            });
        }
    }

    $scope.confirmDeleteProduct = function (row) {
        var ky = null;
        angular.forEach($scope.tsp.productGridData, function (v, k) {
            if (row.productId == v.productId) {
                ky = k;
            }
        });
        $scope.tsp.productGridData.splice(ky, 1);

        $scope.viewTask = '';
        $scope.infoMsgDeleteProduct = "'" + row.productName + "' has been deleted successfully.";
        $timeout(function () { $scope.infoMsgDeleteProduct = ''; }, 1000);

        if ($scope.tsp.productGridData.length <= 0) {
            $scope.repRequired = false;
        }
    }

    $scope.create = function (tsp) {
        $scope.submitted = true;
        if ($scope.tspForm.$invalid == false) {
            if ($scope.tsp.vehiclesGridData.length > 0) {
                if ($scope.tsp.productGridData.length > 0) {
                    
                    if ($stateParams.id) {
                        var tspOBJ = {
                            id: $stateParams.id,
                            name: tsp.details.name,
                            address: tsp.details.address,
                            email: tsp.details.email,
                            telephoneLand: tsp.details.telephoneLand,
                            telephoneMobile: tsp.details.telephoneMobile,
                            isActive: true,
                            vehicles: $scope.tsp.vehiclesGridData,
                            products: $scope.tsp.productGridData
                        };

                        delete tsp.vehiclesGridData;
                        delete tsp.productGridData
                        HavaTSPService.updateTSP(tspOBJ).$promise.then(function (data) {
                            if (data.status == true) {
                            }
                        }); 
                    }
                    else {
                        var tspOBJ = {
                            name: tsp.details.name,
                            address: tsp.details.address,
                            email: tsp.details.email,
                            telephoneLand: tsp.details.telephoneLand,
                            telephoneMobile: tsp.details.telephoneMobile,
                            isActive: true,
                            vehicles: $scope.tsp.vehiclesGridData,
                            products: $scope.tsp.productGridData
                        };

                        delete tsp.vehiclesGridData;
                        delete tsp.productGridData
                        HavaTSPService.create(tspOBJ).$promise.then(function (data) {
                            if (data.status == true) {
                            }
                        });
                    }
                }
                else
                {
                    $scope.productRequired = true;
                }
            }
            else {
                $scope.vehicleRequired = true;
            }
        }
    }

}]);
