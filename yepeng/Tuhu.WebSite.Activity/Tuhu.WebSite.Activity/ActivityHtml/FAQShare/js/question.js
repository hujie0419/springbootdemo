$(function(){
  for (var i = 0; i < $(".comment").length; i++) {
      var m = $(".comment").eq(i);
      if (m.text().length > 94) {
        //将数据存入content中，实际数据要从数据库读出
          m.attr("content", m.html());
          m.html(m.text().substr(0, 94) + '...'+'<span class="open">查看全部</span>');
      }
  }
  $(".qs-list").on("tap",".open",function(){
    //将数据存入content中，实际数据要从数据库读出
    $(this).parent(".comment").html($(this).parent().attr("content") + '<span class="close"></span>');
  });
  $(".qs-list").on("tap",".close",function(){
    $(this).parent().html($(this).parent().attr("content").substr(0, 94) + '...'+'<span class="open">查看全部</span>');
  });
});