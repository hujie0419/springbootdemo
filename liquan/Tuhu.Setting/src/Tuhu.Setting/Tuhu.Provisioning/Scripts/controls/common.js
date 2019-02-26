function rep(c, data) {
    var upurl = c.url;
    if (c.url && c.replacer) {
        for (var i in c.replacer) {
            var rep = c.replacer[i];
            var rg = new RegExp('\\[' + i + '\\]', 'ig');
            upurl = upurl.replace(rg, unescape(data[i]));
        }
    }
    return upurl;
}
function decode(s) {
    return unescape(s).replace(/\+/g, ' ');
}
function overlay(c) {
    var el = document.body.$overlayel$;
    var oel = null;
    if (!el) {
        el = document.createElement('div');
        el.className = 'overlay';
        oel = document.createElement('div');
        oel.className = 'overlaybox';
        el.oel = oel;
        el.hide = function () {
            var oel = this.oel;
            $(oel).hide();
            $(this).hide();
        };
        document.body.appendChild(el);
        document.body.appendChild(oel);
        document.body.$overlayel$ = el;
    } else {
        oel = el.oel;
    }
    if (!c || !c.hide) {
        $(el).show();
        $(oel).show();
    } else {
        $(el).hide();
        $(oel).hide();
    }
    oel.innerHTML = '';
    return oel;
}
function datestr(d) {
    if (!d) {
        d = new Date();
    }
    var s = d.getFullYear() + '/' + (d.getMonth() + 1) + '/' + d.getDate();
    return s;
}
function xhr(c) {
    if (!c) {
        c = {};
    }
    var xr;
    if (window.XMLHttpRequest) { // works with IE7+, Chrome, Firefox, Safari
        xr = new XMLHttpRequest();
    }
    else { // works with IE6, IE5
        xr = new ActiveXObject("Microsoft.XMLHTTP");
    }
    xr.onreadystatechange = function () {
        var self = this;
        if (self.readyState == 4 && self.status == 200) {
            if (c.success) {
                c.success(self.responseText);
            }
        } else if (self.readyState == 4) {
            if (c.error) {
                c.error(self.responseText, self.status);
            }
        } else if (c.statechange) {
            c.statechange(self);
        }
    }
    if (c.url) {
        var method = c.method || 'GET';
        xr.open(method, c.url);
        xr.send(c.form);
    }
    return xr;
}

