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

partnerControllers.controller('PartnerCtrl', ['$scope', '$http', 'HavaPartnerService', '$stateParams', '$state', '$sce', '$window', '$timeout', 'PartnerServiceLocal', 'localStorageService', function ($scope, $http, HavaPartnerService, $stateParams, $state, $sce, $window, $timeout, PartnerServiceLocal, localStorageService) {
    var tmp = PartnerServiceLocal;
    $scope.infoMsg = $sce.trustAsHtml(PartnerServiceLocal.infoMsg);
$scope.create = function () {
        $scope.submitted = true;
        if ($scope.partnerForm.$invalid == false) {
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
        var copyButton = '<a class="link-action action-button" title="Copy" data-dataId="' + o.id + '" data-dataType="' + o.id + '"  data-view="copy"><i class="fa fa-files-o"></i></a>'
        actionButtons += copyButton + editButton + viewButton;
        if (o.status != "Closed")
            actionButtons += deleteButton;
        actionButtons += '</div>';
        return actionButtons;
    }
    var accessToken = localStorageService.get('accessToken');
    $('#datatable-partner').DataTable({
      //  "processing": true,
      //  "serverSide": true,
        'ajax': {
            'url': appUrl + 'Partner/GetList',
            'type': 'GET',
            'beforeSend': function (request) {
                request.setRequestHeader("Authorization", 'Bearer ' + accessToken);
            },
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
            var tmp = PartnerServiceLocal;
            tmp.copyedData = row;
            $state.go('^.add');
        }
        else if (task == 'delete') {
            $scope.data = row;
            $scope.viewTask = 'confirmDelete';
            window.scrollTo(0, 0);

        }
    }
    $scope.delete = function (row) {
        $scope.viewTask = '';
        HavaPartnerService.delete({ id: row.id }).$promise.then(
        function (result) {
            if (result.status == true) {
               
                $scope.viewTask = '';
                $scope.infoMsgDelete = "'" + row.id + "' has been deleted successfully.";
            }
        });
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

partnerControllers.controller('PartnerCreateCtrl', ['$scope', '$http', 'HavaPartnerService', '$stateParams', '$state', '$sce', '$window', '$timeout', 'filterFilter', 'PartnerServiceLocal','$filter', function ($scope, $http, HavaPartnerService, $stateParams, $state, $sce, $window, $timeout, filterFilter, PartnerServiceLocal, $filter) {
    var tmp = PartnerServiceLocal;
    $scope.representative = {};
    $scope.submittedRep = false;
    $scope.representativeGridData = [];
    $scope.products = [];

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
                            $scope.products = result.products;
                            $scope.siteGridData = result.siteGridData;
                            var datatable = $('#productdt').dataTable().api();
                           datatable.clear();
                           datatable.rows.add($scope.products);
                            datatable.draw();
                        });
                  } else if (PartnerServiceLocal.copyedData) {
                      HavaPartnerService.getPartner({ id: PartnerServiceLocal.copyedData.id }).$promise.then(
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
    $scope.representative.status = { 'id': 1, 'name': 'Active' };
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
                                       'userName': repData.userName, 'password': repData.password, 'status': (repData.status) ? repData.status.name : 'Active',
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
                        'userName': repData.userName, 'password': repData.password, 'status': (repData.status) ? repData.status.name : 'Active',
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

        if ($scope.pswMinLength != true && $scope.partnerForm.representativeEmail.$error.email != true && $scope.representativeNameRequired == false) {

            if (repData.password && repData.password != undefined && repData.password != "" && repData.userName) {
                //AutoConceptCustomerService.isUserNamePasswordExists({ username: repData.userName, id: (repData.rId) ? repData.rId : 0, password: repData.password }).$promise.then(
                //      function (result) {
                //          if (result.status == false) {
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


                          //} else {
                          //    $scope.userExist = true;
                          //}
                      //});
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
        $scope.submittedProd = true;
        if ($scope.product == undefined || $scope.product.product == null || $scope.product.product == undefined) {
            $scope.productNameRequired = true;
        }
        else
        {
            $scope.productNameRequired = false;
            $scope.productGridData.push({
                'product': product.product,
                'productId': product.product.id,
                'productName':product.product.name,
                'locationName':product.locationName,
                'fromLocation':product.fromLocation,
                'toLocation':product.toLocation,
                'isAirPortTour': product.isAirPortTour,
                'havaPrice': product.havaPrice != undefined || product.havaPrice != "" ? product.havaPrice : 0,
                'marketPrice': product.marketPrice != undefined || product.marketPrice != "" ? product.marketPrice : 0,
                'partnerSellingPrice': product.partnerSellingPrice != undefined || product.partnerSellingPrice != "" ? product.partnerSellingPrice : 0,
                'isMarkup': product.isMarkup,
                'partnerMarkup': product.isMarkup ? (product.partnerMarkup != "" ? product.partnerMarkup : 0) : 0,
                'partnerPercentage': product.partnerPercentage != undefined || product.partnerPercentage != "" ? product.partnerPercentage : 0,
                'airPortRate': product.airPortRate != undefined || product.airPortRate != "" ? product.airPortRate : 0,
                'havaPriceReturn': product.havaPriceReturn != undefined || product.havaPriceReturn != "" ? product.havaPriceReturn : 0,
                'marketPriceReturn': product.marketPriceReturn != undefined || product.marketPriceReturn != "" ? product.marketPriceReturn : 0,
                'partnerSellPriceReturn': product.partnerSellPriceReturn != undefined || product.partnerSellPriceReturn != "" ? product.partnerSellPriceReturn : 0,
                'additionalDayRate': product.additionalDayRate != undefined || product.additionalDayRate != "" ? product.additionalDayRate : 0,
                'additionalHourRate': product.additionalHourRate != undefined || product.additionalHourRate != "" ? product.additionalHourRate : 0,
                'chufferDailyRate': product.chufferDailyRate != undefined || product.chufferDailyRate != "" ? product.chufferDailyRate : 0,
                'chufferKMRate': product.chufferKMRate != undefined || product.chufferKMRate != "" ? product.chufferKMRate : 0,
                'childSeatRate': product.childSeatRate != undefined || product.childSeatRate != "" ? product.childSeatRate : 0,
            });
            $scope.product = {};
        }  
    }

    $scope.priductViewAction = function (row, task) {
        $scope.updateProduct = false;
        $scope.rowIndex = row.rowIndex;
        if (task == "view") {
            $scope.selectedProd = {};
            var rData = angular.copy(row);
            $scope.selectedProd = rData;
            $('#pro-location-prices').modal('show');
        }
        else if (task == "edit") {
            $scope.updateProduct = true;
            var rData = angular.copy(row);
            $scope.product = rData;
            angular.forEach($scope.productList, function (prod) {
                if (prod.name == $scope.product.productName) {
                    $scope.product.product = prod;
                }
            });
            $scope.product.isMarkup = row.isMarkup == "True" ? true : false;
        } else if (task == "delete") {
            $scope.productRow = row;
            $scope.viewTask = 'confirmDeleteProduct';
        }
    }

    $scope.updateProductFunc = function (prodData) {
        if (prodData.product != null || prodData.product != undefined) {
            var gridData = angular.copy($scope.productGridData);
            var rowIndex = null;
            angular.forEach(gridData, function (v, k) {
                if (v.productId == prodData.productId) {
                    prodData.productName = prodData.product.name;
                    prodData.productId = prodData.product.id;
                    gridData[k] = prodData;
                    $scope.product = {};
                    $scope.updateProduct = false;
                    $scope.productGridData = gridData;
                }
            });
        }
    }

    $scope.confirmDeleteProduct = function (row) {
        var ky = null;
        angular.forEach($scope.productGridData, function (v, k) {
            if (row.productId == v.productId) {
                ky = k;
            }
        });
        $scope.productGridData.splice(ky, 1);

        $scope.viewTask = '';
        $scope.infoMsgDeleteProduct = "'" + row.productName + "' has been deleted successfully.";
        $timeout(function () { $scope.infoMsgDeleteProduct = ''; }, 1000);

        if ($scope.productGridData.length <= 0) {
            $scope.repRequired = false;
        }
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
        $scope.submittedSite = true;
        if (site != null || site != undefined) {
            $scope.siteRequired = false;
        angular.forEach($scope.siteGridData, function (v, k) {
            if (site.id == v.id)
                alreadyAdded = true;
        });
        if (!alreadyAdded) {
            $scope.siteGridData.push({
                'id': '-1' + Math.random(),
                'siteId': site.id,
                'name': site.name,
            });
        }
    } else {
        $scope.siteRequired = true;
    }
    }

    $scope.create = function (partner) {
        $scope.submitted = true;
        if ($scope.partnerForm.$invalid == false) {
            if($scope.representativeGridData.length > 0){
                partner.representativeData = $scope.representativeGridData;
                partner.productGridData = $scope.products;//$scope.productGridData;
                partner.siteGridData = $scope.siteGridData;

                if ($stateParams.id) {
                    HavaPartnerService.updatePartner(partner).$promise.then(function (data) {
                        if (data.status == true) {
                            tmp.infoMsg = 'Partner \'' + partner.name + '\' has been updated successfully.';
                            $state.go('^.list');
                        }
                    });
                }
                else {
                    HavaPartnerService.create(partner).$promise.then(function (data) {
                        if (data.status == true) {
                            tmp.infoMsg = 'Partner \'' + partner.name + '\' has been saved successfully.';
                            $state.go('^.list');
                        }
                    });
                }
            }
        }
    }
   // var products = [];
  //{
  //  "id": 1,
  //  "productName": "test",
  //  "locationName": "test2",
  //  "locationFrom": "test3",
  //  "locationTo": "test4",
  //  "rates": [{
  //    "id": 1,
  //    "havaPrice": 2312312,
  //    "havaPriceReturn": 1212331233,
  //    "marketPrice": 333,
  //    "partnerSellingPrice": 2322
  //    },
  //        {
  //      "id": 2,
  //    "havaPrice": 23,
  //    "havaPriceReturn": 123,
  //    "marketPrice": 333,
  //    "partnerSellingPrice": 2322
  //  }
  //  ]
  //  },
    //  {
    //"id": 2,
    //"productName": "test2",
    //"locationName": "test22",
    //"locationFrom": "test32",
    //"locationTo": "test42",
    //"rates": [{
    //  "id": 1,
    //  "havaPrice": 23,
    //  "havaPriceReturn": 123,
    //  "marketPrice": 333,
    //  "partnerSellingPrice": 2322
    //},
    //{
    //  "id": 2,
    //  "havaPrice": 23,
    //  "havaPriceReturn": 1235555,
    //  "marketPrice": 33355,
    //  "partnerSellingPrice": 2322
    //}
    //]
    //}
    //  ];

    $scope.format = function(table_id) {
        return '<table class="table table-striped" id="productdt_' + table_id + '">' +
        '<thead><tr><th>From Location</th><th>To Location</th><th>Is AirPort Tour</th><th>Location Name</th><th>Hava Price</th><th>Hava Price Return</th><th>Market Price</th><th>Partner Selling Price</th><th>Percentage</th></tr></thead>' +
        '<tr>'+
          '<td></td>'+
          '<td></td>'+
          '<td></td>' +
          '<td></td>' +
            '<td></td>' +
            '<td></td>' +
            '<td></td>' +
            '<td></td>' +
            '<td></td>' +
        '</tr>'
        '</table>';
    }
    var iTableCounter=1;
    var oInnerTable;

    $(document).ready(function () {
        TableHtml = $('#productdt').html();

        var table = $('#productdt').DataTable({
            paging: false,
            searching: false,
            info: false,
            rowId: 'id',
            data: $scope.products,
            columns: [
              {
                  className: 'details-control',
                  orderable: false,
                  data: null,
                  defaultContent: ''
              },
              { data:'id' },
              { data: 'productName' }
            ],
            order: [[1, 'asc']]
        });
        /* Add event listener for opening and closing details
         * Note that the indicator for showing which row is open is not controlled by DataTables,
         * rather it is done here
         */
        $('#productdt tbody').on('click', 'td.details-control', function () {
            var tr = $(this).closest('tr');
            var row = table.row(tr);
            var rowData = table.row(tr).data();
            console.log(rowData.rates);
            if (row.child.isShown()) {
                //  This row is already open - close it
                row.child.hide();
                tr.removeClass('shown');
            }
            else {
                // Open this row
                row.child($scope.format(rowData.id)).show();
                tr.addClass('shown');
                // try datatable stuff
                oInnerTable = $('#productdt_' + rowData.id).dataTable({
                    data: rowData.rates,
                    autoWidth: true,
                    deferRender: true,
                    info: false,
                    lengthChange: false,
                    ordering: false,
                    paging: false,
                    scrollX: false,
                    scrollY: false,
                    searching: false,
                    columns: [
                        { data : 'fromLocation'},
                        { data : 'toLocation'}, 
                        { data : 'isAirPortTour'},
                        { data : 'locationName' },
                        { data : 'havaPrice' },
                        { data : 'havaPriceReturn' },
                        { data : 'marketPrice' },
                        { data: 'partnerSellingPrice' },
                        { data : 'percentage'}

                    ]
                });
                iTableCounter = iTableCounter + 1;
            }
        });
    });

}]);
