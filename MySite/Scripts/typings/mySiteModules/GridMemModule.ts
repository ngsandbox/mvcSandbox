/// <reference path="../angularjs/angular.d.ts" />
/// <reference path="../angularjs/angular-resource.d.ts" />
module iPlayGround.Services {
    export class FileInfo {
        Url: string;
        Name: string;
        inAction: boolean = false;
    }

    export class GridMemoryService {
        public static get gridMemSecondsKey(): string { return "GridMemorySeconds"; }
        public static get gridMemLimitKey(): string { return "GridMemoryLimit"; }
        public static fullName: string = "iPlayGround.Services.GridMemoryService";
        ngResource: ng.resource.IResourceService;
        location: ng.ILocationService;
        title: string = "Grid Memory";
        files: FileInfo[];
        question: string = "";
        questionTemplate: string = "";
        limit: number = 6;
        seconds: number = 10;
        static $inject = ["$resource", "$location"];
        constructor($resource: ng.resource.IResourceService, $location: ng.ILocationService) {
            this.ngResource = $resource;
            this.location = $location;
        }
        init(): void {
            this.question = "",
            this.questionTemplate = $("span[my-gmQuestion]").attr('my-gmQuestion');
            this.limit = Utils.load(GridMemoryService.gridMemLimitKey, 6);
            this.seconds = Utils.load(GridMemoryService.gridMemSecondsKey, 10);
            this.files = this.getFiles();
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
            Utils.save(GridMemoryService.gridMemSecondsKey, sec);
        }

        setLimit(limit) {
            Utils.save(GridMemoryService.gridMemLimitKey, limit);
        }
    }

    export class GridMemoryController {
        private static GridCardColumnClasses: string = "col-lg-{0} col-md-{0} col-sm-{0} col-xs-{0}";
        public static fullName: string = "iPlayGround.Services.GridMemoryController";
        service: GridMemoryService;
        log: ng.ILogService;
        timeout: ng.ITimeoutService;
        startTimeoutHndl: angular.IPromise<any>;

        start: boolean = false;
        currentFile: FileInfo = null;
        cardClasses: string = "";
        restFiles: FileInfo[] = [];
        countSuc: number = 0;
        countErr: number = 0;
        static $inject = [GridMemoryService.fullName, '$timeout', '$log'];
        constructor(service: GridMemoryService, $timeout: ng.ITimeoutService, $log: ng.ILogService) {
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
            this.restFiles = this.service.files;
            this.currentFile = null;
            this.calcCardClasses();
            this.countSuc = this.countErr = 0;
            this.startTimeoutHndl = this.timeout(() => {
                Utils.switchAreas($(".card .front"), $(".card .back"));
                this.start = true;
                this.setNextFile();
            }, this.service.seconds * 1000);
        }

        flipCard(file: FileInfo, $event: any) {
            if (!this.start || file.inAction) {
                return;
            }

            file.inAction = true;
            this.log.log(file);
            if (!this.start || this.restFiles.indexOf(file) < 0) {
                return;
            }

            var back = $($event.currentTarget).find(".back");
            var front = $($event.currentTarget).find(".front");
            Utils.switchAreas(back, front);
            if (this.isSelectedFileRight(file)) {
                this.countSuc++;
                this.restFiles = $.grep(this.restFiles,(f) => (f !== file));
                this.setNextFile();
            }
            else {
                this.countErr++;
                this.timeout(() => {
                    Utils.switchAreas(front, back,() => {
                        file.inAction = false;
                    });
                }, 2000);
            }
        }
        calcCardClasses() {
            var count = this.service.calcCardColsCount(this.service.limit);
            this.cardClasses = Utils.format(GridMemoryController.GridCardColumnClasses, count);
        }
        setNextFile() {
            if (this.restFiles && this.restFiles.length > 0) {
                var pos = Utils.getRandomInt(0, this.restFiles.length);
                this.currentFile = this.restFiles[pos];
                this.service.question = Utils.format(this.service.questionTemplate, this.currentFile.Name);
            }
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