/// <reference path="../angularjs/angular.d.ts" />
/// <reference path="../angularjs/angular-resource.d.ts" />
var iPlayGround;
(function (iPlayGround) {
    var Services;
    (function (Services) {
        var MandalaService = (function () {
            function MandalaService($resource, $location) {
                this.title = "Mandala";
                this.question = "";
                this.questionTemplate = "";
                this.limit = 6;
                this.seconds = 2;
                this.ngResource = $resource;
                this.location = $location;
            }
            Object.defineProperty(MandalaService, "gridMemSecondsKey", {
                get: function () { return "MandalaSeconds"; },
                enumerable: true,
                configurable: true
            });
            Object.defineProperty(MandalaService, "gridMemLimitKey", {
                get: function () { return "MandalaLimit"; },
                enumerable: true,
                configurable: true
            });
            MandalaService.prototype.init = function () {
                this.question = "",
                    this.questionTemplate = $("span[my-gmQuestion]").attr('my-gmQuestion');
                this.limit = Utils.load(MandalaService.gridMemLimitKey, 6);
                this.seconds = Utils.load(MandalaService.gridMemSecondsKey, 10);
            };
            MandalaService.prototype.getFiles = function () {
                var files = this.ngResource(iPlayGround.Routes.getLang() + "Games/GetCards?limit=:limit", { limit: this.limit }).query();
                return (files);
            };
            MandalaService.prototype.calcCardColsCount = function (limit) {
                var result = (limit < 5) ? 6 :
                    (limit < 7) ? 4 :
                        (limit < 13) ? 3 :
                            (limit < 25) ? 2 : 1;
                return result;
            };
            MandalaService.prototype.setTimeout = function (sec) {
                Utils.save(MandalaService.gridMemSecondsKey, sec);
            };
            MandalaService.prototype.setLimit = function (limit) {
                Utils.save(MandalaService.gridMemLimitKey, limit);
            };
            MandalaService.fullName = "iPlayGround.Services.MandalaService";
            MandalaService.$inject = ["$resource", "$location"];
            return MandalaService;
        }());
        Services.MandalaService = MandalaService;
        var MandalaController = (function () {
            function MandalaController(service, $timeout, $log) {
                this.start = false;
                this.currentFile = null;
                this.cardClasses = "";
                this.files = [];
                this.countSuc = 0;
                this.countErr = 0;
                this.service = service;
                this.timeout = $timeout;
                this.log = $log;
                this.init();
            }
            MandalaController.prototype.init = function () {
                var _this = this;
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
                this.startTimeoutHndl = this.timeout(function () {
                    Utils.switchAreas($(".card .front"), $(".card .back"));
                    _this.start = true;
                }, this.service.seconds * 1000);
            };
            MandalaController.prototype.flipCard = function (file, $event) {
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
            };
            MandalaController.prototype.calcCardClasses = function () {
                var count = this.service.calcCardColsCount(this.service.limit);
                this.cardClasses = Utils.format(MandalaController.GridCardColumnClasses, count);
            };
            MandalaController.prototype.isSelectedFileRight = function (file) {
                var res = this.currentFile && file && this.currentFile.Url === file.Url;
                return res;
            };
            MandalaController.prototype.upSeconds = function () {
                this.service.setTimeout(++this.service.seconds);
            };
            MandalaController.prototype.downSeconds = function () {
                this.service.setTimeout(--this.service.seconds);
            };
            MandalaController.prototype.upLimit = function () {
                this.service.setLimit(++this.service.limit);
                this.init();
            };
            MandalaController.prototype.downLimit = function () {
                this.service.setLimit(--this.service.limit);
                this.init();
            };
            MandalaController.GridCardColumnClasses = "col-lg-{0} col-md-{0} col-sm-{0} col-xs-{0}";
            MandalaController.fullName = "iPlayGround.Services.MandalaController";
            MandalaController.$inject = [MandalaService.fullName, '$timeout', '$log'];
            return MandalaController;
        }());
        Services.MandalaController = MandalaController;
    })(Services = iPlayGround.Services || (iPlayGround.Services = {}));
})(iPlayGround || (iPlayGround = {}));
//# sourceMappingURL=MandalaModule.js.map