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
        actionButtons = '<div class="ngCellText"  ng-class="col.colIndex()">';
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

        $scope.clearMsg = function () {
        $scope.infoMsg = '';
        tmp.infoMsg = '';
        $scope.infoMsgDelete = "";
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

partnerControllers.controller('PartnerCreateCtrl', ['$scope', '$http', 'HavaPartnerService', '$stateParams', '$state', '$sce', '$window', '$timeout', 'filterFilter', 'PartnerServiceLocal', '$filter', '$compile', function ($scope, $http, HavaPartnerService, $stateParams, $state, $sce, $window, $timeout, filterFilter, PartnerServiceLocal, $filter, $compile) {
    var tmp = PartnerServiceLocal;
    $scope.representative = {};
    $scope.submittedRep = false;
    $scope.representativeGridData = [];
    $scope.products = [];
    $scope.location = {};
    $scope.locationProducts = [];

    $scope.viewPartnerSites = function (obj) {
        //var url = $state.href('app.site', { 'id': $stateParams.id + 'S' + obj.id});
        var url = 'http://52.14.195.144/hava/home/Sites?P=' + $stateParams.id + 'S' + obj.id;
        $window.open(url, '_blank');
    }

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
                  else {
                      $scope.mainProductDetails = [];
                       HavaPartnerService.getMainProducts().$promise.then(
                         function (result) {
                             $scope.mainProductDetails = result.data;
                             $scope.location.products = $scope.mainProductDetails;
                         });
                       HavaPartnerService.getSubProducts().$promise.then(
                          function (result) {
                              $scope.subProductDetails = result.data;
                          });
                       //HavaPartnerService.getAllLocations().$promise.then(
                       //   function (result) {
                       //       $scope.locationList = result.data;
                      //   });
                       $scope.locationList = [{ id: 1, name: "name1" }, { id: 2, name: "name2" }, { id: 3, name: "name3" }];
                       
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
            if (site.id == v.siteId)
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
                partner.productGridData = $scope.products;
                partner.siteGridData = $scope.siteGridData;
                partner.mainProductDetails = $scope.mainProductDetails;
                partner.subProductDetails = $scope.subProductDetails;
                partner.locationProducts = $scope.locationProducts;

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
  
    $scope.format = function(table_id) {
        return '<table class="table table-striped" id="productdt_' + table_id + '">' +
        '<thead><tr><th>Location Name</th><th>From Location</th><th>To Location</th><th>Partner Selling Price</th><th>Market Price</th><th>Hava Price</th><th></th></tr></thead>' +
        '<tr>'+
          '<td></td>'+
          '<td></td>'+
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
              { data:'productId' },
              { data: 'productName' },
              {
                  "data": null,
                  "bSortable": false,
                  "mRender": function (o) {
                      return $scope.bindActionButtons(o);
                  }
              }
            ],
            order: [[1, 'asc']],
            createdRow: function (row, data, dataIndex) {
                $compile(angular.element(row).contents())($scope);
            }
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
                var ratesData = rowData.rates;
                oInnerTable = $('#productdt_' + rowData.id).DataTable({
                    data: ratesData,
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
                        {data: 'locationName' },
                        {data: 'fromLocation' },
                        {data: 'toLocation' },
                        {data: 'partnerSellingPrice' },
                        {data: 'marketPrice' },
                        { data: 'havaPrice' },
                        {
                            "data": null,
                            "bSortable": false,
                            "mRender": function (o) {
                                return $scope.bindActionButtonsRates(o);
                            }
                        }

                    ],
                    createdRow: function (row, data, dataIndex) {
                        $compile(angular.element(row).contents())($scope);
                    }
                });
                iTableCounter = iTableCounter + 1;
            }
        });



    });
    $scope.bindActionButtons = function (o) {
        var actionButtons = "";
        actionButtons = '<div class="ngCellText" ng-cell-text ng-class="col.colIndex()">';
        var addButton = '<a data-dataId="' + o.id + '" ng-click="showAddRate($event)" class="link-action action-button" data-view="add" title="View"><i class="fa fa-plus"></i></a>';
        var deleteButton = '<a data-dataId="' + o.id + '" ng-click="deleteProduct($event)" class="link-action action-button" data-view="delete" title="Delete"><i class="fa fa-trash"></i></a>';
        actionButtons += addButton + deleteButton;
        
        actionButtons += '</div>';
        return actionButtons;
    }

    $scope.bindActionButtonsRates = function (o) {
        var actionButtons = "";
        actionButtons = '<div class="ngCellText" ng-cell-text ng-class="col.colIndex()">';
        var addButton = '<a data-dataId="' + o.id + '" ng-click="showEditRate($event)" class="link-action action-button" data-view="edit" title="Edit"><i class="fa fa-edit"></i></a>';
        var deleteButton = '<a data-dataId="' + o.id + '" ng-click="deleteRate($event)" class="link-action action-button" data-view="delete" title="Delete"><i class="fa fa-trash"></i></a>';
        actionButtons += addButton + deleteButton;

        actionButtons += '</div>';
        return actionButtons;
    }

    
    $scope.showAddRate = function ($event) {
        var t = $event.currentTarget;
        var tr = $(t).closest('tr');
        var rowData = $('#productdt').DataTable().row(tr).data();
        $scope.selprod = rowData;
        $scope.rate = {};
        $scope.isRateEdit = false;
        $("#pro-location-prices").modal('show');
        
    }
    $scope.deleteProduct = function ($event) {
        var t = $event.currentTarget;
        var tr = $(t).closest('tr');
        var rowData = $('#productdt').DataTable().row(tr).data();
        $scope.prodRow = rowData;
        $scope.viewTask = 'confirmDeleteProduct';

    }
    $scope.confirmDeleteProduct = function (row) {
        var ky = null;
        angular.forEach($scope.productGridData, function (v, k) {
            if (row.productId == v.productId) {
                v.isActive = false;
            }
        });
       
        $scope.viewTask = '';
        $scope.infoMsgDeleteProd = "'" + row.productName + "' has been deleted successfully.";
        $timeout(function () { $scope.infoMsgDeleteProduct = ''; }, 1000);

    }

    $scope.isRateEdit = false;
    $scope.showEditRate = function ($event) {
        var t = $event.currentTarget;
        var tr = $(t).closest('tr');
        var tableName = $(t).closest('table')[0].id;
        var childTbl = $("#" + tableName).DataTable();
        var row = childTbl.row(tr);
        var rowData = childTbl.row(tr).data()

        $scope.rate = rowData;
        $scope.isRateEdit = true;
        $("#pro-location-prices").modal('show');
        
    }
    $scope.editRate = function () {
        angular.forEach($scope.products, function (product, k) {
            angular.forEach(product.rates, function (rt, k) {
                if (rt.id == $scope.rate.id) {
                    rate = {
                        'id': $scope.rate.id,
                        'locationName': $scope.rate.locationName,
                        'fromLocation': $scope.rate.fromLocation,
                        'toLocation': $scope.rate.toLocation,
                        'isAirPortTour': $scope.rate.isAirPortTour,
                        'partnerPercentage': $scope.rate.partnerPercentage,
                        'partnerMarkup': $scope.rate.partnerMarkup,
                        'isMarkup': $scope.rate.isMarkup,
                        'partnerSellingPrice': $scope.rate.partnerSellingPrice,
                        'marketPrice': $scope.rate.marketPrice,
                        'havaPrice': $scope.rate.havaPrice,
                        'airPortRate': $scope.rate.airPortRate,
                        'havaPriceReturn': $scope.rate.havaPriceReturn,
                        'marketPriceReturn': $scope.rate.marketPriceReturn,
                        'partnerSellPriceReturn': $scope.rate.partnerSellPriceReturn,
                        'additionalDayRate': $scope.rate.additionalDayRate,
                        'additionalHourRate': $scope.rate.additionalHourRate,
                        'chufferDailyRate': $scope.rate.chufferDailyRate,
                        'chufferKMRate': $scope.rate.chufferKMRate,
                        'childSeatRate': $scope.rate.childSeatRate,
                        'rate': $scope.rate.rate,
                        'productId': product.productId,
                        'isActive': true,
                        'locationId': ($scope.rate.locationId != undefined && $scope.rate.locationId) > 0 ? $scope.rate.locationId : 0,
                        'productRateId': ($scope.rate.productRateId != undefined && $scope.rate.productRateId) > 0 ? $scope.rate.productRateId : 0
                    }
                    $("#pro-location-prices").modal('hide');
                }
            });
        });
        
    }

    $scope.deleteRate = function ($event) {
        var t = $event.currentTarget;
        var tr = $(t).closest('tr');
        var rowData = $('#productdt').DataTable().row(tr).data();
        $scope.rateRow =rowData;
        $scope.viewTask = 'confirmDeleteRate';
    }
    $scope.confirmDeleteRate = function (row) {
        //var ky = null;
        //angular.forEach($scope.productGridData, function (v, k) {
        //    if (row.productId == v.productId) {
        //        v.isActive = false;
        //    }
        //});

        //$scope.viewTask = '';
        //$scope.infoMsgDeleteProd = "'" + row.productName + "' has been deleted successfully.";
        //$timeout(function () { $scope.infoMsgDeleteProduct = ''; }, 1000);

    }

    $scope.addProduct = function (product) {
        var alreadyAdded = false;
        //$scope.submittedSite = true;
        if (product != null || product != undefined) {
            //$scope.siteRequired = false;
            angular.forEach($scope.products, function (v, k) {
                if (product.id == v.productId)
                    alreadyAdded = true;
            });
            if (!alreadyAdded) {
                var newProd = {
                    'id': 'X' + Math.floor(Math.random()*(999-100+1)+100),
                    'productId': product.id,
                    'productName': product.name,
                    'isActive': true,
                };
                $scope.products.push(newProd);
                var table = $('#productdt').DataTable();
                table.row.add(newProd).draw();
            }
            
        }
    }
    $scope.addRate = function (rate)
    {
       
        angular.forEach($scope.products, function (product, k) {
            if (product.id == $scope.selprod.id)
                if (!product.rates)
                    product.rates = [];
                product.rates.push({
                    'id': 'X' + Math.floor(Math.random()*(999-100+1)+100),
                    'locationName' : $scope.rate.locationName ,
                    'fromLocation' : $scope.rate.fromLocation ,
                    'toLocation' : $scope.rate.toLocation ,
                    'isAirPortTour' : $scope.rate.isAirPortTour ,
                    'productRateId' : 0 ,
                    'partnerPercentage' : $scope.rate.partnerPercentage ,
                    'partnerMarkup' : $scope.rate.partnerMarkup ,
                    'isMarkup' : $scope.rate.isMarkup ,
                    'partnerSellingPrice': $scope.rate.partnerSellingPrice,
                    'marketPrice' : $scope.rate.marketPrice ,
                    'havaPrice' : $scope.rate.havaPrice ,
                    'airPortRate' : $scope.rate.airPortRate ,
                    'havaPriceReturn' : $scope.rate.havaPriceReturn ,
                    'marketPriceReturn' : $scope.rate.marketPriceReturn ,
                    'partnerSellPriceReturn' : $scope.rate.partnerSellPriceReturn ,
                    'additionalDayRate' : $scope.rate.additionalDayRate ,
                    'additionalHourRate' : $scope.rate.additionalHourRate ,
                    'chufferDailyRate' : $scope.rate.chufferDailyRate ,
                    'chufferKMRate' : $scope.rate.chufferKMRate ,
                    'childSeatRate' : $scope.rate.childSeatRate ,
                    'rate': $scope.rate.rate,
                    'productId': $scope.selprod.productId,
                    'isActive':true,
                    'locationId':0,
                    'productRateId':0
                });
                var table = $('#productdt_' + $scope.selprodId).DataTable();
                table.clear().draw();
                table.rows.add(product.rates).draw();
                $("#pro-location-prices").modal('hide');
        });
        
    }
   
    $scope.addLocationRate = function (location) {
        $scope.submittedLoc = true;
        $scope.isError = false;
        if (location.location && location.location.id) {
            $scope.locationRequired = false;
            angular.forEach(location.products, function (v, k) {
                if ((v.marketPrice == undefined || v.marketPrice == "") || (v.havaPrice == undefined || v.havaPrice == "")) {
                    $scope.isError = true;
                }
            });
            if (!$scope.isError) {
                $scope.locationProducts.push(location);
            }
        }
        else {
            $scope.locationRequired = true;
        }
    }

}]);
