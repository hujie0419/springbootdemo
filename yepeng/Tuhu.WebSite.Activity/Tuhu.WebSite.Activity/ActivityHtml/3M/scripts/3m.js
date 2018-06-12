
$(document).bind("tap", function (e) {
    var target = $(e.target);
    if (target.closest(".select").length == 0) {
        $(".select").removeClass('active');
    }
});
