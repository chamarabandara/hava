/**
*   Source File:		            dealer.controller.js

*   Property of Autoconcept All rights reserved.

*   Description:                    Dealer functionalities
*
*   Date		    Author/(Reviewer)		Description
*   -------------------------------------------------------	
*   10 June 2015    Gihan Dilusha	        Creation 
*
*/

var sitesControllers = angular.module('sitesControllers', ['ngCookies', 'siteService', 'angularFileUpload']);

sitesControllers.controller('SitesCreateCtrl', ['$scope', '$http', 'HavaSiteService', '$stateParams', '$state', '$sce', '$window', '$timeout', '$upload', 'SiteServiceLocal', function ($scope, $http, HavaSiteService, $stateParams, $state, $sce, $window, $timeout, $upload, SiteServiceLocal) {
    var tmp = SiteServiceLocal;
    $scope.create = function (sites) {
        $scope.submitted = true;
        if ($scope.productForm.$invalid == false) {
            if ($scope.productLogoImage && $scope.productLogoImage.length > 0) {
                sites.productLogoImage = angular.copy($scope.docFileLogo);
            } else {
                sites.productLogoImage = null;
            }
            HavaSiteService.createSites(sites).$promise.then(function (data) {
                if (data.status == true) {
                    tmp.infoMsg = 'Site \'' + sites.name + '\' has been saved successfully.';
                    $state.go('^.list');
                }
            });

        }
    }
    $scope.queue = [];
    $scope.productLogoImage = [];
    $scope.tempFileData;
    $scope.onFileSelectLogo = function ($files) {
        // if (!$scope.tempFileData)
        $scope.tempFileData = $files;
        var type = $files[0].type;
        if (type == "image/jpeg" || type == "image/png" || type == "image/gif") {
            //$("#productLogoId").trigger('change');
            if ($scope.queue.length > 1 || $scope.productLogoImage.length > 1 || ($scope.productLogoImageUploaded && $scope.productLogoImageUploaded.length > 1)) {
                $scope.queue = [];
                $scope.productLogoImage = [];
                $scope.productLogoImageUploaded = [];
            }
            $scope.productLogoImage = [];
            $scope.productLogoImageUploaded = [];

            Array.prototype.unique = function () {
                var a = this.concat();
                for (var i = 0; i < a.length; ++i) {
                    for (var j = i + 1; j < a.length; ++j) {
                        if (a[i].name === a[j].name)
                            a.splice(j--, 1);
                    }
                }
                return a;
            };
            if ($files.length > 0) {

                //  if ($scope.allowToUpload) {
                $scope.queue = $scope.queue.concat($files).unique();
                $scope.productLogoImage = $scope.queue;

                $scope.uploadProductLogoImage(0);

                $scope.queue = [];
            }
        } else {
            $scope.infoMsg = "Invalid Image Type";
            window.scrollTo(0, 0);
        }
    }

    $scope.upload = [];
    $scope.uploadProductLogoImage = function (index, type) {
        var $file = $scope.queue[index];
        $scope.docFileLogo = {};
        $scope.docFileLogo.name = $file.name;
        $scope.docFileLogo.size = $file.size;
        $scope.upload[index] = $upload.upload({
            url: "api/File/UploadSiteImage", // webapi url
            method: "POST",
            data: { ImportType: 'fileUpload', fileUploadObj: $scope.fileUploadObj },
            file: $file
        }).progress(function (evt) {
            // get upload percentage
            if ($scope.productLogoImage && $scope.productLogoImage[index]) {
                $scope.productLogoImage[index].progress = parseInt(100.0 * evt.loaded / evt.total);
                $scope.productLogoImage[index].isUploading = true;
                $scope.docFileLogo.progress = parseInt(100.0 * evt.loaded / evt.total);
                if ($scope.productLogoImage[index].isError)
                    $scope.productLogoImage[index].progress = 0;
            }

        }).success(function (data, status, headers, config) {
            // file is uploaded successfully
            if ($scope.productLogoImage && $scope.productLogoImage[index]) {
                $scope.productLogoImage[index].isSuccess = true;
                $scope.productLogoImage[index].isError = false;
                $scope.productLogoImage[index].documentPath = data;
                $scope.productLogoImage[index].docUpdated = true;
                $scope.docFileLogo.documentPath = data;
                $scope.docFileLogo.isSuccess = true;
                $scope.docFileLogo.isError = false;
                $scope.docFileLogo.docUpdated = true;
            }

        }).error(function (data, status, headers, config) {
            $scope.queue[index].isError = true;
            $scope.queue[index].progress = 0;
        });
    }

    $scope.removeProductLogoImage = function (id) {
        if (id != 0) {
            AutoConceptProductService.removeProductLogoImage({ productId: id }).$promise.then(function (result) {
                console.log(result);
                if (result.status == true) {
                    $scope.productLogoImageUploaded.splice(0, 1);
                }
            });
        } else {
            $scope.productLogoImage.splice(0, 1);
        }
    }

    $scope.showImage = function (url) {
        if (url)
            $window.open(apiUrl + url, '_blank');
    }
}]);

sitesControllers.controller('SitesListCtrl', ['$scope', '$http', 'HavaSiteService', '$stateParams', '$state', '$sce', '$window', '$timeout', 'SiteServiceLocal', function ($scope, $http, HavaSiteService, $stateParams, $state, $sce, $window, $timeout, SiteServiceLocal) {
    var tmp = SiteServiceLocal;

    $scope.infoMsg = $sce.trustAsHtml(SiteServiceLocal.infoMsg);
    $scope.model = {};
    $scope.submitted = false;
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
                        SiteServiceLocal.infoMsg = "Site " + $scope.dealer.companyDetails.orgName + " has been saved successfully.";
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
            'url': appUrl + 'Sites/GetList',
            'type': 'GET',
         
            contentType: 'application/json',
            'beforeSend': function (request) {
                request.setRequestHeader("Authorization", 'Bearer ' + accessToken);
            },
            "data": function (d) {
                return JSON.stringify(d);
            },
        },

        "aoColumns": [
             {
                 "data": "name", sWidth: "45%", "render": function (data, type, row, meta) {
                     return '<a data-view="view" data-dataId="' + row.rId + '">' + ((data != null) ? data : '<center>-</center>') + '</a>';
                 }
             },
             {
                 "data": "code", sWidth: "45%", "render": function (data, type, row, meta) {
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

sitesControllers.controller('SitesManageCtrl', ['$scope', '$http', 'HavaSiteService', '$stateParams', '$state', '$sce', '$window', '$timeout', function ($scope, $http, HavaSiteService, $stateParams, $state, $sce, $window, $timeout) {
   
    window.location = 'home/Sites?P=' + $stateParams.id;

    angular.element(document).ready(function () {

    });
}]);
