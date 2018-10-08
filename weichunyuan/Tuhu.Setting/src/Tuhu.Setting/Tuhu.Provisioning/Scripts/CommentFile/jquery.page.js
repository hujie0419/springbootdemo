(function ($) {
    var ms = {
        init: function (obj, args) {
            return (function () {
                ms.fillHtml(obj, args);
                ms.bindEvent(obj, args);
            })();
        },
        fillHtml: function (obj, args) {
            return (function () {
                obj.empty();
                if (args.current > 1) {
                    obj.append('<button class="btn btn-white prevPage" type="button"><i class="fa fa-chevron-left"></i></button>');
                } else {
                    obj.append('<button class="btn btn-white active" type="button"><i class="fa fa-chevron-left"></i></button>');
                }
                if (args.current != 1 && args.current >= 4 && args.pageCount != 4) {
                    obj.append('<button class="btn btn-white tcdNumber">' + 1 + '</button>');
                }
                if (args.current - 2 > 2 && args.current <= args.pageCount && args.pageCount > 5) {
                    obj.append('<span style="float:left;margin:5px 10px">...</span>');
                }
                var start = args.current - 2,
                end = args.current + 2;
                if ((start > 1 && args.current < 4) || args.current == 1) {
                    end++;
                }
                if (args.current > args.pageCount - 4 && args.current >= args.pageCount) {
                    start--;
                }
                for (; start <= end; start++) {
                    if (start <= args.pageCount && start >= 1) {
                        if (start != args.current) {
                            obj.append('<button class="btn btn-white tcdNumber">' + start + '</button>');
                        } else {
                            obj.append('<button class="btn btn-white  active">' + start + '</button>');
                        }
                    }
                }
                if (args.current + 2 < args.pageCount - 1 && args.current >= 1 && args.pageCount > 5) {
                    obj.append('<span style="float:left;margin:5px 10px">...</span>');
                }
                if (args.current != args.pageCount && args.current < args.pageCount - 2 && args.pageCount != 4) {
                    obj.append('<button class="btn btn-white tcdNumber">' + args.pageCount + '</button>');
                }
                if (args.current < args.pageCount) {
                    obj.append('<button class="btn btn-white nextPage" type="button"><i class="fa fa-chevron-right"></i> </button>');
                } else {
                    obj.append('<button class="btn btn-white active" type="button"><i class="fa fa-chevron-right"></i> </button>');
                }
            })();
        },
        bindEvent: function (obj, args) {
            return (function () {
                obj.on("click", "button.tcdNumber",
                function () {
                    var current = parseInt($(this).text());
                    ms.fillHtml(obj, {
                        "current": current,
                        "pageCount": args.pageCount
                    });
                    if (typeof (args.backFn) == "function") {
                        args.backFn(current);
                    }
                });
                obj.on("click", "button.prevPage",//上一页
                function () {
                    var current = parseInt(obj.children("button.active").text());
                    ms.fillHtml(obj, {
                        "current": current - 1,
                        "pageCount": args.pageCount
                    });
                    if (typeof (args.backFn) == "function") {
                        args.backFn(current - 1);
                    }
                });
                obj.on("click", "button.nextPage",//下一页
                function () {
                    var current = parseInt(obj.children("button.active").text());
                    ms.fillHtml(obj, {
                        "current": current + 1,
                        "pageCount": args.pageCount
                    });
                    if (typeof (args.backFn) == "function") {
                        args.backFn(current + 1);
                    }
                });
            })();
        }
    }
    $.fn.createPage = function (options) {
        var args = $.extend({
            pageCount: 10,
            current: 1,
            backFn: function () { }
        },
        options);
        ms.init(this, args);
    }
})(jQuery);