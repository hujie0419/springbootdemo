$(document).ready(
                function () {
                    //页面初始化
                    goCenter();

                    //滚动条滚动
                    $(window).scroll(
                        function () {
                            goCenter();
                        }
                    );
                    //拖动浏览器窗口  
                    $(window).resize(
                        function () {
                            goCenter();
                        }
                    );
                }
            );

function goCenter() {
    var h = $(window).height();
    var w = $(window).width();
    var st = $(window).scrollTop();
    var sl = $(window).scrollLeft();

    $(".divv").css("top", ((h - 60) / 2) + st);
    $(".divv").css("left", ((w - 80) / 2) + sl);
}