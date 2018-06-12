(function () {
    var timeCountDown;
    var countDone = function () {
        document.getElementById("hourid").innerHTML = document.getElementById("minuteid").innerHTML = document.getElementById("secondid").innerHTML = "00";

        $("#tixing").hide();
        if ($("#dvWlTop").text().indexOf("免费注册") > 0)
            $(".fist_login").show();
        else {
            $(".qiang_juan").show();
            $(".qiang_start").hide();

            //移动
            $(".qiang_juan").click(function () {
                var dengQiangTips = Tuhu.Dialog.Popup(".deng_qiang_tips", { opacity: 0.2, closeHandle: ".close", draggable: false });
                setTimeout(function () {
                    $.post().complete(function (ajax) {
                        dengQiangTips.close();
                        if (ajax.status == 200) {
                            var result = $.parseJSON(ajax.responseText);
                            if (result > 0) {
                                $(".qiang_juan").hide();
                                Tuhu.Dialog.Popup(".qiang_tips", { opacity: 0.2, closeHandle: ".close", draggable: false });
                            }
                            else if (result == -1) {
                                //活动没有开始
                                location.reload();
                            }
                            else if (result == -2) {
                                $(".qiang_juan").hide();
                                $(".yi_ling").show();
                            }
                            else if (result == -3) {
                                $(".qiang_juan").hide();
                                alert("已抢完！");
                                location.reload();
                            }
                            else if (result == -4) {
                                Tuhu.Dialog.Popup(".no_qiang_tips", { opacity: 0.2, closeHandle: ".close", draggable: false });
                            }
                            else if (result == -99)
                                Tuhu.PopComDialog.LoginDialog(location.href);
                        }
                        else if (ajax.status > 0)
                            alert("服务器错误！");
                    });
                }, Math.random() * 500 + 500);
            });
        }

        setTimeout(function () {
            location.reload();
        }, 600000);
    }
    var startCountDown = function (hour, day) {
        timeCountDown = Tuhu.Utility.TimeCountDown(day.getFullYear() + "/" + (day.getMonth() + 1) + "/" + day.getDate() + " " + hour + ":00:00").progress(function (remainingTime) {
            document.getElementById("hourid").innerHTML = remainingTime.hours;
            document.getElementById("minuteid").innerHTML = remainingTime.minutes;
            document.getElementById("secondid").innerHTML = remainingTime.seconds;

            if (remainingTime.tick < 10 * 60 * 1000) {
                $("#tixing").hide();
                $(".qiang_start").show();
            }
        }).done(countDone);
    }

    var startNext = function (next) {
        var hoursArr = [10, 11, 13, 16, 20, 21];
        var now = new Date();
        var hour = null, tomorrow = null;
        $.each(hoursArr, function () {
            if (now.getHours() < this || !next && now.getHours() == this && now.getMinutes() < 10) {
                hour = this;
                return false;
            }
        });
        if (!hour) {
            hour = hoursArr[0];
            tomorrow = new Date(now.setDate(now.getDate() + 1));
        }

        if (hour == now.getHours()) {
            $.post("/ActivityPhase/IsFinish.html", { activityID: 'BaoYangFree' }).complete(function (ajax) {
                if (ajax.status == 200) {
                    if ($.parseJSON(ajax.responseText))
                        startNext(true);
                    else
                        countDone();
                }
                else if (ajax.status > 0)
                    alert("服务器错误！");
            });
        }
        else
            startCountDown(hour, tomorrow || new Date());
    }

    $(".free_shi").show().arctext({ radius: 1700, dir: -1, rotate: false });
    var remaided = false;
    $("#tixing").click(function () {
        if (remaided)
            Tuhu.Dialog.Popup(".kaiqiang_tips", { opacity: 0.2, closeHandle: ".close", draggable: false });
        else
            $.post("/ActivityPhase/Remaid.html", { activityID: 'BaoYangFree' }).complete(function (ajax) {
                if (ajax.status == 200) {
                    if ($.parseJSON(ajax.responseText) > -1) {
                        remaided = true;
                        Tuhu.Dialog.Popup(".kaiqiang_tips", { opacity: 0.2, closeHandle: ".close", draggable: false });
                    }
                    else
                        Tuhu.PopComDialog.LoginDialog(location.href);
                }
                else if (ajax.status > 0)
                    alert("服务器错误！");
            });
    });
    $(".fist_login").click(function () {
        Tuhu.PopComDialog.LoginDialog(location.href);
    });

    startNext();
    $(function () {
        if ($("#dvWlTop").text().indexOf("免费注册") > 0)
            $(".fist_login").show();
        else if (timeCountDown && timeCountDown.remainingTime().tick > 1000)
            $("#tixing").show();
    });

    //$.getJSON("/BaoYangFree/PrephaseUserList.html", function (list)
    //{
    //	var sb = "";
    //	$.each(list, function ()
    //	{
    //		sb += '<p><span class="left_phone">' + this + '</span><span>将获得1张免单劵</span></p>';
    //	});

    //	$(".Free_dan2 .con2_left").append(sb);
    //});

    $.getJSON("/BaoYangFree/PaidOrder.html", function (list) {
        var sb = "";
        $.each(list, function () {
            sb += '<p><span class="left_phone">' + this.Key + '</span><span>将获得1张<span class="right_jin">' + Math.min(this.Value, 1000).toFixed(0) + '元</span>现金劵</span></p>';
        });
        $("#infobox").html(sb);
    });
})();

$(function () {
    setInterval(marquees, 2000);
    menuActive();
});

function marquees() {
    $("#infobox").find("div:first").appendTo("#infobox");
    //alert('ok');
    //$(".con2_right").find("p:first").appendTo(".con2_right");
}
//当前时间菜单状态
function menuActive() {
    var h = new Date().getHours();
    var menu = $("#free_menu span");
    if (h >= 0 && h < 10) {
        menu.eq(0).addClass("active");
    }
    else if (h >= 10 && h < 11) {
        menu.eq(1).addClass("active");
    }
    else if (h >= 11 && h < 13) {
        menu.eq(2).addClass("active");
    }
    else if (h >= 13 && h < 16) {
        menu.eq(3).addClass("active");
    }
    else if (h >= 16 && h < 20) {
        menu.eq(4).addClass("active");
    }
    else if (h >= 20 && h < 21) {
        menu.eq(5).addClass("active");
    }
    else if (h >= 21 && h < 24) {
        menu.eq(6).addClass("active");
    }
}