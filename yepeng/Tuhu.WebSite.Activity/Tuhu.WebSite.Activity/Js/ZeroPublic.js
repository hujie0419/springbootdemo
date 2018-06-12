var Domain = "http://faxian.tuhu.cn";
function openTip(msg) {
    var $tips = $("#tipsContent");
    var $popTips = $("#popTips");
    $tips.text(msg);
    $popTips.show();
    setTimeout(function () {
        $popTips.hide();
    }, 1500);
}