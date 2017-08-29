app.directive("ngFileSelected", function () {
    return {
        link: function ($scope, el) {
            el.bind("change", function (e) {
                $scope.file = (e.srcElement || e.target).files[0];

                if (e.target) {
                    var tName = e.target.name;
                } else {
                    var tName = e.srcElement.name;
                }

                $scope.getFile(tName);


            });
        }
    }

});
//app.directive('googleplace', function () {
//    return {
//        require: 'ngModel',
//        link: function (scope, element, attrs, model) {
//            var options = {
//                types: [],
//                componentRestrictions: {country:'LK'}
//            };
//            scope.gPlace = new google.maps.places.Autocomplete(element[0], options);

//            google.maps.event.addListener(scope.gPlace, 'place_changed', function () {
//                scope.$apply(function () {
//                    model.$setViewValue(element.val());
//                });
//            });
//        }
//    };
//});
//app.directive('googleplace', [function () {
//    return {
//        require: 'ngModel',
//        scope: {
//            ngModel: '=',
//            details: '=?'
//        },
//        link: function (scope, element, attrs, model) {
//            var colomboBounds = new google.maps.LatLngBounds(
           
//              new google.maps.LatLng(6.8746946, 79.8933806),
//                 new google.maps.LatLng(6.932339, 79.845226));
//            var options = {
//                bounds: colomboBounds,
//               strictBounds: true,
//                componentRestrictions: { country: 'LK' }
//            };

//            scope.gPlace = new google.maps.places.Autocomplete(element[0], options);

//            google.maps.event.addListener(scope.gPlace, 'place_changed', function () {
//                var geoComponents = scope.gPlace.getPlace();
//                var latitude = geoComponents.geometry.location.lat();
//                var longitude = geoComponents.geometry.location.lng();
//                console.log(latitude);
//                console.log(longitude);
//                var addressComponents = geoComponents.address_components;

//                addressComponents = addressComponents.filter(function (component) {
//                    console.log(component);
//                    switch (component.types[0]) {
//                        case "locality": // city
//                            return true;
//                        case "administrative_area_level_1": // state
//                            return true;
//                        case "country": // country
//                            return true;
//                        default:
//                            return false;
//                    }
//                }).map(function (obj) {
//                    return obj.long_name;
//                });

//                addressComponents.push(latitude, longitude);

//                scope.$apply(function () {
//                    scope.details = addressComponents; // array containing each location component
//                    model.$setViewValue(element.val());
//                });
//            });
//        }
//    };
//}]);

app.directive('format', ['$filter', function ($filter) {
    return {
        require: '?ngModel',
        link: function (scope, elem, attrs, ctrl) {
            if (!ctrl) return;
            ctrl.$formatters.unshift(function (a) {
                return $filter(attrs.format)(ctrl.$modelValue, attrs.arg)
            });


            ctrl.$parsers.unshift(function (viewValue) {
                var plainNumber = viewValue.replace(/[^\d|\-+|\.]/g, '');
                //var plainNumber = viewValue.replace(/[^\d.-] | [^\d,-]/g, '');
                //var test = plainNumber.replace('.', ',').replace(/[^0-9.]+/, '');
                //var plainNumber = viewValue.replace(/^(?:\-?\d+\.\d{2}[, ]*)+|(?:\-?\d+\,\d{2}[, ]*)+$/g, '');
                //var temp = viewValue.replace(/^(?:\-?\d+\.\d{2}[, ]*)+|(?:\-?\d+\,\d{2}[, ]*)+$/g, '');
                elem.val($filter(attrs.format)(plainNumber));
                return plainNumber;
            });
        }
    };
}]);
