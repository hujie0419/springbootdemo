function ndoor(c) {
    var jsndoor = {
        tag: 'li'
        , $: {
            tag: 'div', className: 'box', $: {
                tag: 'div', className: 'addnew', $: {
                    tag: 'a', href: '#', $: '添加模块', onclick: function () {
                        var cfg = this.$root.$c$;
                        if (cfg.onadd) {
                            cfg.onadd(this.$root);
                        }
                    }
                }
            }
        }
    };
    var el = joy.jbuilder(jsndoor);
    el.$c$ = c;
    return el;
}
function door(data, c) {
    if (!c) {
        c = {};
    }
    var jsdoor = {
        tag: 'li',
        $: [
            {
                tag: 'div',
                className: 'box',
                $: {
                    tag: 'div',
                    className: 'picbox',
                    $: [
                        {
                            tag: 'div',
                            className: 'line1',
                            $: [
                                { tag: 'div', alias: 'boxstatus', style: { float: 'left', paddingLeft: '10px' } }, { tag: 'div', alias: 'boxexpire', style: { float: 'right', paddingRight: '10px' } }
                            ]
                        }, { tag: 'div', alias: 'title', className: 'line2' }, {
                            tag: 'div',
                            className: 'line3',
                            $: [
                                {
                                    tag: 'div', style: { float: 'left', paddingLeft: '10px' }, $: {
                                        tag: 'a', href: '#', $: '编辑', onclick: function () {
                                            var root = this.$root;
                                            if (root.update) {
                                                root.update();
                                                var version = $("input[name='Version']").val();//获取版本
                                                var modelfloor = $("input[name='modelfloor']").val();//获取楼层
                                                if (version == 1 && modelfloor==2)//在修改页面元素加载完之后，如果是2楼，同时是新的（version:1）的，h5生成按钮才显示出来
                                                    $("#createurl").show();
                                                debugger;
                                            }
                                        }
                                    }
                                },
                                  {
                                      tag: 'div', style: { float: 'left', paddingLeft: '25%'}, $: {
                                          tag: 'a', alias: "onoff", href: '#', $: '地域处理-开', onclick: function () {
                                              var root = this.$root;
                                              var d = root.$d$;
                                              if (d.ModelType == 1) {
                                                  ModelType = 0;
                                              }
                                              else if (d.ModelType == 0) {
                                                  ModelType = 1;
                                              }
                                              else {
                                                  ModelType = 0;
                                              }
                                              location.href = root.onoffurl + "?id=" + d.id + "&ModelType=" + ModelType;
                                          }
                                      }
                                  },
                                {
                                    tag: 'div', style: { float: 'right', paddingRight: '10px' }, $: {
                                        tag: 'a', alias: 'bdel', href: '#', $: '删除', onclick: function () {
                                            if (confirm('确定要删除整个模块吗？')) {
                                                var root = this.$root;
                                                var d = root.$d$;
                                                var u = buildurl(root.rmurl, d);
                                                location.href = u;
                                            }
                                        }
                                    }
                                }
                            ]
                        }, { tag: 'div', alias: 'prodlink', className: 'line4', $: { tag: 'a', alias: 'link', href: '#', $: '产品配置' } }
                    ]
                }
            }
        ],
        built: function (json) {
            if (c.built) {
                c.built(this, data);
            }
        }
        , setval: function (data, c) {
            var root = this;
            var d = new Date(data.overtime);
            var now = new Date();
            root.$link.innerHTML = data.modelname + '产品配置';
            root.$d$ = data;
            root.$title.innerHTML = data.modelname;
            root.$boxexpire.innerHTML = d.getFullYear() + '-' + (d.getMonth() + 1) + '-' + d.getDate();
            root.$boxstatus.innerHTML = data.showstatic == '1' ? '已启用' : '已禁用';
            root.$onoff.innerHTML = data.ModelType == '1' ? '地域处理-关' : '地域处理-开';//地域处理 if 1:地域处理-关 0:地域处理-开
            if (data.showstatic == '1') {
                $(root.$boxstatus).addClass('senabled');
            } else {
                $(root.$boxstatus).addClass('sdisabled');
            }
            if (d <= now) {
                $(root.$boxexpire).addClass('sdisabled');
            } else {
                $(root.$boxexpire).removeClass('sdisabled');
            }
            if (root.onsetval) {
                root.onsetval(data);
            }
        }
    };

    var el = joy.jbuilder(jsdoor);
    joy.extend(el, c);
    el.setval(data, c);
    if (c.added) {
        c.added(el);
    }
    return el;
}
function floor(data, c) {
    var jsfloor = {
        tag: 'div',
        className: 'floor',
        $: [
            { tag: 'div', alias: 'floor', className: 'nfloor' }
            , { tag: 'div', $: { tag: 'ul', alias: 'area' } }
        ],
        setval: function (data, c, index) {
            if (c.title) {
                this.$floor.innerHTML = c.title(data, index);
            }
            var aid = 0, fid = 0;
            for (var i = 0; i < data.length; i++) {
                var d = data[i];
                aid = d.apptype;
                fid = d.modelfloor;
                var del = door(d, c.doors);
                this.$area.appendChild(del);
            }

            if (c.added) {
                c.added({ el: this, aid: aid, fid: fid, onadd: c.onadd });
            }
        }
    };
    var el = joy.jbuilder(jsfloor);
    el.setval(data, c);
    return el;
}

function tower(data, cfg) {
    var jstower = {
        tag: 'div',
        className: 'ModuleList tower',
        $: [{ tag: 'h2', alias: 'caption' }, { tag: 'div', alias: 'box' }],
        setval: function (data, c) {
            if (c.title) {
                this.$caption.innerHTML = c.title(data);
            }
            this.$box.innerHTML = '';
            var cf = c.floors;
            var p = picker({ field: cf.filter });
            for (var i = 0; i < data.length; i++) {
                var d = data[i];
                if (p.diff(d)) {
                    var fdata = p.data();
                    var fel = floor(fdata, cf, i);
                    this.$box.appendChild(fel);
                }
            }
            p.flush();
            var fdata = p.data();
            if (fdata && fdata.length > 0) {
                var fel = floor(fdata, cf);
                this.$box.appendChild(fel);
            }
        }
    };
    var el = joy.jbuilder(jstower)
    el.setval(data, cfg);
    return el;
}
function park(data, cfg) {
    var div = document.createElement('div');
    div.className = 'park';
    div.setval = function (data, c) {
        var fi = c.filter;
        var p = picker({ field: fi });
        for (var i = 0; i < data.length; i++) {
            var d = data[i];
            if (p.diff(d)) {
                var tdata = p.data();
                var tel = tower(tdata, c);
                div.appendChild(tel);
            }
        }

        var tdata = p.data();
        if (tdata && tdata.length > 0) {
            var tel = tower(tdata, c);
            div.appendChild(tel);
        }
    };
    div.setval(data, cfg);
    return div;
}
