/// <reference path="../angularjs/angular.d.ts" />
var iPlayGround;
(function (iPlayGround) {
    var Services;
    (function (Services) {
        var NavLang = (function () {
            function NavLang() {
            }
            return NavLang;
        }());
        Services.NavLang = NavLang;
        var NavService = (function () {
            function NavService($location, $log) {
                this.location = $location;
                this.log = $log;
                this.langs = [
                    {
                        key: "en",
                        url: "/"
                    },
                    {
                        key: "ru",
                        url: "/ru-ru"
                    }
                ];
            }
            NavService.prototype.getCurrent = function () {
            };
            NavService.prototype.goTo = function (nav) {
                window.location.assign(nav.url + window.location.hash);
            };
            NavService.fullName = "iPlayGround.Services.NavService";
            NavService.$inject = ["$location", "$log"];
            return NavService;
        }());
        Services.NavService = NavService;
        var NavController = (function () {
            function NavController(service, $log) {
                this.service = service;
                this.log = $log;
                this.init();
            }
            NavController.prototype.init = function () {
                this.currentLang = this.service.langs[0];
            };
            NavController.prototype.goTop = function () {
                this.service.location.url("/");
                //this.service.goTo(this.currentLang);
            };
            NavController.prototype.changeLang = function (lang) {
                this.currentLang = lang;
                this.service.goTo(this.currentLang);
            };
            NavController.fullName = "iPlayGround.Services.NavController";
            NavController.$inject = [NavService.fullName, '$log'];
            return NavController;
        }());
        Services.NavController = NavController;
    })(Services = iPlayGround.Services || (iPlayGround.Services = {}));
})(iPlayGround || (iPlayGround = {}));
//# sourceMappingURL=NaviModule.js.map