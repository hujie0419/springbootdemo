var setting = {};
setting.base = (function () {
    return {
        msg: function (m,c) {
            if (c) {
                layer.msg(m, function (index) {
                    c(index);
                });
            } else {
                layer.msg(m);
            }
        },
        error: function (m,c) {
            if (c) {
                layer.alert(m, {
                    icon: 2,
                    title: '系统提示'
                }, function (index) {
                    c(index);
                });
            } else {
                layer.alert(m, {
                    icon: 2,
                    title: '系统提示'
                });
            }
        },
        right: function (m, c) {
            if (c) {
                layer.alert(m, {
                    icon:1,
                    title: '系统提示'
                }, function (index) {
                    c(index);
                });
            } else {
                layer.alert(m, {
                    icon: 1,
                    title: '系统提示'
                });
            }
        },
        doubt: function (m, c) {
            if (c) {
                layer.alert(m, {
                    icon: 3,
                    title: '系统提示'
                }, function (index) {
                    c(index);
                });
            } else {
                layer.alert(m, {
                    icon: 3,
                    title: '系统提示'
                });
            }
        },
        lock: function (m, c) {
            if (c) {
                layer.alert(m, {
                    icon: 4,
                    title: '系统提示'
                }, function (index) {
                    c(index);
                });
            } else {
                layer.alert(m, {
                    icon: 5,
                    title: '系统提示'
                });
            }
        },
        alert: function (m, c) {
            if (c) {
                layer.alert(m, {
                    icon: 5,
                    title: '系统提示'
                }, function (index) {
                    c(index);
                });
            } else {
                layer.alert(m, {
                    icon: 5,
                    title: '系统提示'
                });
            }
        },
        load: function () {
            return layer.load(2)
        },
        close: function (index) {
            layer.close(index);
        },
        closeAllLoading: function () {
            layer.closeAll('loading'); 
        }
    };
})();