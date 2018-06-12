function ajax(url, oarg, dcb, ecb) {
    var u = url;
    var args = '';
    if (oarg) {
        for (var i in oarg) {
            var t = typeof (oarg[i]);
            if (t != 'function' && t != 'object') {
                var v = oarg[i];
                args += i + '=' + v + '&';
            }
        }
        u += '?' + args;
    }
    if (!$) {
        console.log('jQuery not found');
        return;
    }
    $.ajax({ url: u }).done(function (data) {
        try {
            var json = JSON.parse(data);
            if (dcb)
                dcb(json);
        } catch (e) {
            if (ecb) {
                ecb(e);
            }
        }
    }).error(function (e) {
        if (ecb) {
            ecb(e);
        }
    });
    return u;
}
function creator(q) {
    var cs = { curt: null, root: null };
    function set(el) {
        if (!cs.root) {
            cs.root = el;
        }
        cs.curt = el;
    }
    function child(el) {
        if (!cs.root) {
            cs.root = el;
        }
        if (!cs.curt) {
            cs.curt = el;
        }
        else {
            cs.curt.appendChild(el);
        }
    }
    function bind(tag, dat, cb) {
        if (dat && cb) {
            for (var i = 0; i < dat.length; i++) {
                var el = document.createElement(tag);
                child(el);
                var d = dat[i];
                cb(el, d, i);
            }
        }
    }
    if (q) {
        var ls = $(q);
        if (ls && ls.length > 0) {
            set(ls[0]);
        }
    }
    var r = {};
    r.on = function (evt, cb) {
        $(cs.curt).on(evt, cb);
        return this;
    };
    r.select = function () {
        var el = document.createElement('select');
        set(el);
        return this;
    };
    r.option = function (c) {
        if (!c) {
            c = {};
        }
        var dat = c.data;
        var cb = c.onbind;
        if (dat) {
            bind('option', dat, cb);
        } else {
            var el = document.createElement('option');
            el.innerHTML = c.defval || '暂无匹配产品';
            child(el);
        }
        return this;
    };
    r.return = function () {
        return cs.root;
    };
    return r;
}
function loading(cb, delay) {
    try{
        var el = document.body.$lding$;
        if (!el) {
            el = document.createElement('div');
            el.className = 'page-loader';
            el.style.display = 'none';
            el.innerHTML = '<div class="spinner"><div class="bounce1"></div><div class="bounce2"></div><div class="bounce3"></div></div>';
            document.body.appendChild(el);
            document.body.$lding$ = el;
        }
        
        $(el).show();
        if (cb) {
            if (typeof (cb) == 'function') {
                if (!delay) {
                    delay = 100;
                }
                window.setTimeout(function () {
                    cb();
                }, delay);
            }
        }
        return {
            hide: function () {
                $(el).hide();
            }
        };
    } catch (e) {
        alert(e);
    }
}
function List() {
    var r = new Array();
    r.add = function (o) {
        this[this.length] = o;
    }
    r.set = function (k, v, undef) {
        if (v === undef) {
            delete r[k];
        } else {
            r[k] = v;
        }
    }
    return r;
}