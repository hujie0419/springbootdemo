function getClientString() {
    var txt = '';
    var url = location.href;
    var lst = url.split('#');
    if (lst.length > 1) {
        txt = lst[1];
    }
    return txt;
}