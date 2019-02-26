function grid(data, c) {
    if (!c) {
        c = {};
    }
    var table = document.createElement('table');
    var tbody = document.createElement('tbody');
    table.className = 'workspace';
    table.appendChild(tbody);
    var cfgcols = c.columns || {};
    for (var i = 0; i < data.length; i++) {
        var tr = document.createElement('tr');
        var row = data[i];
        for (var j in row) {
            var cj = cfgcols[j];
            if (!cj || !cj.onformat) {
                row[j] = unescape(row[j]);
            } else {
                row[j] = cj.onformat(row[j]);
            }
        }
        if (i == 0) {
            for (var cn in row) {
                var td = document.createElement('td');
                td.className = 'col';
                td.innerHTML = cn;
                if (c.columns && c.columns[cn]) {
                    var colcfg = c.columns[cn];
                    joy.extend(td, colcfg);
                }
                tr.appendChild(td);
            }
            tbody.appendChild(tr);
            if (c.columns && c.columns.$added) {
                c.columns.$added(tr, row);
            }
            tr = document.createElement('tr');
        }

        for (var scol in row) {
            var td = document.createElement('td');
            td.className = 'cell';
            td.innerHTML = row[scol];
            if (c.rows && c.rows[scol]) {
                var colcfg = c.rows[scol];
                joy.extend(td, colcfg);
            } else if (c.columns && c.columns[scol] && c.columns[scol].style) {
                joy.extend(td.style, c.columns[scol].style)
            }
            tr.appendChild(td);
        }
        if (c.rows && c.rows.$added) {
            c.rows.$added(tr, row);
        }
        tbody.appendChild(tr);
    }
    return table;
}
function gridOptionAjaxBehavior(url) {
    var c = this;
    xhr({
        url: url, success: function (data) {
            try{
                var r = JSON.parse(data);
                if (r.IsSuccess) {
                    toastr.info('操作成功');
                } else {
                    toastr.info(r.Msg);
                }
                if (c.callback) {
                    c.callback();
                }   
            } catch (e) {
                console.log(e);
            }
        }
    });
}
function gridOptionFormBehavior(url, data) {
    var c = this;
    var el = overlay();
    var tb = formeditor(data, c.roweditor);
    el.appendChild(tb);
}
function gridoption(data, c) {
    if (!c) {
        c = { url: '#', txt: '' };
    }
    var tag = c.tag || 'td';
    var td = document.createElement(tag);
    td.className = 'opt';
    var upurl = rep(c, data);
    if (!c.behavior) {
        td.innerHTML = '<a href="' + upurl + '">' + c.txt + '</a>';
    } else {
        var a = document.createElement('a');
        a.href = '#';
        a.innerHTML = c.txt;
        a.onclick = function () {
            c.behavior(upurl, data);
            if (c.callback) {
                c.callback();
            }
        }
        td.appendChild(a);
    }
    return td;
}
