//---表单POST
function formSubmit(url,args)
{
    $("#fid1").remove();
    var formHtml = "<form id=\"fid1\" method=\"post\" action=\"" + url + "\">";
    for (var name in args) {
        formHtml += "<input type=\"text\" name=\"" + name + "\" value=\"" + args[name] + "\" style=\"display:none;\" />";
    }
    formHtml += "</form>";
    $(document.body).append(formHtml);
    $("#fid1").submit();
}