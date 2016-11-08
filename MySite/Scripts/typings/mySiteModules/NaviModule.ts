/// <reference path="../angularjs/angular.d.ts" />
module iPlayGround.Services {
    export class NavLang {
        key: string;
        url: string;
    }

    export class NavService {
        public static fullName: string = "iPlayGround.Services.NavService";
        location: ng.ILocationService;
        log: ng.ILogService;
        langs: Array<NavLang>;
        static $inject = ["$location", "$log"];
        constructor($location: ng.ILocationService, $log: ng.ILogService) {
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

        getCurrent() {
        }

        goTo(nav: NavLang) {
            window.location.assign(nav.url + window.location.hash);
        }
    }
    export class NavController {
        public static fullName: string = "iPlayGround.Services.NavController";
        service: NavService;
        log: ng.ILogService;
        currentLang: NavLang;
        static $inject = [NavService.fullName, '$log'];
        constructor(service: NavService, $log: ng.ILogService) {
            this.service = service;
            this.log = $log;
            this.init();
        }

        init() {
            this.currentLang = this.service.langs[0];
        }

        goTop() {
            this.service.location.url("/");
            //this.service.goTo(this.currentLang);
        }

        changeLang(lang: NavLang) {
            this.currentLang = lang;
            this.service.goTo(this.currentLang);
        }
    }
}