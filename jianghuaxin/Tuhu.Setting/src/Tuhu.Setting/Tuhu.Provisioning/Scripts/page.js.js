var $groupCurNum=1;var $groupSize=25;var $num=0;$pageSize=0;$total=0;$pageIndex=1;$groupNums=0;var $id="div.pager";$page=$.extend({InitPage:function(args){$($id).find("*").remove().empty();var prevGroup="<a href=\"#\" id=\"prevGroup\" onclick=\"prevClick();return false;\" style=\"display:none;\">上一组</a>";$($id).append(prevGroup);var nextGroup="<a href=\"#\" id=\"nextGroup\" onclick=\"nextClick();return false;\" style=\"display:none;\">下一组</a>";$($id).append(nextGroup);$total=args.Total;$pageSize=args.PageSize;$groupSize=args.GroupSize;$num=Math.floor($total/$pageSize)+($total%$pageSize>0?1:0);var turnPage="<a href=\"#\" id=\"turnGroup\" onclick=\"turnClick();return false;\">转到页</a><input type=\"text\" id=\"txtTurn\" value=\"1\" style=\"width:30px;\" /><span style=\"width:90px;border:0;\">当前页/总页：</span><span id=\"_numInfo\" style=\"width:80px;border:0;text-align:left;\">"+$pageIndex+"/"+$num+"</span>";$($id).append(turnPage);pageOnPage();}});function pageOnPage(){$("a[name='pageA']").remove();$groupNums=Math.floor($num/$groupSize)+($num%$groupSize>0?1:0);if($groupNums==1){$("#prevGroup").css("display","none");$("#nextGroup").css("display","none");}
else if($groupCurNum==1&&$groupNums>1){$("#prevGroup").css("display","none");$("#nextGroup").css("display","");}
else if($groupCurNum<$groupNums&&$groupNums>1){$("#prevGroup").css("display","");$("#nextGroup").css("display","");}
else{$("#prevGroup").css("display","");$("#nextGroup").css("display","none");}
var start=($groupCurNum-1)*$groupSize;var end=($groupCurNum*$groupSize>$num?$num:$groupCurNum*$groupSize);var temp=""
for(var i=start;i<end;i++){if(i==($pageIndex-1))
temp+="<a name=\"pageA\" id=\"_"+(i+1).toString()+"\" href=\"javascript:;\" class=\"current\" onclick=\"pageClick(this,"+(i+1).toString()+");return false;\">"+(i+1).toString()+"</a>";else
temp+="<a name=\"pageA\" id=\"_"+(i+1).toString()+"\" href=\"javascript:;\" onclick=\"pageClick(this,"+(i+1).toString()+");return false;\">"+(i+1).toString()+"</a>";}
$("div.pager a").eq(0).after(temp);}
function prevClick(){$groupCurNum-=1;pageOnPage();}
function nextClick(){$groupCurNum+=1;pageOnPage();}
function turnClick(){var rex=/^[0-9]*$/;if(!rex.test($("#txtTurn").val()))
return;$pageIndex=parseInt($("#txtTurn").val());$pageIndex=($pageIndex>$num?$num:$pageIndex);$("#txtTurn").val($pageIndex);$groupCurNum=Math.floor($pageIndex/$groupSize)+($pageIndex%$groupSize>0?1:0);pageOnPage();pageClick($("#_"+$pageIndex.toString()),$pageIndex);getNumShortInfo();}
function pageClick(e,inum){$("a[name='pageA']").removeClass("current");$(e).addClass("current");$pageIndex=inum;callbackClick($pageIndex);getNumShortInfo();}
function getNumShortInfo()
{var nInfo=$pageIndex+"/"+$num;$("#_numInfo").text(nInfo);return nInfo;}
var callbackClick=function(curNum){return;};