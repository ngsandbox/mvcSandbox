/// <reference path="../angularjs/angular.d.ts" />
/// <reference path="../angularjs/angular-resource.d.ts" />
var iPlayGround;
(function (iPlayGround) {
    var Services;
    (function (Services) {
        var FileInfo = (function () {
            function FileInfo() {
                this.inAction = false;
            }
            return FileInfo;
        }());
        Services.FileInfo = FileInfo;
        var GridMemoryService = (function () {
            function GridMemoryService($resource, $location) {
                this.title = "Grid Memory";
                this.question = "";
                this.questionTemplate = "";
                this.limit = 6;
                this.seconds = 10;
                this.ngResource = $resource;
                this.location = $location;
            }
            Object.defineProperty(GridMemoryService, "gridMemSecondsKey", {
                get: function () { return "GridMemorySeconds"; },
                enumerable: true,
                configurable: true
            });
            Object.defineProperty(GridMemoryService, "gridMemLimitKey", {
                get: function () { return "GridMemoryLimit"; },
                enumerable: true,
                configurable: true
            });
            GridMemoryService.prototype.init = function () {
                this.question = "",
                    this.questionTemplate = $("span[my-gmQuestion]").attr('my-gmQuestion');
                this.limit = Utils.load(GridMemoryService.gridMemLimitKey, 6);
                this.seconds = Utils.load(GridMemoryService.gridMemSecondsKey, 10);
                this.files = this.getFiles();
            };
            GridMemoryService.prototype.getFiles = function () {
                var files = this.ngResource(iPlayGround.Routes.getLang() + "Games/GetCards?limit=:limit", { limit: this.limit }).query();
                return (files);
            };
            GridMemoryService.prototype.calcCardColsCount = function (limit) {
                var result = (limit < 5) ? 6 :
                    (limit < 7) ? 4 :
                        (limit < 13) ? 3 :
                            (limit < 25) ? 2 : 1;
                return result;
            };
            GridMemoryService.prototype.setTimeout = function (sec) {
                Utils.save(GridMemoryService.gridMemSecondsKey, sec);
            };
            GridMemoryService.prototype.setLimit = function (limit) {
                Utils.save(GridMemoryService.gridMemLimitKey, limit);
            };
            GridMemoryService.fullName = "iPlayGround.Services.GridMemoryService";
            GridMemoryService.$inject = ["$resource", "$location"];
            return GridMemoryService;
        }());
        Services.GridMemoryService = GridMemoryService;
        var GridMemoryController = (function () {
            function GridMemoryController(service, $timeout, $log) {
                this.start = false;
                this.currentFile = null;
                this.cardClasses = "";
                this.restFiles = [];
                this.countSuc = 0;
                this.countErr = 0;
                this.service = service;
                this.timeout = $timeout;
                this.log = $log;
                this.init();
            }
            GridMemoryController.prototype.init = function () {
                var _this = this;
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
                this.startTimeoutHndl = this.timeout(function () {
                    Utils.switchAreas($(".card .front"), $(".card .back"));
                    _this.start = true;
                    _this.setNextFile();
                }, this.service.seconds * 1000);
            };
            GridMemoryController.prototype.flipCard = function (file, $event) {
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
                    this.restFiles = $.grep(this.restFiles, function (f) { return (f !== file); });
                    this.setNextFile();
                }
                else {
                    this.countErr++;
                    this.timeout(function () {
                        Utils.switchAreas(front, back, function () {
                            file.inAction = false;
                        });
                    }, 2000);
                }
            };
            GridMemoryController.prototype.calcCardClasses = function () {
                var count = this.service.calcCardColsCount(this.service.limit);
                this.cardClasses = Utils.format(GridMemoryController.GridCardColumnClasses, count);
            };
            GridMemoryController.prototype.setNextFile = function () {
                if (this.restFiles && this.restFiles.length > 0) {
                    var pos = Utils.getRandomInt(0, this.restFiles.length);
                    this.currentFile = this.restFiles[pos];
                    this.service.question = Utils.format(this.service.questionTemplate, this.currentFile.Name);
                }
            };
            GridMemoryController.prototype.isSelectedFileRight = function (file) {
                var res = this.currentFile && file && this.currentFile.Url === file.Url;
                return res;
            };
            GridMemoryController.prototype.upSeconds = function () {
                this.service.setTimeout(++this.service.seconds);
            };
            GridMemoryController.prototype.downSeconds = function () {
                this.service.setTimeout(--this.service.seconds);
            };
            GridMemoryController.prototype.upLimit = function () {
                this.service.setLimit(++this.service.limit);
                this.init();
            };
            GridMemoryController.prototype.downLimit = function () {
                this.service.setLimit(--this.service.limit);
                this.init();
            };
            GridMemoryController.GridCardColumnClasses = "col-lg-{0} col-md-{0} col-sm-{0} col-xs-{0}";
            GridMemoryController.fullName = "iPlayGround.Services.GridMemoryController";
            GridMemoryController.$inject = [GridMemoryService.fullName, '$timeout', '$log'];
            return GridMemoryController;
        }());
        Services.GridMemoryController = GridMemoryController;
    })(Services = iPlayGround.Services || (iPlayGround.Services = {}));
})(iPlayGround || (iPlayGround = {}));
//# sourceMappingURL=GridMemModule.js.map