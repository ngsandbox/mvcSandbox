/// <reference path="../angularjs/angular.d.ts" />
/// <reference path="../angularjs/angular-resource.d.ts" />
module iPlayGround.Services {
    export class MandalaService {
        public static get gridMemSecondsKey(): string { return "MandalaSeconds"; }
        public static get gridMemLimitKey(): string { return "MandalaLimit"; }
        public static fullName: string = "iPlayGround.Services.MandalaService";
        ngResource: ng.resource.IResourceService;
        location: ng.ILocationService;
        title: string = "Mandala";
        question: string = "";
        questionTemplate: string = "";
        limit: number = 6;
        seconds: number = 2;
        static $inject = ["$resource", "$location"];
        constructor($resource: ng.resource.IResourceService, $location: ng.ILocationService) {
            this.ngResource = $resource;
            this.location = $location;
        }
        init(): void {
            this.question = "",
            this.questionTemplate = $("span[my-gmQuestion]").attr('my-gmQuestion');
            this.limit = Utils.load(MandalaService.gridMemLimitKey, 6);
            this.seconds = Utils.load(MandalaService.gridMemSecondsKey, 10);
        }

        getFiles(): FileInfo[] {
            var files = this.ngResource(Routes.getLang() + "Games/GetCards?limit=:limit", { limit: this.limit }).query();
            return <any>(files);
        }

        calcCardColsCount(limit) {
            var result = (limit < 5) ? 6 :
                (limit < 7) ? 4 :
                    (limit < 13) ? 3 :
                        (limit < 25) ? 2 : 1;
            return result;
        }

        setTimeout(sec): void {
            Utils.save(MandalaService.gridMemSecondsKey, sec);
        }

        setLimit(limit) {
            Utils.save(MandalaService.gridMemLimitKey, limit);
        }
    }

    export class MandalaController {
        private static GridCardColumnClasses: string = "col-lg-{0} col-md-{0} col-sm-{0} col-xs-{0}";
        public static fullName: string = "iPlayGround.Services.MandalaController";
        service: MandalaService;
        log: ng.ILogService;
        timeout: ng.ITimeoutService;
        startTimeoutHndl: angular.IPromise<any>;

        start: boolean = false;
        currentFile: FileInfo = null;
        cardClasses: string = "";
        files: FileInfo[] = [];
        countSuc: number = 0;
        countErr: number = 0;
        static $inject = [MandalaService.fullName, '$timeout', '$log'];
        constructor(service: MandalaService, $timeout: ng.ITimeoutService, $log: ng.ILogService) {
            this.service = service;
            this.timeout = $timeout;
            this.log = $log;
            this.init();
        }

        init() {
            if (this.startTimeoutHndl) {
                this.timeout.cancel(this.startTimeoutHndl);
                this.startTimeoutHndl = null;
            }

            this.start = false;
            this.service.init();
            this.files = this.service.getFiles();
            this.currentFile = null;
            this.calcCardClasses();
            this.countSuc = this.countErr = 0;
            this.startTimeoutHndl = this.timeout(() => {
                Utils.switchAreas($(".card .front"), $(".card .back"));
                this.start = true;
            }, this.service.seconds * 1000);
        }

        flipCard(file: FileInfo, $event: any) {
            if (file.inAction) {
                return;
            }

            file.inAction = true;
            this.log.log(file);
            if (!this.start || this.files.indexOf(file) < 0) {
                return;
            }

            var back = $($event.currentTarget).find(".back");
            var front = $($event.currentTarget).find(".front");
            Utils.switchAreas(back, front);
            if (!this.isSelectedFileRight(file)) {
                this.countErr++;
            }
        }
        calcCardClasses() {
            var count = this.service.calcCardColsCount(this.service.limit);
            this.cardClasses = Utils.format(MandalaController.GridCardColumnClasses, count);
        }
        isSelectedFileRight(file: FileInfo) {
            var res = this.currentFile && file && this.currentFile.Url === file.Url;
            return res;
        }

        upSeconds() {
            this.service.setTimeout(++this.service.seconds);
        }

        downSeconds() {
            this.service.setTimeout(--this.service.seconds);
        }

        upLimit() {
            this.service.setLimit(++this.service.limit);
            this.init();
        }

        downLimit() {
            this.service.setLimit(--this.service.limit);
            this.init();
        }
    }
}