window.Tuhu = window.Tuhu || (function (window, document, $)
{
    var Tuhu = {};

    var $win = $(window);
    var $doc = $(document);
    window.ieVersion = undefined;

    //#region Extensions
    //#region 获得IE版本
    if (window.ActiveXObject)
    {
        if (!window.XMLHttpRequest)
            window.ieVersion = 6;
        else if (!document.querySelector)
            window.ieVersion = 7;
        else if (!window.Geolocation)
            window.ieVersion = 8;
        else if (!window.cancelAnimationFrame)
            window.ieVersion = 9;
        else
            window.ieVersion = 10;
    }
    //#endregion

    location.queryString = (function ()
    {
        var queryString = [];
        var search = location.search.substring(1);
        if (search)
        {
            var array = search.split("&");
            for (var index = 0; index < array.length; index++)
            {
                var pair = array[index].split("=");
                var value = decodeURIComponent(pair[1]);
                queryString[index] = value;
                queryString[pair[0]] = value;
            }
        }
        return queryString;
    })();

    //#region String扩展方法
    if (!String.prototype.trim)
        String.prototype.trim = function ()
        {
            ///	<signature>
            ///		<summary>获取字符串字节长度（单字节占一位，多字节占两位）</summary>
            ///		<returns type="Number" />
            ///	</signature>
            return $.trim(this);
        };

    String.prototype.startsWith = function (suffix, ignoreCase)
    {
        ///	<signature>
        ///		<summary>判断字符串是否是以输入的字符串结束</summary>
        ///		<param name="suffix" type="String">结尾字符串</param>
        ///		<returns type="Boolean" />
        ///	</signature>
        ///	<signature>
        ///		<summary>判断字符串是否是以输入的字符串结束</summary>
        ///		<param name="suffix" type="String">结尾字符串</param>
        ///		<param name="ignoreCase" type="Boolean">是否忽略大小写</param>
        ///		<returns type="Boolean" />
        ///	</signature>
        var str = this;
        if (ignoreCase)
        {
            str = str.toLowerCase();
            suffix = suffix.toLowerCase();
        }
        return str.indexOf(suffix) === 0;
    };

    String.prototype.endsWith = function (suffix, ignoreCase)
    {
        ///	<signature>
        ///		<summary>判断字符串是否是以输入的字符串结束</summary>
        ///		<param name="suffix" type="String">结尾字符串</param>
        ///		<returns type="Boolean" />
        ///	</signature>
        ///	<signature>
        ///		<summary>判断字符串是否是以输入的字符串结束</summary>
        ///		<param name="suffix" type="String">结尾字符串</param>
        ///		<param name="ignoreCase" type="Boolean">是否忽略大小写</param>
        ///		<returns type="Boolean" />
        ///	</signature>
        var str = this;
        if (ignoreCase)
        {
            str = str.toLowerCase();
            suffix = suffix.toLowerCase();
        }
        return str.indexOf(suffix, this.length - suffix.length) !== -1;
    };
    //#endregion

    //#region Array扩展方法
    (function ()
    {
        //#region Before
        function DefaultEqualityComparer(a, b)
        {
            if (typeof a === 'undefined')
                return typeof b === 'undefined'
            if (a === null)
                return b === null;

            return a === b || a.valueOf() === b.valueOf();
        };

        function DefaultSortComparer(a, b)
        {
            if (a === b) return 0;
            if (a == null) return -1;
            if (b == null) return 1;
            if (typeof a == "string") return a.toString().localeCompare(b.toString());
            return a.valueOf() - b.valueOf();
        };

        function DefaultPredicate()
        {
            return true;
        };

        function DefaultSelector(t)
        {
            return t;
        };
        //#endregion

        //#region JavaScript LINQ Selectors
        Array.prototype.select = Array.prototype.map || function (selector, context)
        {
            context = context || window;
            var arr = [];
            var l = this.length;
            for (var i = 0; i < l; i++)
                arr.push(selector.call(context, this[i], i, this));
            return arr;
        };

        Array.prototype.selectMany = function (selector, resSelector)
        {
            resSelector = resSelector || function (i, res)
            {
                return res;
            };
            return this.aggregate(function (a, b, i)
            {
                return a.concat(selector(b, i).select(function (res)
                {
                    return resSelector(b, res)
                }));
            }, []);
        };

        Array.prototype.take = function (c)
        {
            return this.slice(0, c);
        };

        Array.prototype.skip = function (c)
        {
            return this.slice(c);
        };

        Array.prototype.first = function (predicate, def)
        {
            var l = this.length;
            if (!predicate) return l ? this[0] : def == null ? null : def;
            for (var i = 0; i < l; i++)
                if (predicate(this[i], i, this))
                    return this[i];
            return def == null ? null : def;
        };

        Array.prototype.last = function (predicate, def)
        {
            var l = this.length;
            if (!predicate) return l ? this[l - 1] : def == null ? null : def;
            while (l-- > 0)
                if (predicate(this[l], l, this))
                    return this[l];
            return def == null ? null : def;
        };

        Array.prototype.union = function (arr)
        {
            return this.concat(arr).distinct();
        };

        Array.prototype.except = function (arr, comparer)
        {
            if (!(arr instanceof Array)) arr = [arr];
            comparer = comparer || DefaultEqualityComparer;
            var l = this.length;
            var res = [];
            for (var i = 0; i < l; i++)
            {
                var k = arr.length;
                var t = false;
                while (k-- > 0)
                {
                    if (comparer(this[i], arr[k]) === true)
                    {
                        t = true;
                        break;
                    }
                }
                if (!t) res.push(this[i]);
            }
            return res;
        };

        Array.prototype.distinct = function (comparer)
        {
            var arr = [];
            var l = this.length;
            for (var i = 0; i < l; i++)
            {
                if (!arr.contains(this[i], comparer))
                    arr.push(this[i]);
            }
            return arr;
        };

        Array.prototype.intersect = function (second)
        {
            ///	<signature>
            ///		<summary>取两个数组相同项</summary>
            ///		<param name="second" type="Array">另一个数组</param>
            ///		<returns type="Array" />
            ///	</signature>
            var source = [];

            var length = this.length;
            for (var index = 0; index < length; index++)
                if (second.indexOf(this[index]) >= 0)
                    source.push(this[index]);

            return source;
        }

        Array.prototype.zip = function (arr, selector)
        {
            return this
				.take(Math.min(this.length, arr.length))
				.select(function (t, i)
				{
				    return selector(t, arr[i]);
				});
        };

        Array.prototype.indexOf = Array.prototype.indexOf || function (o, index)
        {
            var l = this.length;
            for (var i = Math.max(Math.min(index, l), 0) || 0; i < l; i++)
                if (this[i] === o) return i;
            return -1;
        };

        Array.prototype.lastIndexOf = Array.prototype.lastIndexOf || function (o, index)
        {
            var l = Math.max(Math.min(index || this.length, this.length), 0);
            while (l-- > 0)
                if (this[l] === o) return l;
            return -1;
        };

        Array.prototype.remove = function (item)
        {
            var i = this.indexOf(item);
            if (i != -1)
                this.splice(i, 1);
        };

        Array.prototype.removeAt = function (index)
        {
            this.splice(index, 1);
        }

        Array.prototype.removeAll = function (predicate)
        {
            var item;
            var i = 0;
            while (item = this.first(predicate))
            {
                i++;
                this.remove(item);
            }
            return i;
        };

        Array.prototype.insert = function (index, item)
        {
            this.splice(index, 0, item);
        }

        Array.prototype.orderBy = function (selector, comparer)
        {
            comparer = comparer || DefaultSortComparer;
            var arr = this.slice(0);
            var fn = function (a, b)
            {
                return comparer(selector(a), selector(b));
            };

            arr.thenBy = function (selector, comparer)
            {
                comparer = comparer || DefaultSortComparer;
                return arr.orderBy(DefaultSelector, function (a, b)
                {
                    var res = fn(a, b);
                    return res === 0 ? comparer(selector(a), selector(b)) : res;
                });
            };

            arr.thenByDescending = function (selector, comparer)
            {
                comparer = comparer || DefaultSortComparer;
                return arr.orderBy(DefaultSelector, function (a, b)
                {
                    var res = fn(a, b);
                    return res === 0 ? -comparer(selector(a), selector(b)) : res;
                });
            };

            return arr.sort(fn);
        };

        Array.prototype.orderByDescending = function (selector, comparer)
        {
            comparer = comparer || DefaultSortComparer;
            return this.orderBy(selector, function (a, b)
            {
                return -comparer(a, b)
            });
        };

        Array.prototype.innerJoin = function (arr, outer, inner, result, comparer)
        {
            comparer = comparer || DefaultEqualityComparer;
            var res = [];

            this.forEach(function (t)
            {
                arr.where(function (u)
                {
                    return comparer(outer(t), inner(u));
                })
					.forEach(function (u)
					{
					    res.push(result(t, u));
					});
            });

            return res;
        };

        Array.prototype.groupJoin = function (arr, outer, inner, result, comparer)
        {
            comparer = comparer || DefaultEqualityComparer;
            return this
				.select(function (t)
				{
				    var key = outer(t);
				    return {
				        outer: t,
				        inner: arr.where(function (u)
				        {
				            return comparer(key, inner(u));
				        }),
				        key: key
				    };
				})
				.select(function (t)
				{
				    t.inner.key = t.key;
				    return result(t.outer, t.inner);
				});
        };

        Array.prototype.groupBy = function (selector, comparer)
        {
            var grp = [];
            var l = this.length;
            comparer = comparer || DefaultEqualityComparer;
            selector = selector || DefaultSelector;

            for (var i = 0; i < l; i++)
            {
                var k = selector(this[i]);
                var g = grp.first(function (u)
                {
                    return comparer(u.key, k);
                });

                if (!g)
                {
                    g = [];
                    g.key = k;
                    grp.push(g);
                }

                g.push(this[i]);
            }
            return grp;
        };

        Array.prototype.toDictionary = function (keySelector, valueSelector)
        {
            var o = {};
            var l = this.length;
            while (l-- > 0)
            {
                var key = keySelector(this[l]);
                if (key == null || key == "") continue;
                o[key] = valueSelector(this[l]);
            }
            return o;
        };
        //#endregion

        //#region JavaScript LINQ Aggregations
        Array.prototype.aggregate = Array.prototype.reduce || function (func, seed)
        {
            var arr = this.slice(0);
            var l = this.length;
            if (seed == null) seed = arr.shift();

            for (var i = 0; i < l; i++)
                seed = func(seed, arr[i], i, this);

            return seed;
        };

        Array.prototype.min = function (s)
        {
            s = s || DefaultSelector;
            var l = this.length;
            var min = s(this[0]);
            while (l-- > 0)
                if (s(this[l]) < min) min = s(this[l]);
            return min;
        };

        Array.prototype.max = function (s)
        {
            s = s || DefaultSelector;
            var l = this.length;
            var max = s(this[0]);
            while (l-- > 0)
                if (s(this[l]) > max) max = s(this[l]);
            return max;
        };

        Array.prototype.sum = function (s)
        {
            s = s || DefaultSelector;
            var l = this.length;
            var sum = 0;
            while (l-- > 0) sum += s(this[l]);
            return sum;
        };
        //#endregion

        //#region JavaScript LINQ Predicates
        Array.prototype.where = Array.prototype.filter || function (predicate, context)
        {
            context = context || window;
            var arr = [];
            var l = this.length;
            for (var i = 0; i < l; i++)
                if (predicate.call(context, this[i], i, this) === true) arr.push(this[i]);
            return arr;
        };
        Array.prototype.any = function (predicate, context)
        {
            context = context || window;
            var f = this.some || function (p, c)
            {
                var l = this.length;
                if (!p) return l > 0;
                while (l-- > 0)
                    if (p.call(c, this[l], l, this) === true) return true;
                return false;
            };
            return f.apply(this, [predicate, context]);
        };
        Array.prototype.all = function (predicate, context)
        {
            context = context || window;
            predicate = predicate || DefaultPredicate;
            var f = this.every || function (p, c)
            {
                return this.length == this.where(p, c).length;
            };
            return f.apply(this, [predicate, context]);
        };
        Array.prototype.takeWhile = function (predicate)
        {
            predicate = predicate || DefaultPredicate;
            var l = this.length;
            var arr = [];
            for (var i = 0; i < l && predicate(this[i], i) === true; i++)
                arr.push(this[i]);

            return arr;
        };
        Array.prototype.skipWhile = function (predicate)
        {
            predicate = predicate || DefaultPredicate;
            var l = this.length;
            var i = 0;
            for (i = 0; i < l; i++)
                if (predicate(this[i], i) === false) break;

            return this.skip(i);
        };
        Array.prototype.contains = function (o, comparer)
        {
            comparer = comparer || DefaultEqualityComparer;
            var l = this.length;
            while (l-- > 0)
                if (comparer(this[l], o) === true) return true;
            return false;
        };
        //#endregion

        //#region JavaScript LINQ Iterations
        Array.prototype.forEach = Array.prototype.forEach || function (callback, context)
        {
            context = context || window;
            var l = this.length;
            for (var i = 0; i < l; i++)
                callback.call(context, this[i], i, this);
        };

        Array.prototype.defaultIfEmpty = function (val)
        {
            return this.length == 0 ? [val == null ? null : val] : this;
        };
        //#endregion
    })();
    //#endregion

    //#region jQuery扩展方法
    $.fn.drag = function (dragHandle)
    {
        ///	<signature>
        ///		<summary>拖动元素</summary>
        ///		<param name="dragHandle" type="selector">默认为自己</param>
        ///		<returns type="this" />
        ///	</signature>
        var target = this;
        dragHandle = dragHandle ? $(dragHandle, target) : target;
        dragHandle.css("cursor", "move");

        var diffX = 0,
			diffY = 0;
        var mousemove = function (event)
        {
            target.css({
                "left": event.clientX - diffX,
                "top": event.clientY - diffY
            });
        }

        var mouseup = function (event)
        {
            $doc.off("mousemove", mousemove).off("mouseup", mouseup);
        }

        dragHandle.mousedown(function (event)
        {
            diffX = event.pageX - target.offset().left;
            diffY = event.pageY - target.offset().top;
            $doc.on("mousemove", mousemove).on("mouseup", mouseup);
        });
        return this;
    };

    if (window.ActiveXObject && !('oninput' in window)) (function ()
    {
        var isInput = function (elem)
        {
            return (elem.nodeName === "INPUT" && elem.type === "text") || elem.nodeName === "TEXTAREA";
        };
        var isFirstChange = true;
        var input = function (event)
        {
            if (isFirstChange && event.propertyName === "value")
            {
                isFirstChange = false;
                $.event.trigger('input', null, event.srcElement);
                isFirstChange = true;
            }
        };
        $.event.special.input = {
            setup: function ()
            {
                if (!isInput(this))
                    return false;

                this.attachEvent("onpropertychange", input);
            },
            teardown: function ()
            {
                this.detachEvent("onpropertychange", input);
            }
        };
    })();
    $.fn.input = function (callback)
    {
        ///	<signature>
        ///		<summary>触发文本输入事件</summary>
        ///		<returns type="jQuery" />
        ///	</signature>
        ///	<signature>
        ///		<summary>绑定文本输入事件</summary>
        ///		<param name="callback" type="Event">事件处理程序</param>
        ///		<returns type="jQuery" />
        ///	</signature>
        return callback ? this.on('input', callback) : this.trigger('input');
    };

    (function ()
    {
        var isInput = function (elem)
        {
            return (elem.nodeName === "INPUT" && elem.type === "text") || elem.nodeName === "TEXTAREA";
        };

        function placeholder(placeholderClassName, eventType)
        {
            if (!$.data(this, "__placeholder__"))
            {
                var focus = function ()
                {
                    if (!hasValue)
                    {
                        flag = false;
                        this.value = "";
                        $(this).removeClass(placeholderClassName);
                    } else
                        flag = true;
                }
                var blur = function ()
                {
                    if (!hasValue)
                    {
                        flag = false;
                        this.value = this.getAttribute("placeholder");
                        $(this).addClass(placeholderClassName);
                    }
                }
                var hasValue = this.value;
                if (!hasValue)
                    blur.apply(this);
                var flag = true;

                $(this).focus(focus).blur(blur).input(function ()
                {
                    if (flag)
                        hasValue = this.value;
                    flag = true;
                });
                eventType === "focusin" ? focus.apply(this) : blur.apply(this);

                $.data(this, "__placeholder__", true);
            }
        }

        $.fn.placeholder = function (placeholderClassName)
        {
            if (!placeholderClassName)
                placeholderClassName = "placeholder";

            if (!("placeholder" in document.createElement("input")))
            {
                this.each(function ()
                {
                    if (isInput(this))
                        placeholder.call(this, placeholderClassName);
                    else
                    {
                        this.on("focusin", function (event)
                        {
                            if (isInput(this))
                                placeholder.call(this, placeholderClassName, event.type);
                        }).on("focusout", function (event)
                        {
                            if (isInput(this))
                                placeholder.call(this, placeholderClassName, event.type);
                        });
                    }
                });
            }
            return this;
        }
    })();

    $.nonblockEach = function (collection, callback)
    {
        ///	<signature>
        ///		<summary>不阻塞遍历集合</summary>
        ///		<param name="collection" type="Array">集合</param>
        ///		<param name="callback(indexInArray, valueOfElement)" type="Function">处理程序</param>
        ///   <returns type="Deferred" />
        ///	</signature>
        var deferred = $.Deferred(); // 新建一个Deferred对象

        if ($.isArray(collection))
        {
            var length = collection.length,
				index = 0;
            var action = function ()
            {
                try
                {
                    var value = callback.call(collection[index], index, collection[index]);

                    deferred.notify(index++);

                    if (value === false)
                    {
                        deferred.resolve(false);
                        return false;
                    }
                } catch (e)
                {
                    deferred.reject();
                }

                if (index < length)
                    setTimeout(action, 1);
                else
                    deferred.resolve(true);
            }
            setTimeout(action, 1);
        } else
            throw new Error("collection不是数组");

        return deferred.promise(); // 返回promise对象
    }
    //#endregion
    //#endregion

    Tuhu.Domain = (function ()
    {
        var domainConfig = {};

        domainConfig.TopDomain = ".tuhu.cn";
        if (/\.tuhu\.(\w+)$/.test(location.host))
            domainConfig.TopDomain = ".tuhu." + RegExp.$1;

        domainConfig.ApiSite = location.protocol + "//api" + domainConfig.TopDomain;
        domainConfig.ProductSite = location.protocol + "//item" + domainConfig.TopDomain;
        domainConfig.ResourceSite = location.protocol + "//resource" + domainConfig.TopDomain;
        domainConfig.WwwSite = location.protocol + "//www" + domainConfig.TopDomain;
        domainConfig.MySite = location.protocol + "//my" + domainConfig.TopDomain;
        domainConfig.Beautify = location.protocol + "//mr" + domainConfig.TopDomain;
        domainConfig.BaoYangSite = location.protocol + "//by" + domainConfig.TopDomain;
        domainConfig.InOutSite = location.protocol + "//inout" + domainConfig.TopDomain;

    	//document.domain = domainConfig.TopDomain.substring(1);
        //window.name = location.href;
        return domainConfig;
    })();

    Tuhu.Cookie = {
        Get: function (name)
        {
            /// <signature>
            ///		<summary>获得cookie</summary>
            ///		<param name="name" type="String">cookie名称</param>
            ///		<returns type="String" />
            /// </signature>
            if (!name)
            {
                throw new Error("cookie名称是必须的");
            }
            var cookieName = encodeURIComponent(name) + "=";
            var cookieStart = document.cookie.indexOf(cookieName);
            var cookieValue = null;
            if (cookieStart > -1)
            {
                var cookieEnd = document.cookie.indexOf(";", cookieStart);
                if (cookieEnd === -1)
                {
                    cookieEnd = document.cookie.length;
                }
                cookieValue = decodeURIComponent(document.cookie.substring(cookieStart + cookieName.length, cookieEnd).replace(/\+/g, ' '));
            }
            return cookieValue;
        },
        Set: function (name, value, expires, domain, path, secure)
        {
            /// <signature>
            ///		<summary>设置cookie</summary>
            ///		<param name="name" type="String">cookie名称</param>
            ///		<param name="value" type="String">cookie值</param>
            /// </signature>
            /// <signature>
            ///		<summary>设置cookie</summary>
            ///		<param name="name" type="String">cookie名称</param>
            ///		<param name="value" type="String">cookie值</param>
            ///		<param name="expires" type="Date">cookie有效斯</param>
            /// </signature>
            /// <signature>
            ///		<summary>设置cookie</summary>
            ///		<param name="name" type="String">cookie名称</param>
            ///		<param name="value" type="String">cookie值</param>
            ///		<param name="expires" type="Date">cookie有效斯</param>
            ///		<param name="domain" type="String">cookie域</param>
            /// </signature>
            /// <signature>
            ///		<summary>设置cookie</summary>
            ///		<param name="name" type="String">cookie名称</param>
            ///		<param name="value" type="String">cookie值</param>
            ///		<param name="expires" type="Date">cookie有效斯</param>
            ///		<param name="domain" type="String">cookie域</param>
            ///		<param name="path" type="String">cookie路径</param>
            /// </signature>
            /// <signature>
            ///		<summary>设置cookie</summary>
            ///		<param name="name" type="String">cookie名称</param>
            ///		<param name="value" type="String">cookie值</param>
            ///		<param name="expires" type="Date">cookie有效斯</param>
            ///		<param name="domain" type="String">cookie域</param>
            ///		<param name="path" type="String">cookie路径</param>
            ///		<param name="secure" type="Boolean">true或false</param>
            /// </signature>
            if (!name || arguments.length < 2)
            {
                throw new Error("cookie名称和值是必须的");
            }
            var cookieText = encodeURIComponent(name) + "=" + encodeURIComponent(value);
            if (expires instanceof Date)
                cookieText += "; expires=" + expires.toGMTString();
            if (domain)
                cookieText += "; domain=" + domain;
            cookieText += "; path=" + (path || "/");
            if (secure)
                cookieText += "; secure";
            document.cookie = cookieText;
        },
        Unset: function (name, domain, path, secure)
        {
            /// <signature>
            ///		<summary>删除cookie</summary>
            ///		<param name="name" type="String">cookie名称</param>
            /// </signature>
            /// <signature>
            ///		<summary>删除cookie</summary>
            ///		<param name="name" type="String">cookie名称</param>
            ///		<param name="domain" type="String">cookie域</param>
            /// </signature>
            /// <signature>
            ///		<summary>删除cookie</summary>
            ///		<param name="name" type="String">cookie名称</param>
            ///		<param name="domain" type="String">cookie域</param>
            ///		<param name="path" type="String">cookie路径</param>
            /// </signature>
            /// <signature>
            ///		<summary>删除cookie</summary>
            ///		<param name="name" type="String">cookie名称</param>
            ///		<param name="domain" type="String">cookie域</param>
            ///		<param name="path" type="String">cookie路径</param>
            ///		<param name="secure" type="Boolean">true或false</param>
            /// </signature>
            this.Set(name, "", new Date(0), domain, path, secure);
        },
        GetSub: function (name, subName)
        {
            /// <signature>
            ///		<summary>获得子cookie</summary>
            ///		<param name="name" type="String">cookie父名称</param>
            ///		<param name="subName" type="String">子cookie名称</param>
            ///		<returns type="String" />
            /// </signature>
            if (!name || !subName)
            {
                throw new Error("cookie父名称和子名称是必须的");
            }
            var subCookies = this.getAll(name);
            if (!!subCookies)
                return subCookies[subName];
            return null;
        },
        GetAll: function (name)
        {
            /// <signature>
            ///		<summary>获得所有子cookie</summary>
            ///		<param name="name" type="String">cookie名称</param>
            ///		<returns type="Array" />
            /// </signature>
            if (!name)
            {
                throw new Error("cookie名称是必须的");
            }
            var cookieValue = this.get(name);
            if (!!cookieValue)
            {
                var subCookies = cookieValue.split('&');
                var result = {};
                for (var index = 0; index < subCookies.length; index++)
                {
                    var parts = subCookies[index].split("=");
                    result[decodeURIComponent(parts[0].replace(/\+/g, ' '))] = decodeURIComponent(parts[1].replace(/\+/g, ' '));
                }
                return result;
            }
            return null;
        },
        SetSub: function (name, subName, value, expires, domain, path, secure)
        {
            /// <signature>
            ///		<summary>设置子cookie</summary>
            ///		<param name="name" type="String">cookie名称</param>
            ///		<param name="subName" type="String">子cookie名称</param>
            ///		<param name="value" type="String">子cookie值</param>
            /// </signature>
            /// <signature>
            ///		<summary>设置子cookie</summary>
            ///		<param name="name" type="String">cookie名称</param>
            ///		<param name="subName" type="String">子cookie名称</param>
            ///		<param name="value" type="String">子cookie值</param>
            ///		<param name="expires" type="Date">cookie有效斯</param>
            /// </signature>
            /// <signature>
            ///		<summary>设置子cookie</summary>
            ///		<param name="name" type="String">cookie名称</param>
            ///		<param name="subName" type="String">子cookie名称</param>
            ///		<param name="value" type="String">子cookie值</param>
            ///		<param name="expires" type="Date">cookie有效斯</param>
            ///		<param name="domain" type="String">cookie域</param>
            /// </signature>
            /// <signature>
            ///		<summary>设置子cookie</summary>
            ///		<param name="name" type="String">cookie名称</param>
            ///		<param name="subName" type="String">子cookie名称</param>
            ///		<param name="value" type="String">子cookie值</param>
            ///		<param name="expires" type="Date">cookie有效斯</param>
            ///		<param name="domain" type="String">cookie域</param>
            ///		<param name="path" type="String">cookie路径</param>
            /// </signature>
            /// <signature>
            ///		<summary>设置子cookie</summary>
            ///		<param name="name" type="String">cookie名称</param>
            ///		<param name="subName" type="String">子cookie名称</param>
            ///		<param name="value" type="String">子cookie值</param>
            ///		<param name="expires" type="Date">cookie有效斯</param>
            ///		<param name="domain" type="String">cookie域</param>
            ///		<param name="path" type="String">cookie路径</param>
            ///		<param name="secure" type="Boolean">true或false</param>
            /// </signature>
            if (!name || !subName || !!value)
            {
                throw new Error("cookie父名称、子名称和子值是必须的");
            }
            var subCookies = this.getAll(name) || {};
            subCookies[subName] = value;
            this.SetAll(name, subCookies, expires, domain, path, secure);
        },
        SetAll: function (name, subCookies, expires, domain, path, secure)
        {
            /// <signature>
            ///		<summary>设置cookie</summary>
            ///		<param name="name" type="String">cookie名称</param>
            ///		<param name="subCookies" type="Object">cookie值对象</param>
            /// </signature>
            /// <signature>
            ///		<summary>设置cookie</summary>
            ///		<param name="name" type="String">cookie名称</param>
            ///		<param name="subCookies" type="Object">cookie值对象</param>
            ///		<param name="expires" type="Date">cookie有效斯</param>
            /// </signature>
            /// <signature>
            ///		<summary>设置cookie</summary>
            ///		<param name="name" type="String">cookie名称</param>
            ///		<param name="subCookies" type="Object">cookie值对象</param>
            ///		<param name="expires" type="Date">cookie有效斯</param>
            ///		<param name="domain" type="String">cookie域</param>
            /// </signature>
            /// <signature>
            ///		<summary>设置cookie</summary>
            ///		<param name="name" type="String">cookie名称</param>
            ///		<param name="subCookies" type="Object">cookie值对象</param>
            ///		<param name="expires" type="Date">cookie有效斯</param>
            ///		<param name="domain" type="String">cookie域</param>
            ///		<param name="path" type="String">cookie路径</param>
            /// </signature>
            /// <signature>
            ///		<summary>设置cookie</summary>
            ///		<param name="name" type="String">cookie名称</param>
            ///		<param name="subCookies" type="Object">cookie值对象</param>
            ///		<param name="expires" type="Date">cookie有效斯</param>
            ///		<param name="domain" type="String">cookie域</param>
            ///		<param name="path" type="String">cookie路径</param>
            ///		<param name="secure" type="Boolean">true或false</param>
            /// </signature>
            if (!name || arguments.length < 2 || typeof (subCookies) !== "object")
            {
                throw new Error("cookie名称和值对象是必须的");
            }
            var cookieText = encodeURIComponent(name) + "=";
            var subCookieParts = [];
            for (var subName in subCookies)
            {
                if (subName.length > 0 && subCookies.hasOwnProperty(subName))
                {
                    subCookieParts.push(encodeURIComponent(subName) + "=" + encodeURIComponent(subCookies[subName]));
                }
            }
            this.Set(name, cookieText + subCookieParts.join("&"), expires, domain, path, secure);
        }
    };

    Tuhu.Dialog = (function ()
    {
        var dialogList = []; //弹出框列表
        var isDialogShow = false;
        var WindowsList = {}; //窗体列表
        return {
            Popup: function (div, option)
            {
                /// <signature>
                ///		<summary>创建模态窗口</summary>
                ///		<param name="div" type="Element">需要被创建为模态窗口的引用</param>
                ///		<param name="option" type="Object">选项，默认非全屏</param>
                ///		<returns type="Element" />
                /// </signature>
                option = $.extend({
                    fullScreen: true,
                    opacity: 0.1,
                    clone: true,
                    bgColor: "black",
                    zIndex: 9999999,
                    draggable: false,
                    disableScroll: false
                }, option);
                var jqDialog = $(div);
                //jqDialog.find("#noSelectCar").click(function () {
                //    jqDialog = $('<form><label for="myModel">您的车型</label><input type="text" id="myModel" /><br /><label for="myTel"><span class="required">*</span>手机号码</label><input type="text" id="myTel" /></form>');
                //    console.log(jqDialog);
                //});
                var background = undefined;
                if (option.clone)
                    jqDialog = jqDialog.clone(true, true);
                jqDialog.appendTo(document.body).show();
                var dialog = jqDialog[0];
                dialog.close = function (event)
                {
                    $win.off("resize scroll", dialog.adjustPosition);
                    if (background)
                        document.body.removeChild(background);
                    document.body.removeChild(dialog);
                    dialog.adjustPosition = undefined;
                    event && event.preventDefault();
                    jqDialog.triggerHandler("close");
                    jqDialog.off("close");
                    if (option.disableScroll)
                        $doc.off("mousewheel DOMMouseScroll", false);
                };
                document.body.appendChild(dialog);
                dialog.style.zIndex = option.zIndex;
                jqDialog.close = function ()
                {
                    dialog.close();
                };

                var documentElement = document.documentElement;

                if (option.fullScreen)
                {
                    background = document.createElement("div");
                    background.style.position = "fixed";
                    background.style.zIndex = option.zIndex - 1;
                    background.style.backgroundColor = option.bgColor;
                    background.style.filter = 'alpha(opacity=' + option.opacity * 100 + ')';
                    background.style.opacity = option.opacity;
                    document.body.appendChild(background);

                    background.style.top = background.style.left = background.style.bottom = background.style.right = 0;
                }
                dialog.style.position = "fixed";
                dialog.adjustPosition = function ()
                {
                    if (navigator.appName.toLowerCase().indexOf("Microsoft".toLowerCase()) == -1)
                    {
                        dialog.style.top = (documentElement.clientHeight - dialog.clientHeight) / 2 + "px";
                        dialog.style.left = (documentElement.clientWidth - dialog.clientWidth) / 2 + "px";
                    } else
                    {
                        var zoom = document.body.style.zoom;
                        if (zoom.endsWith("%"))
                            zoom = parseFloat(zoom) / 100;
                        else
                            zoom = parseFloat(zoom);

                        if (isNaN(zoom))
                            zoom = 1.0;
                        dialog.style.top = (documentElement.clientHeight / zoom - dialog.clientHeight) / 2 + "px";
                        dialog.style.left = (documentElement.clientWidth / zoom - dialog.clientWidth) / 2 + "px";
                    }
                };

                if (typeof (option.onclose) === "function")
                    jqDialog.on("close", option.onclose);

                if (option.disableScroll)
                    $doc.on("mousewheel DOMMouseScroll", false);

                (option.closeHandle ? $(option.closeHandle, jqDialog) : jqDialog).click(dialog.close);
                $win.on("resize", dialog.adjustPosition);
                dialog.adjustPosition();

                if (option.draggable || option.dragHandle || dialog.getAttribute("draggable") === "true")
                    (option.fullScreen ? jqDialog : jqDialog.add(background)).drag(option.dragHandle);

                return jqDialog;
            },
            PopupDialog: function (panle, option)
            {
                /// <summary>
                /// 将指定的元素显示在外面（含外框）
                /// </summary>
                option = $.extend({
                    closeButton: true,
                    title: "提示",
                    fullScreen: true,
                    opacity: 0.1
                }, option);

                var Popup = this.Popup;
                var dialog = $(panle);

                dialog.on("close", function ()
                {
                    var obj = dialogList.shift();
                    if (obj)
                        Popup(obj.dialog, obj.option);
                    else
                        isDialogShow = false;
                });
                if (isDialogShow)
                    dialogList.push({
                        dialog: dialog,
                        option: option
                    });
                else
                {
                    isDialogShow = true;
                    Popup(dialog, option);
                }
            },
            //console.log: function (message, option)
            //{
            //	option = $.extend({ title: "友情提示", closeButton: true, closor: ".confirm", width: (message || "").byteLength() * 10 + 100, height: 160 }, option);
            //	var panle = $("<div class='message'></div><div class='confirm'><div class='border'><div>确定</div></div></div>");
            //	panle.filter(".message").text(message);
            //	this.PopupDialog(panle, option);
            //},
            //error: function (message, option)
            //{
            //	option = $.extend({ title: "提示", closeButton: false, closor: ".confirm", width: (message || "").byteLength() * 10 + 100, height: 160 }, option);
            //	var panle = $("<div class='message'></div><div class='confirm'><div class='border'><div>确定</div></div></div>");
            //	panle.filter(".message").text(message);
            //	this.PopupDialog(panle, option);
            //},
            //confirm: function (message, callback, option)
            //{
            //	option = $.extend({ title: "提示", closeButton: true, closor: ".confirm, .cancel", width: (message || "").byteLength() * 10 + 100, height: 160 }, option);
            //	var panle = $("<div class='message'></div><div class='buttons'><div class='confirm'><div class='border'><div>确定</div></div></div><div class='cancel'><div>取消</div></div></div>");
            //	panle.filter(".message").text(message);
            //	if (typeof (callback) === "function")
            //	{
            //		panle.find(".confirm").click(function ()
            //		{
            //			callback(true);
            //		});
            //		panle.find(".cancel").click(function ()
            //		{
            //			callback(false);
            //		});
            //	}
            //	this.PopupDialog(panle, option);
            //},
            openCenterWindow: function (url, name, width, height)
            {
                /// <signature>
                ///		<summary>打开居中窗口</summary>
                ///		<param name="url" type="String">Url</param>
                ///		<param name="name" type="String">窗口名称，默认为url的MD5值</param>
                ///		<param name="width" type="Number">窗口宽，默认400px</param>
                ///		<param name="height" type="Number">窗口高，默认300px</param>
                /// </signature>
                name = name || Ebdoor.Security.ComputeMD5(url);
                if (WindowsList[name] && !WindowsList[name].closed)
                {
                    var winHandle = WindowsList[name];
                    winHandle.focus();
                    return;
                }
                width = width || 400;
                height = height || 300;
                var sw = screen.availWidth || screen.width;
                var sh = screen.availHeight || screen.height;
                var l = width < sw ? (sw - width) / 2 : 0;
                var t = height < sh ? (sh - height) / 2 : 0;
                var features = ' width=' + width + ',height=' + height + ',left=' + l + ',top=' + t;
                var winHandle = window.open(url, name, features);
                WindowsList[name] = winHandle;
                winHandle.focus();
            }
        };
    })();

    Tuhu.Helper = (function ()
    {
        return {
            LoadJavascript: function (src)
            {
                var script = document.createElement("script");
                script.src = src;
                document.head.appendChild(script);
            }
        };
    })();

    Tuhu.BaoYang = (function ()
    {
        return {
            BindBaoYangVehicle: function (brandSelect, vehicleSelect, paiLiangSelect, yearSelect, bindStart, bindComplete)
            {
                ///	<signature>
                ///		<summary>绑定保养</summary>
                ///		<param name="brandSelect" type="String">品牌下拉框</param>
                ///		<param name="vehicleSelect" type="String">车型下拉框</param>
                ///	</signature>
                ///	<signature>
                ///		<summary>绑定保养</summary>
                ///		<param name="brandSelect" type="String">品牌下拉框</param>
                ///		<param name="vehicleSelect" type="String">车型下拉框</param>
                ///		<param name="paiLiangSelect" type="String">排量下拉框架</param>
                ///		<param name="yearSelect" type="String">年份下拉框</param>
                ///		<param name="bindComplete" type="Function">绑定完成回调</param>
                ///	</signature>
                ///	<signature>
                ///		<summary>绑定保养</summary>
                ///		<param name="brandSelect" type="String">品牌下拉框</param>
                ///		<param name="vehicleSelect" type="String">车型下拉框</param>
                ///		<param name="paiLiangSelect" type="String">排量下拉框架</param>
                ///		<param name="yearSelect" type="String">年份下拉框</param>
                ///		<param name="bindStart" type="Function">绑定开始回调</param>
                ///		<param name="bindComplete" type="Function">绑定完成回调</param>
                ///	</signature>
                if (!bindComplete)
                {
                    bindComplete = bindStart;
                    bindStart = undefined;
                }

                brandSelect = $(brandSelect);
                vehicleSelect = $(vehicleSelect);

                if (yearSelect)
                {
                    yearSelect = $(yearSelect);
                    //选择年份
                    if (bindComplete)
                        yearSelect.change(function ()
                        {
                            if (this.value)
                                bindComplete();
                        });

                    if (paiLiangSelect)
                    {
                        paiLiangSelect = $(paiLiangSelect);

                        //加载年份
                        paiLiangSelect.change(function ()
                        {
                            yearSelect.style.display = "none";
                            yearSelect.options.length = 0;

                            if (bindStart)
                                bindStart();

                            $.ajax({
                                url: "http://api.tuhu.cn/Car/SelOneCar",
                                jsonpCallback: "__SelOneCar__",
                                dataType: "jsonp",
                                cache: true,
                                data: {
                                    PaiLiang: this.value,
                                    ProductID: vehicleSelect.value.split("|")[0]
                                },
                                success: function (data)
                                {
                                    if (data.Code == "3")
                                    {
                                        bindComplete();
                                    } else
                                    {
                                        yearSelect.options.add(new Option("请选择年份", ""));
                                        $.each(data.Nian, function (index, Nian)
                                        {
                                            changeCar_carNian.options.add(new Option(Nian.Value, Nian.Value));
                                        });
                                        yearSelect.style.display = "block";
                                    }
                                }
                            });
                        });
                    }

                    //加载请选择排量
                    vehicleSelect.change(function ()
                    {
                        yearSelect.style.display = "none";
                        changeCar_carPaiLiang.style.display = "none";
                        paiLiangSelect.options.length = 0;

                        if (bindStart)
                            bindStart();

                        $.ajax({
                            url: "http://api.tuhu.cn/Car/SelOneCar",
                            jsonpCallback: "__SelOneCar__",
                            dataType: "jsonp",
                            cache: true,
                            data: {
                                ProductID: this.value.split("|")[0]
                            },
                            success: function (data)
                            {
                                if (data.Code == "3")
                                {
                                    bindComplete();
                                } else
                                {
                                    paiLiangSelect.options.add(new Option("请选择排量", ""));
                                    $.each(data.PaiLiang, function (index, PaiLiang)
                                    {
                                        paiLiangSelect.options.add(new Option(PaiLiang.Value, PaiLiang.Value));
                                    });
                                    paiLiangSelect.style.display = "block";
                                }
                            }
                        });
                    });

                    paiLiangSelect = paiLiangSelect[0];
                    yearSelect = yearSelect[0];
                }

                //加载车型
                brandSelect.change(function ()
                {
                    vehicleSelect.options.length = 0;
                    vehicleSelect.disabled = true;

                    if (yearSelect)
                    {
                        paiLiangSelect.style.display = "none";
                        yearSelect.style.display = "none";
                    }

                    if (bindStart)
                        bindStart();

                    if (this.value)
                    {
                        vehicleSelect.options.add(new Option("正在读取车型...", ""));
                        $.ajax({
                            url: "http://api.tuhu.cn/Car/SelOneBrand",
                            jsonpCallback: "__SelOneBrand__",
                            dataType: "jsonp",
                            cache: true,
                            data: {
                                Brand: this.value
                            },
                            success: function (data)
                            {
                                vehicleSelect.options.length = 0;
                                vehicleSelect.options.add(new Option("请选择车型", ""));
                                $.each(data.OneBrand, function (index, vehicle)
                                {
                                    //if (vehicle.Tires && vehicle.OriginalIsBaoyang)
                                    var option = new Option(vehicle.Vehicle, vehicle.ProductID);
                                    $.data(option, "vehicle", vehicle);
                                    vehicleSelect.options.add(option);
                                });
                                vehicleSelect.disabled = false;
                            }
                        });
                    } else
                        vehicleSelect.options.add(new Option("请选择车型", ""));
                });

                brandSelect = brandSelect[0];
                vehicleSelect = vehicleSelect[0];

                //加载品牌
                $.ajax({
                    url: "http://api.tuhu.cn/Car/GetCarBrands",
                    jsonpCallback: "__GetCarBrands__",
                    cache: true,
                    dataType: "jsonp",
                    success: function (data)
                    {
                        brandSelect.disabled = false;
                        brandSelect.options.length = 0;
                        brandSelect.options.add(new Option("请选择品牌", ""));
                        $.each(data.Brand, function (index, brand)
                        {
                            brandSelect.options.add(new Option(brand.Brand, brand.Brand));
                        });
                    }
                });
            }
        };
    })();

    Tuhu.Cart = (function ()
    {
        return {
            GetCartQuantity: function (callback)
            {
                ///	<signature>
                ///		<summary>获得购物车数量</summary>
                ///	</signature>
                ///	<signature>
                ///		<summary>获得购物车数量</summary>
                ///		<param name="callback" type="function">回调</param>
                ///	</signature>
                if (typeof (callback) !== "function")
                {
                    if ($(".simpleCart_quantity, #MyCartQuantity").length)
                    {
                        callback = function (quantity)
                        {
                            $(".simpleCart_quantity, #MyCartQuantity").text(quantity);
                        }
                    } else
                    {
                        return;
                    }

                }

                var quantity = Tuhu.Cookie.Get("shoppingCart_quantity");
                if (quantity)
                    callback(quantity);
                else
                    $.ajax({
                        url: Tuhu.Domain.WwwSite + "/Cart/GetCartDetail.aspx",
                        jsonpCallback: "__GetCartInfo__",
                        dataType: "jsonp",
                        success: function ()
                        {
                            Tuhu.Cart.GetCartQuantity(callback);
                        }
                    });
            },
            AddToCart: function (settings)
            {
                ///	<signature>
                ///		<summary>添加到购物车</summary>
                ///		<param name="settings" type="Object">{ productID, quantity, required, success, error, complete }</param>
                ///	</signature>

                if (!settings || (!settings.productID && !settings.data))
                    return;

                $.ajax({
                    url: Tuhu.Domain.WwwSite + "/Cart/AddItems.aspx?method=" + (settings.data ? "addItems" : "addItem"),
                    jsonpCallback: "__AddToCart__",
                    data: settings.data || {
                        pid: settings.productID,
                        quantity: settings.quantity || 1,
                        required: settings.required || false
                    },
                    dataType: "jsonp",
                    error: function ()
                    {
                        !settings.complete || settings.complete(false);
                        !settings.error || settings.error();
                    },
                    success: function (result)
                    {
                        !settings.complete || settings.complete(result);
                        if (result)
                        {
                            Tuhu.Cart.GetCartQuantity();
                            !settings.success || settings.success();
                        } else !settings.error || settings.error();
                    }
                });
            }
        };
    })();

    Tuhu.CarHistory = (function ()
    {
        var CarDialogHtml = '<div class="body"><div style="height: 45px"><div style="float: left;background: url(http://Resource.tuhu.cn/Css/images/icon_modal.png);	width: 20px;	height: 16px;	background-position: 0px -180px;	margin-top: 14px;"></div><div class="SelectCar">选择车型</div><div style="background: url(http://Resource.tuhu.cn/Css/images/icon_modal.png);width: 20px;height: 16px;background-position: 0px -220px;margin-top: 14px;float: right" class="close"></div></div><div style="border: 1px solid #d5d5d5; height: 37px; background-color: #ffffff; "><span class="head_div2" id="cx1"><span class="round" id="cxspan1">1</span>选择品牌</span> <span class="head_div3" id="cx2"><span class="round2" id="cxspan2">2</span>选择车系</span> <span class="head_div5" id="cx3"><span class="round2" id="cxspan3">3</span>选择排量</span> <span class="head_div5" id="cx4"><span class="round2" id="cxspan4">4</span>生产年份</span></div><div style=" padding-bottom: 10px;  margin-top: 6px; " id="div2" class="clearfix"><div style=" padding-top: 16px"><div style="margin-top: -4px;width:710px;height:32px;"><div class="CarZiMu1" style="display: none;">热门</div></div></div><div id="CarBrands"></div></div><div id="Carhistory" style="display:none"><div style=" padding-top: 3px; padding-bottom: 9px"><span style="font-size: 16px; color:#333; font-weight: bold"><span class="history_img"></span>浏览过的车型 :</span></div><div style="max-height: 75px; overflow-y: auto;" class="div" id="Carhistory2"></div></div><div id="div7"><div id="div40" class="clearfix"><div id="div4">已选车型：</div></div><div id="div5"></div></div><div id="div8"><div class="over"></div><div class="succeed">车型选择成功！</div><hr class="hr2" /><div id="CarOver"></div></div><input type="hidden" id="CarName" /></div>';

        var brandGroups = null;

        return {
            AddCarHistory: function (vehicleID, paiLiang, nian, carPlate, callback, liYangID)
            {
                switch (arguments.length)
                {
                    case 2:
                        callback = paiLiang
                        paiLiang = undefined;
                        break;
                    case 3:
                        carPlate = paiLiang;
                        callback = nian;
                        paiLiang = undefined;
                        nian = undefined;
                        break;
                    case 4:
                        callback = carPlate
                        carPlate = undefined;
                        break;
                    default:
                }

                var data = {
                    vehicleID: vehicleID
                };
                if (paiLiang)
                    data["paiLiang"] = paiLiang;
                if (paiLiang)
                    data["nian"] = nian;
                if (paiLiang)
                    data["carPlate"] = carPlate;
                if (liYangID)
                    data["liYangID"] = liYangID;
                return $.ajax({
                    url: Tuhu.Domain.ProductSite + "/CarHistory/AddCarHistory.html",
                    jsonpCallback: "__AddCarHistory__",
                    data: data,
                    dataType: "jsonp"
                }).done(function (result)
                {
                    if (result >= 0)
                        Tuhu.CarHistory.GetCarHistoryQuantity();
                    else
                        console.log("AddCarHistory: " + result);
                    callback(result);
                });
            },
            DeleteCarHistory: function (historyDetailID, callback)
            {
                ///	<signature>
                ///		<summary>获得购物车数量</summary>
                ///		<param name="historyDetailID" type="Number">记录号</param>
                ///		<param name="callback" type="Funciton">回调</param>
                ///	</signature>
                return $.ajax({
                    url: Tuhu.Domain.ProductSite + "/CarHistory/DeleteCarHistory.html",
                    jsonpCallback: "__DeleteCarHistory__",
                    data: {
                        historyDetailID: historyDetailID
                    },
                    dataType: "jsonp"
                }).done(function (result)
                {
                    Tuhu.CarHistory.Refresh && Tuhu.CarHistory.Refresh();
                    if (result >= 0)
                        Tuhu.CarHistory.GetCarHistoryQuantity(function () { });
                    else
                        console.log("AddCarHistory: " + result);
                    callback(result);
                });
            },
            GetCarHistoryQuantity: function (callback)
            {
                ///	<signature>
                ///		<summary>获得购物车数量</summary>
                ///	</signature>
                ///	<signature>
                ///		<summary>获得购物车数量</summary>
                ///		<param name="callback" type="function">回调</param>
                ///	</signature>
                var quantity = Tuhu.Cookie.Get("carHistoryQuantity");
                if (quantity) { }
                    //callback(quantity);
                else
                    $.ajax({
                        url: Tuhu.Domain.ProductSite + "/CarHistory/GetCarHistoryQuantity.html",
                        jsonpCallback: "__GetCarHistoryQuantity__",
                        dataType: "jsonp"
                    }).done(function ()
                    {
                        if (callback)
                            Tuhu.CarHistory.GetCarHistoryQuantity(callback);
                    });
            },
            SelectCarHistory: function (callback)
            {
                ///	<signature>
                ///		<summary>获取所有车型浏览记录</summary>
                ///		<param name="callback" type="Funciton">回调</param>
                ///	</signature>
                return $.ajax({
                    url: Tuhu.Domain.ProductSite + "/CarHistory/SelectCarHistory.html",
                    jsonpCallback: "__SelectCarHistory__",
                    dataType: "jsonp"
                }).done(callback);
            },
            GetDefaultCar: function (callback)
            {
                var defaultCar = (Tuhu.Cookie.Get("defaultCar") || '').split('|'),
					o;
                if (defaultCar.length >= 8)
                {
                    o = {};
                    o.PKID = defaultCar[0];
                    o.VehicleID = defaultCar[1];
                    o.Brand = defaultCar[2];
                    o.Vehicle = defaultCar[3];
                    o.PaiLiang = defaultCar[4];
                    o.Nian = defaultCar[5];
                    o.Tire = defaultCar[6];
                    o.LiYangID = defaultCar[7];
                    if (defaultCar.length >= 10)
                    {
                        o.TripDistance = defaultCar[8];
                        o.OnRoadMonth = defaultCar[9];
                    }
                }
                callback(o);
                return !!o;
            },
            SwitchDefaultCar: function (historyDetailID, callback)
            {
                return $.ajax({
                    url: Tuhu.Domain.ProductSite + "/CarHistory/SwitchDefaultCar.html",
                    jsonpCallback: "__SwitchDefaultCar__",
                    data: {
                        historyDetailID: historyDetailID
                    },
                    dataType: "jsonp"
                }).done(function (result)
                {
                    if (result != 0)
                        console.log("SwitchDefaultCar: " + result);
                    if (typeof (callback) === "function")
                        callback(result);
                });
            },
            OnCarChanged: (function ()
            {
                var OnCarChangedCallback = [];

                var obj = function (vehicle, context)
                {
                    ///	<signature>
                    ///		<summary>获得购物车数量</summary>
                    ///		<param name="vehicle" type="Object">车型</param>
                    ///	</signature>
                    ///	<signature>
                    ///		<summary>获得购物车数量</summary>
                    ///		<param name="vehicle" type="Object">车型</param>
                    ///		<param name="context" type="Object">方法上下文</param>
                    ///	</signature>

                    for (var index = 0; index < OnCarChangedCallback.length; index++)
                    {
                        if (context)
                            OnCarChangedCallback[index].call(context, vehicle);
                        else
                            OnCarChangedCallback[index](vehicle);
                    }
                }
                obj.add = function (callback)
                {
                    if (typeof (callback) === "function")
                        OnCarChangedCallback.push(callback);
                }
                obj.remove = function (callback)
                {
                    if (typeof (callback) === "function")
                        OnCarChangedCallback.remove(callback);
                }

                return obj;
            })(),
            LoadDialog: function (type, callback, CarBrand, CarvehicleName, CarvehicleID, PaiLiang)
            {
                var VehicleID, PL = null,
					N = null,
					LiYangID = null,
					SalesName = '',
					CP_Tire_Width, CP_Tire_AspectRatio, CP_Tire_Rim, Brand, VehicleName,
					_IsNormally = false;
                var dialog = Tuhu.Dialog.Popup(CarDialogHtml, {
                    clone: false,
                    draggable: false,
                    closeHandle: ".close",
                    disableScroll: false,
                    opacity: 0.3,
                    onclose: function ()
                    {
                        if (PL == "null" || PL == "")
                        {
                            PL = null;
                        }
                        if (N == "null" || N == "")
                        {
                            N = null;
                        }

                        var CarHistorys = {
                            VehicleID: VehicleID,
                            PaiLiang: PL,
                            Nian: N,
                            LiYangID: LiYangID,
                            CP_Tire_Width: CP_Tire_Width,
                            CP_Tire_AspectRatio: CP_Tire_AspectRatio,
                            CP_Tire_Rim: CP_Tire_Rim,
                            Brand: Brand,
                            VehicleName: VehicleName
                        };

                        if (_IsNormally == true && CarHistorys.VehicleID)
                        {
                            if (callback === false || callback(CarHistorys) === false)
                                Tuhu.CarHistory.OnCarChanged(CarHistorys);
                        }
                    }
                });

                function loadBrand(brandGroups)
                {
                    var sb = "";
                    $.each(brandGroups, function (key, brands)
                    {
                        if (key != "hot")
                            sb += '<div class="CarZiMu2">' + key + '</div>';
                    });
                    $(".CarZiMu1", dialog).show().after(sb).mouseenter(function ()
                    {
                        $(this).removeClass("CarZiMu1NotSel").addClass("CarZiMu1").siblings().removeClass("CarZiMuSelect").addClass("CarZiMu2");

                        bindBrand(brandGroups["hot"]);
                    }).siblings().mouseenter(function ()
                    {
                        $(".CarZiMuSelect").removeClass("CarZiMuSelect").addClass("CarZiMu2");
                        $(".CarZiMu1").removeClass("CarZiMu1").addClass("CarZiMu1NotSel");

                        bindBrand(brandGroups[$(this).removeClass("CarZiMu2").addClass("CarZiMuSelect").text()]);
                    });

                    bindBrand(brandGroups["hot"]);

                    if (CarBrand && CarvehicleName && CarvehicleID)
                    {
                        VehicleID = CarvehicleID;
                        Brand = CarBrand;
                        VehicleName = CarvehicleName;

                        SelPaiLiang(CarvehicleName, CarvehicleID, true, true, dialog);
                    }
                }

                function bindBrand(brands)
                {
                    var sb = "";
                    $.each(brands, function (index, dom)
                    {
                        if (index >= 16)
                            return;

                        if (index % 5 == 0)
                        {
                            if (dom.Brand.split('-')[1].length > 7)
                            {
                                sb += " <div class='CarBrand' title='" + dom.Brand.split('-')[1] + "' data-Brand='" + dom.Brand + "'><div style='float:left'><img src='http://api.tuhu.cn" + dom.Url + "' ></img></div><div class='Line'></div>" + dom.Brand.split('-')[1].substring(0, 7) + "</div>"
                            } else
                            {
                                sb += " <div class='CarBrand' data-Brand='" + dom.Brand + "'><div style='float:left'><img src='http://api.tuhu.cn" + dom.Url + "' ></img></div><div class='Line'></div>" + dom.Brand.split('-')[1].substring(0, 7) + "</div>"
                            }
                        } else
                        {
                            if (dom.Brand.split('-')[1].length > 7)
                            {
                                sb += " <div class='CarBrand2' title='" + dom.Brand.split('-')[1] + "' data-Brand='" + dom.Brand + "'><div style='float:left'><img src='http://api.tuhu.cn" + dom.Url + "' ></img></div><div class='Line'></div>" + dom.Brand.split('-')[1].substring(0, 7) + "</div>"
                            } else
                            {
                                sb += " <div class='CarBrand2' data-Brand='" + dom.Brand + "'><div style='float:left'><img src='http://api.tuhu.cn" + dom.Url + "' ></img></div><div class='Line'></div>" + dom.Brand.split('-')[1].substring(0, 7) + "</div>"
                            }
                        }
                    });

                    $("#CarBrands", dialog).html(sb);
                }

                var special_Vehi = ""; //品牌
                var special_Brand = ""; //厂商(华晨宝马)
                var special_serise = ""; //系
                function SelVehicle(object, statu, dialog)
                {
                    special_Vehi = object;
                    if (object == 'B - 宝马' || object == 'B - 奔驰')
                    {
                        SelOneVel(object, true, dialog);
                    } else
                    {
                        special_Brand = "";
                        special_Vehi = "";
                        special_serise = "";
                        var obj = object;
                        $.ajax({
                            url: Tuhu.Domain.ProductSite + "/Car/SelOneBrand",
                            jsonpCallback: "__GetCarBrands__",
                            dataType: "jsonp",
                            data: {
                                Brand: obj
                            },
                            success: function (data)
                            {
                                Brand = obj;
                                var sb = "";
                                var ar = new Array();
                                var dt = data;
                                $.each(data.OneBrand, function (index, dom)
                                {
                                    //console.log(dom.Brand);
                                    if (ar.indexOf(dom.BrandType) < 0)
                                    {
                                        ar.push(dom.BrandType);
                                    }
                                })
                                if (statu)
                                {
                                    var sv = "";
                                    if (obj.split('-')[1].length > 5)
                                    {
                                        sv = "<div class='CarHistoryTitlediv'><div class='CarHistoryTitle' title='" + obj.split('-')[1] + "'>" + obj.split('-')[1].substring(0, 5) + "</div><div class='CarHistoryTitleDel' data-index=1></div></div>";
                                    } else
                                    {
                                        sv = "<div class='CarHistoryTitlediv'><div class='CarHistoryTitle' >" + obj.split('-')[1].substring(0, 5) + "</div><div class='CarHistoryTitleDel' data-index=1></div></div>";
                                    }
                                    //$("#div40").append(sv);
                                    $("#div40").html("<div id='div4'>已选车型</div>" + sv);

                                    //$(".CarHistoryTitlediv").hover(function ()
                                    //{
                                    //	$(this).children().css("border-color", "#d12a3e").css("color", "#d12a3e");
                                    //	$(this).children().next().css("background-color", "#d12a3e").css("order-color", "#d12a3e").css("color", "#FFFFFF");

                                    //}, function ()
                                    //{
                                    //	$(this).children().css("border-color", "#c1c1c1").css("color", "#8c8c8c");
                                    //	$(this).children().next().css("background-color", "#c1c1c1").css("color", "#FFFFFF");
                                    //});
                                }
                                $(".CarHistoryTitleDel").click(function ()
                                {
                                    var obj = $(this);
                                    var index = $(this).attr("data-index");

                                    if (index == 1)
                                    {
                                        PL = "";
                                        N = "";
                                        $("#cx2").removeClass("head_div5").removeClass("head_div4").addClass("head_div3")
                                        $("#cx3").removeClass("head_div3").removeClass("head_div4").addClass("head_div5")
                                        $("#cx4").removeClass("head_div3").removeClass("head_div4").addClass("head_div5")
                                        $("#cxspan2").removeClass("round").addClass("round2");
                                        $("#cxspan3").removeClass("round").addClass("round2");
                                        $("#cxspan4").removeClass("round").addClass("round2");
                                        $("#div2").show();
                                        if ($(".Histroy").length > 0)
                                        {
                                            $("#Carhistory").show();
                                        }
                                        for (var i = 0; i < $(".CarHistoryTitlediv").length; i++)
                                        {
                                            $(obj).parent().next().remove();
                                        }
                                        $(obj).parent().remove();
                                        $("#div7").hide();
                                    }

                                });
                                var idx = 0;
                                $.each(ar, function (index, dom)
                                {
                                    idx = 0;
                                    var ss = dom.trim().split('-');
                                    var rr = ss[ss.length - 1];

                                    sb += "<div class='CarBrandTitle'><span class='CarBrandTitlespan'></span>" + rr + ":</div>";
                                    $.each(dt.OneBrand, function (i, d)
                                    {
                                        // console.log(d.BrandType);
                                        if (d.BrandType == dom)
                                        {
                                            var result = "";
                                            var str = d.Vehicle.split('-');
                                            if (str.length > 1)
                                            {
                                                for (var i = 0; i < str.length - 1; i++)
                                                {
                                                    if (i === str.length - 2)
                                                    {
                                                        result += str[i];
                                                    } else
                                                    {
                                                        result += str[i] + "-";
                                                    }

                                                    //console.log(str[i]);
                                                }
                                            } else
                                            {
                                                result = str[0];
                                            }
                                            if (idx % 5 == 0)
                                            {
                                                if (result.length > 10)
                                                {
                                                    sb += "<div class='CarVecl1' title='" + result + "' data-id=" + d.ProductID + " data-Vehicle=" + d.Vehicle.replace(" ", "") + ">" + result.substring(0, 10) + "</div>";
                                                } else
                                                {
                                                    sb += "<div class='CarVecl1' data-id=" + d.ProductID + " data-Vehicle=" + d.Vehicle.replace(" ", "") + ">" + result.substring(0, 10) + "</div>";
                                                }

                                            } else
                                            {
                                                if (result.length > 10)
                                                {
                                                    sb += "<div class='CarVecl2'  title='" + result + "' data-id=" + d.ProductID + " data-Vehicle=" + d.Vehicle.replace(" ", "") + ">" + result.substring(0, 10) + "</div>";
                                                } else
                                                {
                                                    sb += "<div class='CarVecl2'  data-id=" + d.ProductID + " data-Vehicle=" + d.Vehicle.replace(" ", "") + ">" + result.substring(0, 10) + "</div>";
                                                }
                                            }
                                            idx++;
                                        }
                                    })
                                })
                                $("#Carhistory").hide();
                                $("#div2").hide();
                                $("#div5").html(sb);
                                $("#cx2").removeClass("head_div3").addClass("head_div4");
                                $("#cx3").removeClass("head_div5").addClass("head_div3");
                                $("#cxspan2").removeClass("round2").addClass("round");
                                $("#div7").show();

                                $(".CarVecl2,.CarVecl1", dialog).click(function ()
                                {
                                    var Vehicle = $(this).attr("data-Vehicle");
                                    var pid = $(this).attr("data-id");
                                    if (type == "Tires" || type == "Hub")
                                    {
                                        VehicleID = pid;
                                        VehicleName = Vehicle;

                                        if (type == "Tires" || type == "Hub" || type == "BaoYang")
                                        {
                                            $.ajax({
                                                url: Tuhu.Domain.ProductSite + "/Car/GetTires",
                                                jsonpCallback: "__GetCarBrands__",
                                                dataType: "jsonp",
                                                data: {
                                                    ProductID: pid
                                                },
                                                success: function (data)
                                                {
                                                    var d = data;

                                                    //console.log(d.Code);
                                                    if (d.Code == "1")
                                                    {
                                                        var tires = d.Tires.Value.split(';')[0];
                                                        if (tires != "")
                                                        {
                                                            CP_Tire_Width = tires.split('/')[0];
                                                            CP_Tire_AspectRatio = tires.split('/')[1].split('R')[0];
                                                            CP_Tire_Rim = tires.split('/')[1].split('R')[1];
                                                        }
                                                        Tuhu.CarHistory.AddCarHistory(VehicleID, PL, N, function ()
                                                        {
                                                            _IsNormally = true;
                                                            dialog.close();
                                                        });
                                                    } else if (d.Code == "3")
                                                    {

                                                        Tuhu.CarHistory.AddCarHistory(VehicleID, PL, N, function ()
                                                        {
                                                            _IsNormally = true;
                                                            dialog.close();
                                                        });
                                                    }
                                                }
                                            });
                                        }

                                    } else
                                    {

                                        SelPaiLiang(Vehicle, pid, true, false, dialog)
                                    }
                                })
                            }
                        });
                    }
                }

                function SelOneVel(object, statu, dialog)
                {
                    var obj3 = object;
                    $.ajax({
                        url: Tuhu.Domain.ProductSite + "/Car/SelOneVel",
                        jsonpCallback: "__GetVel__",
                        dataType: "jsonp",
                        data: {
                            Vel: obj3
                        },
                        success: function (data3)
                        {
                            Brand = obj3;
                            var sb3 = "";
                            var ar3 = new Array();
                            var dt3 = data3;
                            $.each(data3.OneVel, function (index, dom)
                            {
                                if (ar3.indexOf(dom.BrandType) < 0)
                                {
                                    ar3.push(dom.BrandType);
                                }
                            });

                            if (statu)
                            {

                                var sv = "";
                                if (obj3.split('-')[1].length > 5)
                                {
                                    sv = "<div class='CarHistoryTitlediv'><div class='CarHistoryTitle' title='" + obj3.split('-')[1] + "'>" + obj3.split('-')[1].substring(0, 5) + "</div><div class='CarHistoryTitleDel' data-index=1></div></div>";
                                } else
                                {
                                    sv = "<div class='CarHistoryTitlediv'><div class='CarHistoryTitle' >" + obj3.split('-')[1].substring(0, 5) + "</div><div class='CarHistoryTitleDel' data-index=1></div></div>";
                                }
                                $("#div40").html("<div id='div4'>已选车型</div>" + sv);
                            }

                            $(".CarHistoryTitleDel").click(function ()
                            {
                                var obj = $(this);
                                var index = $(this).attr("data-index");

                                if (index == 1)
                                {
                                    PL = "";
                                    N = "";
                                    $("#cx2").removeClass("head_div5").removeClass("head_div4").addClass("head_div3")
                                    $("#cx3").removeClass("head_div3").removeClass("head_div4").addClass("head_div5")
                                    $("#cx4").removeClass("head_div3").removeClass("head_div4").addClass("head_div5")
                                    $("#cxspan2").removeClass("round").addClass("round2");
                                    $("#cxspan3").removeClass("round").addClass("round2");
                                    $("#cxspan4").removeClass("round").addClass("round2");
                                    $("#div2").show();
                                    if ($(".Histroy").length > 0)
                                    {
                                        $("#Carhistory").show();
                                    }
                                    for (var i = 0; i < $(".CarHistoryTitlediv").length; i++)
                                    {
                                        $(obj).parent().next().remove();
                                    }
                                    $(obj).parent().remove();
                                    $("#div7").hide();
                                }

                            });
                            var idx3 = 0;
                            $.each(ar3, function (index, dom)
                            {
                                idx = 0;
                                var ss = dom.trim().split('-');
                                var rr = ss[ss.length - 1];

                                sb3 += "<div class='CarBrandTitle'><span class='CarBrandTitlespan'></span>" + rr + ":</div>";
                                var ar4 = new Array();
                                $.each(dt3.OneVel, function (i, d)
                                {
                                    if (d.BrandType == dom)
                                    {
                                        if (ar4.indexOf(d.VehicleSeries) < 0)
                                        {
                                            ar4.push(d.VehicleSeries);
                                        }
                                    }
                                })
                                $.each(ar4, function (i, d)
                                {
                                    if (idx3 % 5 == 0)
                                    {

                                        sb3 += "<div class='CarVecl1' data-id=" + rr + " data-Vehicle=" + d + ">" + d + "</div>";

                                    } else
                                    {

                                        sb3 += "<div class='CarVecl2'  data-id=" + rr + " data-Vehicle=" + d + ">" + d + "</div>";

                                    }
                                    idx3++;
                                })
                            })
                            $("#Carhistory").hide();
                            $("#div2").hide();
                            $("#div5").html(sb3);
                            $("#cx2").removeClass("head_div3").addClass("head_div4");
                            $("#cx3").removeClass("head_div5").addClass("head_div3");
                            $("#cxspan2").removeClass("round2").addClass("round");
                            $("#div7").show();

                            $(".CarVecl2,.CarVecl1", dialog).click(function ()
                            {
                                //var serise = $(this).attr("data-Vehicle");
                                var serise = $(this).text();
                                special_serise = serise;
                                var brand = $(this).attr("data-id");
                                special_Brand = brand;
                                $.ajax({
                                    url: Tuhu.Domain.ProductSite + "/Car/SelOneBrand3",
                                    jsonpCallback: "__GetBrand3__",
                                    dataType: "jsonp",
                                    data: {
                                        serise: serise,
                                        brand: brand
                                    },
                                    success: function (data)
                                    {
                                        var sb = "";
                                        var ar = new Array();
                                        var dt = data;
                                        $.each(data.OneBrand, function (index, dom)
                                        {
                                            if (ar.indexOf(dom.BrandType) < 0)
                                            {
                                                ar.push(dom.BrandType);
                                            }
                                        });

                                        if (statu)
                                        {
                                            var sv = "";

                                            sv = "<div class='CarHistoryTitlediv'><div class='CarHistoryTitle' >" + serise + "</div><div class='CarHistoryTitleDel' data-index=12></div></div>";
                                            $("#div40").append(sv);

                                        }
                                        $(".CarHistoryTitleDel").click(function ()
                                        {
                                            var obj = $(this);
                                            var index = $(this).attr("data-index");

                                            if (index == 12)
                                            {
                                                PL = "";
                                                N = "";
                                                $("#cx2").removeClass("head_div5").removeClass("head_div4").addClass("head_div3")
                                                $("#cx3").removeClass("head_div3").removeClass("head_div4").addClass("head_div5")
                                                $("#cx4").removeClass("head_div3").removeClass("head_div4").addClass("head_div5")
                                                $("#cxspan2").removeClass("round").addClass("round2");
                                                $("#cxspan3").removeClass("round").addClass("round2");
                                                $("#cxspan4").removeClass("round").addClass("round2");
                                                SelVehicle(Brand, true, dialog);
                                                $(obj).parent().remove();
                                            }

                                        });
                                        var idx = 0;
                                        $.each(ar, function (index, dom)
                                        {
                                            idx = 0;
                                            var ss = dom.trim().split('-');
                                            var rr = ss[ss.length - 1];

                                            sb += "<div class='CarBrandTitle'><span class='CarBrandTitlespan'></span>" + rr + ":</div>";
                                            $.each(dt.OneBrand, function (i, d)
                                            {
                                                // console.log(d.BrandType);
                                                if (d.BrandType == dom)
                                                {
                                                    var result = "";
                                                    var str = d.Vehicle.split('-');
                                                    if (str.length > 1)
                                                    {
                                                        for (var i = 0; i < str.length - 1; i++)
                                                        {
                                                            if (i === str.length - 2)
                                                            {
                                                                result += str[i];
                                                            } else
                                                            {
                                                                result += str[i] + "-";
                                                            }

                                                            //console.log(str[i]);
                                                        }
                                                    } else
                                                    {
                                                        result = str[0];
                                                    }
                                                    if (idx % 5 == 0)
                                                    {
                                                        if (result.length > 10)
                                                        {
                                                            sb += "<div class='CarVecl1' title='" + result + "' data-id=" + d.ProductID + " data-Vehicle=" + d.Vehicle.replace(" ", "") + ">" + result.substring(0, 10) + "</div>";
                                                        } else
                                                        {
                                                            sb += "<div class='CarVecl1' data-id=" + d.ProductID + " data-Vehicle=" + d.Vehicle.replace(" ", "") + ">" + result.substring(0, 10) + "</div>";
                                                        }

                                                    } else
                                                    {
                                                        if (result.length > 10)
                                                        {
                                                            sb += "<div class='CarVecl2'  title='" + result + "' data-id=" + d.ProductID + " data-Vehicle=" + d.Vehicle.replace(" ", "") + ">" + result.substring(0, 10) + "</div>";
                                                        } else
                                                        {
                                                            sb += "<div class='CarVecl2'  data-id=" + d.ProductID + " data-Vehicle=" + d.Vehicle.replace(" ", "") + ">" + result.substring(0, 10) + "</div>";
                                                        }
                                                    }
                                                    idx++;
                                                }
                                            })
                                        })
                                        $("#Carhistory").hide();
                                        $("#div2").hide();
                                        $("#div5").html(sb);
                                        $("#cx2").removeClass("head_div3").addClass("head_div4");
                                        $("#cx3").removeClass("head_div5").addClass("head_div3");
                                        $("#cxspan2").removeClass("round2").addClass("round");
                                        $("#div7").show();

                                        $(".CarVecl2,.CarVecl1", dialog).click(function ()
                                        {
                                            var Vehicle = $(this).attr("data-Vehicle");
                                            var pid = $(this).attr("data-id");
                                            //console.log(type);
                                            if (type == "Tires" || type == "Hub")
                                            {
                                                VehicleID = pid;
                                                VehicleName = Vehicle;

                                                if (type == "Tires" || type == "Hub" || type == "BaoYang")
                                                {
                                                    $.ajax({
                                                        url: Tuhu.Domain.ProductSite + "/Car/GetTires",
                                                        jsonpCallback: "__GetCarBrands__",
                                                        dataType: "jsonp",
                                                        data: {
                                                            ProductID: pid
                                                        },
                                                        success: function (data)
                                                        {
                                                            var d = data;
                                                            //console.log(d.Code);
                                                            if (d.Code == "1")
                                                            {
                                                                var tires = d.Tires.Value.split(';')[0];
                                                                if (tires != "")
                                                                {
                                                                    CP_Tire_Width = tires.split('/')[0];
                                                                    CP_Tire_AspectRatio = tires.split('/')[1].split('R')[0];
                                                                    CP_Tire_Rim = tires.split('/')[1].split('R')[1];
                                                                }
                                                                Tuhu.CarHistory.AddCarHistory(VehicleID, PL, N, function ()
                                                                {
                                                                    _IsNormally = true;
                                                                    dialog.close();
                                                                });
                                                            } else if (d.Code == "3")
                                                            {

                                                                Tuhu.CarHistory.AddCarHistory(VehicleID, PL, N, function ()
                                                                {
                                                                    _IsNormally = true;
                                                                    dialog.close();
                                                                });
                                                            }
                                                        }
                                                    });
                                                }

                                            } else
                                            {

                                                SelPaiLiang(Vehicle, pid, true, false, dialog)
                                            }
                                        })
                                    }
                                });

                            })
                        }
                    });
                }

                function SelPaiLiang(Vehicle, pid, statu, Mode, dialog)
                {
                    //console.log(1);
                    if (Mode)
                    {
                        if (statu)
                        {
                            var sv2 = "";
                            var sv = "";
                            if (Brand.split('-')[1].length > 5)
                            {
                                sv = "<div class='CarHistoryTitlediv'><div class='CarHistoryTitle' title='" + Brand.split('-')[1] + "'>" + Brand.split('-')[1].substring(0, 5) + "</div><div class='CarHistoryTitleDel' data-index=1></div></div>";
                            } else
                            {
                                sv = "<div class='CarHistoryTitlediv'><div class='CarHistoryTitle' >" + Brand.split('-')[1].substring(0, 5) + "</div><div class='CarHistoryTitleDel' data-index=1></div></div>";
                            }

                            if (Brand == 'B - 宝马' || Brand == 'B - 奔驰')
                            {
                                special_Brand = Vehicle.split('-')[1];
                                special_Vehi = Brand;
                                $.ajax({
                                    url: Tuhu.Domain.ProductSite + "/Car/SelSeriseByVehicleBrand",
                                    jsonpCallback: "__Serise__",
                                    dataType: "jsonp",
                                    data: {
                                        productId: pid
                                    },
                                    success: function (data2)
                                    {
                                        //sv2=data2.Serise;
                                        sv2 = "<div class='CarHistoryTitlediv'><div class='CarHistoryTitle' title='" + data2.Serise + "'>" + data2.Serise + "</div><div class='CarHistoryTitleDel' data-index=12></div></div>";
                                        $("#div40").append(sv2);
                                        special_serise = data2.Serise;
                                        $(".CarHistoryTitleDel").click(function ()
                                        {
                                            var obj = $(this);
                                            var index = $(this).attr("data-index");

                                            if (index == 12)
                                            {
                                                PL = "";
                                                N = "";
                                                $("#cx2").removeClass("head_div5").removeClass("head_div4").addClass("head_div3")
                                                $("#cx3").removeClass("head_div3").removeClass("head_div4").addClass("head_div5")
                                                $("#cx4").removeClass("head_div3").removeClass("head_div4").addClass("head_div5")
                                                $("#cxspan2").removeClass("round").addClass("round2");
                                                $("#cxspan3").removeClass("round").addClass("round2");
                                                $("#cxspan4").removeClass("round").addClass("round2");
                                                SelVehicle(Brand, true, dialog);
                                                $(obj).parent().remove();
                                            }

                                        });
                                    },
                                    error: function (XMLHttpRequest, textStatus, errorThrown)
                                    {
                                        console.log(XMLHttpRequest.status);
                                        console.log(XMLHttpRequest.readyState);
                                        console.log(textStatus);
                                    }
                                });
                            }
                            $("#div40").append(sv);
                            $(".CarHistoryTitleDel", dialog).click(function ()
                            {
                                var obj = $(this);
                                var index = $(this).attr("data-index");
                                if (index == 1)
                                {
                                    if (index == 1)
                                    {
                                        PL = "";
                                        N = "";
                                        $("#cx2").removeClass("head_div5").removeClass("head_div4").addClass("head_div3")
                                        $("#cx3").removeClass("head_div3").removeClass("head_div4").addClass("head_div5")
                                        $("#cx4").removeClass("head_div3").removeClass("head_div4").addClass("head_div5")
                                        $("#cxspan2").removeClass("round").addClass("round2");
                                        $("#cxspan3").removeClass("round").addClass("round2");
                                        $("#cxspan4").removeClass("round").addClass("round2");
                                        $("#div2").show();
                                        if ($(".Histroy").length > 0)
                                        {
                                            $("#Carhistory").show();
                                        }
                                        for (var i = 0; i < $(".CarHistoryTitlediv").length; i++)
                                        {
                                            $(obj).parent().next().remove();
                                        }
                                        $(obj).parent().remove();
                                        $("#div7").hide();
                                    }
                                }
                            });



                        }
                    }

                    if (type == "Tires" || type == "Hub" || type == "BaoYang")
                    {
                        $.ajax({
                            url: Tuhu.Domain.ProductSite + "/Car/GetTires",
                            jsonpCallback: "__GetCarBrands__",
                            dataType: "jsonp",
                            data: {
                                ProductID: pid
                            },
                            success: function (data)
                            {
                                var d = data;
                                if (d.Code == "1")
                                {
                                    var tires = d.Tires.Value.split(';')[0];
                                    if (tires != "")
                                    {
                                        CP_Tire_Width = tires.split('/')[0];
                                        CP_Tire_AspectRatio = tires.split('/')[1].split('R')[0];
                                        CP_Tire_Rim = tires.split('/')[1].split('R')[1];
                                    }
                                }

                                $.ajax({
                                    url: Tuhu.Domain.ProductSite + "/Car/SelOneCar",
                                    jsonpCallback: "__GetCarBrands__",
                                    dataType: "jsonp",
                                    data: {
                                        ProductID: pid
                                    },
                                    success: function (data)
                                    {
                                        VehicleID = pid;
                                        VehicleName = Vehicle;
                                        Pai(data, Vehicle, dialog, statu);
                                    }
                                });
                            }
                        });
                    } else
                    {
                        $.ajax({
                            url: Tuhu.Domain.ProductSite + "/Car/SelOneCar",
                            jsonpCallback: "__GetCarBrands__",
                            dataType: "jsonp",
                            data: {
                                ProductID: pid
                            },
                            success: function (data)
                            {
                                VehicleID = pid;
                                VehicleName = Vehicle;
                                Pai(data, Vehicle, dialog, statu);
                            }
                        });
                    }
                    if (Mode)
                    {
                        $("#div7").show();
                        $("#div2").hide();
                        $("#Carhistory").hide();
                        $("#cx2").removeClass("head_div2").addClass("head_div4");
                        $("#cxspan2").removeClass("round2").addClass("round");
                    }
                }



                function Pai(data, Vehicle, dialog, statu)
                {
                    if (data.Code == "1")
                    {
                        if (statu)
                        {
                            var sv = "";
                            var result = "";
                            var str = Vehicle.split('-');
                            if (str.length > 1)
                            {
                                for (var i = 0; i < str.length - 1; i++)
                                {
                                    if (i === str.length - 2)
                                    {
                                        result += str[i];
                                    } else
                                    {
                                        result += str[i] + "-"
                                    }
                                }
                            } else
                            {
                                result = str[0];
                            }
                            if (result.length > 5)
                            {
                                sv = "<div class='CarHistoryTitlediv'><div class='CarHistoryTitle' title='" + result + "'>" + result.substring(0, 5) + "</div><div class='CarHistoryTitleDel' data-index=2></div><div>";
                            } else
                            {
                                sv = "<div class='CarHistoryTitlediv'><div class='CarHistoryTitle'>" + result.substring(0, 5) + "</div><div class='CarHistoryTitleDel' data-index=2></div><div>";
                            }

                            $("#div40").append(sv);
                        }
                        $(".CarHistoryTitleDel", dialog).click(function ()
                        {
                            var obj = $(this);
                            var index = $(this).attr("data-index");

                            if (index == 2)
                            {
                                PL = "";
                                N = "";
                                $("#cx3").removeClass("head_div5").removeClass("head_div4").addClass("head_div3");
                                $("#cx4").removeClass("head_div3").removeClass("head_div4").addClass("head_div5");
                                $("#cxspan3").removeClass("round").addClass("round2");
                                $("#cxspan4").removeClass("round").addClass("round2");
                                //alert(special_Brand);
                                //console.log(special_Vehi + "special_Vehi");
                                if (special_Vehi == 'B - 宝马' || special_Vehi == 'B - 奔驰')
                                {
                                    var serise = special_serise
                                    special_serise = serise;
                                    var brand = special_Brand;
                                    $.ajax({
                                        url: Tuhu.Domain.ProductSite + "/Car/SelOneBrand3",
                                        jsonpCallback: "__GetCarBrands__",
                                        dataType: "jsonp",
                                        data: {
                                            serise: serise,
                                            brand: brand
                                        },
                                        success: function (data)
                                        {
                                            var sb = "";
                                            var ar = new Array();
                                            var dt = data;
                                            $.each(data.OneBrand, function (index, dom)
                                            {
                                                if (ar.indexOf(dom.BrandType) < 0)
                                                {
                                                    ar.push(dom.BrandType);
                                                }
                                            });
                                            var idx = 0;
                                            $.each(ar, function (index, dom)
                                            {
                                                idx = 0;
                                                var ss = dom.trim().split('-');
                                                var rr = ss[ss.length - 1];

                                                sb += "<div class='CarBrandTitle'><span class='CarBrandTitlespan'></span>" + rr + ":</div>";
                                                $.each(dt.OneBrand, function (i, d)
                                                {
                                                    // console.log(d.BrandType);
                                                    if (d.BrandType == dom)
                                                    {
                                                        var result = "";
                                                        var str = d.Vehicle.split('-');
                                                        if (str.length > 1)
                                                        {
                                                            for (var i = 0; i < str.length - 1; i++)
                                                            {
                                                                if (i === str.length - 2)
                                                                {
                                                                    result += str[i];
                                                                } else
                                                                {
                                                                    result += str[i] + "-";
                                                                }
                                                            }
                                                        } else
                                                        {
                                                            result = str[0];
                                                        }
                                                        if (idx % 5 == 0)
                                                        {
                                                            if (result.length > 10)
                                                            {
                                                                sb += "<div class='CarVecl1' title='" + result + "' data-id=" + d.ProductID + " data-Vehicle=" + d.Vehicle.replace(" ", "") + ">" + result.substring(0, 10) + "</div>";
                                                            } else
                                                            {
                                                                sb += "<div class='CarVecl1' data-id=" + d.ProductID + " data-Vehicle=" + d.Vehicle.replace(" ", "") + ">" + result.substring(0, 10) + "</div>";
                                                            }

                                                        } else
                                                        {
                                                            if (result.length > 10)
                                                            {
                                                                sb += "<div class='CarVecl2'  title='" + result + "' data-id=" + d.ProductID + " data-Vehicle=" + d.Vehicle.replace(" ", "") + ">" + result.substring(0, 10) + "</div>";
                                                            } else
                                                            {
                                                                sb += "<div class='CarVecl2'  data-id=" + d.ProductID + " data-Vehicle=" + d.Vehicle.replace(" ", "") + ">" + result.substring(0, 10) + "</div>";
                                                            }
                                                        }
                                                        idx++;
                                                    }
                                                })
                                            })
                                            $("#Carhistory").hide();
                                            $("#div2").hide();
                                            $("#div5").html(sb);
                                            $("#cx2").removeClass("head_div3").addClass("head_div4");
                                            $("#cx3").removeClass("head_div5").addClass("head_div3");
                                            $("#cxspan2").removeClass("round2").addClass("round");
                                            $("#div7").show();

                                            $(".CarVecl2,.CarVecl1", dialog).click(function ()
                                            {
                                                var Vehicle = $(this).attr("data-Vehicle");
                                                var pid = $(this).attr("data-id");
                                                //

                                                //



                                                if (type == "Tires" || type == "Hub")
                                                {
                                                    VehicleID = pid;
                                                    VehicleName = Vehicle;

                                                    //if (type == "Tires" || type == "Hub") {
                                                    //    $.ajax({
                                                    //        url: Tuhu.Domain.ProductSite + "/Car/GetTires",
                                                    //        jsonpCallback: "__GetCarBrands__",
                                                    //        dataType: "jsonp",
                                                    //        data: {
                                                    //            ProductID: pid
                                                    //        },
                                                    //        success: function (data) {
                                                    //            var d = data;
                                                    //            //console.log(d.Code);
                                                    //            if (d.Code == "1") {
                                                    //                var tires = d.Tires.Value.split(';')[0];
                                                    //                if (tires != "") {
                                                    //                    CP_Tire_Width = tires.split('/')[0];
                                                    //                    CP_Tire_AspectRatio = tires.split('/')[1].split('R')[0];
                                                    //                    CP_Tire_Rim = tires.split('/')[1].split('R')[1];
                                                    //                }
                                                    //                Tuhu.CarHistory.AddCarHistory(VehicleID, PL, N, function () {
                                                    //                    _IsNormally = true;
                                                    //                    dialog.close();
                                                    //                });
                                                    //            }
                                                    //            else if (d.Code == "3") {

                                                    //                Tuhu.CarHistory.AddCarHistory(VehicleID, PL, N, function () {
                                                    //                    _IsNormally = true;
                                                    //                    dialog.close();
                                                    //                });
                                                    //            }
                                                    //        }
                                                    //    });
                                                    //}

                                                } else
                                                {

                                                    SelPaiLiang(Vehicle, pid, true, false, dialog)
                                                }
                                            })
                                        }
                                    });
                                } else
                                {
                                    SelVehicle(Brand, false, dialog);
                                }
                                for (var i = 0; i < $(".CarHistoryTitlediv").length; i++)
                                {
                                    $(obj).parent().next().remove();
                                }
                                $(obj).parent().remove();
                            }

                        });
                        if ($("#CarName").val().indexOf(Vehicle) < 0)
                        {
                            $("#CarName").val($("#CarName").val() + " " + Vehicle);
                        }
                        var sb = "";
                        $.each(data.PaiLiang, function (index, dom)
                        {
                            if (index % 5 == 0)
                            {
                                sb += "<div class='CarPaiLiang1' data-id=" + dom.Key + " data-PaiLiang=" + dom.Value + ">" + dom.Value + "</div>";
                            } else
                            {
                                sb += "<div class='CarPaiLiang2' data-id=" + dom.Key + " data-PaiLiang=" + dom.Value + ">" + dom.Value + "</div>";
                            }


                        })
                        $("#cx3").removeClass("head_div3").removeClass("head_div5").addClass("head_div4");
                        $("#cx4").removeClass("head_div5").addClass("head_div3");
                        $("#cxspan3").removeClass("round2").addClass("round");

                        if (type == "Tires")
                        {
                            sb += "<div class='Tires' >查看适配该车系的轮胎 </div>";
                        }
                        if (type == "Hub")
                        {
                            sb += "<div class='Tires' >查看适配该车系的轮毂 </div>";
                        }
                        $("#div5").html(sb);

                        if (type == "Tires" || type == "Hub")
                        {
                            $(".Tires").click(function ()
                            {
                                Tuhu.CarHistory.AddCarHistory(VehicleID, PL, N, function ()
                                {
                                    _IsNormally = true;
                                    dialog.close();
                                });
                            })
                        }

                        $(".CarPaiLiang1,.CarPaiLiang2", dialog).click(function ()
                        {
                            var PaiLiang2 = $(this).attr("data-PaiLiang");
                            $.ajax({
                                url: Tuhu.Domain.ProductSite + "/Car/SelOneCarNew",
                                jsonpCallback: "__GetCarBrands__",
                                dataType: "jsonp",
                                data: {
                                    ProductID: $(this).attr("data-id"),
                                    PaiLiang: $(this).attr("data-PaiLiang")
                                },
                                success: function (data)
                                {
                                    NianFen(data, PaiLiang2, dialog, true);
                                }
                            });
                        })

                    } else if (type == "Tires" || type == "Hub" || type == "other" || type == "BaoYang")
                    {
                        $("#CarOver").html(Brand + " " + VehicleName);
                        $("#div7").hide();
                        $("#div8").show();
                        var flag = false;
                        setTimeout(function ()
                        {
                            if (flag)
                            {
                                _IsNormally = true;
                                dialog.close();
                            } else
                                flag = true;
                        }, 2000);
                        Tuhu.CarHistory.AddCarHistory(VehicleID, PL, N, function ()
                        {
                            if (flag)
                            {
                                _IsNormally = true;
                                dialog.close();
                            } else
                                flag = true;
                        });
                    }

                }

                function NianFen(data, PaiLiang2, dialog, statu)
                {
                    $("#cx4").removeClass("head_div3").addClass("head_div4");
                    $("#cxspan4").removeClass("round2").addClass("round");

                    if (data.Code == "1")
                    {
                        PL = PaiLiang2;
                        if (statu)
                        {
                            var sv = "";
                            sv = "<div class='CarHistoryTitlediv'><div class='CarHistoryTitle'>" + PaiLiang2 + "</div><div class='CarHistoryTitleDel' data-index=3></div></div>";
                            $("#div40").append(sv);

                            //$(".CarHistoryTitlediv").hover(function ()
                            //{
                            //	$(this).children().css("border-color", "#d12a3e").css("color", "#d12a3e");
                            //	$(this).children().next().css("background-color", "#d12a3e").css("order-color", "#d12a3e").css("color", "#FFFFFF");

                            //}, function ()
                            //{
                            //	$(this).children().css("border-color", "#c1c1c1").css("color", "#8c8c8c");
                            //	$(this).children().next().css("background-color", "#c1c1c1").css("color", "#FFFFFF");
                            //});
                        }
                        $(".CarHistoryTitleDel", dialog).click(function ()
                        {
                            var obj = $(this);
                            var index = $(this).attr("data-index");
                            //console.log(special_Brand);//object == 'B - 宝马' || object == 'B - 奔驰'
                            if (index == 3 && (special_Vehi == 'B - 宝马' || special_Vehi == 'B - 奔驰'))
                            {
                                PL = "";
                                N = "";
                                //$("#cx3").removeClass("head_div5").removeClass("head_div4").addClass("head_div3");
                                $("#cx4").removeClass("head_div3").removeClass("head_div4").addClass("head_div5");
                                $("#cxspan4").removeClass("round").addClass("round2");
                                //SelPaiLiang(VehicleName, VehicleID, true, true, dialog);
                                $(obj).parent().remove();
                            }
                            if ($(".CarHistoryTitleDel").length == index)
                            {
                                if (index == 3)
                                {

                                    PL = "";
                                    N = "";
                                    //$("#cx3").removeClass("head_div5").removeClass("head_div4").addClass("head_div3");
                                    $("#cx4").removeClass("head_div3").removeClass("head_div4").addClass("head_div5");
                                    $("#cxspan4").removeClass("round").addClass("round2");
                                    SelPaiLiang(VehicleName, VehicleID, false, false, dialog);
                                    $(obj).parent().remove();
                                }
                            }
                        });
                        if ($("#CarName").val().indexOf(PaiLiang2) < 0)
                        {
                            $("#CarName").val($("#CarName").val() + " " + PaiLiang2);
                        }
                        var sb = "<p id='CarNianTips'><img src='//resource.tuhu.cn/image/www/light.png'/><span class='wenxin_tit'>温馨提示：</span><span class='wenxin_tips'>“生产年份”与“年款”、“上路年份”未必相同（尤其是1、2月份购买时），选错生产年份往往会导致配件出错无法安装，<a href='" + Tuhu.Domain.WwwSite + "/mingpai.aspx' target='_blank' style='color:#2c59b8'>如何知道生产年份</a></span></p>";
                        $.each(data.Nian, function (index, dom)
                        {
                            if (index % 5 == 0)
                            {
                                sb += "<div class='CarNian1' data-id=" + dom.Key + "  data-Nian=" + dom.Value + ">" + dom.Value + "年生产</div>";
                            } else
                            {
                                sb += "<div class='CarNian2' data-id=" + dom.Key + "  data-Nian=" + dom.Value + ">" + dom.Value + "年生产</div>";
                            }
                        })

                        if (type == "Tires")
                        {
                            sb += "<div class='Tires' >查看适配该车系的轮胎 </div>";
                        }
                        if (type == "Hub")
                        {
                            sb += "<div class='Tires' >查看适配该车系的轮毂 </div>";
                        }

                        $("#div5").html(sb);

                        if (type == "Tires" || type == "Hub")
                        {
                            $(".Tires").click(function ()
                            {
                                Tuhu.CarHistory.AddCarHistory(VehicleID, PL, N, function ()
                                {
                                    _IsNormally = true;
                                    dialog.close();
                                });
                            })
                        }

                        $(".CarNian1,.CarNian2", dialog).click(function ()
                        {
                            if ($("#CarName").val().indexOf($(this).attr("data-Nian")) < 0)
                            {
                                $("#CarName").val($("#CarName").val() + " " + $(this).attr("data-Nian"));;
                            }
                            N = $(this).attr("data-Nian");
                            //over(dialog);
                            //获取力洋数据
                            //获取五级车型数据
                            //强制选择第五级
                            if (CarBrand === true)
                            {
                                $.ajax({
                                    url: Tuhu.Domain.ProductSite + "/Car/SelSalesName",
                                    jsonpCallback: "__GetCarBrands__",
                                    dataType: "jsonp",
                                    data: {
                                        ProductID: VehicleID,
                                        PaiLiang: PL,
                                        Nian: N
                                    },
                                    success: function (data)
                                    {
                                        LiYang(data, N, dialog, true);
                                    }
                                });
                            } else
                            {
                                $.ajax({
                                    url: Tuhu.Domain.ProductSite + "/Car/SelOneCarNew",
                                    jsonpCallback: "__GetCarBrands__",
                                    dataType: "jsonp",
                                    data: {
                                        ProductID: VehicleID,
                                        PaiLiang: PL,
                                        Nian: N
                                    },
                                    success: function (data)
                                    {
                                        LiYang(data, N, dialog, true);
                                    }
                                });
                            }
                        })
                    }
                }

                function LiYang(data, Nian, dialog, statu)
                {
                    var sv = "<div class='CarHistoryTitlediv'><div class='CarHistoryTitle'>" + N + "</div><div class='CarHistoryTitleDel' data-index=4></div><div>";
                    $("#div40").append(sv);
                    if (data.Code == "1" && data.SalesName.length !== 0)
                    {

                        //隐藏headtitle
                        $('#cx1').parent().empty().attr('style', '').addClass('moreCarInfoTip')
                        	.text('该生产年份存在多款车型，您的是？');
                        $('#div7').css({
                            //border: '1px solid #e3e3e3'
                        });
                        $('#div40').empty()
							.append('<span class="hs1">已选车辆信息：</span><span class="hs2">' + [Brand, VehicleName, PL, N].join(' ') + '</span>');
                        // var CarHistoryTitles = $(".CarHistoryTitlediv");
                        // CarHistoryTitles.find('.CarHistoryTitleDel').remove();
                        //显示数据
                        var _salesName = {};
                        data.SalesName = data.SalesName.filter(function (v)
                        {
                            if (v.name in _salesName)
                                return false;
                            _salesName[v.name] = !0;
                            return true
                        });
                        $("#div5").empty().append(data.SalesName.map(function (v)
                        {
                            return ['<div class="CarLiYang" data-ly="', v.id, '">', v.name, '</div>'].join('');
                        }).join(''));
                        $("#div5").on('click', '.CarLiYang', function ()
                        {
                            var _this = $(this);
                            LiYangID = _this.attr('data-ly');
                            SalesName = _this.text();
                            over(dialog);
                        });
                    } else
                    {
                        over(dialog);
                    }
                }

                function over(dialog)
                {
                    Tuhu.CarHistory.AddCarHistory(VehicleID, PL, N, null, function ()
                    {
                        $('.body .moreCarInfoTip').hide();
                        $("#CarOver").html("已选车型：" + [Brand, VehicleName, PL, N, SalesName].join(' '));
                        $("#div7").hide();
                        $("#div8").show();
                        _IsNormally = true;
                        setTimeout(function ()
                        {
                            dialog.close();
                        }, 2000);
                    }, LiYangID);
                }

                if (brandGroups)
                    loadBrand(brandGroups);
                else
                    $.ajax({
                        url: Tuhu.Domain.ProductSite + "/Car/GetCarBrands2",
                        jsonpCallback: "__GetCarBrands__",
                        dataType: "jsonp",
                        success: function (data)
                        {
                            brandGroups = data;
                            loadBrand(data);
                        }
                    });

                Tuhu.CarHistory.SelectCarHistory(function (data)
                {
                    var sb = ""
                    $.each(data, function (index, dom)
                    {
                        sb += "<div class='HistroyHead'><span class='Histroy' data-liyangID='" + dom.LiYangID + "' data-id='" + dom.PKID + "' data-brand='" + dom.Brand + "' data-vehiclename='" + dom.VehicleName + "' data-pailiang='" + (dom.PaiLiang != null ? dom.PaiLiang : "") + "' data-nian='" + (dom.Nian != null ? dom.Nian : "") + "' data-vehicleid='" + dom.VehicleID + "'>" + (dom.Brand != null ? dom.Brand : " ") + "" + (dom.VehicleName != null ? dom.VehicleName : " ") + " " + (dom.PaiLiang != null ? dom.PaiLiang : " ") + " " + (dom.Nian != null ? dom.Nian : " ") + " </span>";
                        if (dom.IsDefaultCar)
                        {
                            sb += "<span></span></div>"
                        } else
                        {
                            sb += "<span class='Delte' data-id='" + dom.PKID + "'>X</span></div>"
                        };
                    });
                    $("#Carhistory2").html(sb);
                    $("#noSelectCar").on("click", function ()
                    {
                        var byurl;
                        if (/\.tuhu\.(\w+)$/.test(location.host))
                            byurl = location.protocol + "//by" + ".tuhu." + RegExp.$1;
                        $(".body").children().not($(".body").children().eq(0)).hide();
                        $(".SelectCar").html("没有我的车型");
                        var sbs = '<div class="noMyModel"><div class="userinfos"><label for="myModel">您的车型：</label><input type="text" id="myModel" /><br><label for="myTel"><span class="required">*</span>手机号码：</label><input type="text" id="myTel" /><span class="errors"></span></div><button class="subBtnModel" id="subBtnModel">提交</button></div>';
                        $(".body").append(sbs);
                        $(".body:first-child").show();
                        $("#subBtnModel").on("click", function ()
                        {
                            var patrn = new RegExp("^((13[0-9])|15([0-3]|[5-9])|(17[6-8])|(18[0-9]))([0-9]{8}$)");
                            if (patrn.test($("#myTel").val()) == false)
                            {
                                $(".errors").text("请输入正确的手机号");
                                return;
                            }
                            $(".errors").text("");
                            $.ajax({
                                url: byurl + "/BaoYang/MyModels.html",
                                dataType: "jsonp",
                                jsonpCallback: "callback",
                                data: {
                                    myModel: $("#myModel").val(),
                                    myTel: $("#myTel").val()
                                },
                                success: function (json)
                                {
                                    if (json)
                                    {
                                        $(".body").next("div").remove();
                                        $("div").remove(".body");
                                        Tuhu.Promote.ShowPromoteDialog(Tuhu.Domain.ResourceSite + "/image/www/ok.png", "提交成功！", 120);
                                        Tuhu.Promote.ClosePromoteDialog(function () { }, {
                                            obj: 2
                                        });
                                    }
                                }
                            });

                        });
                    });
                    if (sb)
                    {
                        if (CarBrand && CarvehicleName && CarvehicleID) { } else
                            $("#Carhistory").show();
                        $(".HistroyHead").hover(function ()
                        {
                            $(this).children().next().show();
                            //console.log($(this).children().next().show());
                            $(this).children().css("color", "#d12a3e")
                            $(this).children().next().css("color", "#ffffff")
                        }, function ()
                        {
                            $(this).children().next().hide();
                            $(this).children().css("color", "#666666")
                            $(this).children().next().css("color", "#ffffff")
                        })


                        $(".Delte").click(function ()
                        {
                            var self = $(this).parent();
                            Tuhu.CarHistory.DeleteCarHistory($(this).attr("data-id"), function (data)
                            {
                                if (data >= -1)
                                {
                                    $(self).remove();
                                }

                                if (data == -4)
                                {
                                    console.log("默认车型不能删除！");
                                }
                            });
                        })

                        $(window).resize();
                    }
                });

                dialog.on("click", ".CarBrand2,.CarBrand", function ()
                {
                    SelVehicle($(this).attr("data-Brand"), true, dialog);
                }).on("click", ".Histroy", function ()
                {
                    var car = $(this);
                    VehicleID = $(car).attr("data-VehicleID");
                    PL = $(car).attr("data-PaiLiang");
                    N = $(car).attr("data-Nian");
                    Brand = $(car).attr("data-brand");
                    VehicleName = $(car).attr("data-vehiclename");
                    LiYangID = $(car).attr("data-liyangID");
                    var id = $(car).attr("data-id");

                    if (type == "Tires" || type == "Hub" || type == "BaoYang")
                    {
                        $.ajax({
                            url: Tuhu.Domain.ProductSite + "/Car/GetTires",
                            jsonpCallback: "__GetCarBrands__",
                            dataType: "jsonp",
                            data: {
                                ProductID: VehicleID
                            },
                            success: function (data)
                            {
                                var d = data;
                                //console.log(d.Code);
                                if (d.Code == "1")
                                {
                                    var tires = d.Tires.Value.split(';')[0];
                                    if (tires != "")
                                    {
                                        CP_Tire_Width = tires.split('/')[0];
                                        CP_Tire_AspectRatio = tires.split('/')[1].split('R')[0];
                                        CP_Tire_Rim = tires.split('/')[1].split('R')[1];
                                    }
                                }

                                Tuhu.CarHistory.SwitchDefaultCar(id, function ()
                                {
                                    _IsNormally = true;
                                    dialog.close();
                                });
                            }
                        });
                    } else
                    {
                        Tuhu.CarHistory.SwitchDefaultCar(id, function ()
                        {
                            _IsNormally = true;
                            dialog.close();
                        });
                    }
                });
            }
        };
    })();

    Tuhu.Utility = (function ()
    {
        return {
            EditableQuantity: function (option)
            {
                /// <signature>
                ///		<summary>数量加减</summary>
                ///		<param name="option" type="Object">{ increaseButton: null, decreaseButton: null, minValue: 1, maxValue: 9999, quantityTextbox: null, defaultValue: 2 }</param>
                /// </signature>
                option = $.extend({
                    increaseButton: null,
                    decreaseButton: null,
                    minValue: 1,
                    maxValue: 9999,
                    quantityTextbox: null,
                    defaultValue: 2,
                    disabledClass: "disabled"
                }, option);

                var decrease = $(option.decreaseButton);
                var input = $(option.quantityTextbox);
                var increase = $(option.increaseButton);

                if (input.length && !input[0].disabled)
                {
                    var inputValue = parseInt(input.val());
                    if (isNaN(inputValue))
                        inputValue = option.defaultValue;

                    function isValid(value)
                    {
                        return value === parseInt(value).toString();
                    }

                    decrease.click(function ()
                    {
                        if (inputValue > option.minValue)
                        {
                            inputValue--;
                            input.val(inputValue);
                            input.trigger("change");
                            input.trigger("input");

                            if (inputValue <= option.minValue)
                                decrease.addClass(option.disabledClass);

                            increase.removeClass(option.disabledClass);
                        }
                    });
                    input.input(function (event)
                    {
                        if (this.value)
                        {
                            if (isValid(this.value) && parseInt(this.value) > 0)
                                inputValue = parseInt(this.value);
                            else
                                this.value = inputValue;
                        }

                        if (inputValue <= option.minValue)
                            decrease.addClass(option.disabledClass);
                        else
                            decrease.removeClass(option.disabledClass);

                        if (inputValue >= option.maxValue)
                            increase.addClass(option.disabledClass);
                        else
                            increase.removeClass(option.disabledClass);
                    }).blur(function ()
                    {
                        if (!this.value || !isValid(this.value))
                            this.value = inputValue;
                    });
                    increase.click(function ()
                    {
                        if (inputValue < option.maxValue)
                        {
                            inputValue++;
                            input.val(inputValue);
                            input.trigger("change");
                            input.trigger("input");

                            decrease.removeClass(option.disabledClass);

                            if (inputValue >= option.maxValue)
                                increase.addClass(option.disabledClass);
                            else
                                increase.removeClass(option.disabledClass);
                        }
                    });
                }

                return input;
            },
            GetCurrentCity: function (callback, getcity)
            {
                if (!callback)
                    callback = function (region)
                    {
                        var city = region.City || region.Province;
                        city = city.replace("黎族苗族自治县", "").replace("黎族自治县", "").replace("自治县", "").replace("县", "").replace("地区", "").replace("盟", "").replace("林区", "");
                        if (city.endsWith("市"))
                            city = city.substring(0, city.length - 1);
                        else if (city.endsWith("州") && city.length > 2)
                            city = city.substring(0, city.length - 1);
                        $("#headSection #topBar .city").html("<i></i>" + city + "<span></span>");
                    }

                var position = Tuhu.Cookie.Get("city");
                if (position && Tuhu.Cookie.Get("shopcover") != null)
                {
                    position = decodeURIComponent(position).split('|');
                    callback({
                        ProvinceID: parseInt(position[0]),
                        Province: position[1],
                        ProvinceIsInstall: position[2] == "1",
                        CityID: parseInt(position[3]),
                        City: position[4],
                        CityIsInstall: position[5] == "1"
                    });
                } else if (getcity !== false)
                    $.ajax({
                        url: Tuhu.Domain.WwwSite + "/GetCurrentCity.aspx",
                        jsonpCallback: "__GetCurrentCity__",
                        dataType: "jsonp",
                        cache: true,
                        success: function ()
                        {
                            Tuhu.Utility.GetCurrentCity(callback, false);
                        }
                    });
                else
                {
                    Tuhu.Cookie.Set("city", "1|上海市|1|1|上海市|1");
                    callback({
                        ProvinceID: 1,
                        Province: '上海市',
                        ProvinceIsInstall: true,
                        CityID: 1,
                        City: '上海市',
                        CityIsInstall: true
                    });
                }
            },
            TimeCountDown: function (endDateTime)
            {
                var deferred = $.Deferred(),
					timer, prefTick = Date.now();
                if (!(endDateTime instanceof Date))
                    endDateTime = Date.parse(endDateTime);

                var createTime = function (tick)
                {
                    var days = Math.floor(tick / (24 * 60 * 60 * 1000)),
					hours = Math.floor((tick % (24 * 60 * 60 * 1000)) / (60 * 60 * 1000)),
					minutes = Math.floor((tick % (60 * 60 * 1000)) / (60 * 1000)),
					seconds = Math.floor((tick % (60 * 1000)) / 1000);
                    return {
                        tick: tick,
                        time: new Date(tick - 8 * 60 * 60 * 1000),
                        days: days < 10 ? "0" + days.toString() : days.toString(),
                        hours: hours < 10 ? "0" + hours.toString() : hours.toString(),
                        minutes: minutes < 10 ? "0" + minutes.toString() : minutes.toString(),
                        seconds: seconds < 10 ? "0" + seconds.toString() : seconds.toString()
                    }
                }

                var action = function ()
                {
                    var currentTicks = Date.now();
                    var expire = (currentTicks - prefTick) % 1000;
                    timer = setTimeout(function ()
                    {
                        var now = Date.now();
                        if (endDateTime > now)
                        {
                            deferred.notify(createTime(endDateTime - now));
                            action();
                        } else
                        {
                            timer = undefined;
                            deferred.resolve();
                        }
                    }, (expire > 900 ? 1000 : 0) + 1000 - expire);
                    prefTick = currentTicks;
                }
                action();

                var promise = deferred.promise(); // 返回promise对象
                promise.stop = function ()
                {
                    if (timer)
                    {
                        clearTimeout(timer);
                        deferred.reject();
                    }
                }
                promise.remainingTime = function ()
                {
                    return createTime(Math.max(0, endDateTime - Date.now()));
                }

                return promise;
            },
            BrowseStatistics: function (type, objectID)
            {
                new Image().src = Tuhu.Domain.InOutSite + "/Browse.aspx?type=" + encodeURIComponent(type) + "&objectID=" + encodeURIComponent(objectID);
            },
            GetPromotionCode: function (type) {
                $.ajax({
                    url: Tuhu.Domain.WwwSite + "/Home/GetPromotionCode.aspx",
                    jsonpCallback: "__UserPromotionCode__",
                    dataType: "jsonp",
                    data: {
                        type: type
                    }
                }).done(function (returndata) {
                    if (returndata == -99) {
                        Tuhu.PopComDialog.LoginDialog(location.href);
                    }
                    if (returndata == -1) {
                        Tuhu.Dialog.Popup('<div class="quan_layer"><span class="quan_close">X</span><p class="got">已领取过</p><a target="_blank" href="http://my.tuhu.cn/PromotionCode/Index.html">查看我的优惠券</a></div>', { opacity: 0.2, closeHandle: ".quan_close", draggable: false });
                    }
                    if (returndata == -2) {
                        alert("领券失败,请重新领取");
                    }
                    if (returndata > 0) {
                        Tuhu.Dialog.Popup('<div class="quan_layer"><span class="quan_close">X</span><p class="success">领取成功</p><a target="_blank" href="http://my.tuhu.cn/PromotionCode/Index.html">查看我的优惠券</a></div>', { opacity: 0.2, closeHandle: ".quan_close", draggable: false });
                    }
                });
            }
        }
    })();

    Tuhu.Shop = (function ()
    {
        return {
            GetAllShop: function (callback)
            {
                if (location.protocol + "//" + location.host == Tuhu.Domain.WwwSite)
                    $.getJSON("/Shop/Map.aspx", callback);
                else
                    $.ajax({
                        url: Tuhu.Domain.WwwSite + "/Shop/Map.aspx",
                        jsonpCallback: "__ShopMap__",
                        dataType: "jsonp"
                    }).done(callback);
            },
            FilterShop: function (shops, services)
            {
                ///	<signature>
                ///		<summary>过滤满足条件的门店</summary>
                ///		<param name="services" type="Array">数据或全部列出</param>
                ///		<returns type="Number" />
                ///	</signature>

                if (!$.isArray(services))
                {
                    services = Array.apply(this, arguments);
                    services.shift();
                }
                services.forEach(function (service)
                {
                    shops = shops.where(function (shop)
                    {
                        return shop.Service["轮胎服务"] && shop.Service["轮胎服务"][service] || shop.Service["保养服务"] && shop.Service["保养服务"][service] || shop.Service["美容服务"] && shop.Service["美容服务"][service];
                    });
                });
                return shops;
            },
            GetRegionFromShops: function (shops)
            {
                var region = {};
                $.each(shops, function ()
                {
                    var shop = this;
                    region[shop.Province] = region[shop.Province] || {};

                    $.each(shop.Cover, function ()
                    {
                        if (region[shop.Province][this])
                            region[shop.Province][this][shop.PKID] = shop;
                        else
                        {
                            var city = {};
                            city[shop.PKID] = shop;
                            region[shop.Province][this] = city;
                        }
                    });
                });

                return region;
            }, //获得所有区域,区域下有门店的就得到门店
            GetAllRegionAndRegionShops: function (provinces, shops)
            {
                var region = {};
                $.each(provinces, function ()
                {
                    var province = this;
                    region[province.RegionName] = region[province.RegionName] || {};

                    $.each(province.ChildrenRegion, function ()
                    {
                        var c = this;
                        if (region[province.RegionName][c.RegionName])
                        {
                            $.each(shops, function ()
                            {
                                var shop = this;
                                $.each(shop.Cover, function ()
                                {
                                    if (this == c.RegionName)
                                    {
                                        region[province.RegionName][c.RegionName][shop.PKID] = shop;
                                    }
                                })
                            });
                        } else
                        {
                            var city = {};
                            $.each(shops, function ()
                            {
                                var shop = this;
                                $.each(shop.Cover, function ()
                                {
                                    if (this == c.RegionName)
                                    {
                                        city[shop.PKID] = shop;
                                    }
                                });
                            });
                            region[province.RegionName][c.RegionName] = city;
                        }
                    });
                });
                return region;
            }
        };
    })();

    Tuhu.UserLogin = (function ()
    {
        var LoginDialogHtml = '<div class="userinfobody loginbodyheight"><div class="userinfoarea">登录<div class="closebtn"></div></div><hr class="seperateline"/><div class="loginerror"><img style="float: left" height="15" width="15"/><div style="color: RGB(209, 42, 62);height: 15px; line-height: 15px; float: left;margin-left: 10px;">您输入的账户或密码错误，请核对后重新输入</div></div><div class="userinfoform loginformheight"><form action="#" method="post"><table class="userinfotable"><tr><td>手机/邮箱</td></tr><tr><td><input type="text" class="input username" name="username"/></td></tr><tr><td>密码</td></tr><tr><td><input type="password" class="input password" name="password"/></td></tr><tr><td><input type="checkbox" id="autologin" checked="checked"/><label for="autologin">两周内自动登录</label><div id="forgetpassword"><a href="#" onclick="Tuhu.UserLogin.SwitchToOther.SwitchToForgetDialog()">忘记密码？</a></div></td></tr><tr><td><div class="userinfosubmit login" onclick="Tuhu.UserLogin.CheckUserInfo()">立即登录</div></td></tr></table><hr/><label id="loginexten">合作网站账号登录：</label><div id="qqloginicon" onclick="Tuhu.UserLogin.QQLoginClick()" onmouseover="Tuhu.UserLogin.QQLoginMouseOver()" onmouseout="Tuhu.UserLogin.QQLoginMouseOut()"></div></form></div><div class="pagefoot1">还不是途虎养车网用户？<div class="pagefoot2" onclick="Tuhu.UserLogin.SwitchToOther.SwitchToRegistDialog()">免费注册</div></div></div>';

        var m_returnurl = undefined;

        var dialog = undefined;

        function ShowLoginError()
        {
            $(".loginerror img").attr("src", Tuhu.Domain.ResourceSite + "/Image/Www/error.png");
            $(".loginerror").show();
            $(".userinfobody.loginbodyheight").height(425);
            $(".userinfobody.loginformheight").height(325);
            $(".input.password").val("");
            $(".input").focus(function ()
            {
                $(".loginerror").hide();
                $(".userinfobody.loginbodyheight").height(380);
                $(".userinfobody.loginformheight").height(280);
                $(".input").unbind("focus", true);
            });
        }

        function RemoveQQLoginJSNode()
        {
            var qqloginjs = document.getElementById("qqloginjshead");
            var headerelem = document.getElementsByTagName("head")[0];
            headerelem.removeChild(qqloginjs);
        }

        function AddQQLoginJSNode()
        {
            var appendedelem = document.createElement("script");
            appendedelem.setAttribute("type", "text/javascript");
            appendedelem.setAttribute("src", "http://qzonestyle.gtimg.cn/qzone/openapi/qc_loader.js");
            appendedelem.setAttribute("data-redirecturi", Tuhu.Domain.WwwSite + "/backcall/qq.htm");
            appendedelem.setAttribute("data-appid", "100719549");
            appendedelem.setAttribute("charset", "utf-8");
            appendedelem.setAttribute("id", "qqloginjshead");
            var headerelem = document.getElementsByTagName("head")[0];
            headerelem.appendChild(appendedelem);
        }

        return {
            QQLoginClick: function ()
            {
                var redirect_url = Tuhu.Domain.WwwSite + "/backcall/qq.htm";
                window.open("http://openapi.qzone.qq.com/oauth/show?which=Login&display=pc&client_id=100719549&response_type=token&scope=all&redirect_uri=" + redirect_url.split("/", 10).join("%2f").replace(":", "%3a"), "oauth2Login_10127", "height=525,width=585, toolbar=no, menubar=no, scrollbars=no, status=no, location=yes, resizable=yes, left=520, top=140");
            },
            QQLoginMouseOver: function ()
            {
                $(".userinfobody #qqloginicon").css("background", "url('" + Tuhu.Domain.ResourceSite + "/Image/Www/QQ.png') no-repeat scroll 0 -55px rgba(0, 0, 0, 0)");
            },
            QQLoginMouseOut: function ()
            {
                $(".userinfobody #qqloginicon").css("background", "url('" + Tuhu.Domain.ResourceSite + "/Image/Www/QQ.png') no-repeat scroll 0 0 rgba(0, 0, 0, 0)");
            },
            SwitchToOther: {
                SwitchToForgetDialog: function (event)
                {
                    dialog.close();
                    Tuhu.ForgetPassWord.ShowForgetDialog();
                },
                SwitchToRegistDialog: function (event)
                {
                    dialog.close();
                    Tuhu.UserRegist.ShowRegistDialog();
                }
            },
            ShowLoginDialog: function (returnurl)
            {
                m_returnurl = returnurl;
                AddQQLoginJSNode();
                dialog = Tuhu.Dialog.Popup(LoginDialogHtml, {
                    clone: false,
                    draggable: false,
                    closeHandle: ".closebtn",
                    disableScroll: false,
                    opacity: 0.3,
                    onclose: function ()
                    {
                        RemoveQQLoginJSNode();
                        $(document.body).unbind("keydown");
                    }
                });
                $(".input.username").input(function ()
                {
                    $(".input.password").val("");
                });
                $(document.body).keydown(function (event)
                {
                    if (window.event)
                    {
                        if (window.event.keyCode == 13)
                        {
                            Tuhu.UserLogin.CheckUserInfo();
                            return false;
                        }
                    } else
                    {
                        if (event.keyCode == 13)
                        {
                            Tuhu.UserLogin.CheckUserInfo();
                            return false;
                        }
                    }
                });
            },
            CheckUserInfo: function (callback)
            {
                var username = $("input.input.username").val();
                var password = $("input.input.password").val();
                var checked = $("input[type='checkbox']")[0].checked;
                if (username == "" || password == "")
                {
                    ShowLoginError();
                }
                $.ajax({
                    url: Tuhu.Domain.WwwSite + "/User/ULoginIn.aspx",
                    jsonpCallback: "__UserLogin__",
                    dataType: "jsonp",
                    data: {
                        username: username,
                        password: password,
                        ischecked: checked
                    },
                    beforeSend: function () {
                        $(".userinfosubmit").text("正在登录···")
                    }
                }).done(function (returndata)
                {
                    if (returndata < 0)
                    {
                        ShowLoginError();
                        $(".userinfosubmit").text("立即登录");
                    } else if (returndata == 0)
                    {
                        if (m_returnurl != undefined)
                        {
                            location.href = m_returnurl;
                        } else
                        {
                            window.location.reload(true);
                        }
                    }
                });
            }
        };
    })();

    Tuhu.UserRegist = (function ()
    {
        var RegistDialogHtml = '<div class="userinfobody registbodyheight"><div class="userinfoarea">免费注册<div class="closebtn"></div></div><hr class="seperateline"/><div class="userinfoform registformheight"><form action="#" method="post"><table class="userinfotable"><tr><td>手机</td></tr><tr><td><input type="text" id="username" class="input username" placeholder="请输入手机号码"/></td></tr><tr><td class="promotion registpromote"></td></tr><tr><td>密码</td></tr><tr><td><input type="password" id="password1" class="input password" placeholder="请输入8位以上密码" /></td></tr><tr><td class="promotion registpromote"></td></tr><tr><td>确认密码</td></tr><tr><td><input type="password" id="password2" class="input password" placeholder="请重复输入密码" /></td></tr><tr><td class="promotion registpromote"></td></tr><tr><td>短信验证码</td></tr><tr><td><input type="text" id="checkcode" class="input checkcode"><a class="ccbtn" onclick="Tuhu.UserRegist.RegistCheckVCode()">获取短信验证码</a></td></tr> <tr><td class="promotion registpromote"></td></tr><tr><td class="checkinputtd"><input type="checkbox" id="hasread" checked="checked"/><label for="hasread">已阅读并同意</label><a href="http://www.tuhu.cn/Article_1/Article_Agreement.aspx" target="_blank">《途虎养车网用户注册条款》</a></td></tr><tr><td><div class="userinfosubmit regist">免费注册</div></td></tr></table></form></div><div class="pagefoot1">已有账户？<div class="pagefoot2"onclick="Tuhu.UserRegist.SwitchToOther.SwitchToLoginDialog()">立即登录</div></div></div>';

        var dialog = undefined;
        function ShowErrorPro(element, errortext)
        {
            var nextele = element.parent("td").parent("tr").next().children("td");
            nextele.text(errortext);
            nextele.show();
            element.focus(function ()
            {
                nextele.hide();
                element.unbind("focus", true);
            });
        }

        function ClearPassWord()
        {
            $("#password1").val("");
            $("#password2").val("");
        }

        function IsCheckEventHandler()
        {
            $("input[type='checkbox']").click(function ()
            {
                var submitbtn = $(".userinfosubmit.regist");
                if (this.checked == false)
                {
                    submitbtn.removeAttr("onclick");
                    submitbtn.css("background-color", "#999");
                } else
                {
                    submitbtn.attr("onclick", "Tuhu.UserRegist.RegistUserInfo()");
                    submitbtn.css("background-color", "#d12a3e");
                }
            });
        }
        function ReSendTimer()
        {
            timerid = setInterval("Tuhu.UserRegist.IntervalRun()", 1000);
            var submitbtn = $(".ccbtn");
            submitbtn.removeAttr("onclick");
            submitbtn.css("background-color", "#999");
            submitbtn.text(90 + "秒后重新获取");
        }

        function myInterval()
        {
            if (Tuhu.UserRegist.TimeOutNum >= 2)
            {
                var temp = --Tuhu.UserRegist.TimeOutNum;
                var submitbtn = $(".ccbtn");
                submitbtn.removeAttr("onclick");
                submitbtn.css("background-color", "#999");
                submitbtn.text(temp + "秒后重新获取");
            } else
            {
                var submitbtn = $(".ccbtn");
                clearInterval(timerid);
                Tuhu.UserRegist.TimeOutNum = 90;
                submitbtn = $(".ccbtn");
                submitbtn.attr("onclick", "Tuhu.UserRegist.RegistCheckVCode()");
                submitbtn.text("获取短信验证码");
                submitbtn.css("background-color", "#f58863");
            }
        }
        function CheckInfoFormat(event)
        {
            var username = $("#username").val();
            var patrn = new RegExp(/^(1\d{10}$)/);
            var flag = true;
            var password = $("#password1").val();
            var repassword = $("#password2").val();
            var vcode = $("#checkcode").val();
            if (patrn.test(username) == false)
            {
                ShowErrorPro($(".input.username"), "请输入正确的手机号");
                flag = false;
            }
            if (password == "")
            {
                ShowErrorPro($("#password1"), "请输入密码");
                flag = false;
            }
            if (repassword == "")
            {
                ShowErrorPro($("#password2"), "请再次输入密码");
                flag = false;
            }
            if (password != "" && password.length < 8)
            {
                ShowErrorPro($("#password1"), "密码长度不足8位");
                ClearPassWord();
                flag = false;
            }
            if (password != "" && password != repassword)
            {
                ShowErrorPro($("#password2"), "两次输入的密码不一致");
                ClearPassWord();
                flag = false;
            }
            if (vcode == "")
            {
                ShowErrorPro($("#checkcode"), "请输入验证码");
                flag = false;
            }
            return flag;
        }

        function CheckUserName(event)
        {
            var username = $("#username").val();
            var patrn = new RegExp(/^(1\d{10}$)/);
            var flag = true;
            if (patrn.test(username) == false)
            {
                ShowErrorPro($(".input.username"), "请输入正确的手机号");
                flag = false;
            }
            return flag;
        }

        return {
            TimeOutNum: 90,
            IntervalRun: function ()
            {
                myInterval();
            },
            InputGetFocus: function ()
            {
                $(this).removeAttr("placeholder");
            },
            InputLoseFocus: function (text)
            {
                $(this).attr("placeholder", text);
            },
            SwitchToOther: {
                SwitchToLoginDialog: function (event)
                {
                    dialog.close();
                    Tuhu.UserLogin.ShowLoginDialog();
                }
            },
            ShowRegistDialog: function ()
            {
                dialog = Tuhu.Dialog.Popup(RegistDialogHtml, {
                    clone: false,
                    draggable: false,
                    closeHandle: ".closebtn",
                    disableScroll: false,
                    opacity: 0.3,
                    onclose: function ()
                    {
                        $(document.body).unbind("keydown");
                    }
                });
                IsCheckEventHandler();
                $(".userinfosubmit.regist").click(Tuhu.UserRegist.RegistUserInfo);
                $(document.body).keydown(function (event)
                {
                    if (window.event)
                    {
                        if (window.event.keyCode == 13)
                        {
                            Tuhu.UserRegist.RegistUserInfo();
                            return false;
                        }
                    } else
                    {
                        if (event.keyCode == 13)
                        {
                            Tuhu.UserRegist.RegistUserInfo();
                            return false;
                        }
                    }
                });
            },
            RegistUserInfo: function (callback)
            {
                if ($("input[type='checkbox']")[0].checked == false)
                {
                    return null;
                }
                if (CheckInfoFormat())
                {
                    var username = $("#username").val();
                    var password = $("#password1").val();
                    var vcode = $("#checkcode").val();
                    $.ajax({
                        url: Tuhu.Domain.WwwSite + "/User/URegister.aspx",
                        jsonpCallback: "__RegistUserInfo__",
                        dataType: "jsonp",
                        data: {
                            username: username,
                            password: password,
                            vcode: vcode
                        },
                        beforeSend: function () {
                            $(".userinfosubmit").text("正在注册，请稍后···");
                        }
                    }).done(function (returndata)
                    {
                        if (returndata == -11)
                        {
                            ShowErrorPro($("#checkcode"), "验证码有误");
                            $(".userinfosubmit").text("免费注册");
                            return;
                        }
                        if (returndata < 0)
                        {
                            ShowErrorPro($("#username"), "手机号已存在");
                            $("#checkcode").val("");
                            ClearPassWord();
                            $(".userinfosubmit").text("免费注册");
                            return;
                        } else if (returndata >= 0)
                        {
                            dialog.close();
                            Tuhu.Promote.ShowPromoteDialog(Tuhu.Domain.ResourceSite + "/image/www/ok.png", "恭喜您注册成功！", 75);
                            Tuhu.Promote.ClosePromoteDialog(function ()
                            {
                                window.location.reload(true);
                            }, {
                                obj: 1
                            });
                        }
                    });
                }
            },
            RegistCheckVCode: function (callback)
            {
                if (CheckUserName())
                {
                    ReSendTimer();
                    var username = $("#username").val();
                    $.ajax({
                        url: Tuhu.Domain.WwwSite + "/User/URegisterVcode.aspx",
                        jsonpCallback: "__RegistUserInfo__",
                        dataType: "jsonp",
                        data: {
                            username: username
                        }
                    });
                }
            }
        }
    })();

    Tuhu.ForgetPassWord = (function ()
    {
        var ForgetDialogHtml = '<div class="userinfobody forgetbodyheight"><div class="userinfoarea">找回密码<div class="closebtn"></div></div><hr class="seperateline"/><div class="userinfoform forgetformheight"><form id="passwordgetter"><table class="userinfotable"><tr><td>手机/邮箱</td></tr><tr><td><input type="text" class="input username" /></td></tr><tr><td id="upro" class="promotion"></td></tr><tr><td><div class="userinfosubmit forgot" onclick="Tuhu.ForgetPassWord.PassWordRecv()">获取密码</div></td></tr></table></form></div><div class="pagefoot1"><div class="pagefoot2" onclick="Tuhu.ForgetPassWord.SwitchToOther.SwitchToLoginDialog()">立即登录</div></div></div>';

        var dialog = undefined;

        function CheckInfoFormat(event)
        {
            var username = $(".input.username").val();
            var patrn = new RegExp(/^(1\d{10}$)/);
            var patrn1 = new RegExp("[(A-z)|(0-9)]+@[(A-z)|(0-9)]+.[A-z]+");
            if (!patrn.test(username) && !patrn1.test(username))
            {
                ShowError("手机/邮箱错误，请核对后重新输入");
                return false;
            }
            return true;
        }

        function ShowError(str)
        {
            $("#upro").text(str);
            $("#upro").show();
            $(".input.username").focus(function ()
            {
                $("#upro").hide();
                $(".input.username").unbind("focus", true);
            });
        }
        var timerid = 0;

        function ReSendTimer()
        {
            timerid = setInterval("Tuhu.ForgetPassWord.IntervalRun()", 1000);
            var submitbtn = $(".userinfosubmit.forgot");
            submitbtn.removeAttr("onclick");
            submitbtn.css("background-color", "#999");
            submitbtn.text(90 + "秒后重新获取");
        }

        function myInterval()
        {
            if (Tuhu.ForgetPassWord.TimeOutNum >= 2)
            {
                var temp = --Tuhu.ForgetPassWord.TimeOutNum;
                var submitbtn = $(".userinfosubmit.forgot");
                submitbtn.removeAttr("onclick");
                submitbtn.css("background-color", "#999");
                submitbtn.text(temp + "秒后重新获取");
            } else
            {
                var submitbtn = $(".userinfosubmit.forgot");
                clearInterval(timerid);
                Tuhu.ForgetPassWord.TimeOutNum = 90;
                submitbtn = $(".userinfosubmit.forgot");
                submitbtn.attr("onclick", "Tuhu.ForgetPassWord.PassWordRecv()");
                submitbtn.text("获取密码");
                submitbtn.css("background-color", "#d12a3e");
            }
        }

        return {
            TimeOutNum: 90,
            IntervalRun: function ()
            {
                myInterval();
            },
            SwitchToOther: {
                SwitchToLoginDialog: function (event)
                {
                    dialog.close();
                    Tuhu.UserLogin.ShowLoginDialog();
                }
            },
            ShowForgetDialog: function ()
            {
                dialog = Tuhu.Dialog.Popup(ForgetDialogHtml, {
                    clone: false,
                    draggable: false,
                    closeHandle: ".closebtn",
                    disableScroll: false,
                    opacity: 0.3,
                    onclose: function () { }
                });
                $("form#passwordgetter").submit(function (event)
                {
                    event.preventDefault();
                    Tuhu.ForgetPassWord.PassWordRecv();
                })
            },
            PassWordRecv: function (callback)
            {
                var username = $(".input.username").val();
                if (CheckInfoFormat())
                {
                    ReSendTimer();
                    $.ajax({
                        url: Tuhu.Domain.WwwSite + "/User/UForgotPassWord.aspx",
                        jsonpCallback: "__ResetPassWord__",
                        dataType: "jsonp",
                        data: {
                            username: username
                        }
                    }).done(function (returndata)
                    {
                        if (returndata <= 0)
                        {
                            if (returndata == -15)
                            {
                                ShowError("发送短信失败，请您稍后再试");
                            } else if (returndata == -2)
                            {
                                ShowError("一分钟前您已更改密码，请稍后再试");
                            } else
                            {
                                ShowError("您的手机/邮箱还未注册");
                            }
                        }
                    });
                }
            }
        }
    })();

    Tuhu.UserLogOut = (function ()
    {
        return {
            Logout: function ()
            {
                $.ajax({
                    url: Tuhu.Domain.WwwSite + "/User/ULoginOut.aspx",
                    jsonpCallback: "__ULoginOut__",
                    dataType: "jsonp"
                }).done(function (data)
                {
                    Tuhu.Promote.ShowPromoteDialog(Tuhu.Domain.ResourceSite + "/image/www/ok.png", "退出成功！", 120);
                    Tuhu.Promote.ClosePromoteDialog(function ()
                    {
                        location.href = data;
                    }, {
                        obj: 1
                    });
                });
            }
        }
    })();

    Tuhu.Promote = (function ()
    {
        var PromoteDialogFirstHtml = '<div class="userinfobody promotebodyheight"><div class="promotecontent" style="padding: 25px 0px 25px ';
        var PromoteDialogSecondHtml = 'px;"><img src="';
        var PromoteDialogThirdHtml = '"  width="27" height="27" class="promoteimage"/><div class="promotetext"><b>';
        var PromoteDialogFourHtml = '</b></div></div></div>';
        var dialog = undefined;
        var TimeOutNum = 0;
        var timerid = 0;
        var callback = null;

        function SetCloseTimer(callbackfun)
        {
            timerid = setInterval("Tuhu.Promote.ClosePromote()", TimeOutNum * 1000);
            callback = callbackfun;
        }

        return {
            ShowPromoteDialog: function (iconPath, promotetext, position)
            {
                dialog = Tuhu.Dialog.Popup(PromoteDialogFirstHtml + position + PromoteDialogSecondHtml + iconPath + PromoteDialogThirdHtml + promotetext + PromoteDialogFourHtml, {
                    clone: false,
                    draggable: false,
                    closeHandle: ".closebtn",
                    disableScroll: false,
                    opacity: 0.3,
                    onclose: function () { }
                });
            },
            ClosePromoteDialog: function (callbackfun, nTimeOut)
            {
                if (nTimeOut != null && nTimeOut.obj > 0)
                {
                    TimeOutNum = nTimeOut.obj;
                    SetCloseTimer(callbackfun);
                } else if (nTimeOut == null || nTimeOut.obj <= 0)
                {
                    callbackfun();
                    dialog.close();
                }
            },
            ClosePromote: function ()
            {
                clearInterval(timerid);
                dialog.close();
                callback();
            }
        };
    })();

    Tuhu.PopComDialog = (function ()
    {
        return {
            LoginDialog: function (returnurl)
            {
                $.ajax({
                    url: Tuhu.Domain.WwwSite + "/User/IsUserLoginOn.aspx",
                    jsonpCallback: "__IsUserLoginOn__",
                    dataType: "jsonp"
                }).done(function (data)
                {
                    if (returnurl == undefined || typeof returnurl == "undefined")
                    {
                        if (data == 0)
                        {
                            Tuhu.UserLogin.ShowLoginDialog();
                        } else
                        {
                            location.reload(true);
                        }
                    } else
                    {
                        if (data == 0)
                        {
                            Tuhu.UserLogin.ShowLoginDialog(returnurl);
                        } else
                        {
                            location.href = returnurl;
                        }
                    }
                });
            }
        }
    })();

    Tuhu.CustomerService = (function ()
    {
        return {
            LoadScript: function (param)
            {
                window.NTKF_PARAM = $.extend({
                    siteid: "kf_9739",              //企业id
                    settingid: "kf_9739_1427361593282",           //应答客服组id，在客户端生成代码界面创建
                    uid: window.u_user_id || "",                //用户id，登录用户必填，游客为空字符串
                    uname: window.u_nickname || "",             //用户名，登录用户必填，游客为空字符串
                    isvip: "0"            //是否为vip用户，登录用户可选，游客为"0"
                }, param);

                var script = document.createElement("script");
                script.src = "http://dl.ntalker.com/js/xn6/ntkfstat.js?siteid=kf_9739";
                //script.src = "http://chat.53kf.com/kf.php?arg=tuhukf&style=1";
                script.async = "async";
                script.defer = "defer";

                var dialog = $('<img src="http://resource.tuhu.cn/Image/kf.jpg" class="kf_fixed" />').click(function ()
                {
                    Tuhu.CustomerService.OpenChat();
                });

                var s = $("#scriptSection script:first").before(dialog)[0];
                s.parentNode.insertBefore(script, s);

                dialog.css("top", Math.max(0, document.documentElement.clientHeight - dialog.height()) / 3);
                $win.resize(function ()
                {
                    dialog.css("top", Math.max(0, document.documentElement.clientHeight - dialog.height()) / 3);
                });

                var action = function ()
                {
                    var floatDialog = Tuhu.Dialog.Popup('<div id="ivtzdy_53kf_0" style="position:relative;overflow: hidden;background-image: url(http://resource.tuhu.cn/Image/53kf/zdyivt_53kf_1404446222.png);background-repeat: no-repeat;font-size: 12px;text-align:left;width: 400px;height: 165px;z-index: 9998;border-style: none;font-family:宋体;word-break:break-all;;"><div id="ivtzdy_53kf_title" style="width:100%;height:33px;"><div id="ivtzdy_53kf_1" style="overflow: hidden;position:absolute;background-image: url(http://resource.tuhu.cn/Image/53kf/zdyivt_53kf_1405064697.png);background-repeat: no-repeat;font-size: 12px;width: 22px;height: 22px;left: 7px;top: 6px;z-index: 1;"></div><div id="ivtzdy_53kf_2" style="cursor:pointer;overflow: hidden;position:absolute;background-image: url(http://resource.tuhu.cn/Image/53kf/dialog_15.png);background-repeat: no-repeat;font-size: 12px;color: #000000;width: 11px;height: 10px;left: 379px;top: 10px;z-index: 2;font-family:宋体;word-break:break-all;;"></div><div id="ivtzdy_53kf_3" style="overflow: hidden;position:absolute;background-repeat: no-repeat;line-height:120%;font-size: 12px;color: #FFFFFF;text-align:left;width: 338px;height: 13px;left: 35px;top: 9px;z-index: 3;">途虎养车网客服，为你服务！</div></div><div id="ivtzdy_53kf_4" style="overflow: hidden;position:absolute;background-repeat: no-repeat;line-height:120%;font-size: 12px;color: #000000;text-align:left;width: 260px;height: 60px;left: 140px;top: 50px;z-index: 5;font-family:宋体;word-break:break-all;;">买轮胎，晒单返200！ <br>全国266个城市，2400家门店免费安装。 <br>请问有什么可以帮您？ </div><div id="ivtzdy_53kf_5" style="cursor:pointer;overflow: hidden;position:absolute;background-image: url(http://resource.tuhu.cn/Image/53kf/zdyivt_53kf_1404450009.png);background-repeat: no-repeat;font-size: 12px;color: #FFFFFF;font-weight: bold;width: 101px;height: 34px;left: 139px;top: 118px;z-index: 6;font-family:宋体;word-break:break-all;text-align:center;line-height:34px;">点击咨询</div><div id="ivtzdy_53kf_6" style="cursor:pointer;overflow: hidden;position:absolute;background-image: url(http://resource.tuhu.cn/Image/53kf/zdyivt_53kf_1404450022.png);background-repeat: no-repeat;font-size: 12px;color: #000000;width: 101px;height: 34px;left: 258px;top: 118px;z-index: 7;font-family:宋体;word-break:break-all;text-align:center;line-height:34px;color:#FF6600">下次再说</div></div>', { fullScreen: false, zIndex: 9999999, draggable: true, closeHandle: "#ivtzdy_53kf_2, #ivtzdy_53kf_6", dragHandle: "#ivtzdy_53kf_title" });

                    $("#ivtzdy_53kf_5", floatDialog).click(function ()
                    {
                        Tuhu.CustomerService.OpenChat();
                    });

                    $("#ivtzdy_53kf_6", floatDialog).click(function ()
                    {
                        _gaq.push(['_trackEvent', 'consult', 'consult_nexttime', 'ok']);
                    });

                    $("#ivtzdy_53kf_2, #ivtzdy_53kf_6", floatDialog).click(function ()
                    {
                    	var dt = new Date();
                    	dt.setMinutes(dt.getMinutes() + 10);
                    	Tuhu.Cookie.Set("kf_hide2", "true", dt, Tuhu.Domain.TopDomain);
                    });
                }

                //action();
                if (!Tuhu.Cookie.Get("kf_hide2"))
                	setTimeout(action, 15000);

                $doc.on("click", ".window-containter .window-button-close", function ()
                {
                    _gaq.push(['_trackEvent', 'consult', 'close', 'ok']);
                });
            },
            OpenChat: function (settingid)
            {
                //setIsinvited();
                //window.open('http://www.tuhu.cn/dialog_1.htm?arg=tuhukf&amp;style=1&amp;kflist=off&amp;kf=8962,8989,8990,8991,8993,8995,8996,8997,8998,8999,9000,9001,8980,8979,8963,8965,8968,8969,8970,8971,8972,8973,8976,8978,8512,8551,9075,9080,9082,9095,9097,9098,9100,9101,9103,9105,9106,9073,9072,9019,9022,9088,9110,9058,9067,9069,9071,9112,8892,8893,8895,8896,8897,8898,8900,8910,8911,8912,8837,8909,8906,8875,8885,8883,8855,8889,8891,8899,8890,8886,8902,8905,8915,8936,8937,8938,8939,8950,8952,8953,8955,8956,8957,8960,8935,8933,8919,8920,8923,8926,8927,8928,8929,8930,8931,8932,8961&amp;zdkf_type=1&amp;language=zh-cn&amp;charset=utf-8&amp;lytype=0&amp;referer={hz6d_referer}&amp;keyword=&amp;tfrom=1&amp;tpl=crystal_blue', '_blank', 'height=473,width=703,top=200,left=200,status=yes,toolbar=no,menubar=no,resizable=yes,scrollbars=no,location=no,titlebar=no');
                if (window.NTKF && NTKF.im_openInPageChat)
                {
                    new Image().src = Tuhu.Domain.InOutSite + "/Action.aspx?t=1&_" + Date.now();

                    _gaq.push(['_trackEvent', 'consult', 'click', 'ok']);
                    //_hmt.push(['_trackEvent', 'click', 'zixun', 'ok']);

                    NTKF.im_openInPageChat(settingid);
                }
            }
        };
    })();

    //#region 公共脚本
    $doc.on("click", "a[href=#]", function (event)
    {
        event.preventDefault();
    }).on("click", "#GotoTop", function (event)
    {
        event.preventDefault();
        $("html, body").animate({
            scrollTop: 0
        }, 120);
    });

    (function ()
    {
        var reloadCar = true;
        Tuhu.CarHistory.Refresh = function ()
        {
            Tuhu.CarHistory.SelectCarHistory(createPanel);
            console.log("refresh");
        }
        var showPanel = function ()
        {
            $("#historyCar").stop(true, true).slideToggle();

            $doc.one("click", function ()
            {
                $("#historyCar").slideUp();
            });
        };
        var createPanel = function (data)
        {
            var list = $("#historyCar .historylist ol");
            list.html("");
            $.each(data, function (index, vehicle)
            {
                if (vehicle.IsDefaultCar)
                    $("#historyCar .currentCar").html("<i></i> " + [vehicle.Brand, vehicle.VehicleName, vehicle.PaiLiang, vehicle.Nian].join(" ")).data("car", vehicle);
                else
                {
                    var li = $("<li>" + [vehicle.Brand, vehicle.VehicleName, vehicle.PaiLiang, vehicle.Nian].join(" ") + "<span class='del'>X</span></li>");
                    li.appendTo(list).data("car", vehicle);
                }
            });
        };
        $("#myCar a:first").click(function (event)
        {
            event.preventDefault();

            if (reloadCar)
            {
                var self = $(this);

                if (Tuhu.Cookie.Get("defaultCar"))
                {
                    Tuhu.CarHistory.SelectCarHistory(createPanel);
                    reloadCar = false;
                    showPanel();
                } else
                {
                    Tuhu.CarHistory.LoadDialog("BaoYang", function (vehicle)
                    {
                        if (vehicle && vehicle.VehicleID)
                        {
                            var target = self.next().next().children();

                            vehicle.IsDefaultCar = true;

                            createPanel([vehicle]);

                            showPanel();
                        }
                        return false;
                    });
                }
            } else
                showPanel();
            return false;
        });

        function _SwitchHistoryCar()
        {
            var currentCar = $("#historyCar .currentCar"),
                car = currentCar.data("car"),
                carName = currentCar.text(),
                self = Tuhu.CarHistory.OnCarChanged.self;
            if (!self)
            {
                //新增元素

            } else
            {
                //有切换的元素
                self.children().remove();
                currentCar.html("<i></i>" + self.text()).data("car", self.data("car"));
                self.html(carName + "<span class='del'>X</span>").data("car", car);
                Tuhu.CarHistory.OnCarChanged.self = null;
            }

        }

        Tuhu.CarHistory.OnCarChanged.add(function (vehicle)
        {
            if (vehicle && (vehicle.vehicleID || vehicle.VehicleID))
            {
                //替换我的爱车当前车型
                var target = $("#myCar span>a");
                target.text("更换");
                var text = [vehicle.Brand, vehicle.VehicleName, vehicle.PaiLiang || vehicle.paiLiang, vehicle.Nian || vehicle.nian].join(" ") + "(" + target[0].outerHTML + ")";
                target.parent().html(text);
                //替换历史车型
                _SwitchHistoryCar();

                reloadCar = true;
            }
        });

        window.SelectBaoYangVehicle = function ()
        {
            Tuhu.CarHistory.LoadDialog("BaoYang", false);
        }
        $("#myCar span").on("click", "a", function (event)
        {
            event.preventDefault();
            window.SelectBaoYangVehicle();
        });

        $("#historyCar").click(function (event)
        {
            event.stopPropagation();
        });

        $("#historyCar ol").on("click", ".del", function (event)
        {
            event.stopPropagation();

            var self = $(this).parent();

            var car = self.data("car");
            Tuhu.CarHistory.DeleteCarHistory(car.PKID, function ()
            {
                self.remove();
            });
        });

        $("#historyCar").on("click", "li", function (event)
        {
            var self = $(this);
            var car = self.data("car");

            Tuhu.CarHistory.SwitchDefaultCar(car.PKID, function ()
            {
                car.vehicleID = car.VehicleID;
                Tuhu.CarHistory.OnCarChanged(car);
            });
            $doc.trigger("click");
        });
    })();

    (function ()
    {
        var searchBox = $(".i_searchArea :text");
        var suggestUl = $(".i_searchArea ul");
        var submitBtn = $(".i_searchArea .btn");
        var form = $(".i_searchArea>form");
        //缓存搜索结果
        var cacheObj = {};

        var hideSuggestUl = function ()
        {
            suggestUl.hide();
        }
        var showSuggestUl = function ()
        {
            suggestUl.show();
            $doc.one("click", function ()
            {
                hideSuggestUl();
            });
        }

        var bindSuggest = function (data, key)
        {
            if (key)
                cacheObj[key] = data;

            suggestUl.html("");
            if (data.length > 0)
            {
                $.each(data, function (index, item)
                {
                    var li = document.createElement("li");
                    var a = document.createElement("a");

                    li.appendChild(a);
                    suggestUl.append(li);

                    a.target = "_blank";
                    a.href = item.link;
                    a.innerText = a.textContent = item.label || item.value;
                });

                showSuggestUl();
            } else
                hideSuggestUl();
        }

        searchBox.focus(function (e)
        {
            if (!this.value)
            {
                suggestUl.html("");
            } else if (suggestUl.children().length)
            {
                showSuggestUl();
            }
        }).click(false).on(window.ieVersion == 8 ? "keypress" : "input", function ()
        {
            var self = this;
            clearTimeout(self.timer);
            if (self.value)
            {
                if (self.value in cacheObj)
                    bindSuggest(cacheObj[self.value], self.value);
                else
                    self.timer = setTimeout(function ()
                    {
                        $.ajax({
                            url: Tuhu.Domain.WwwSite + "/AjaxApi.aspx",
                            jsonpCallback: "__SearchSuggest__",
                            dataType: "jsonp",
                            cache: true,
                            data: {
                                term: self.value
                            },
                            success: function (data)
                            {
                                bindSuggest(data, self.value);
                            }
                        });
                    }, 300);
            } else
            {
                suggestUl.html("");
                hideSuggestUl();
            }
        });
        submitBtn.click(function ()
        {
            if (!searchBox.val())
                return;
            window.location.href = form.attr("action") + "?s=" + searchBox.val();
        });

        suggestUl.on("mousemove", "a", function ()
        {
            searchBox.val($(this).text());
        })
    })();

    $("#itemNavi").on("mouseenter", ".grid_2:last", function ()
    {
        $(this).find("ul").show();
        return false;
    });

    $("#itemNavi").on("mouseleave", ".grid_2:last", function (e)
    {
        //var target = $(e.target);
        //alert(target.parent(".grid_2").length);
        //if (target.parent(".grid_2").length == 0)
        //{
        //    $(this).hide();
        //}
        $(this).find("ul").hide();
        return false;

    });



    $("#dvWlTop").on("click", "a.pleaseloginin", function (event)
    {
        Tuhu.PopComDialog.LoginDialog();
    }).on("click", "a.link-regist", function (event)
    {
        Tuhu.UserRegist.ShowRegistDialog();
    });
    $(".grid_11.right").on("click", "a.link-mycount", function (event)
    {
        Tuhu.PopComDialog.LoginDialog(Tuhu.Domain.MySite);
    });
    $(".i_top_right").on("click", "a.link-mycount", function (event)
    {
        Tuhu.PopComDialog.LoginDialog(Tuhu.Domain.MySite);
    });

    Tuhu.CarHistory.GetDefaultCar(function (car)
    {
        car && $("#myCar .tCar span:first").html([car.Brand, car.Vehicle, car.PaiLiang, car.Nian, '(<a href="#" class="openVehicleWin">更换</a>)'].join(' '));
    });

    //购物车数量
    Tuhu.Cart.GetCartQuantity();
    Tuhu.Utility.GetCurrentCity();

    $("img:not([onerror])").on("error", function ()
    {
        $(this).attr("src", Tuhu.Domain.ResourceSite + "/Image/Public/Empty.png").css('background', "url(" + Tuhu.Domain.ResourceSite + "/Image/Public/NoImg.png) center center no-repeat #f5f5f5");
    }).filter(":not([src])").attr("src", Tuhu.Domain.ResourceSite + "/Image/Public/Empty.png").css('background', "url(" + Tuhu.Domain.ResourceSite + "/Image/Public/NoImg.png) center center no-repeat #f5f5f5").end().filter("[src]").each(function ()
    {
        this.src = this.src;
    });
    //#endregion

    return Tuhu;
})(window, window.document, window.jQuery);

//提示插件
//Globa.css
//eg. $('#id').Tips({text:'hello'})
(function ($, win, doc)
{
    var $doc = $(doc.documentElement);
    var $win = $(win);
    var $tuhu_tips = $('<div class="Tuhu_Tips"></div>');

    function tips(opts)
    {
        var options = {
            direction: 'top', //bottom|right|top|bottom
            animate: 'opacity',
            component: 'tooltip', //toolerror|tooltip
            tail: '', //bottomTail
            distance: 20, //距离目标元素距离
        };
        options = $.extend(options, opts);
        var $target = $(opts.target);
        var $new_tuhu_tips = $tuhu_tips.clone();
        $new_tuhu_tips.append(opts.html ? opts.html : opts.text);
        $doc.append($new_tuhu_tips.addClass(options.component).addClass(options.tail));

        function resize()
        {
            var offset = $target.offset(),
				tipOffset = $new_tuhu_tips.offset(),
				_top, _left,
				_start = {},
				_end = {};

            function GetHorizontalAlignTop()
            {
                return offset.top + $doc.scrollTop();
            };

            function GetVerticalAlignLeft()
            {
                return offset.left;
            };
            //判断当前位置与目标位置是否在安全区域
            function IsInSafeRange(_top, _left)
            {
                return Math.abs(_top - tipOffset.top) < 10 && Math.abs(_left - tipOffset.left) < 10;
            };
            switch (options.direction)
            {
                case 'top':
                    _top = GetHorizontalAlignTop() - $new_tuhu_tips.outerHeight() - options.distance;
                    _left = GetVerticalAlignLeft() + ($target.outerWidth() - $new_tuhu_tips.outerWidth()) / 2;
                    if (IsInSafeRange(_top, _left)) return;
                    _start = {
                        top: _top + 10,
                        left: _left,
                        opacity: 0
                    };
                    _end = {
                        top: _top,
                        opacity: 1
                    };
                    break;
                case 'right':
                    _top = GetHorizontalAlignTop() + ($target.outerHeight() - $new_tuhu_tips.outerHeight()) / 2;
                    _left = GetVerticalAlignLeft() + $target.outerWidth() + options.distance;
                    if (IsInSafeRange(_top, _left)) return;
                    _start = {
                        top: _top,
                        left: _left - 10,
                        opacity: 0
                    };
                    _end = {
                        left: _left,
                        opacity: 1
                    };
                    break;
                case 'bottom':
                    _top = GetHorizontalAlignTop() + $target.outerHeight() + options.distance;
                    _left = GetVerticalAlignLeft() + ($target.outerWidth() - $new_tuhu_tips.outerWidth()) / 2;
                    if (IsInSafeRange(_top, _left)) return;
                    _start = {
                        top: _top - 10,
                        left: _left,
                        opacity: 0
                    };
                    _end = {
                        top: _top,
                        opacity: 1
                    };
                    break;
                case 'left':
                    _top = GetHorizontalAlignTop() + ($target.outerHeight() - $new_tuhu_tips.outerHeight()) / 2;
                    _left = GetVerticalAlignLeft() - $target.outerWidth() - options.distance;
                    if (IsInSafeRange(_top, _left)) return;
                    _start = {
                        top: _top,
                        left: _left + 10,
                        opacity: 0
                    };
                    _end = {
                        left: _left,
                        opacity: 1
                    };
                    break;
            }

            //调整位置添加动画
            $new_tuhu_tips.css(_start).animate(_end, 300);
        };
        setTimeout(function ()
        {
            resize();
        }, 1)

        $win.on('resize', resize);
        setTimeout(function ()
        {
            $new_tuhu_tips.remove();
        }, 3000);
        return $new_tuhu_tips;
    };
    $.fn.Tips = function (opts)
    {
        $(this).each(function ()
        {
            if ($(this).data('poptips'))
            {
                $(this).data('poptips').remove();
            }
            var options = $.extend(opts, {
                target: this
            });
            $(this).data('poptips', tips(options));
        });
    };
}(jQuery, window, document));
