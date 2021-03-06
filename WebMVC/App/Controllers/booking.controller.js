﻿///**
//*   Source File:		            dealer.controller.js

//*   Property of Hava All rights reserved.

//*   Description:                    Dealer functionalities
//*
//*   Date		    Author/(Reviewer)		Description
//*   -------------------------------------------------------	
//*   17 Aug 2017    Chamara Bandara	        Creation 
//*
//*/



site.controller('BookingCreateCtrl', ['$scope', '$http', 'HavaSiteService', '$state', '$stateParams', '$timeout', '$injector', 'localStorageService', function ($scope, $http, HavaSiteService, $state, $stateParams, $timeout, $injector, localStorageService) {
    console.log('test');
    $scope.search = {};
    $scope.loginForm = true,
    $scope.makeRegister = false;
    $scope.additional = {};
    $scope.AdHoursUnit = 0;
    $scope.cildSetUnit = 0;
    $scope.AditionalDayUnit = 0;
    $scope.AdditionalKMRate = 0;
    $scope.additional.countAddHours = 0;
    $scope.additional.countAddDay = 0;
    $scope.additional.countAddhildSet = 0;
    $scope.additional.AdditionalKM = 0;
    $scope.passengerGridData = []
    cookieToken = localStorageService.get('accessToken');
    if (cookieToken) {
        $scope.isUserLogged = true;
        HavaSiteService.getAppUser().$promise.then(
                 function (result) {
                     $scope.UserName = result.data;
                 });

    }
    else
        $scope.isUserLogged = false;
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

    $scope.isNotColombo = function (chkb) {
        debugger;
        var checkbox = chkb.target;
        if (checkbox.checked) {
            $('#returnLocation').show();
        } else {
            $('#returnLocation').hide();
        }
    }

    $scope.gotoLogin = function (st) {
        if (st == true) {
            $scope.loginForm = true;
        } else {
            $scope.loginForm = false;
        }

    }

    $scope.Login = function (login) {
        $scope.submitted = true;

        if (login.loginUsername && login.loginPassword) {

            var loginData = {
                grant_type: 'password',
                username: login.loginUsername,
                password: login.loginPassword,
                client_id: 'HavaApp'
            };
            //window.location.href = appUrl;

            $http.post(appUrl + '/Token', $.param(loginData)).
            success(function (data, status, headers, config) {
                $scope.UserId = 0;
                $scope.UserName = data.userName;
                var expireTime = Date.now() + parseInt(data.refreshToken_timeout) * 60000;
                localStorageService.set('accessToken', data.access_token);
                localStorageService.set('refreshToken', data.refresh_token);
                localStorageService.set('refreshTokenTimeOut', parseInt(data.refreshToken_timeout));
                localStorageService.set('refreshOn', expireTime);
                $scope.isUserLogged = true;
                $scope.navigateSteps(4);

            }).
            error(function (data, status, headers, config) {
                $scope.invalidUserNamePassword = true;
            });

        } else {
        }
    }
    $scope.urlparms = $scope.parseQueryString(window.location.href);
    $scope.paramData = $scope.urlparms.P.split('S');
    $scope.PartnerIdTemp = $scope.paramData[0];
    $scope.siteIdTemp = $scope.paramData[1];

    HavaSiteService.getLocations({ id: parseInt($scope.PartnerIdTemp) }).$promise.then(
                 function (result) {
                     $scope.locations = result.data;
                 });

    HavaSiteService.getCardTypes().$promise.then(
             function (result) {
                 $scope.cardTypes = result.data;
             });

    HavaSiteService.getCountries().$promise.then(
             function (result) {
                 $scope.countries = result.data;
             });

    HavaSiteService.getPartnerIstest({ partnerId: parseInt($scope.PartnerIdTemp), siteId: parseInt($scope.siteIdTemp) }).$promise.then(
             function (result) {
                 $scope.siteDetails = result.data;
                 if ($scope.siteDetails && $scope.siteDetails.bannerPath != '') {
                     $("#slide-3 .bcg").css('cssText', 'background-image :url(/hava' + $scope.siteDetails.bannerPath + ') !important');

                 }

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
        debugger;
        if (($('#searchTextField').val() != "" && (($scope.dropLocation != undefined && $scope.dropLocation) || $scope.duration != undefined)) || ($('#searchTextField2').val() != "")) {
            $scope.isMain = false;
            debugger;
            $scope.search.pickupLocation = $('#searchTextField').val();
            $scope.search.dropLocation = $scope.dropLocation;
            //$scope.search.isAirportTransfer = $scope.isAirportTransfer;

            if ($scope.search.isAirportTransfer) {
                $scope.search.pickupDate = $('#inputGroupSuccessDate').val();
                $scope.search.pickupTime = $('#timepicker').val();
            }
            else {
                $scope.search.pickupDate = $('#inputGroupSuccessDate2').val();
                $scope.search.returnDate = $('#inputGroupSuccessDate3').val();
                $scope.search.pickupTime = $('#timepicker2').val();
                
                $scope.search.returnPickupLocation = $('#searchTextField3').val();

                var date2 = new Date($('#inputGroupSuccessDate3').val());
                var date1 = new Date($('#inputGroupSuccessDate2').val());
                var timeDiff = Math.abs(date2.getTime() - date1.getTime());
                var diffDays = Math.ceil(timeDiff / (1000 * 3600 * 24));
                var summary = diffDays + " Days - " + (diffDays * 100) + " Kilometers free.";
                document.getElementById("tripSummary").innerHTML = summary;
            }
            //if ($scope.isAirportTransfer)

            //else
            //    $scope.search.pickupTime = $('#timepicker2').val();

            var urlparms = $scope.parseQueryString(window.location.href);

            if ($scope.search.isAirportTransfer) {
                HavaSiteService.getProductDetails({ partnerId: parseInt($scope.PartnerIdTemp), locationId: ($scope.search.dropLocation != undefined) ? $scope.search.dropLocation.Id : 0, PromotionCode: ($scope.promotionCode != undefined) ? $scope.promotionCode : 0 }).$promise.then(
                         function (result) {
                             $scope.Products = result.data;

                         });
            } else {
                var promoCd = ($scope.promotionCode != undefined) ? $scope.promotionCode : 0;
                $scope.promotionCode
                // HavaSiteService.getChauffeurProductDetails({ partnerId: parseInt($scope.PartnerIdTemp), PromotionCode: ($scope.promotionCode != undefined) ? $scope.promotionCode : 0 }).$promise.then(
                HavaSiteService.getChauffeurProductDetails({ partnerId: parseInt($scope.PartnerIdTemp), pickupDate: ($scope.search.pickupDate), returnDate: ($scope.search.returnDate), PromotionCode: ($scope.promotionCode != undefined) ? $scope.promotionCode : 0 }).$promise.then(
                function (result) {
                    $scope.Products = result.data;

                });
            }
        } else
            return true;

    }

    $scope.step = 1;
    $scope.navigateSteps = function (stp) {
        $scope.step = stp;
        if (stp == 2) {
            HavaSiteService.getPartnerSubProducts({ partnerId: parseInt($scope.PartnerIdTemp) }).$promise.then(
                 function (result) {
                     if ($scope.bookingSubProducts == undefined || $scope.bookingSubProducts.length <= 0)
                         $scope.bookingSubProducts = result.data;
                 });
        }

    }

    $scope.whatClassIsIt = function (st) {
        if (st == $scope.step)
            return "active"
        else
            return "";
    }
    $scope.BookingOptions = {};
    $scope.BookingPayments = {};
    $scope.setBookingProduct = function (product) {
        debugger;
        $scope.selectedProduct = product;
        $scope.totalCost = angular.copy($scope.selectedProduct.PartnerSellingPrice);
        $scope.prodPrice = angular.copy($scope.selectedProduct.PartnerSellingPrice);

        $scope.AditionalDayUnit = $scope.selectedProduct.AdditionaDayRate != null ? $scope.selectedProduct.AdditionaDayRate : 0;
        $scope.AdHoursUnit = $scope.selectedProduct.AdditionaHourRate != null ? $scope.selectedProduct.AdditionaHourRate : 0;
        $scope.cildSetUnit = $scope.selectedProduct.ChildSeatRate != null ? $scope.selectedProduct.ChildSeatRate : 0;
        $scope.AdditionalKMRate = $scope.selectedProduct.AdditionalKMRate != null ? $scope.selectedProduct.AdditionalKMRate : 0;
        $scope.BookingOptions.PickupAddress = $scope.search.pickupLocation;
        // console.log(result.data[0]);

    }
    //calculate price
    $scope.calculateTotal = function (hours, sets) {
        var hrs = (hours != undefined && hours != "") ? parseInt(hours) : 0;
        var st = (sets != undefined && sets != "") ? parseInt(sets) : 0;
        var hrsTotal = 0, stTotal = 0, partPrice = 0;
        var hrsTotal = (hrs * parseInt($scope.selectedProduct.AdditionaHourRate));
        var partPrice = parseInt(angular.copy($scope.selectedProduct.PartnerSellingPrice));
        var stTotal = ($scope.selectedProduct.ChildSeatRate != null) ? (st * parseInt($scope.selectedProduct.ChildSeatRate)) : 0;
        $scope.totalCost = partPrice + hrsTotal + stTotal;
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
                    $scope.UserName = user.UserName;
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

    $scope.addPasengger = function (repData) {

        $scope.submittedRep = true;
        if (repData.FirstName != undefined && repData.FirstName != "")
            $scope.PassengerFirstNameRequired = false;
        else
            $scope.PassengerFirstNameRequired = true;
        if (repData.Mobile != undefined && repData.Mobile != "")
            $scope.PassengerMobileRequired = false;
        else
            $scope.PassengerMobileRequired = true;
        if (repData.Email != undefined && repData.Email != "")
            $scope.PassengerEmailRequired = false;
        else
            $scope.PassengerEmailRequired = true;
        if (repData.Country != undefined && repData.Country != "")
            $scope.PassengerCountryRequired = false;
        else
            $scope.PassengerCountryRequired = true;

        if ($scope.PassengerFirstNameRequired == false && $scope.PassengerMobileRequired != true && $scope.PassengerEmailRequired == false && $scope.PassengerCountryRequired == false) {
            {
                $scope.passengerGridData.push({
                    'FirstName': (repData.FirstName) ? repData.FirstName : null,
                    'Mobile': (repData.Mobile) ? repData.Mobile : null,
                    'Email': repData.Email,
                    'LastName': repData.LastName,
                    'Country': (repData.Country), // ? repData.Country.name : null,
                    'id': '-1' + Math.random()
                });
                $scope.pasenggers = {};
                $scope.pasenggers = null;
                repData.FirstName = "";
                repData.Mobile = "";
                repData.Email = "";
                repData.LastName = "";
                repData.Country = "";
            }
        }



        $scope.updateRep = false;
        $scope.updatePasengger = function (repData) {
            $scope.submittedRep = true;
            $scope.updateRep = true;
            if (repData.FirstName != undefined && repData.FirstName != "")
                $scope.PassengerFirstNameRequired = false;
            else
                $scope.PassengerFirstNameRequired = true;
            if (repData.Mobile != undefined && repData.Mobile != "")
                $scope.PassengerMobileRequired = false;
            else
                $scope.PassengerMobileRequired = true;
            if (repData.Email != undefined && repData.Email != "")
                $scope.PassengerEmailRequired = false;
            else
                $scope.PassengerEmailRequired = true;
            if (repData.Country != undefined && repData.Country != "")
                $scope.PassengerCountryRequired = false;
            else
                $scope.PassengerCountryRequired = true;

            if ($scope.PassengerFirstNameRequired == false && $scope.PassengerMobileRequired != true && $scope.PassengerEmailRequired == false && $scope.PassengerCountryRequired == false) {
                //  {
                var gridData = angular.copy($scope.passengerGridData);
                var rowIndex = null;
                angular.forEach(gridData, function (v, k) {
                    if (v.id == repData.id) {
                        rowIndex = k;
                    }
                });
                gridData[rowIndex] = repData;
                $scope.pasenggers = {};

                $scope.updateRep = false;
                $scope.passengerGridData = gridData;

            }
            $scope.updateRep = false;
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
            $scope.pasenggers = rData;
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

    $scope.cancelDelete = function () {
        $scope.viewTask = '';
    }

    $scope.saveBookingOptions = function (optionData) {

        var BookingSubProducts = [];
        angular.forEach($scope.bookingSubProducts, function (v, k) {
            debugger;
            BookingSubProducts.push({
                'Id': v.id,
                'HavaPrice': v.HavaPrice,
                'MarketPrice': v.MarketPrice,
                'Quantity': v.Quantity,
                'Price': v.Price,
                'ProductId' : v.productId
            });
        });
        optionData.BookingSubProducts = BookingSubProducts;
        $scope.bookingOptionData = optionData;
        $scope.navigateSteps(3);
    }

    $scope.passengerDetails = function (booking) {
        //$scope.PassengerFirstNameRequired = false;
        //$scope.PassengerMobileRequired = false;
        //$scope.PassengerEmailRequired = false;
        //$scope.PassengerCountryRequired = false;
        //debugger;
        //if (booking.PassengerFirstName != "" && booking.PassengerMobile != "" && booking.PassengerEmail != "" && booking.PassengerCountry != undefined && booking.PassengerCountry != "") {
        //    $scope.BookingOptions.PassengerFirstName = booking.PassengerFirstName;
        //    $scope.BookingOptions.PassengerMobile = booking.PassengerMobile;
        //    $scope.BookingOptions.PassengerEmail = booking.PassengerEmail;
        //    $scope.BookingOptions.PassengerCountry = booking.PassengerCountry;
        //    $scope.BookingOptions.PassengerLastName = booking.PassengerLastName;

        if ($scope.passengerGridData.length > 0) {
            debugger;
            var data = {
                "Id": 0,
                "BookingType": {
                    "Id": 1
                },
                "UserId": $scope.UserId,
                "UserName": $scope.UserName,
                'BookingProducts': [$scope.selectedProduct],
                "BookingStatu": {
                    "Id": 1,
                    "Name": "Pending",
                    "IsActive": true
                },
                "PickupLocation": $scope.search.pickupLocation,
                "PickupDate": $scope.search.pickupDate,
                "ReturnDate": $scope.search.returnDate,
                "PickupTime": $scope.search.pickupTime,
                "DropLocation": ($scope.dropLocation != undefined ? $scope.dropLocation : null),
                "BookingOptions": [$scope.bookingOptionData],
                "BookingPassenger": $scope.passengerGridData,
                "BookingSubProducts": $scope.bookingOptionData.BookingSubProducts,
                "ReturnPickupLocation": $scope.search.returnPickupLocation,
                //"BookingPayments": [{
                //    "CardHolderName": booking.CardHolderName,
                //    "CardNo": booking.CardNo,
                //    "ExpireDate": booking.ExpireDateMM + "/" + booking.ExpireDateYY,
                //    "CardType": booking.CardType.id,
                //    "CVV": booking.CVV
                //}],
                "BookingPayments": [$scope.BookingPayments],
                "UserConfirmed": true,
                "IsAirportTransfer": $scope.search.isAirportTransfer,
                "Partner": $scope.selectedProduct.Partner,
                "BookingType": {
                    "Id": 1,
                    "type": "Online",
                    "IsActive": true
                },
                "IsReturn": false,
                "TotalCost": $scope.totalCost
            }

            debugger;
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
            if (booking.PassengerFirstName == undefined || booking.PassengerFirstName == "")
                $scope.PassengerFirstNameRequired = true;
            if (booking.PassengerMobile == undefined || booking.PassengerMobile == "")
                $scope.PassengerMobileRequired = true;
            if (booking.PassengerEmail == undefined || booking.PassengerEmail == "")
                $scope.PassengerEmailRequired = true;
            if (booking.PassengerCountry == undefined || booking.PassengerCountry == "")
                $scope.PassengerCountryRequired = true;

        }
    }


    $scope.creditCardDetails = function (booking) {
        $scope.CardHolderNameRequired = false;
        $scope.CVVRequired = false;
        $scope.CardNoRequired = false;
        debugger;
        if (booking.CardHolderName != undefined && booking.CardHolderName != "" && booking.CardNo != undefined && booking.CardNo != "" && booking.CardType != undefined && booking.CardType != "") {
            if (($scope.urlparms.P) === parseInt(($scope.urlparms.P), 10)) {
                $scope.selectedProduct.Partner.Id = parseInt($scope.urlparms.P);
            }
            else {
                $scope.selectedProduct.Partner.Id = parseInt(($scope.urlparms.P).slice(0, -2));
            }


            if ($scope.totalCost > 0)
                $scope.selectedProduct.price = $scope.total;

            $scope.selectedProduct.AdditionalHours = $scope.additional.countAddHours;
            $scope.selectedProduct.NoOfChildSeats = $scope.additional.countAddhildSet;
            $scope.selectedProduct.AdditionalDays = $scope.additional.countAddDay;
            $scope.selectedProduct.AdditionalKM = $scope.additional.AdditionalKM;


            $scope.BookingPayments.CardHolderName = booking.CardHolderName;
            $scope.BookingPayments.CardNo = booking.CardNo;
            $scope.BookingPayments.ExpireDate = booking.ExpireDateMM + "/" + booking.ExpireDateYY;
            $scope.BookingPayments.CardType = booking.CardType.id;
            $scope.BookingPayments.CVV = booking.CVV;

            $scope.navigateSteps(5);
        } else {
            if (booking.CardHolderName == undefined || booking.CardHolderName == "")
                $scope.CardHolderNameRequired = true;
            if (booking.CardNo == undefined || booking.CardNo == "")
                $scope.CardNoRequired = true;
            if (booking.CardType == undefined || booking.CardHolderName == "")
                $scope.CVVRequired = true;

        }
    }

    $scope.getSubTotal = function (val) {
        return (val == null) ? 0 : val;
    }

    $scope.addCount = function (type, isAdd) {
        //  if (inp == 0) {
        if (type == "ah") {

            if (isAdd == "1") {
                if ($scope.additional.countAddHours <= 0)
                    $scope.additional.countAddHours = 0;
                else
                    $scope.additional.countAddHours++;
            }
            else {
                if ($scope.additional.countAddHours >= 0)
                    $scope.additional.countAddHours = 0;
                else
                    $scope.additional.countAddHours--;

            }


        }
        if (type == "ad") {

            if (isAdd == "1") {
                if ($scope.additional.countAddDay <= 0)
                    $scope.additional.countAddDay = 0;
                else
                    $scope.additional.countAddDay++;


            }
            else {
                if ($scope.additional.countAddDay >= 0)
                    $scope.additional.countAddDay = 0;
                else
                    $scope.additional.countAddDay--;


            }


        }
        if (type == "ac") {
            if (isAdd == "1") {
                if ($scope.additional.countAddhildSet <= 0)
                    $scope.additional.countAddhildSet = 0;
                else
                    $scope.additional.countAddhildSet++;


            }
            else {
                if ($scope.additional.countAddhildSet >= 0)
                    $scope.additional.countAddhildSet = 0;
                else
                    $scope.additional.countAddhildSet--;


            }



        }
        if (type == "km") {
            if (isAdd == "1") {
                if ($scope.additional.AdditionalKM <= 0)
                    $scope.additional.AdditionalKM = 0;
                else
                    $scope.additional.AdditionalKM++;


            }
            else {
                if ($scope.additional.AdditionalKM >= 0)
                    $scope.additional.AdditionalKM = 0;
                else
                    $scope.additional.AdditionalKM--;


            }



        }


        $scope.getTotal();
    }
    $scope.total = 0;
    $scope.getTotal = function () {

        var partPrice = 0;
        var partPrice = parseInt(angular.copy($scope.selectedProduct.PartnerSellingPrice));
        $scope.total = 0;
        if ($scope.bookingSubProducts) {
            angular.forEach($scope.bookingSubProducts, function (v, k) {
                $scope.total += ((v.Quantity != undefined ? v.Quantity : 0) * (v.MarketPrice != undefined ? v.MarketPrice : 0));
            });
        }
        // $scope.total = (parseInt($scope.additional.countAddDay) * parseInt($scope.AditionalDayUnit)) + (parseInt($scope.additional.countAddhildSet) * parseInt($scope.cildSetUnit)) + (parseInt($scope.additional.countAddHours) * parseInt($scope.AdHoursUnit)) + (parseInt($scope.additional.AdditionalKM) * parseInt($scope.AdditionalKMRate));
        if ($scope.selectedProduct.promotionAmount == null)
            $scope.selectedProduct.promotionAmount = 0;
        $scope.totalCost = partPrice + $scope.total - $scope.selectedProduct.promotionAmount;
        $scope.productPrice = partPrice - $scope.selectedProduct.promotionAmount;
    }


    angular.element(document).ready(function () {
        //$(function () {
        //    $("#datepicker").datepicker();
        //});
        var dateToday = new Date();
        //$('#timepicker').timepicker({
        //    startDate: dateToday,
        //    timeFormat: 'HH:mm:ss',
        //    showSecond: true,
        //    ampm: false
        //});
        //$('#timepicker2').timepicker({
        //    timeFormat: 'HH:mm:ss',
        //    startDate: dateToday,
        //    showSecond: true,
        //    ampm: false
        //});



        $('#inputGroupSuccessDate').datepicker({
            format: 'yyyy-mm-dd',
            startDate: dateToday,
            todayBtn: 'linked',
            autoclose: true,
            minDate: new Date()
        });

        $('#inputGroupSuccessDate2').datepicker({
            format: 'yyyy-mm-dd',
            startDate: dateToday,
            todayBtn: 'linked',
            autoclose: true,
            minDate: new Date()
        });

        $('#inputGroupSuccessDate3').datepicker({
            format: 'yyyy-mm-dd',
            startDate: dateToday,
            todayBtn: 'linked',
            autoclose: true,
            minDate: new Date()
        });
    });
    //logout function
    $scope.LogOut = function () {
        $scope.Login = {};
        localStorageService.remove('accessToken');
        localStorageService.remove('refreshToken');
        localStorageService.remove('refreshToken');
        localStorageService.remove('refreshTokenTimeOut');
        localStorageService.remove('refreshOn');

        localStorageService.remove('queries');
        $scope.isUserLogged = false;
        location.reload();
    }

    $scope.goToHistory = function () {
        window.location = appUrl + '#/' + 'booking/history';
    }

    $scope.MainLogin = function (login) {
        $scope.submitted = true;

        if ($scope.loginForm.$invalid == false) {

            var loginData = {
                grant_type: 'password',
                username: login.loginUsername,
                password: login.loginPassword,
                client_id: 'HavaApp'
            };
            //window.location.href = appUrl;

            $http.post(appUrl + '/Token', $.param(loginData)).
            success(function (data, status, headers, config) {
                var expireTime = Date.now() + parseInt(data.refreshToken_timeout) * 60000;
                localStorageService.set('accessToken', data.access_token);
                localStorageService.set('refreshToken', data.refresh_token);
                localStorageService.set('refreshTokenTimeOut', parseInt(data.refreshToken_timeout));
                localStorageService.set('refreshOn', expireTime);
                $scope.isUserLogged = true;
                location.reload();

            }).
            error(function (data, status, headers, config) {
                $scope.invalidUserNamePassword = true;
            });

        } else {
        }
    }


}]);


site.directive('onlyDigits', function ($filter) {
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

site.directive('changeOnBlur', function () {
    return {
        restrict: 'A',
        require: 'ngModel',
        link: function (scope, elm, attrs, ngModelCtrl) {
            if (attrs.type === 'radio' || attrs.type === 'checkbox')
                return;

            var expressionToCall = attrs.changeOnBlur;

            var oldValue = null;
            elm.bind('focus', function () {
                scope.$apply(function () {
                    oldValue = elm.val();
                    console.log(oldValue);
                });
            })
            elm.bind('blur', function () {
                scope.$apply(function () {
                    var newValue = elm.val();
                    console.log(newValue);
                    if (newValue !== oldValue) {
                        scope.$eval(expressionToCall);
                    }
                    //alert('changed ' + oldValue);
                });
            });
        }
    };
});
