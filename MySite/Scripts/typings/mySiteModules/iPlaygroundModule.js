/// <reference path="../angularjs/angular.d.ts" />
/// <reference path="../angularjs/angular-route.d.ts" />
/// <reference path="../jquery.cookie/jquery.cookie.d.ts" />
var iPlayGround;
(function (iPlayGround) {
    var Routes = (function () {
        function Routes() {
        }
        Routes.getLang = function () {
            var res = "/";
            var pathName = window.location.pathname;
            if (pathName) {
                res = pathName + (pathName[pathName.length - 1] !== "/" ? "/" : "");
                console && console.log(res);
            }
            return res;
        };
        Routes.configureRoutes = function ($routeProvider) {
            $routeProvider.when('/home', {
                templateUrl: function () { return Routes.getLang() + "Home/Index"; }
            });
            $routeProvider.when('/games', {
                templateUrl: function () { return Routes.getLang() + "Games"; }
            });
            $routeProvider.when('/games/GridMemory', {
                templateUrl: function () { return Routes.getLang() + "Games/GridMemory"; },
                controller: "iPlayGround.Services.GridMemoryController",
                controllerAs: "gridMemCtrl"
            });
            $routeProvider.when('/games/Mandala', {
                templateUrl: function () { return Routes.getLang() + "Games/Mandala"; },
                controller: "iPlayGround.Services.MandalaController",
                controllerAs: "mandalaCtrl"
            });
            $routeProvider.otherwise({ redirectTo: "/" });
        };
        Routes.$inject = ["$routeProvider"];
        return Routes;
    }());
    iPlayGround.Routes = Routes;
})(iPlayGround || (iPlayGround = {}));
(function () {
    angular.module("iPlaygroundApp", ['ngRoute', 'ngResource', 'ngTouch'])
        .config(iPlayGround.Routes.configureRoutes)
        .service(iPlayGround.Services.NavService.fullName, iPlayGround.Services.NavService)
        .controller(iPlayGround.Services.NavController.fullName, iPlayGround.Services.NavController)
        .service(iPlayGround.Services.GridMemoryService.fullName, iPlayGround.Services.GridMemoryService)
        .controller(iPlayGround.Services.GridMemoryController.fullName, iPlayGround.Services.GridMemoryController)
        .service(iPlayGround.Services.MandalaService.fullName, iPlayGround.Services.MandalaService)
        .controller(iPlayGround.Services.MandalaController.fullName, iPlayGround.Services.MandalaController);
    angular.module("iPlaygroundApp")
        .directive("mySpinner", function () {
        return {
            restrict: "E",
            scope: {
                ngOnRight: "&",
                ngOnLeft: "&",
                ngBdgClass: "@",
                ngCount: "="
            },
            replace: true,
            require: "^ngCount",
            templateUrl: "/Content/Templates/MySpinner.html"
        };
    });
})();
var Utils;
(function (Utils) {
    function switchAreas(hide, show, atLatsFn) {
        if (atLatsFn === void 0) { atLatsFn = null; }
        hide.removeClass("show").addClass("hidden");
        show.removeClass("hidden").addClass("show");
        if (atLatsFn) {
            atLatsFn();
        }
    }
    Utils.switchAreas = switchAreas;
    function getRandomInt(min, max) {
        return Math.floor(Math.random() * (max - min)) + min;
    }
    Utils.getRandomInt = getRandomInt;
    function format() {
        var args = [];
        for (var _i = 0; _i < arguments.length; _i++) {
            args[_i - 0] = arguments[_i];
        }
        // The string containing the format items (e.g. "{0}")
        // will and always has to be the first argument.
        var theString = args[0];
        // start with the second argument (i = 1)
        for (var i = 1; i < args.length; i++) {
            // "gm" = RegEx options for Global search (more than one instance)
            // and for Multiline search
            var regEx = new RegExp("\\{" + (i - 1) + "\\}", "gm");
            theString = theString.replace(regEx, args[i]);
        }
        return theString;
    }
    Utils.format = format;
    var playgroundCookieKey = "PlaygroundOptions";
    function save(name, value) {
        var options = {};
        var cook = $.cookie(playgroundCookieKey);
        if (cook) {
            options = JSON.parse(cook);
        }
        options[name] = value;
        $.cookie(playgroundCookieKey, JSON.stringify(options));
    }
    Utils.save = save;
    function load(name, defVal) {
        var cook = $.cookie(playgroundCookieKey);
        if (cook) {
            var options = JSON.parse(cook);
            if (options) {
                var ret = options[name];
                if (ret || ret === 0 || ret === "") {
                    return ret;
                }
            }
        }
        return defVal;
    }
    Utils.load = load;
})(Utils || (Utils = {}));
//# sourceMappingURL=iPlaygroundModule.js.map