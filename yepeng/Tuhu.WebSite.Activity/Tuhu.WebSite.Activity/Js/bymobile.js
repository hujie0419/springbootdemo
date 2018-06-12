var hithours = [10, 16, 20];
function tick() {
    //alert('ok');
    var d = now();
    var h = d.getHours();
    var m = d.getMinutes();
    updatetime(h, m);
    //debugger;
    //updatetime(10,00);
    window.setTimeout(tick, 1000);
}

function preparing() {
    showmsg('马上开抢     不要走开！', true);
}
function ordering() {
    bswith('ordernow');
}
window.msgshow = false;
function showhint(msg, duration) {
    msgshow = true;
    $('#msgbox').html(msg);
    bswith('msgbox');
    $('#schedule').show();
    if (duration) {
        window.setTimeout(function () {
            location.reload();
        }, duration);
    }
    return {
        hide: function () {
            msgshow = false;
        }
    }
}
function showmsg(msg, isdisplay, duration) {
    //alert(msg);
    msgshow = true;
    $('#getready').html(msg);
    bswith('getready');
    if (!isdisplay) {
        $('.nextAction').hide();
    }
    return {
        hide: function () {
            if (duration) {
                window.setTimeout(function () {
                    msgshow = false;
                }, duration);
            } else {
                msgshow = false;
            }
        }
    }
}
function bswith(id) {
    $('#schedule').hide();
    $('#getready').hide();
    $('#ordernow').hide();
    $('#' + id).show();
}
function now() {
    var r = new Date();
    return r;
}
function initdtime() {
    var placeholder = $('#dhours')[0];
    placeholder.innerHTML = '';  
    for (var i = 0; i < hithours.length; i++) {
        var sp = document.createElement('span');
        var h = hithours[i];
        $(sp).attr('dtimespan', h);
        sp.innerHTML = '第' + (i + 1) + '波 ' + h + ':00 开始';
        placeholder.appendChild(sp);
    }
}
function updatetime(chour, cmin) {
    //alert(cmin + ',' + msgshow);
    var hith = hithours[0];
    $('#dtime').show();
    for (var i = 0; i < hithours.length; i++) {
        $('[dtimespan=' + hithours[i] + ']').removeClass('active');
        if ((hithours[i] == chour && cmin < 1) || hithours[i] > chour) {
            hith = hithours[i];
            break;
        }
    }
    if (!msgshow) {
        if (chour + 1 == hith && cmin > 50) {
            $('#phnum').hide();
            preparing();
        } else if (chour == hith && cmin < 1) {
            $('#phnum').hide();
            $('#dtime').hide();
            ordering();
        }
        $('[dtimespan=' + hith + ']').addClass('active');
        $('#hbox').html('<span class="time">' + parseInt(hith / 10) + '</span><span class="time">' + hith % 10 + '</span>');
    }
}
function loadorders() {
    $.getJSON("http://faxian.tuhu.cn/BaoYangFree/PaidOrder.html", function (list) {
        var sb = "";
        $.each(list, function () {
            sb += '<div><span class="left_phone">' + this.Key + '</span><span>获得1张<span class="right_jin">' + Math.min(this.Value, 1000).toFixed(0) + '</span>元现金劵</span></div>';
        });
        $("#infobox").html(sb).attr('lmcount', list.length);
    });
}
function marquees() {
    try {
        var n = parseInt($("#infobox").attr('lmcount'));
        if (n && n >= 8) {
            $("#infobox").find("div:first").appendTo("#infobox");
        }
    } catch (e) {
        alert(e);
    }
}
function popdialog(q, c) {
    var es = $(q);
    es.fadeIn(100);
    es.find('.close').click(function () {
        es.fadeOut(100);
    });
    return es;
}
