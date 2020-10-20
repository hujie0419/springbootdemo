/**
 * cloudjs
 * author: generzhang@tencent.com
 * date: 2015-07-01
 */
(function (global, undefined) {

    var projectName = 'cloudjs', //default name
        //compsPath = './', //relative path of project/*
        compsPath = '//xian.qq.com/js/commonjs/cloudjs/', //relative path of project/*
        defaultZIndex = 1000;

    if (global[projectName]) return;

    var project, charset, remove, force, env, themes, uniq, preload, resolve,
        _projectSrc, _loadedComps, _keyword, _comps, _this, _preload, _noop, _inited, _state, _args, _init, _uri, _proxy, _doc, _html, _comRes, _debug, _srcs, _toString, _aliasMap, _compEnv,
        _isFun, _isArr, _isObj, _isBoo, _isStr, _isNum, _isHop, _arrPro, _loadHandler, _cacheHandler, _taskArr, _defaultVer = '__default_ver_',
        _state_ = '_state_', _args_ = '_args_', _project_ = '_' + projectName + '_', prejectAttr = 'data-' + projectName, _require_ = 'require',
        PRELOAD = 0, PRELOADING = 1, PRELOADED = 2, LOADING = 3, LOADED = 4;
    ABSOLUTE = /^\/\/.|:\//, ROOT = /^.*?\/\/.*?\//, DIR = /[^?#]*\//, DOT = /\/\.\//g, DOUBLE_DOT = /\/[^/]+\/\.\.\//, MULTI_SLASH = /([^:/])\/+\//g;

    _doc = global.document;
    _html = _tags('html')[0];
    _toString = {}.toString;
    _debug = (location.href.indexOf(_project_) > 0);

    _srcs = {};
    _args = {};
    _state = {};
    _comRes = {};
    _compEnv = {};
    _aliasMap = {};
    _loadHandler = {};
    _cacheHandler = {};

    _isFun = function (o) { return _toString.call(o) === '[object Function]'; };
    _isArr = function (o) { return _toString.call(o) === '[object Array]'; };
    _isObj = function (o) { return _toString.call(o) === '[object Object]'; };
    _isBoo = function (o) { return _toString.call(o) === '[object Boolean]'; };
    _isStr = function (o) { return _toString.call(o) === '[object String]'; };
    _isNum = function (o) { return _toString.call(o) === '[object Number]'; };
    _isHop = function (o, p) { return o.hasOwnProperty(p) };
    _arrPro = [];
    _taskArr = [];
    _loadedComps = [];

    /**
     * public components
     */
    _comps = {
        resizable: '',
        draggable: '',
        paging: '',
        table: '',
        cascade: '',
        menu: '',
        clip: '',
        cookie: '',
        calendar: '',
        monthpicker: '',
        tab: '',
        ctab: '',
        slide: '',
        message: '',
        dialog: '',
        tips: '',
        validate: '',
        combobox: '',
        totop: '',
        mask: '',
        calculate: '',
        string: '',
        progressbar: ''
    };

    /**
     * define keyword, developer cant't use
     */
    _keyword = ['charset', 'force', 'include', 'env', 'themes', 'uniq', 'get', 'define', 'callback'];

    /**
     * constructor
     */
    global[projectName] = global.$[projectName] = project = function () {
        var args = arguments;

        if (args.length === 0) {
            _this = undefined;
        } else if (typeof jQuery !== 'undefined') {// if use jquery
            _this = jQuery.apply(global, args);
        } else {
            _this = args[0];
        }

        return project;
    };

    _noop = function () { };

    function _extend(target, source, deep) {

        var i = 1, options;

        target = target || {};
        if (typeof source === 'undefined' || _isBoo(source)) {
            return target;
        }
        if (!_isObj(source) && !_isFun(source)) {
            source = {};
        }

        while (i <= 2) {
            options = i === 1 ? target : source;
            if (options != null) {
                for (var name in options) {
                    var src = target[name], copy = options[name];
                    if (target === copy)
                        continue;
                    if (deep && copy && typeof copy === 'object'
                        && !copy.nodeType)
                        target[name] = _extend(src
                            || (copy.length != null ? [] : {}), copy, deep);
                    else if (copy !== undefined)
                        target[name] = copy;
                }
            }
            i++;
        }
        return target;
    };

    function _tags(tag) {
        return _doc.getElementsByTagName(tag);
    }


    project.zIndex = function (zIndex) {
        if (zIndex) {
            defaultZIndex = zIndex;
        } else {
            defaultZIndex++;
        }
        return defaultZIndex;
    };

    /**
     * set root path
     */
    project.env = function (_env) {
        var _env_ = env;
        if (_env) {
            env = _env.replace(/(^\s*)|(\s*$)/g, "");
            if (!_endWith('/', env)) {
                env += '/';
            }
        }
        return _env_;
    };

    /**
     * set themes
     */
    project.themes = function (_themes) {
        var _themes_ = themes;
        if (_themes) {
            themes = _themes.replace(/(^\s*)|(\s*$)/g, "");
            //_changeCompThemes();
        }
        return _themes_;
    };

    /**
     * change comp themes
     */
    function _changeCompThemes() {
        var reloadCss = [], css;
        for (var i = 0; i < _loadedComps.length; i++) {
            if (css = _comps[_loadedComps[i]]) {
                if (!_isArr(css)) {
                    css = [css];
                }
                for (var k = 0; k < css.length; k++) {
                    reloadCss.push(resolve(compsCssPath + themes + '/' + css[k] + '?r_' + cloudjs.uniq(), _projectSrc));
                }
            }
        }
        force(reloadCss, true);
    }

    function _addToLoadedComps(comp) {
        var fonud = false;
        for (var i = 0; i < _loadedComps.length; i++) {
            if (_loadedComps[i] === comp) {
                fonud = true;
                break;
            }
        }

        if (!fonud && comp) {
            _loadedComps.push(comp);
        }
    }

    /**
     * set charset
     */
    project.charset = function (_charset) {
        var _charset_ = charset;
        charset = (_charset || charset).replace(/(^\s*)|(\s*$)/g, "");
        return _charset_;
    };

    /**
     * arguments sequence
     */
    function _argsSeq(arg) {
        if (_isArr(arg)) {
            return 5
        } else if (_isStr(arg)) {
            return 4;
        } else if (_isBoo(arg)) {
            return 3;
        } else if (_isNum(arg)) {
            return 2;
        } else if (_isFun(arg)) {
            return 1;
        } else {
            return 0;
        }
    }

    /**
     * before use mod/comp, must include them
     */
    project.include = function () {

        var comp, path, alias, _preload, _delay, args = arguments;

        if (args.length > 1) {
            _arrPro.sort.call(args, function (a, b) { return _argsSeq(b) - _argsSeq(a); });
        }

        if (!_isStr(args[1])) {
            _arrPro.splice.call(args, 1, 0, '');
        }
        if (!_isBoo(args[2])) {
            _arrPro.splice.call(args, 2, 0, false);
        }
        if (!_isNum(args[3])) {
            _arrPro.splice.call(args, 3, 0, 0);
        } else {
            args[2] = true;
        }

        path = args[0];
        alias = args[1];
        _preload = args[2];
        _delay = args[3];

        if (_isStr(path)) {
            path = _js(path);
            comp = path.split('/').pop().split('.js')[0];
            comp = _aliasMap[comp] = alias || comp;
            _compEnv[comp] = env;
            _add(comp, path);
            (function (_preload, _delay, comp) {
                setTimeout(function () {
                    _preload && preload(comp);
                }, _delay);
            })(_preload, _delay, comp);
        } else if (path) {
            project.util.error('error: path is must be string type! ' + path);
        }

        return comp || '';
    };

    /**
     * when_force is true. the resource will be added at the last of document
     */
    force = project.force = function (obj, comp, _force, success, fail) {
        var temp, arr, call, res, obj, comp, _force, items, loadFun, args = arguments;

        _init._res();

        if (args.length > 1) {
            _arrPro.sort.call(args, function (a, b) { return _argsSeq(b) - _argsSeq(a); });
        }

        if (!_isStr(args[1])) {
            _arrPro.splice.call(args, 1, 0, '');
        }
        if (!_isBoo(args[2])) {
            _arrPro.splice.call(args, 2, 0, false);
        }
        if (!_isFun(args[3])) {
            _arrPro.splice.call(args, 3, 0, _noop);
        }
        if (!_isFun(args[4])) {
            _arrPro.splice.call(args, 4, 0, _noop);
        }

        obj = args[0];
        comp = args[1];
        _force = args[2];
        success = args[3];
        fail = args[4];

        if (!obj || !obj.length) {
            success();
        }

        items = _getItems(obj, comp);

        loadFun = ('async' in _doc.createElement("script") || "MozAppearance" in _doc.documentElement.style || global.opera) ? _loadAsync : _loadInIE;

        loadFun(items, _force, success, fail);
    };

    function _path(url) {
        var index = url.indexOf('?');
        return (index === -1) ? url : (url.substring(0, index));
    }

    function _setState(item) {
        if (_state[item.name] === undefined) {
            _state[item.name] = [item];
        } else {
            _state[item.name].push(item);
        }
    }

    function _getItems(obj, comp) {
        var temp, items = [], i, len;
        if (typeof obj === 'string') {
            items.push({ name: _path(obj), url: obj, comp: comp });
            _setState(items[0]);
        } else if (_isArr(obj)) {
            obj = obj.concat();
            for (i = 0, len = obj.length; i < len; i++) {
                temp = obj[i];
                temp = { name: _path(temp), url: temp, comp: comp };
                items.push(temp);
                _setState(temp);

            }
        }
        return items;
    }

    function _loadAsync(items, _force, success, fail) {
        _each(items, function (item) {
            _loadRes(item, _force, function () {
                for (var i = 0, len = items.length; i < len; i++) {
                    if (!(items[i].state) || (items[i].state !== LOADED)) return;
                }
                success();
            }, fail);
        });
    }

    function _loadInIE(items, _force, success, fail) {
        var l = function () {
            var len = items.length, ok = true;
            if (len === 0) return;
            (function (i) {
                var args = arguments, obj = items[i];
                _loadRes(obj, _force, function () {
                    if (ok && obj._err) {
                        ok = false;
                    }
                    if (i + 1 === len) {
                        ok && success();
                    } else {
                        args.callee(i + 1);
                    }
                }, fail);
            })(0);
        }
        _each(items, function (item, success) {
            _preLoadCache(item, _force, function () {
                for (var i = 0, len = items.length; i < len; i++) {
                    if (!(items[i].state) || items[i].state === PRELOADING) return;
                }
                l();
            });
        });
    }

    function _preLoadCache(obj, _force, success, fail) {
        if (_cacheHandler[obj.name] !== undefined) {
            if (_cacheHandler[obj.name].length) {
                obj.state = PRELOADING;
                _cacheHandler[obj.name].push(success);
            } else {
                obj.state = PRELOADED;
                success();
            }
        } else {
            _cacheHandler[obj.name] = [success];
            obj.state = PRELOADING;
            _load({ name: obj.name, url: obj.url, type: "cache" }, function (fn) {
                _dealHandler(obj, PRELOADED, _cacheHandler);
            }, fail);
        }
    }

    function _dealHandler(obj, state, handler) {
        _flagState(obj.name, state);
        if (handler[obj.name] !== undefined) {
            while (fn = handler[obj.name].shift()) {
                _one(fn);
            }
        }
    }

    function _flagState(name, state) {
        var objs = _state[name];
        if (objs !== undefined) {
            for (var i = 0, len = objs.length; i < len; i++) {
                objs[i].state = state;
            }
        }
    }

    function _loadRes(obj, _force, success, fail) {
        var suffix = obj.name.substr(obj.name.lastIndexOf('.') + 1), arr, arr;
        if (_loadHandler[obj.name] !== undefined) {
            if (obj.comp) {
                res = _getRes(obj.name);
                _addResAttr(res, obj.comp);
                _comRes[obj.comp][obj.name] = res;
            }
            if (_force) {
                if (suffix === 'css') {
                    //_appendCss(obj.name);
                    //_appr();
                } else if (suffix === 'js') {
                    arr = _loadHandler[obj.name];
                    delete _loadHandler[obj.name];
                    force(obj.url, obj.comp, _force, function () {
                        while (arr && (call = arr.shift())) {
                            call();
                        }
                        success();
                    }, fail);
                    return;
                }
            }
            if (_loadHandler[obj.name].length) {
                obj.state = LOADING;
                _loadHandler[obj.name].push(success);
            } else {
                obj.state = LOADED;
                success();
            }
            return;
        } else {
            _loadHandler[obj.name] = [success];
        }

        function _appr() {
            obj.state = LOADING;
            res = _load(obj, function (fn) {
                _dealHandler(obj, LOADED, _loadHandler);
            }, fail);
            if (obj.comp) {
                _comRes[obj.comp] = _comRes[obj.comp] || {};
                _comRes[obj.comp][obj.name] = res;
                _addResAttr(res, obj.comp);
            }
        }

        _appr();
    }

    function _each(arr, callback) {
        for (var i = 0, l = arr.length; i < l; i++) {
            callback.call(arr, arr[i], i);
        }
    }

    function _one(fn) {
        fn = fn || noop;
        if (fn._done) {
            return;
        }
        fn._done = 1;
        fn();
    }

    function _addResAttr(res, comp) {
        var attr;
        if (res && comp) {
            attr = res.getAttribute(prejectAttr) || '_';
            if (attr.indexOf('_' + comp + '_') < 0) {
                res.setAttribute(prejectAttr, attr + comp + '_');
            }
        }
    }

    function _getRes(url) {
        var p, re, res;
        for (p in _comRes) {
            if (_isHop(_comRes, p)) {
                re = _comRes[p];
                if (res = re[url]) {
                    break;
                }
            }
        }
        return res;
    }

    /**
     * provide a function for user to create uuid
     */
    uniq = project.uniq = function () {
        var o, random = Math.random(), date = new Date(), ms, fmt = 'yyyyMMddHHmmss', k;
        random = ('00000' + random).replace(/\./g, '');
        random = random.substr(random.length - 5);

        o = {
            "M+": date.getMonth() + 1,
            "d+": date.getDate(),
            "H+": date.getHours(),
            "m+": date.getMinutes(),
            "s+": date.getSeconds(),
            "q+": Math.floor((date.getMonth() + 3) / 3),
            "S": date.getMilliseconds()
        };

        if (/(y+)/.test(fmt)) {
            fmt = fmt.replace(RegExp.$1, (date.getFullYear() + "").substr(4 - RegExp.$1.length));
        }
        for (k in o) {
            if (new RegExp("(" + k + ")").test(fmt)) {
                fmt = fmt.replace(RegExp.$1, (RegExp.$1.length == 1) ? (o[k]) : (("00" + o[k]).substr(("" + o[k]).length)));
            }
        }

        ms = "00" + date.getMilliseconds();
        ms = ms.substr(ms.length - 3);
        return fmt + ms + random;
    };

    /**
    *delete resource , such as css
    */
    remove = project.remove = function (comp) {
        var reses = _comRes[comp] || {}, res, attr, p, url;

        for (p in reses) {
            if (_isHop(reses, p)) {
                res = reses[p];
                attr = res.getAttribute(prejectAttr);
                if (attr) {
                    attr = attr.replace(comp + '_', '');
                    if (attr === '_') {
                        url = res.href || res.src;
                        if (url) {
                            url = url.split('?')[0];
                            delete _loadHandler[url];
                            try { _html.removeChild(res); } catch (e) { };
                        }
                    } else {
                        res.setAttribute(prejectAttr, attr);
                    }
                }
            }
        }
    };

    /***
     * append css to the last of document
     */
    function _appendCss(src) {
        var links = _tags('link'), target, link, href, i, len;

        for (i = 0, len = links.length; i < len; i++) {
            link = links[i];
            href = link.getAttribute('href');
            if (href && href.split('?')[0] === src) {
                target = link;
                break;
            }
        }

        if (target) {
            _html.appendChild(target.cloneNode(true));
            target.parentNode.removeChild(target);
            return true;
        } else {
            return false;
        }
    }

    function _js(path) {

        if (!_endWith('.css', path) && path.lastIndexOf('.css?') < 0 && !_endWith('.js', path) && path.lastIndexOf('.js?') < 0) {
            if (path.lastIndexOf('?') > 0) {
                path = path.split('?');
                path = path[0] + '.js?' + path[1];
            } else {
                path = path + '.js';
            }
        }
        return path;
    }

    /**
     * add version for require resource
     */
    function _reqs(rs, c) {

        var i, len, r, v = '', env = project.env(_compEnv[c]);
        for (i = 0, len = rs.length; i < len; i++) {
            r = resolve(rs[i], _srcs[c]);

            if (v = v || '') {
                rs[i] = r.split('?')[0] + '?' + v;
            } else {
                rs[i] = r;
            }
        }

        project.env(env);
        return rs;
    }

    /**
     * load resource js and css
     */
    function _load(obj, success, fail) {

        var res, type, temp, url = obj.url;

        type = url.substr(url.lastIndexOf('.') + 1).split('?')[0];

        if (type === 'js' || obj.type === 'cache') {
            res = _doc.createElement('script');
            res.charset = charset ? charset : 'utf-8';

            res.src = url;
            if ((type = obj.type) === 'cache') {
                res._del = true;
            } else {
                type = 'javascript';
            }
            res.type = 'text/' + type;
            res.del = true;
            res.defer = false;
            res.async = false;

        } else if (type === 'css') {
            res = _doc.createElement('link');
            res.href = url;
            res.rel = 'stylesheet';
            res.type = "text/css";
        } else {
            project.util.error('error: src type is invalidate! ' + url);
        }

        if (res.readyState) {
            res.onreadystatechange = function () {
                if (res.readyState === 'loaded' || res.readyState === 'complete') {
                    res.onreadystatechange = null;
                    if (!res._del && res.readyState !== 'complete' && res.readyState !== 'loaded') {
                        obj._err = true;
                        fail();
                    }
                    loaded();
                }
            };
        } else {
            res.onload = function () {
                loaded();
            };
            res.onerror = function () {
                if (type === 'css') {
                    loaded();
                } else {
                    fail();
                }
            };
        }

        function loaded() {
            success();
            (res._del || res.del && !_debug) && _html.removeChild(res);
        };

        _html.appendChild(res);

        return res;
    };

    /**
     * resolve path
     */
    resolve = project.resolve = function (path, _uri, isDir) {
        var ret, first, m;

        if (!isDir) {
            path = _js(path);
        }

        first = path.charAt(0);

        if (ABSOLUTE.test(path)) {
            ret = path;
        } else if (first === ".") {
            ret = _realpath(_uri.match(DIR)[0] + path);
        } else if (first === "/") {
            m = _uri.match(ROOT);
            ret = m ? m[0] + path.substring(1) : path
        } else {
            ret = env + path;
        }

        if (ret.indexOf("//") === 0) {
            ret = location.protocol + ret;
        }

        return ret;
    };

    function _endWith(end, str) {
        return new RegExp(end + '$').test(str);
    }

    function _realpath(path) {
        path = path.replace(DOT, "/");
        path = path.replace(MULTI_SLASH, "$1/");
        while (path.match(DOUBLE_DOT)) {
            path = path.replace(DOUBLE_DOT, "/");
        }
        return path;
    }

    function _filterRes(reqs, comps) {
        var i, r;
        for (i = reqs.length - 1; i >= 0; i--) {
            r = reqs[i];
            if (!(_endWith('.js', r) || _endWith('.css', r) || r.indexOf('.js?') >= 0 || r.indexOf('.css?') >= 0)) {
                comps.unshift(reqs.splice(i, 1).pop().split('?')[0]);
            }
        }
        return reqs;
    }

    function _reqsComps(comp, comps, callback) {
        var env = project.env(_compEnv[comp]), args = [], i, len;
        for (i = 0, len = comps.length; i < len; i++) {
            args.push(project.include(comps[i]));
        }
        project.env(env);

        args.push(function () {
            callback.call(undefined, this);
        });

        project.require.apply(project, args);

    }

    /**
     * provide a function for developer to create component or module
     */
    project.define = function (obj) {
        var p, _fun, fun, _require = project[_require_];

        project[_require_] = function () {
            return [];
        };

        for (p in obj) {
            if (_isHop(obj, p)) {
                if (!fun && (!_require || p !== _require_) && p.indexOf('_') < 0) {
                    (function (obj) {
                        fun = _isFun(obj) ? obj : function () {
                            return obj;
                        };
                    })(obj[p]);
                    delete obj[p];
                    p = _aliasMap[p] || p;
                    (function (fun) {
                        _fun = obj[p] = function () {
                            (function (args, _t, ret, callback, _callback, reqs, comps) {
                                var cssReqs = [], jsReqs = [], req;
                                reqs = _reqs(_filterRes(fun[_require_].apply(_t, args) || [], comps = []), _fun._comp);

                                while (req = reqs.shift()) {
                                    if (project.util.isEndWith('.css', req) || req.indexOf('.css?') > 0) {
                                        cssReqs.push(req);
                                    } else {
                                        jsReqs.push(req);
                                    }
                                }

                                if (jsReqs.length) {
                                    force(jsReqs, _fun._comp, _css);
                                } else {
                                    _css();
                                }

                                function _css() {
                                    if (cssReqs.length) {
                                        force(cssReqs, _fun._comp, _do);
                                        var _t = setInterval(function () {
                                            if (!_do.done) {
                                                force(cssReqs, _fun._comp, true, _do);
                                            } else {
                                                clearInterval(_t);
                                            }
                                        }, 500);
                                    } else {
                                        _do();
                                    }
                                }

                                function _do() {
                                    if (_do.done) {
                                        return;
                                    }
                                    _do.done = true;
                                    _uri = _srcs[_fun._comp];
                                    if (comps.length) {
                                        _reqsComps(_fun._comp, comps, _f);
                                    } else {
                                        _f();
                                    }
                                    function _f(obj) {
                                        !_isFun(args[args.length - 1]) && _arrPro.push.call(args, _noop);
                                        _callback = project.callback;
                                        callback = project.callback = _arrPro.pop.call(args);
                                        if (obj) {
                                            _t = (_t === global || _t === undefined) ? {} : _t;
                                            _extend(_t, obj, true);
                                        }
                                        (ret = fun.apply(_t, args)) !== undefined && callback.call(ret);
                                        project.callback = _callback;
                                        _this = undefined;
                                    }

                                }

                                return _t;
                            })(arguments, _this);
                        };
                        _fun._comp = p;
                    })(fun);
                } else if (p === _require_) {
                    if (_isArr(obj[_require_])) {
                        (function () {
                            var args = arguments;
                            project[_require_] = function () {
                                return Array.apply(null, args);
                            };
                        }).apply(this, obj[_require_]);

                    } else if (_isStr(obj[_require_])) {
                        (function (arr) {
                            project[_require_] = function () {
                                return arr;
                            };
                        })([obj[_require_]]);
                    } else if (_isFun(obj[_require_])) {
                        project[_require_] = obj[_require_];
                    }
                    delete obj[_require_];
                } else if (p.indexOf('_') < 0) {
                    project.util.error('error: there at most has one name for mod/comp!');
                }
                p = '';
            }

        }

        if (!_fun) {
            project.util.error('error: mod/comp must has one validate name!');
            return;
        }

        _fun[_require_] = fun[_require_] = project[_require_];

        project[_require_] = _require;

        _extend(project, obj, true);
    }

    /**
     * preload resource
     */
    preload = project.preload = function () {
        var i, len, c, args = arguments, callback;

        if (!_isFun(callback = args[args.length - 1])) {
            callback = _noop;
        } else {
            _arrPro.pop.call(args);
        }

        args = _arrPro.slice.call(args);

        for (i = 0, len = args.length; i < len; i++) {
            if (_isStr(c = args[i])) {
                (function (c) {
                    if (_args[c + _state_] === LOADED) {
                        callback(c);
                    } else {
                        _preload = true;
                        project[c](function () {
                            _preload = false;
                            callback(c);
                        });
                        _preload = false;
                    }
                })(c);
            } else if (_isArr(c)) {
                c.push(callback);
                preload.apply(project, c);
            }
        }

        return project;
    };

    function _add(comp, src) {
        if (_keyword[comp] === undefined) {
            if (!_isFun(project[comp]) && _args[comp + _state_] === undefined) {
                _args[comp + _state_] = PRELOAD;
                _args[comp + _args_] = [];
                _srcs[comp] = resolve(src, _uri);
                project[comp] = function () {
                    var args = arguments,
                        _t = _this;
                    _addToLoadedComps(comp);
                    if (_taskArr) {
                        _taskArr.push((function (_t, _preload, comp, args) {
                            return function () {
                                if (_preload) {
                                    project.preload(comp, args[0]);
                                } else {
                                    _this = _t;
                                    project[comp].apply(project, args);
                                }
                            };
                        })(_t, _preload, comp, args));
                        _this = _preload = undefined;
                    } else if (_args[comp + _state_] === PRELOAD) {
                        _arrPro.push.call(args, _this);
                        _this = undefined;
                        _arrPro.push.call(args, _preload);
                        _args[comp + _state_] = LOADING;
                        _args[comp + _args_].push(args);
                        project.util.log(projectName + ' load: ' + comp);
                        project.force(_srcs[comp], comp, function () {
                            var compArgs = _args[comp + _args_], i, len, comps, reqs, req = function () { return [] };
                            _args[comp + _state_] = LOADED;
                            for (i = 0, len = compArgs.length; i < len; i++) {
                                args = compArgs[i];
                                _preload = _arrPro.pop.call(args);
                                _this = _arrPro.pop.call(args);
                                if (_preload) {
                                    reqs = _reqs(_filterRes((project[comp][_require_] || req).apply(_this, args) || [], comps = []), comp);
                                    if (comps.length) {
                                        _uri = _srcs[comp];
                                        for (var j = 0, length = comps.length; j < length; j++) {
                                            comps[j] = project.include(comps[j]);
                                        }
                                        preload(comps, _f);
                                    } else {
                                        _f.call();
                                    }
                                    function _f() {
                                        force(reqs, comp, args[0]);
                                    }
                                } else {
                                    project[comp].apply(_this, args);
                                }
                            }
                            _this = _preload = undefined;
                        });
                    } else if (_args[comp + _state_] === LOADING) {
                        _arrPro.push.call(args, _this);
                        _arrPro.push.call(args, _preload);
                        _args[comp + _args_].push(args);
                        _this = _preload = undefined;
                    } else if (_args[comp + _state_] === LOADED) {
                        _loadHandler = {};
                        _args[comp + _state_] = PRELOAD;
                        if (_proxy !== project[comp].toString()) {
                            project[comp].apply(_this, args);
                        } else {
                            project.util.error('error: file ' + comp + '.js occur error');
                        }
                        _this = _preload = undefined;
                    } else {
                        project.util.error('error: load comp/mod ' + comp + ' fail!');
                    }

                    return _t;
                };

                _proxy = _proxy || project[comp].toString();
            }
        } else {
            project.util.error('error: can not use keyword ' + comp + ' as the name of comp/mod, it has already existed!!');
        }

    };


    /**
    * realize require
    **/
    project.define({
        require: function () {
            var args = arguments, k = 0, i = 0, len = arguments.length, mod, callback = project.callback,
                _this = (this === global) ? undefined : this, obj = {}, funs = [], backs = [], _mod, _fun;
            for (; i < len; i++) {
                mod = args[i];
                _mod = [_noop];
                if (_isArr(mod)) {
                    _mod = mod;
                    mod = _mod.shift();
                    !_isFun(_mod[_mod.length - 1]) && _mod.push(_noop);
                }

                if (_isStr(mod) && project[mod]) {
                    (function (mod, back) {
                        _fun = funs[i] = function () {
                            project(_this)[mod].apply(project, arguments);
                        };
                        _fun._args = _mod;
                        backs[i] = function () {
                            back.call(obj[mod] = this);
                            if (++k === len) {
                                callback.call(obj);
                            }
                        };
                    })(mod, _mod.pop());
                } else {
                    project.util.error('error: need include com/mod: ' + mod);
                }
            }

            (function () {
                var fun, args = arguments;
                if (fun = funs.shift()) {
                    fun._args.push(function () {
                        backs.shift().call(this);
                        args.callee();
                    });
                    fun.apply(project, fun._args);
                }
            })();

        }
    });

    function init() {
        _init = {
            _res: function () {
                var r, n, len, s, l, scripts, links;

                if (_inited) {
                    return;
                }

                _inited = true;

                scripts = _tags('script');
                for (n = 0, len = scripts.length; n < len; n++) {
                    s = scripts[n];
                    if (s.src) {
                        r = s.src.split('?')[0];
                        _loadHandler[r] = [];
                    }
                }
                links = _tags('link');
                for (n = 0, len = links.length; n < len; n++) {
                    l = links[n];
                    if (l.href) {
                        r = l.href.split('?')[0];
                        _loadHandler[r] = [];
                    }
                }
            },
            _keyword: function () {
                var i = 0, len = _keyword.length;
                while (i < len) {
                    _keyword[_keyword[i]] = true;
                    i++;
                }
            },
            _uri: function () {
                _uri = (!location.href || location.href.indexOf('about:') === 0) ? '' : location.href.match(DIR)[0];
                project.env(location.href.split(location.pathname)[0]);//if has comps, this will be override
            },
            _main: function () {
                (function (uri) {
                    var script = _tags('script'), flag = true, rs, main, require, reqs = [], comp, path;
                    script = script[script.length - 1];
                    _projectSrc = script.src;
                    main = script.getAttribute('data-main');
                    require = script.getAttribute('data-require');
                    path = script.getAttribute('data-path');
                    themes = script.getAttribute('data-themes');
                    path = path || compsPath;

                    function d() {
                        if (_doc.addEventListener || event.type === "load" || _doc.readyState === "complete") {
                            if (flag && main) {
                                project.util.log('main ...');
                                flag = false;
                                main = main.replace(new RegExp(';', 'gm'), ',').split(',');
                                for (var i = 0, len = main.length; i < len; i++) {
                                    _uri = uri;
                                    (rs = main[i]) && f(rs, project.include(rs));
                                }
                            }
                        }
                    }

                    (function () {
                        if (require) {
                            project.util.log('require ...');
                            require = require.replace(new RegExp(';', 'gm'), ',').split(',');
                            for (var i = 0, len = require.length; i < len; i++) {
                                if (rs = require[i]) {
                                    reqs.push(resolve(rs, uri));
                                }
                            }
                            force(reqs, m);
                        } else {
                            m();
                        }
                    })();

                    (function () {
                        project.env(resolve(path, _projectSrc, true));
                        for (var p in _comps) {
                            if (_isHop(_comps, p)) {
                                project.include(p);
                            }
                        }

                        project.env(resolve(path, _projectSrc, true));
                        for (var i = 0, len = _taskArr.length; i < len; i++) {
                            setTimeout(_taskArr[i], 0);
                        }
                        _taskArr = null;
                    })();

                    function m() {
                        if (_doc.readyState === "complete") {
                            setTimeout(d);
                        } else if (_doc.addEventListener) {
                            _doc.addEventListener("DOMContentLoaded", d, false);
                            global.addEventListener("load", d, false);
                        } else {
                            _doc.attachEvent("onreadystatechange", d);
                            global.attachEvent("onload", d);
                        }
                    }

                    function f(path, comp) {
                        force(resolve(path || '', _uri), function () {
                            if (_proxy !== project[comp].toString()) {
                                project[comp]();
                            }
                        });
                    }
                })(_uri);
            },
            _browser: function () {
                var browser = { ie: 'ie', firefox: 'firefox', chrome: 'chrome', opera: 'opera', safari: 'safari', others: 'others', type: 'others', shell: '', ver: '0', version: '0' }, sys = {}, ua = navigator.userAgent.toLowerCase(), s;

                (s = ua.match(/msie ([\d.]+)/)) ? sys.ie = s[1] :
                    (s = ua.match(/firefox\/([\d.]+)/)) ? sys.firefox = s[1] :
                        (s = ua.match(/chrome\/([\d.]+)/)) ? sys.chrome = s[1] :
                            (s = ua.match(/opera.([\d.]+)/)) ? sys.opera = s[1] :
                                (s = ua.match(/version\/([\d.]+).*safari/)) ? sys.safari = s[1] : 0;

                if (sys.ie) {
                    browser.type = 'ie';
                } else if (sys.firefox) {
                    browser.type = 'firefox';
                } else if (sys.chrome) {
                    browser.type = 'chrome';
                } else if (sys.opera) {
                    browser.type = 'opera';
                } else if (sys.safari) {
                    browser.type = 'safari';
                }
                if (browser.type !== 'others') {
                    browser.ver = sys[browser.type].split('.')[0];
                    browser.version = sys[browser.type];
                } else {
                    browser.ver = '';
                    browser.version = '';
                }

                browser.mode = document.documentMode || browser.ver;

                browser.shell =
                    (ua.indexOf('360ee') > -1) ? '360ee' :
                        (ua.indexOf('360se') > -1) ? '360se' :
                            (ua.indexOf('se') > -1) ? 'sougou' :
                                (ua.indexOf('aoyou') > -1) ? 'aoyou' :
                                    (ua.indexOf('theworld') > -1) ? 'theworld' :
                                        (ua.indexOf('worldchrome') > -1) ? 'worldchrome' :
                                            (ua.indexOf('greenbrowser') > -1) ? 'greenbrowser' :
                                                (ua.indexOf('qqbrowser') > -1) ? 'qqbrowser' :
                                                    (ua.indexOf('baidu') > -1) ? 'baidu' :
                                                        (ua.indexOf('liebao') > -1) ? 'liebao' :
                                                            'unknow';

                project.browser = browser;
            }
        };

        _init._keyword();
        _init._browser();
        _init._uri();
        _init._main();
    };

    init();

})(this);

(function (global, undefined) {

    var util = cloudjs.util = {};

    /**
     * 判断字符串str是否以某字符串end结尾
     * 参数：end 结尾字符串，str 判断的字符串
     * 返回：true，false
     */
    util.isEndWith = function (end, str) {
        return new RegExp(end + '$').test(str);
    };

    /**
     * 判断字符串str是否以某字符串start开始
     * 参数：start 开始字符串，str 判断的字符串
     * 返回：true，false
     */
    util.isStartWith = function (start, str) {
        return new RegExp('^' + start).test(str);
    };

    /**
     * 兼容低于IE9版本浏览器中Array的indexOf方法
     * 参数：obj 为数组或字符串，point 查询的目标字符串或数组元素，formIndex 开始的索引位置
     * 返回：point的位置,不存在则返回-1
     */
    util.indexOf = function (obj, point, fromIndex) {
        var i, len;
        fromIndex = fromIndex || 0;
        if (typeof obj === 'string') {
            return obj.indexOf(point, fromIndex);
        } else {
            for (i = fromIndex, len = obj.length; i < len; i++) {
                if (obj[i] === point) {
                    return i;
                }
            }
            return -1;
        }
    };

    /**
     * 和浏览器默认的console.log一致
     * 参数个数不限，会把所有参数一一打印出来，中间空格隔开
     */
    util.log = function () {
        var _console = global.console;
        try {
        	/*if(_console && _console.log && _console.log.apply){
                [].unshift.call(arguments, '[cloudjs.util.log]');
                _console.log.apply(_console, arguments);
            }*/
        } catch (e) { }
    };

    /**
     * 和浏览器默认的console.error一致，打印的信息会红色标记
     * 参数个数不限，会把所有参数一一打印出来，中间空格隔开
     */
    util.error = function () {
        var _console = global.console;
        try {
            if (_console && _console.error && _console.error.apply) {
                [].unshift.call(arguments, '[cloudjs.util.error]');
                _console.error.apply(_console, arguments);
            }
        } catch (e) { }
    }

    /**
     * 和浏览器默认的console.trace一致，能打印调用栈，这对于调试调用链比较有帮助
     * 参数个数不限，会把所有参数一一打印出来，中间空格隔开，同时还会打印调用栈
     */
    util.trace = function () {
        var _console = global.console;
        try {
            if (_console && _console.trace && _console.trace.apply) {
                [].unshift.call(arguments, '[cloudjs.util.trace]');
                _console.trace.apply(_console, arguments);
            }
        } catch (e) { }
    }

})(this);
/*  |xGv00|608c9de5e20099873df0c8c3f0c6bc13 */