
function ObterDataAtual() {
    var date = new Date();
    var yyyy = date.getFullYear().toString();
    var mm = (date.getMonth() + 1).toString();
    var dd = date.getDate().toString();
    return yyyy + '-' + (mm[1] ? mm : "0" + mm[0]) + '-' + (dd[1] ? dd : "0" + dd[0]);
}

function IniciarEventos() {
    $('#external-events .fc-event').each(function () {
        $(this).data('event', {
            title: $.trim($(this).text()),
            stick: true
        });
        $(this).draggable({
            zIndex: 999,
            revert: true,
            revertDuration: 0
        });
    });
}

function formatTitle(title, start, end) {
    var result = title;
    start = new Date(start);
    end = new Date(end);
    var diffDays = Math.ceil((end - start) / (24 * 60 * 60 * 1000));
    if (diffDays < 2 && title.length > 11) {
        result = title.substr(0, 11) + "...";
    }
    return result;
}

function InitFullCalendar() {
    $('#calendar').fullCalendar({
        header: {
            left: 'prev,next today',
            center: 'title',
            right: 'month,agendaWeek,agendaDay'
        },
        theme: true,
        editable: false,
        eventStartEditable: false,
        allDaySlot: false,
        timeFormat: 'HH:mm',
        defaultDate: ObterDataAtual(),
        monthNames: ["1月", "2月", "3月", "4月", "5月", "6月", "7月", "8月", "9月", "10月", "11月", "12月"],
        monthNamesShort: ["1", "2", "3", "4", "5", "6", "7", "8", "9", "10", "11", "12"],
        dayNames: ["星期日", "星期一", "星期二", "星期三", "星期四", "星期五", "星期六"],
        dayNamesShort: ["周日", "周一", "周二", "周三", "周四", "周五", "周六"],
        allDayText: '全天',
        firstDay: 1,
        buttonText: {
            today: '本月',
            month: '月',
            week: '周',
            day: '日',
            prev: '上一月',
            next: '下一月'
        },
        events: function (start, end, s, callback) {
            $.ajax({
                type: "post",
                url: "/ActivityBoard/GetActivityList",
                dataType: "json",
                async:false,
                data: {
                    start: moment(start, 'DD.MM.YYYY HH:mm:ss').format('YYYY-MM-DD HH:mm:ss'),
                    end: moment(end, 'DD.MM.YYYY HH:mm:ss').format('YYYY-MM-DD HH:mm:ss'),
                    type: $(".moduleType").val(),
                    title: $("#activity-title").val(),
                    createdUser: $("#activity-createdUser").val(),
                    owner: $("#activity-owner").val(),
                    typeStr: GetActivityType(),
                    channel: $("#channel").val()
                },
                success: function (data) {
                    if (data) {
                        var events = [];
                        $.each(data, function (i) {
                            events.push({
                                id: i,
                                activityId: data[i].ActivityId,
                                title: formatTitle(data[i].Title, formatTime(data[i].StartTime), formatTime(data[i].EndTime)),
                                start: formatTime(data[i].StartTime),
                                end: formatTime(data[i].EndTime),
                                color: ConvertColorForActivityType(data[i].ActivityType),
                                moduleType: data[i].ActivityBoardType,
                                className: data[i].ActivityType + "-class",
                                allDay: false,
                                isNew: data[i].IsNew,
                                hashKey: data[i].HashKey
                            });
                        });
                        callback(events);
                        $(".fc-time").hide();
                    }
                }
            });
        },
        eventMouseover: function (event, jsEvent, view) {
            var tophead = $(".fc-view.fc-month-view.fc-basic-view  table thead tr td table thead tr").find("th");
            var currentdayRow = $(this).parent().parent().parent().siblings('thead').find('td');
            var weeds = ["周一", "周二", "周三", "周四", "周五", "周六", "周日"];
            var graycolor = "style='color:gray;'";
            for (var i = 0; i < currentdayRow.length; i++) {
                var hasothermonth = $(currentdayRow[i]).hasClass('fc-other-month');
                if (hasothermonth) {
                    $(tophead[i]).css("color", "gray").html(weeds[i]).html(weeds[i] + " (" + $(currentdayRow[i]).html() + '日)');
                } else {
                    $(tophead[i]).css("color", "black").html(weeds[i]).html(weeds[i] + " (" + $(currentdayRow[i]).html() + '日)');
                };
            };
        },
        //droppable: true,
        //drop: function (date) {
        //    $(this).remove();
        //    var titulo = $(this).html();
        //    $.ajax({
        //        type: 'POST',
        //        url: "/ActivityBoard/AdicionarEvento",
        //        dataType: "json",
        //        contentType: "application/json",
        //        success: function () {
        //            $("#calendar").fullCalendar('removeEvents');
        //            $("#calendar").fullCalendar("refetchEvents");
        //        }
        //    });
        //},
        eventClick: function (calEvent, jsEvent, view) {
            EditActivityBoard(calEvent.moduleType, calEvent.activityId, "edit-activity", calEvent.isNew, calEvent.hashKey);
        },
        dayClick: function (date, allDay, jsEvent, view) {
            AddThirdActivity($(".moduleType").val(), "add-activity")
        },
        editable: true,
        //eventDrop: function (event) {
        //    atualizarEvento(event.id, event.start, event.end);
        //},
        //eventResize: function (event) {
        //    atualizarEvento(event.id, event.start, event.end);
        //}
    });

    $("input[name='activity_type']").on("click", function () {
        var key = $(this).attr("data-key");
        if ($(this).prop("checked")) {
            $("." + key + "-class").show();
        } else {
            $("." + key + "-class").hide();
        }
    });

    //function atualizarEvento(eventId, eventStart, eventEnd) {
    //    var visualizacaoAtual = $('#calendar').fullCalendar('getView').name;
    //    var dataRow = {
    //        'id': eventId,
    //        'newEventStart': eventStart,
    //        'newEventEnd': eventEnd,
    //        'visualizacaoAtual': visualizacaoAtual
    //    };
    //    $.ajax({
    //        type: 'POST',
    //        url: "/ActivityBoard/AtualizarEvento",
    //        dataType: "json",
    //        contentType: "application/json",
    //        data: JSON.stringify(dataRow)
    //    });
    //}
    //$("#btn-editar-save").click(function () {
    //    var events = [];
    //    events.push({
    //        id: 324,
    //        title: $("#activity-name").val(),
    //        start: $("#activity-startTime").val(),
    //        end: $("#activity-endTime").val(),
    //        color: $("#activity-principal").val(),
    //        className: $(".activity-type").val(),
    //        allDay: false,
    //        allDayDefault: true,
    //    });
    //    $('#calendar').fullCalendar('addEventSource', events);
    //    $('.modal-editar-evento').modal('close');
    //});
}

function EditActivityBoard(moduleType, activityId, classType, isNew, hashKey) {
    var cancelBtn = { value: "取消" };
    var effectBtn = {
        value: "活动效果",
        callback: function () {
            if (isNew == true || isNew == "true" )
                window.open("/ActivityBoard/ActivityEffect?moduleType=" + moduleType + "&activityId=" + hashKey);
            else
                window.open("/ActivityBoard/ActivityEffect?moduleType=" + moduleType + "&activityId=" + activityId);
            return false;
        }
    }
    var buttons = [];
    if (moduleType === "ThirdPartyActivity") {
        var editBtn = {
            value: "修改",
            callback: function () {
                var activityName = $("." + classType + " #activity-name").val();
                var activityType = $("." + classType + " .activity-type").val();
                var h5Url = $("." + classType + " #activity-h5Url").val();
                var webUrl = $("." + classType + " #activity-webUrl").val();
                var principal = $("." + classType + " #activity-principal").val();
                var activityRules = $("." + classType + " #activity-rules").val();
                var startTime = $("." + classType + " #activity-startTime").val();
                var endTime = $("." + classType + " #activity-endTime").val();
                var channel = $("." + classType + " #activity-channel").val();
                if (VerifyParameters(activityName, activityType, h5Url, webUrl, principal, activityRules, startTime, endTime)) {
                    var thirdparty = new Object();
                    thirdparty.PKID = activityId;
                    thirdparty.ActivityName = activityName;
                    thirdparty.ActivityType = activityType;
                    thirdparty.H5Url = h5Url;
                    thirdparty.WebUrl = webUrl;
                    thirdparty.ActivityRules = encodeURIComponent(activityRules);
                    thirdparty.StartTime = startTime;
                    thirdparty.EndTime = endTime;
                    thirdparty.Owner = principal;
                    thirdparty.Channel = channel;
                    if (confirm("是否确认修改？")) {
                        $.post("UpdateThirdPartActivity", { thirdPartActivity: JSON.stringify(thirdparty), moduleType: moduleType }, function (result) {
                            if (result.status == true) {
                                alert("修改成功");
                                location.reload()
                            } else {
                                alert(result.msg);
                            }
                        });
                    }
                } else {
                    return false;
                }
            }
        };
        if ($(".IsViewEffect").val()) {
            //buttons = [editBtn, cancelBtn, effectBtn];
            buttons = [editBtn, cancelBtn];
        } else {
            buttons = [editBtn, cancelBtn];
        }
    } else {
        if ($(".IsViewEffect").val()) {
            //buttons = [cancelBtn, effectBtn];
            buttons = [cancelBtn];
        } else {
            buttons = [cancelBtn];
        }
    }

    var d = dialog({
        title: "编辑",
        width: 600,
        height: 400,
        button: buttons,
    });
    var html = "<div class='" + classType + "'>" + $('.modal-content').html() + "</div>";
    d.content(html);
    if (moduleType === "ThirdPartyActivity") {
        $.post("GetThirdPartActivityByPKID", { pkid: activityId }, function (result) {
            if (result.status == true) {
                var item = result.data;
                $("." + classType + " .activity-type").text(ConvertActivityType(item.ActivityType)).val(item.ActivityType);
                $("." + classType + " #activity-name").val(item.ActivityName);
                $("." + classType + " #activity-h5Url").val(item.H5Url);
                $("." + classType + " #activity-webUrl").val(item.WebUrl);
                $("." + classType + " #activity-principal").val(item.Owner);
                $("." + classType + " #activity-principal").attr("disabled", "disabled");
                $("." + classType + " #activity-rules").val(item.ActivityRules);
                $("." + classType + " #activity-startTime").val(formatTime(item.StartTime));
                $("." + classType + " #activity-endTime").val(formatTime(item.EndTime));
                $("." + classType + " #activity-channel").val(item.Channel);
                d.showModal();
                $(".ui-popup-focus").addClass("paddingTop");
            } else {
                alert(result.msg);
            }
        });
    } else {
        $.post("GetTuhuOrOutSideActivity", { pkid: activityId, isNew: isNew}, function (result) {
            if (result.status == true) {
                var item = result.data;
                var content = "";
                $("." + classType + " .activity-type").text(ConvertActivityType(item.ActivityConfigType)).val(item.ActivityConfigType);
                $("." + classType + " #activity-name").val(item.Title);
                if (isNew == "false" || isNew == false) {
                    $("." + classType + " #activity-h5Url").val("https://wx.tuhu.cn/staticpage/activity/list.html?id=" + activityId);
                    $("." + classType + " #activity-webUrl").val("http://item.tuhu.cn/Activity/act/" + activityId + ".html");
                } else {
                    $("." + classType + " #activity-h5Url").val("https://wx.tuhu.cn/staticpage/promotion/activity/?id=" + hashKey + "&tuhu");
                    $("." + classType + " #activity-webUrl").val(" https://item.tuhu.cn/promotion/activity/" + hashKey + ".html");
                }
                $("." + classType + " #activity-principal").val(item.PersonWheel);
                $("." + classType + " #activity-createdUser").val(item.CreatetorUser);
                $("." + classType + " #activity-startTime").val(formatTime(item.StartDT));
                $("." + classType + " #activity-endTime").val(formatTime(item.EndDate));
                $("." + classType + " #activity-principal").attr("disabled", "disabled");
                $("." + classType + " #activity-createdUser").attr("disabled", "disabled");
                $.each(result.des, function (index) {
                    content += "活动 " + (index + 1) + ": ";
                    content += result.des[index];
                });
                $("." + classType + " #activity-rules").val(content);
                d.showModal();
                $(".ui-popup-focus").addClass("paddingTop");
            }
        });
    }
    $("." + classType + " .dropdown-menu").on('click', 'li a', function () {
        var classeBotao = $(this).attr("class");
        $("." + classType + " .activity-type").text($(this).text()).val(classeBotao);
    });

    BindUrl(classType);
}

function AddThirdActivity(moduleType, classType) {
    if (moduleType == "ThirdPartyActivity") {
        var addBtn = {
            value: "添加",
            callback: function () {
                var activityName = $("." + classType + " #activity-name").val().trim();
                var activityType = $("." + classType + " .activity-type").val().trim();
                var h5Url = $("." + classType + " #activity-h5Url").val().trim();
                var webUrl = $("." + classType + " #activity-webUrl").val().trim();
                var principal = $("." + classType + " #activity-principal").val().trim();
                var activityRules = $("." + classType + " #activity-rules").val().trim();
                var startTime = $("." + classType + " #activity-startTime").val().trim();
                var endTime = $("." + classType + " #activity-endTime").val().trim();
                var channel = $("." + classType + " #activity-channel").val();
                if (VerifyParameters(activityName, activityType, h5Url, webUrl, principal, activityRules, startTime, endTime)) {
                    var thirdparty = new Object();
                    thirdparty.ActivityName = activityName;
                    thirdparty.ActivityType = activityType;
                    thirdparty.H5Url = h5Url;
                    thirdparty.WebUrl = webUrl;
                    thirdparty.ActivityRules = encodeURIComponent(activityRules);
                    thirdparty.StartTime = startTime;
                    thirdparty.EndTime = endTime;
                    thirdparty.Owner = principal;
                    thirdparty.Channel = channel;
                    if (confirm("是否确认添加？")) {
                        $.post("InsertThirdPartActivity", { thirdPartActivity: JSON.stringify(thirdparty), moduleType: moduleType }, function (result) {
                            if (result.status == true) {
                                alert("添加成功");
                                location.reload()
                            } else {
                                alert(result.msg);
                                return false;
                            }
                        });
                    }
                } else {
                    return false;
                }
            }
        };
        var cancelBtn = { value: "取消" };

        var buttons = [addBtn, cancelBtn];
        var d = dialog({
            title: "添加第三方活动",
            width: 600,
            height: 400,
            button: buttons,
        });
        var html = "<div  class='" + classType + "'>" + $('.modal-content').html() + "</div>";
        d.content(html);
        d.showModal();
        $("." + classType + " .dropdown-menu").on('click', 'li a', function () {
            var classeBotao = $(this).attr("class");
            $("." + classType + " .activity-type").text($(this).text()).val(classeBotao);
        });
    }
    $(".ui-popup-focus").addClass("paddingTop");
    BindUrl(classType);
}

function BindUrl(classType) {
    $(".gotoUrl").on("click", function () {
        var url = $(this).parent().parent().find("div").find("input").val();
        if (url != "") {
            window.open(url);
        }
    });
}

function VerifyParameters(activityName, activityType, h5Url, webUrl, principal, activityRules, startTime, endTime) {
    if (activityType == "" || activityType == "None") {
        alert("请选择活动类型");
        return false;
    }
    if (principal == "") {
        alert("请输入负责人");
        return false;
    }
    if (!/\w*@tuhu.cn$/.test(principal)) {
        alert("请输入途虎的邮箱");
        return false;
    }
    if (activityName == "") {
        alert("请输入活动名称");
        return false;
    }
    if (h5Url == "" || webUrl == "") {
        alert("请输入链接");
        return false;
    }
    if (!/[a-zA-z]+:\/\/[^\s]*/.test(h5Url) || !/[a-zA-z]+:\/\/[^\s]*/.test(webUrl)) {
        alert("请输入正确的链接");
        return false;
    }
    if (startTime == "" || endTime == "") {
        alert("活动时间不能为空");
        return false;
    }
    return true;
}

$(".dropdown-menu").on('click', 'li a', function () {
    var classeBotao = $(this).attr("class");
    $(".activity-type").text($(this).text()).val(classeBotao);
    $("#activityType").val(classeBotao);
});

function formatTime(str) {
    var d = eval('new ' + str.substr(1, str.length - 2));
    var ar_date = [d.getFullYear(), d.getMonth() + 1, d.getDate(), d.getHours(), d.getMinutes(), d.getSeconds()];
    for (var i = 0; i < ar_date.length; i++) ar_date[i] = dFormat(ar_date[i]);
    return ar_date.slice(0, 3).join('-') + ' ' + ar_date.slice(3).join(':');

    function dFormat(i) { return i < 10 ? "0" + i.toString() : i; }
}

function GetActivityType() {
    var type = "";
    $("input[name='activity_type']:checked").each(function () {
        if (this.checked) {
            type += $(this).attr("data-key") + ",";
        }
    })
    return type;
}


function ConvertModuleType(key) {
    var name = "";
    switch (key) {
        case 0:
            name = "第三方活动";
            break;
        //case 1:
        //    name = "外部投放活动";
        //    break;
        case 2:
            name = "途虎活动";
            break;
    }
    return name;
}

function ConvertColorForActivityType(type) {
    var color = "#e74c3c";
    switch (type) {
        case "ZongHe":
            color = "#e74c3c";
            break;
        case "Tires":
            color = "#f1c40f";
            break;
        case "BaoYang":
            color = "#8e44ad";
            break;
        case "ChePin":
            color = "#3498db";
            break;
        case "GaiZhuang":
            color = "#2ecc71";
            break;
        case "Outside":
            color = "#1abc9c";
            break;
    }
    return color;
}


function ConvertActivityType(key) {
    var name = "";
    switch (key) {
        case 0:
            name = "综合";
            break;
        case 1:
            name = "车品";
            break;
        case 2:
            name = "保养";
            break;
        case 3:
            name = "轮胎";
            break;
        case 4:
            name = "美容改装";
            break;
        case 5:
            name = "外部投放";
            break;
        case 6:
            name = "请选择";
            break;
    }
    return name;
}