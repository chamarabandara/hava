angular.module('filters', []).
    filter('truncate', function () {
        return function (text, length, end) {
            if (isNaN(length))
                length = 10;

            if (end === undefined)
                end = "...";

            if (text && (text.length <= length || text.length - end.length <= length)) {
                return text;
            }
            else {
                return String(text).substring(0, length - end.length) + end;
            }

        };
    }).
    filter('percentage', ['$filter', function ($filter) {
        return function (input, decimals) {
            if (input != null) {
                return $filter('number')(input, decimals) + '%';
                ///return input + '%';
            }
        };
    }]).
    filter('htmlToPlaintext', function () {
        return function (text) {
            return text ? String(text).replace(/<[^>]+>/gm, '').replace(/&[^;]+;/g, ' ') : '';
        };
    })
    .filter('numberFixedLen', function () {
        return function (n, len) {
            var num = parseInt(n, 10);
            len = parseInt(len, 10);
            if (isNaN(num) || isNaN(len)) {
                return n;
            }
            num = '' + num;
            while (num.length < len) {
                num = '0' + num;
            }
            return num;
        };
    }).

    filter("toArray", function () {
        return function (obj) {
            var result = [];
            angular.forEach(obj, function (val, key) {
                result.push(val);
            });
            return result;
        };
    });