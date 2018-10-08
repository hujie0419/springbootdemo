function intvalidator(event) {
    if (event.keyCode) {
        data = event.keyCode;
        return (/[\d]/.test(String.fromCharCode(event.keyCode)));
    }
    return parseInt(event) == event;
}
function requiredvalidator(data) {
    if (!data || data == '') {
        return false;
    }
    return true;
}
function floatvalidator(event) {
    if (event.keyCode) {
        return (/[\d.]/.test(String.fromCharCode(event.keyCode)));
    }
    return parseFloat(event) == event;
}

function definputcreator(editors, row, i) {
    var input = document.createElement('input');
    input.name = i;
    input.getval = function () {
        return this.value;
    };
    input.setval = function (val) {
        this.value = val;
    };
    editors[i] = input;
    return input;
}

function definputcreatorToRadio(editors, row, i, val, j) {
    var input = document.createElement('input');
    input.name = i;
    input.type = 'radio';
    input.value = j;
    input.style.width = "50px";
    if (val == j) {
        input.checked = 'checked';
    }
    input.getval = function () {
        return this.value;
    };
    input.setval = function (val) {
        this.value = val;
    };
    editors[i] = input;
    return input;
}

function inputbtncreator(editors, row, i) {
    var c = this.c;
    if (!c) {
        c = editors[i];
    }
    var json = {
        tag: 'div'
        , className: 'inputbtncreator'
        , setval: function (data) {
            this.$box.value = data;
        }
        , getval: function () {
            return this.$box.value;
        }
        , $: [
            { tag: 'input', alias: 'box', name: i }, {
                tag: 'input', alias: 'btn', type:'button', value:c.btntext, onclick: function () {
                    var root = this.$root;
                    if (root.click) {
                        root.click(editors);
                    }
                }
            }
        ]
    };
    var el = joy.jbuilder(json);
    joy.extend(el, c);
    return el;
}
function imgupcreator(editors, row, i) {
    var c = this.c;
    if (!c) {
        c = editors[i];
    }
    var json = {
        tag: 'div'
        , className: 'imgupcreator'
        , setval: function (data) {
            this.$img.src = data;
            this.$box.value = data;
            if (data && data != '') {
                $(this.$img).show();
            }
        }
        , getval: function (data) {
            return this.$box.value;
        }
        , $: [
            { tag: 'img', alias: 'img', style: { display: 'none' } }
            , {
                tag: 'input', alias: 'input', name: 'up_' + i, type: 'file', onchange: function () {
                    var root = this.$root;
                    $.ajaxFileUpload({
                        url: c.url,
                        secureuri: false,
                        fileElement: root.$input,
                        dataType: 'JSON',
                        data: {},
                        type: "post",
                        success: function (result) {
                            var r = JSON.parse(result);
                            console.log(r);
                            if (r && r.BImage != "" && r.SImage != "") {
                                //r.SImage = r.SImage.replace(/image.tuhu/ig, "img.tuhu");
                                console.log(r.SImage);
                                root.setval(r.SImage);
                                $(root.$img).show();
                            } else {
                                alert("上传失败！");
                            }
                        }
                    });
                }
            }
            , { tag: 'input', alias: 'box', name: i }
        ]
    };
    var el = joy.jbuilder(json);
    joy.extend(el, c);
    return el;
}
function trigger(o, args) {
    if (typeof (o) == 'function') {
        return o(args);
    }
    return o;
}
function formeditor(row, c) {
    if (!c) {
        c = {};
    }
    var json = {
        tag: 'div'
        , className: 'formeditor'
        , $: {
            tag: 'form', alias: 'edtform', name: 'frm', method: 'post', action: trigger.call(c, c.submit, row), enctype: "multipart/form-data", $: [
                {
                    tag: 'div', className: 'title', $: [
                        { tag: 'span', className: 'content', $: trigger(c.title, row) }
                        , {
                            tag: 'div', className: 'toolbar', $: [
                                {
                                    tag: 'a', className: 'btn save', href: '#', $: '保存', onclick: function () {
                                        var editors = this.$root.$$editors;
                                        var error = false;
                                        if (editors) {
                                            for (var i in editors) {
                                                var edt = editors[i];
                                                if (edt.validate && !edt.validate()) {
                                                    $(edt).addClass('error');
                                                    error = true;
                                                } else {
                                                    $(edt).removeClass('error');
                                                }
                                            }
                                        }
                                        if (error) {
                                            return;
                                        }
                                        if (c.save) {
                                            var o = {};
                                            for (var i in editors) {
                                                var edt = editors[i];
                                                var v = edt.getval();
                                                o[i] = v;
                                            }
                                            var upurl = rep(c, o);
                                            c.save(upurl, o);
                                            overlay({ hide: true });
                                        } else if (c.submit) {
                                            if (!c.onbeforesubmit || c.onbeforesubmit(this)) {
                                                var frm = this.$root.$edtform;
                                                frm.submit();
                                            }
                                        }
                                    }
                                }
                                , {
                                    tag: 'a', className: 'btn cancel', href: '#', $: '取消', onclick: function () {
                                        if (c.cancel) {
                                            c.cancel();
                                        }
                                    }
                                }
                            ]
                        }
                    ]
                }
                , {
                    tag: 'div', className: 'formbody', $: [
                        { tag: 'div', className: 'editors', alias: 'form' }
                        , {
                            tag: 'div', className: 'cmd', alias: 'cmd'
                        }
                    ]
                }

            ]
        },
        show: function () {
            var el = overlay();
            var tb = this;
            el.appendChild(tb);
        }
    };
    var div = joy.jbuilder(json);
    var table = document.createElement('table');
    var tbody = document.createElement('tbody');
    table.appendChild(tbody);
    if (!row) {
        return table;
    }
    var editors = {};
    for (var i in row) {
        var tr = document.createElement('tr');
        var tdlabel = document.createElement('td');
        tdlabel.innerHTML = i;
        tdlabel.className = 'label';
        tr.appendChild(tdlabel);
        var tdedit = document.createElement('td');
        var edt = null;

        var val = row[i];
        if (c.editor && c.editor[i] && c.editor[i].creator) {
            var f = c.editor[i].creator;
            edt = f.call(c.editor[i], c.editor, row, i);
            editors[i] = edt;
            edt.$editors$ = editors;
            edt.setval(val);
            tdedit.appendChild(edt);
        } else {
            if (i.toString().toLowerCase() != "showstatic") {
                edt = definputcreator(editors, row, i);
                edt.$editors$ = editors;
                edt.setval(val);
                tdedit.appendChild(edt);
            }
            else {
                for (var j = 1; j >= 0; j--) {
                    edt = definputcreatorToRadio(editors, row, i, val, j);
                    edt.$editors$ = editors;
                    //edt.setval(val);
                    tdedit.appendChild(edt);
                    var radiotext = (j == 0 ? "否" : "是");
                    edt.insertAdjacentHTML("afterEnd", radiotext);
                }
            }
        }
        if (edt && c.editor && c.editor[i]) {
            joy.extend(edt, c.editor[i])
            edt.validators = c.editor[i].validators;
        }
        edt.validate = function () {
            var list = this.validators;
            if (list) {
                var r = true;
                if (list.length) {
                    for (var i = 0; i < list.length; i++) {
                        var it = list[i];
                        r = r && it(this.getval(), this);
                        if (!r) {
                            break;
                        }
                    }
                } else {
                    for (var i in list) {
                        var f = list[i];
                        if (typeof (f) == 'function') {
                            r = r && f(this.getval());
                            if (!r) {
                                break;
                            }
                        }
                    }
                }
                return r;
            }
            return true;
        };

        if (c.fields && c.fields[i]) {
            if (c.fields[i].hint) {
                var hint = document.createElement('span');
                hint.className = 'hint';
                hint.innerHTML = trigger(c.fields[i].hint);
                tdedit.appendChild(hint);
            }
        }

        tr.appendChild(tdedit);
        tbody.appendChild(tr);
        if (c.fields && c.fields[i]) {
            if (i == 'vin') {
            }
            joy.extend(tr, c.fields[i]);
        }
    }
    div.$$editors = editors;
    div.$table = table;
    div.$form.appendChild(table);
    return div;
}
