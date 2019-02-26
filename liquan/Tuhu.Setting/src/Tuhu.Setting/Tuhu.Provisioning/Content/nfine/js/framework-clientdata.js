var clients = [];
$(function () {
    clients = $.clientsInit();
})
$.clientsInit = function () {
    var dataJson = {
        dataItems: [],
        organize: [],
        role: [],
        duty: [],
        user: [],
        authorizeMenu: [],
        authorizeButton: []
    };
    var init = function () {
        $.ajax({
            url: "/Activity/ActivityUnity/GetClientsDataJson",
            type: "get",
            dataType: "json",
            async: false,
            success: function (data) {
                dataJson.dataItems = null//; data.dataItems;
                dataJson.organize = null;// data.organize;
                dataJson.role = null;// data.role;
                dataJson.duty = null;//data.duty;
                dataJson.authorizeMenu = data;
                dataJson.authorizeButton = null; //data.authorizeButton;
            }
        });

    }
    init();
    return dataJson;
}