var getCookie = function (cname) {
    var name = cname + "=";
    var ca = document.cookie.split(';');
    for (var i = 0; i < ca.length; i++) {
        var c = ca[i];
        while (c.charAt(0) == ' ') c = c.substring(1);
        if (c.indexOf(name) == 0) return c.substring(name.length, c.length);
    }
    return "";
}
var apiUrl = 'http://localhost/havadip/';
var tokenKey = 'accessToken';
var refreshTokenKey = 'refreshToken';
var appUrl = 'http://localhost/hava/';
var loginUrl = appUrl + 'home/Login';
var token = sessionStorage.getItem(tokenKey);
var translateKey = getCookie("translateKey");
var cookieToken = '';
var googleAPIUrl = 'https://www.googleapis.com/language/translate/v2';
var gridPagingSize = { 'mainGrid': 50, 'popupGrid': 25, 'pageGrid': 15, 'popupClaimGrid': 15, 'maxPageSize': 1000 };
var DNNClaimUrl = getCookie("DNNClaimUrl");
var jsVersion = getCookie("jsVersion");
defaultCountryId = getCookie("defaultCountry");
defaultLanguageId = getCookie("defaultLanguage");
defaultInsurerId = getCookie("defaultInsurerId");

