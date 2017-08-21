/**
*   Source File:		            dealer.controller.js

*   Property of Autoconcept All rights reserved.

*   Description:                    Dealer functionalities
*
*   Date		    Author/(Reviewer)		Description
*   -------------------------------------------------------	
*   10 July 2017    Chamara Bandara	        Creation 
*
*/

var partnerControllers = angular.module('partnerControllers', ['ngCookies', 'partnerService']);

partnerControllers.controller('PartnerCtrl', ['$scope', '$http', 'HavaPartnerService', '$stateParams', '$state', '$sce', '$window', '$timeout', function ($scope, $http, HavaPartnerService, $stateParams, $state, $sce, $window, $timeout) {
    $scope.create = function () {
        $scope.submitted = true;
        if ($scope.customerForm.$invalid == false) {
            $scope.isLegelNameExistsValidation();
            $scope.isAccNoExistsVaidation();
            $scope.isOrgRegNoExistsVaidation();
            if (!$scope.isLegelNameExistStatus && !$scope.isAccExistStatus && !$scope.isOrgRegNoExistsStatus) {
                $http.post(apiUrl + '/api/Sales/AddDealer', $scope.dealer).
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
            'url': appUrl + 'Partner/GetList',
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

partnerControllers.controller('PartnerCreateCtrl', ['$scope', '$http', 'HavaPartnerService', '$stateParams', '$state', '$sce', '$window', '$timeout', 'filterFilter', function ($scope, $http, HavaPartnerService, $stateParams, $state, $sce, $window, $timeout, filterFilter) {
    //var tmp = PartnerServiceLocal;
    $scope.representative = {};
    $scope.submittedRep = false;
    $scope.representativeGridData = [];

    HavaPartnerService.getProduct().$promise.then(
              function (result) {
                  $scope.productList = result.data;
                  HavaPartnerService.getSites().$promise.then(
                     function (result) {
                         $scope.siteList = result.data;
                     });

                  if ($stateParams.id) {
                      HavaPartnerService.getPartner({ id: $stateParams.id }).$promise.then(
                        function (result) {
                            $scope.model = result;
                            $scope.representativeGridData = result.representativeGridData;
                            $scope.productGridData = result.productGridData;
                            $scope.siteGridData = result.siteGridData;
                        });
                  }
              });

    
    //status
    $scope.userStatus = [
     { 'id': 1, 'name': 'Active' }, { 'id': 0, 'name': 'Inactive' }
    ];
    $scope.representative.status = {'id':1}
    $scope.addRepresentative = function (repData) {
        $scope.submittedRep = true;
        $scope.userExist = false;

        if (repData.name != "" && repData.name != undefined)
            $scope.representativeNameRequired = false;
        else
            $scope.representativeNameRequired = true;

        if (repData.password && repData.password != undefined && repData.password != "") {
            if (repData.password != "" && repData.password != undefined && repData.password.length > 2) {
                $scope.pswMinLength = false;
            } else {
                $scope.pswMinLength = true;
            }
        } else {
            $scope.pswMinLength = false;
        }


        if ($scope.pswMinLength != true && $scope.representativeNameRequired == false && $scope.partnerForm.representativeEmail.$error.email != true) {
            if (repData.password && repData.password != undefined && repData.password != "" && repData.userName) {
             
                                   $scope.representativeGridData.push({
                                       'name': (repData.name) ? repData.name : null, 'teleNo': (repData.teleNo) ? repData.teleNo : null,
                                       'mobileNo': repData.mobileNo, 'email': (repData.email) ? repData.email : null,
                                       'userName': repData.userName, 'password': repData.password, 'status': (repData.status) ? repData.status.name : null,
                                       'id': '-1' + Math.random()
                                   });
                                   $scope.representative = {};
                                   $scope.representative.status = { 'id': 1, 'name': 'Active' };
                                   $scope.repRequired = false;
                             

            } else {

                var repD = filterFilter($scope.representativeGridData, function (item) {
                    if (item.userName && item.userName == repData.userName && item.password == repData.password) {
                        return item;
                    }
                });
                if (repD.length > 0) {
                    $scope.userExist = true;
                } else {
                    $scope.representativeGridData.push({
                        'name': (repData.name) ? repData.name : null, 'teleNo': (repData.teleNo) ? repData.teleNo : null,
                        'mobileNo': repData.mobileNo, 'email': (repData.email) ? repData.email : null,
                        'userName': repData.userName, 'password': repData.password, 'status': (repData.status) ? repData.status.name : null,
                        'id': '-1' + Math.random()
                    });
                    $scope.representative = {};
                    $scope.representative.status = { 'id': 1, 'name': 'Active' };
                    $scope.repRequired = false;
                }

            }


        }


    }

    $scope.updateRepresentative = function (repData) {
        $scope.submittedRep = true;
        $scope.userExist = false;

        if (repData.name != "" && repData.name != undefined)
            $scope.representativeNameRequired = false;
        else
            $scope.representativeNameRequired = true;


        if (repData.password && repData.password != undefined && repData.password != "") {
            if (repData.password != "" && repData.password != undefined && repData.password.length > 2) {
                $scope.pswMinLength = false;
            } else {
                $scope.pswMinLength = true;
            }
        } else {
            $scope.pswMinLength = false;
        }

        if ($scope.pswMinLength != true && $scope.customerForm.representativeEmail.$error.email != true && $scope.representativeNameRequired == false) {

            if (repData.password && repData.password != undefined && repData.password != "" && repData.userName) {
                AutoConceptCustomerService.isUserNamePasswordExists({ username: repData.userName, id: (repData.rId) ? repData.rId : 0, password: repData.password }).$promise.then(
                      function (result) {
                          if (result.status == false) {
                              var repD = filterFilter($scope.representativeGridData, function (item) {
                                  if (item.userName == repData.userName && item.password == repData.password && item.id != repData.id) {
                                      return item;
                                  }
                              });
                              if (repD.length > 0) {
                                  $scope.userExist = true;
                              } else {
                                  var gridData = angular.copy($scope.representativeGridData);
                                  var rowIndex = null;
                                  angular.forEach(gridData, function (v, k) {
                                      if (v.id == repData.id) {
                                          rowIndex = k;
                                      }
                                  });
                                  repData.status = repData.status.name;
                                  gridData[rowIndex] = repData;
                                  $scope.representative = {};
                                  $scope.representative.status = { 'id': 1, 'name': 'Active' };
                                  $scope.updateRep = false;
                                  $scope.representativeGridData = gridData;
                              }


                          } else {
                              $scope.userExist = true;
                          }
                      });
            } else {

                var repD = filterFilter($scope.representativeGridData, function (item) {
                    if (item.userName && item.userName == repData.userName && item.password == repData.password && item.id != repData.id) {
                        return item;
                    }
                });
                if (repD.length > 0) {
                    $scope.userExist = true;
                } else {
                    var gridData = angular.copy($scope.representativeGridData);
                    var rowIndex = null;
                    angular.forEach(gridData, function (v, k) {
                        if (v.id == repData.id) {
                            rowIndex = k;
                        }
                    });
                    repData.status = repData.status.name;
                    gridData[rowIndex] = repData;
                    $scope.representative = {};
                    $scope.representative.status = { 'id': 1, 'name': 'Active' };
                    $scope.updateRep = false;
                    $scope.representativeGridData = gridData;
                }
            }
        }

    }

    $scope.repGridAction = function (row, task) {
        $scope.representative = {};

        $scope.repRequired = false;
        $scope.updateRep = false;
        $scope.rowIndex = row.rowIndex;
        if (task == "edit") {
            $scope.updateRep = true;
            $scope.submittedRep = false;
            var rData = angular.copy(row);
            $scope.representative = rData;
            $scope.representative.status = ($scope.representative.status == "Active") ? { 'id': 1, 'name': 'Active' } : { 'id': 0, 'name': 'Inactive' };
            //$scope.representative.status = { 'id': 1, 'name': 'Active' };
        } else if (task == "delete") {
            $scope.repRow = row;
            $scope.viewTask = 'confirmDeleteRepresentative';

        }
    }

    $scope.confirmDeleteRep = function (row) {
        var ky = null;
        angular.forEach($scope.representativeGridData, function (v, k) {
            if (row.id == v.id) {
                ky = k;
            }
        });
        $scope.representativeGridData.splice(ky, 1);

        $scope.viewTask = '';
        $scope.infoMsgDeleteRep = "'" + row.name + "' has been deleted successfully.";
        $timeout(function () { $scope.infoMsgDeleteRep = ''; }, 1000);

        if ($scope.representativeGridData.length <= 0) {
            $scope.repRequired = false;
        }
    }

    $scope.productGridData = [];
    $scope.addProducts = function (product) {
        if (product != null || product != undefined)
            $scope.productGridData.push({
                'havaPrice': product.havaPrice,
                'isMarkup': product.isMarkup,
                'marketPrice': product.marketPrice,
                'partnerSellingPrice': product.partnerSellingPrice,
                'product': product.product,
                'partnerSellingPrice': product.partnerSellingPrice,
                'partnerPercentage': product.partnerPercentage,
                'partnerMarkup': product.partnerMarkup,
                'productId':product.product.id
            });

        console.log($scope.productGridData);
    }

    $scope.deleteSite = function(row){
        $scope.siteRow = row;
        $scope.viewTask = 'confirmDeleteProduct';
    }
    $scope.confirmDeleteSite = function (row) {
        var ky = null;
        angular.forEach($scope.siteGridData, function (v, k) {
            if (row.id == v.id) {
                ky = k;
            }
        });
        $scope.siteGridData.splice(ky, 1);

        $scope.viewTask = '';
        $scope.infoMsgDeleteSite = "'" + row.name + "' has been deleted successfully.";
        $timeout(function () { $scope.infoMsgDeleteSite = ''; }, 1000);
    }

    $scope.siteGridData = [];
    $scope.addSite = function (site) {
    var alreadyAdded = false;
        if (site != null || site != undefined){
            angular.forEach($scope.siteGridData, function (v, k) {
                    if (site.id == v.id)
                        alreadyAdded = true;
                });
            if (!alreadyAdded) {
                $scope.siteGridData.push({
                    'id': site.id,
                    'name': site.name,
                
                });
            }
            }
            console.log($scope.siteGridData)
    }

    $scope.create = function (partner) {
        $scope.submitted = true;
        if ($scope.partnerForm.$invalid == false) {
            partner.representativeData = $scope.representativeGridData;
            partner.productGridData = $scope.productGridData;
            partner.siteGridData = $scope.siteGridData;
            HavaPartnerService.create(partner).$promise.then(function (data) {
                if (data.status == true) {
                  tmp.infoMsg = 'Customer \'' + sites.name + '\' has been saved successfully.';
                    $state.go('^.list');
                }
            });

        }
    }
}]);
