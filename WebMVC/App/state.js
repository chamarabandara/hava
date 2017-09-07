app.constant('_START_REQUEST_', '_START_REQUEST_');
app.constant('_END_REQUEST_', '_END_REQUEST_');
//page number length

app.config(['$stateProvider', '$httpProvider', '$urlRouterProvider', '_START_REQUEST_', '_END_REQUEST_', '$ocLazyLoadProvider', function ($stateProvider, $httpProvider, $urlRouterProvider, _START_REQUEST_, _END_REQUEST_, $ocLazyLoadProvider) {
    //  $httpProvider.defaults.headers.post['Content-Type'] = 'application/x-www-form-urlencoded;charset=utf-8';
    var $http,
    interceptor = ['$q', '$injector', function ($q, $injector) {
        $urlRouterProvider.otherwise('/error404');

        var rootScope;

        var stateParams = $injector.get('$stateParams');
        return {
            request: function (config) {
                // get $http via $injector because of circular dependency problem
                $http = $http || $injector.get('$http');
                var reqUrl = config.url.split('/');
                var lastString = reqUrl[reqUrl.length - 1];
                //// don't send notification until all requests are complete
                if ($http.pendingRequests.length < 1 && lastString != 'Validate') {
                    // && apiControllerName != 'common' && apiControllerName != 'GetPeopleForNavBar'
                    // get $rootScope via $injector because of circular dependency problem
                    rootScope = rootScope || $injector.get('$rootScope');
                    // send a notification requests are complete
                    if (!rootScope.eventStartd) {
                        rootScope.$broadcast(_START_REQUEST_);

                    }
                    //rootScope.test = true;
                }
                return config;
            },
            response: function (response) {
                // get $http via $injector because of circular dependency problem
                $http = $http || $injector.get('$http');
                // don't send notification until all requests are complete

                // get $rootScope via $injector because of circular dependency problem
                rootScope = rootScope || $injector.get('$rootScope');

                if ($http.pendingRequests.length < 1) {

                    rootScope.$broadcast(_END_REQUEST_);

                } else {
                    rootScope.$broadcast(_START_REQUEST_);
                }

                return response;
            }
        }
    }];
    $ocLazyLoadProvider.config({
        debug: false,
        modules: [
         //controller modules
           { name: 'HomeCtrl', serie: true, files: ['App/Controllers/home.controller.js?v=' + jsVersion] },
           { name: 'partnerControllers', serie: true, files: ['App/Controllers/partner.controller.js?v=' + jsVersion] },
           { name: 'NavBarControllers', serie: true, files: ['App/Controllers/navbar.controller.js?v=' + jsVersion] },
           { name: 'productControllers', serie: true, files: ['App/Controllers/product.controller.js?v=' + jsVersion] },
           { name: 'sitesControllers', serie: true, files: ['App/Controllers/sites.controller.js?v=' + jsVersion] },
           { name: 'tspControllers', serie: true, files: ['App/Controllers/tsp.controller.js?v=' + jsVersion] },
            { name: 'bookingControllers', serie: true, files: ['App/Controllers/booking.controller.js?v=' + jsVersion] },
            { name: 'filterString', files: ['App/filterstring.js?v=' + jsVersion] },
       //service modules  
         { name: 'partnerService', serie: true, files: ['App/Services/partnerservice.js?v=' + jsVersion] },
           { name: 'navBarService', serie: true, files: ['App/Services/navbarservice.js?v=' + jsVersion] },
           { name: 'siteService', serie: true, files: ['App/Services/siteservice.js?v=' + jsVersion] },
           { name: 'tspService', serie: true, files: ['App/Services/tspservice.js?v=' + jsVersion] }

        ]
    });
    $httpProvider.interceptors.push('authHttpResponseInterceptor');
    $httpProvider.interceptors.push(interceptor);

    $urlRouterProvider.when('', '/home');
    $stateProvider
        .state('app', {
            url: '',
            abstract: true,
            controller: ['$ocLazyLoad', function ($ocLazyLoad) {

                return $ocLazyLoad.load('HomeCtrl');
            }],
            templateUrl: 'Layouts/index',
            views: {
                'navbar': {
                    templateUrl: 'Widgets/navbar',
                    controller: 'NavBarCtrl',
                    resolve: {
                        loadMyController: ['$ocLazyLoad', '$stateParams', function ($ocLazyLoad, $stateParams) {
                            return $ocLazyLoad.load('NavBarControllers');
                        }],
                        loadMyService: ['$ocLazyLoad', '$stateParams', function ($ocLazyLoad, $stateParams) {
                            return $ocLazyLoad.load(
                                [
                                    'navBarService', 'filterString'
                                ]);
                        }]
                    }

                },
                'main': {
                    template: '<div class="col-sm-12"><div class="" ui-view></div></div>'
                },
               
            },
            resolve: {
                loadMyController: ['$ocLazyLoad', '$stateParams', function ($ocLazyLoad, $stateParams) {
                    //return $ocLazyLoad.load(['TilesCtrl', 'NavBarCtrl', 'StaticsCtrl']);
                }],
                loadMyService: ['$ocLazyLoad', '$stateParams', function ($ocLazyLoad, $stateParams) {

                }]
            }
        })
      .state('app.home', {
          url: '/home',
          controller: ['$ocLazyLoad', function ($ocLazyLoad) {

              return $ocLazyLoad.load('HomeCtrl');
          }],
          templateUrl: 'Layouts/index',
      })
        .state('app.login', {
            url: '/login',
            controller: function ($stateParams) { window.location = 'home/Login' },
            templateUrl: 'App/Views/Users/list.html',
        })
         .state('app.site', {
            url: '/sites-{id:string}',
           // controller: function ($scope, $stateParams, $controller) {
              //  alert($stateParams.);
                //alert($stateParams.url);
              //  window.location = 'home/Sites?P=' + $stateParams.id;
          //  },
            controller: function ($scope, $stateParams, $controller) { $controller("SitesManageCtrl", { $scope: $scope }); },
            resolve: {
                loadMyController: ['$ocLazyLoad', '$stateParams', function ($ocLazyLoad, $stateParams) {
                    return $ocLazyLoad.load('sitesControllers');
                }],
                loadMyService: ['$ocLazyLoad', '$stateParams', function ($ocLazyLoad, $stateParams) {
                    return $ocLazyLoad.load(['siteService']);
                }]
            },

            templateUrl: 'App/Views/Users/list.html',
        })
     .state('app.partner', {
         url: '/partner',
         templateUrl: 'Partner/Index',
     })
      .state('app.partner.list', {
          url: '/list',
          templateUrl: 'Partner/List',
          controller: function ($scope, $stateParams, $controller) { $controller("PartnerCtrl", { $scope: $scope }); },
          resolve: {
              loadMyController: ['$ocLazyLoad', '$stateParams', function ($ocLazyLoad, $stateParams) {
                  return $ocLazyLoad.load('partnerControllers');
              }],
              loadMyService: ['$ocLazyLoad', '$stateParams', function ($ocLazyLoad, $stateParams) {
                  return $ocLazyLoad.load(['partnerService']);
              }]
          }

      })
        
         //.state('app.partner.view', {
         //    abstract: true,
         //    url: '/{id:int}/view',
         //    templateUrl: 'Partner/PartnerView',
         //    controller: function ($scope, $stateParams, $controller) { $controller("PartnerCtrl", { $scope: $scope }); },
         //    resolve: {
         //        loadMyController: ['$ocLazyLoad', '$stateParams', function ($ocLazyLoad, $stateParams) {
         //            return $ocLazyLoad.load('partnerControllers');
         //        }],
         //        loadMyService: ['$ocLazyLoad', '$stateParams', function ($ocLazyLoad, $stateParams) {
         //            return $ocLazyLoad.load(['partnerService']);
         //        }],
                 
         //    }
         //})
        .state('app.partner.add', {
            url: '/form',
            templateUrl: 'Partner/Add',
            controller: function ($scope, $stateParams, $controller) { $controller("PartnerCreateCtrl", { $scope: $scope }); },
            resolve: {
                loadMyController: ['$ocLazyLoad', '$stateParams', function ($ocLazyLoad, $stateParams) {
                    return $ocLazyLoad.load('partnerControllers');
                }],
                loadMyService: ['$ocLazyLoad', '$stateParams', function ($ocLazyLoad, $stateParams) {
                    return $ocLazyLoad.load(['partnerService']);
                }],
                loadMyFilter: ['$ocLazyLoad', '$stateParams', function ($ocLazyLoad, $stateParams) {
                    return $ocLazyLoad.load(['filterString']);
                }]
               
            }

        })
      .state('app.partner.update', {
          url: '/{id:int}/update',
          templateUrl: 'Partner/Add',
          controller: function ($scope, $stateParams, $controller) { $controller("PartnerCreateCtrl", { $scope: $scope }); },
          resolve: {
              loadMyController: ['$ocLazyLoad', '$stateParams', function ($ocLazyLoad, $stateParams) {
                  return $ocLazyLoad.load('partnerControllers');
              }],
              loadMyService: ['$ocLazyLoad', '$stateParams', function ($ocLazyLoad, $stateParams) {
                     return $ocLazyLoad.load(['partnerService']);
              }],
              loadMyFilter: ['$ocLazyLoad', '$stateParams', function ($ocLazyLoad, $stateParams) {
                  return $ocLazyLoad.load(['filterString']);
              }]
          }

      })
        .state('app.tsp', {
            url: '/tsp',
            templateUrl: 'TSP/Index',
        })
      .state('app.tsp.list', {
          url: '/list',
          templateUrl: 'TSP/List',
          controller: function ($scope, $stateParams, $controller) { $controller("TSPCtrl", { $scope: $scope }); },
          resolve: {
              loadMyController: ['$ocLazyLoad', '$stateParams', function ($ocLazyLoad, $stateParams) {
                  return $ocLazyLoad.load('tspControllers');
              }],
              loadMyService: ['$ocLazyLoad', '$stateParams', function ($ocLazyLoad, $stateParams) {
                  return $ocLazyLoad.load(['tspService']);
              }]
          }

      })

        .state('app.tsp.add', {
            url: '/form',
            templateUrl: 'TSP/Add',
            controller: function ($scope, $stateParams, $controller) { $controller("TSPCreateCtrl", { $scope: $scope }); },
            resolve: {
                loadMyController: ['$ocLazyLoad', '$stateParams', function ($ocLazyLoad, $stateParams) {
                    return $ocLazyLoad.load('tspControllers');
                }],
                loadMyService: ['$ocLazyLoad', '$stateParams', function ($ocLazyLoad, $stateParams) {
                    return $ocLazyLoad.load(['tspService']);
                }],

            }

        })
      .state('app.tsp.update', {
          url: '/{id:int}/update',
          templateUrl: 'TSP/Add',
          controller: function ($scope, $stateParams, $controller) { $controller("TSPCreateCtrl", { $scope: $scope }); },
          resolve: {
              loadMyController: ['$ocLazyLoad', '$stateParams', function ($ocLazyLoad, $stateParams) {
                  return $ocLazyLoad.load('tspControllers');
              }],
              loadMyService: ['$ocLazyLoad', '$stateParams', function ($ocLazyLoad, $stateParams) {
                  return $ocLazyLoad.load(['tspService']);
              }],
              //loadMyFilter: ['$ocLazyLoad', '$stateParams', function ($ocLazyLoad, $stateParams) {
              //    return $ocLazyLoad.load(['filterString']);
              //}]
          }

      })
        .state('app.tsp.view', {
            url: '/{id:int}/view',
            templateUrl: 'TSP/TSPView',
            controller: function ($scope, $stateParams, $controller) { $controller("TSPViewCtrl", { $scope: $scope }); },
            resolve: {
                loadMyController: ['$ocLazyLoad', '$stateParams', function ($ocLazyLoad, $stateParams) {
                    return $ocLazyLoad.load('tspControllers');
                }],
                loadMyService: ['$ocLazyLoad', '$stateParams', function ($ocLazyLoad, $stateParams) {
                    return $ocLazyLoad.load(['tspService']);
                }],

            }
        })
         .state('app.product', {
             url: '/product',
             templateUrl: 'Product/Index',
         })
      .state('app.product.list', {
          url: '/list',
          templateUrl: 'Product/List',
          controller: function ($scope, $stateParams, $controller) { $controller("ProductListCtrl", { $scope: $scope }); },
          resolve: {
              loadMyController: ['$ocLazyLoad', '$stateParams', function ($ocLazyLoad, $stateParams) {
                  return $ocLazyLoad.load('productControllers');
              }],
              loadMyService: ['$ocLazyLoad', '$stateParams', function ($ocLazyLoad, $stateParams) {
                  return $ocLazyLoad.load(['partnerService']);
              }]
          }

      })

         .state('app.product.view', {
             url: '/{id:int}/view',
             templateUrl: 'Product/ProductView',
             controller: function ($scope, $stateParams, $controller) { $controller("ProductCreateCtrl", { $scope: $scope }); },
             resolve: {
                 loadMyController: ['$ocLazyLoad', '$stateParams', function ($ocLazyLoad, $stateParams) {
                     return $ocLazyLoad.load('productControllers');
                 }],
                 loadMyService: ['$ocLazyLoad', '$stateParams', function ($ocLazyLoad, $stateParams) {
                     return $ocLazyLoad.load(['partnerService']);
                 }],

             }
         })
        .state('app.product.add', {
            url: '/form',
            templateUrl: 'Product/Add',
            controller: function ($scope, $stateParams, $controller) { $controller("ProductCreateCtrl", { $scope: $scope }); },
            resolve: {
                loadMyController: ['$ocLazyLoad', '$stateParams', function ($ocLazyLoad, $stateParams) {
                    return $ocLazyLoad.load('productControllers');
                }],
                loadMyService: ['$ocLazyLoad', '$stateParams', function ($ocLazyLoad, $stateParams) {
                    return $ocLazyLoad.load(['partnerService']);
                }],

            }

        })
      .state('app.product.update', {
          url: '/{id:int}/update',
          templateUrl: 'Product/Add',
          controller: function ($scope, $stateParams, $controller) { $controller("PartnerCreateCtrl", { $scope: $scope }); },
          resolve: {
              loadMyController: ['$ocLazyLoad', '$stateParams', function ($ocLazyLoad, $stateParams) {
                  return $ocLazyLoad.load('productControllers');
              }],
              loadMyService: ['$ocLazyLoad', '$stateParams', function ($ocLazyLoad, $stateParams) {
                  return $ocLazyLoad.load(['partnerService']);
              }],
              //loadMyFilter: ['$ocLazyLoad', '$stateParams', function ($ocLazyLoad, $stateParams) {
              //    return $ocLazyLoad.load(['filterString']);
              //}]
          }

      })
         .state('app.sites', {
             url: '/sites',
             templateUrl: 'Sites/Index',
         })
      .state('app.sites.list', {
          url: '/list',
          templateUrl: 'Sites/List',
          controller: function ($scope, $stateParams, $controller) { $controller("SitesListCtrl", { $scope: $scope }); },
          resolve: {
              loadMyController: ['$ocLazyLoad', '$stateParams', function ($ocLazyLoad, $stateParams) {
                  return $ocLazyLoad.load('sitesControllers');
              }],
              loadMyService: ['$ocLazyLoad', '$stateParams', function ($ocLazyLoad, $stateParams) {
                  return $ocLazyLoad.load(['siteService']);
              }]
          }

      })

         .state('app.sites.view', {
             url: '/{id:int}/view',
             templateUrl: 'Sites/SitesView',
             controller: function ($scope, $stateParams, $controller) { $controller("SitesCreateCtrl", { $scope: $scope }); },
             resolve: {
                 loadMyController: ['$ocLazyLoad', '$stateParams', function ($ocLazyLoad, $stateParams) {
                     return $ocLazyLoad.load('sitesControllers');
                 }],
                 loadMyService: ['$ocLazyLoad', '$stateParams', function ($ocLazyLoad, $stateParams) {
                     return $ocLazyLoad.load(['siteService']);
                 }],

             }
         })
        .state('app.sites.add', {
            url: '/form',
            templateUrl: 'Sites/Add',
            controller: function ($scope, $stateParams, $controller) { $controller("SitesCreateCtrl", { $scope: $scope }); },
            resolve: {
                loadMyController: ['$ocLazyLoad', '$stateParams', function ($ocLazyLoad, $stateParams) {
                    return $ocLazyLoad.load('sitesControllers');
                }],
                loadMyService: ['$ocLazyLoad', '$stateParams', function ($ocLazyLoad, $stateParams) {
                    return $ocLazyLoad.load(['siteService']);
                }],

            }

        })
      .state('app.sites.update', {
          url: '/{id:int}/update',
          templateUrl: 'Sites/Add',
          controller: function ($scope, $stateParams, $controller) { $controller("SitesCreateCtrl", { $scope: $scope }); },
          resolve: {
              loadMyController: ['$ocLazyLoad', '$stateParams', function ($ocLazyLoad, $stateParams) {
                  return $ocLazyLoad.load('sitesControllers');
              }],
              loadMyService: ['$ocLazyLoad', '$stateParams', function ($ocLazyLoad, $stateParams) {
                  return $ocLazyLoad.load(['siteService']);
              }],
              //loadMyFilter: ['$ocLazyLoad', '$stateParams', function ($ocLazyLoad, $stateParams) {
              //    return $ocLazyLoad.load(['filterString']);
              //}]
          }

      })
         .state('app.booking', {
             url: '/booking',
             templateUrl: 'Booking/Index',
         })
      .state('app.booking.list', {
          url: '/list',
          templateUrl: 'Booking/List',
          controller: function ($scope, $stateParams, $controller) { $controller("BookingCtrl", { $scope: $scope }); },
          resolve: {
              loadMyController: ['$ocLazyLoad', '$stateParams', function ($ocLazyLoad, $stateParams) {
                  return $ocLazyLoad.load('bookingControllers');
              }],
              loadMyService: ['$ocLazyLoad', '$stateParams', function ($ocLazyLoad, $stateParams) {
                  return $ocLazyLoad.load(['siteService']);
              }]
          }

      })
    .state('app.error', {
        template: '<div class="col-sm-12"><div class="" ui-view></div></div>'
    })
    .state('app.error.list404', {
        url: '/error404',
        templateUrl: 'Errors/Error404',
        controller: function ($scope, $stateParams, $controller) {
        },
    })
     .state('app.error.list500', {
         url: '/error500',
         templateUrl: 'Errors/Error500',
         controller: function ($scope, $stateParams, $controller) {
         },
     })
}]);




login.config(['$stateProvider', function ($stateProvider) {
    $stateProvider
        .state('login', {
            url: '',
            controller: 'LoginCtrl'
        })
}]);

app.factory('authHttpResponseInterceptor', ['$q', '$location', '$cookies', '$rootScope', '_END_REQUEST_', '$injector','localStorageService', function ($q, $location, $cookies, $rootScope, _END_REQUEST_, $injector, localStorageService) {

    return {
        response: function (response) {
            if (response.status === 401) {
                console.log("Response 401");
            }
            return response || $q.when(response);
        },
        responseError: function (rejection) {

            if (rejection.status === 401) {
                console.log("Response Error 401", rejection);
                localStorageService.remove('accessToken');
                localStorageService.remove('refreshToken');
                 delete $cookies.accessToken;
                 delete $cookies.refreshToken;
                $location.path('/login').search('returnTo', $location.path());

            } else if (rejection.status === 404) {
                $rootScope.$broadcast(_END_REQUEST_);
                var stateService = $injector.get('$state');
                stateService.go('app.error.list404');
            }
            else if (rejection.status === 500) {
                $rootScope.$broadcast(_END_REQUEST_);
                $('#500errorsDialog').modal('show');
            }
            else if (rejection.status === 403) {
                $rootScope.$broadcast(_END_REQUEST_);
                // $('#500errorsDialog').modal('show');
            }
            return $q.reject(rejection);
        }
    }
}])
