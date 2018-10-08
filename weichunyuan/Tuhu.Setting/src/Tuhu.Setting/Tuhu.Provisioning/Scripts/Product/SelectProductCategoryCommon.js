//产品类别公共类选择窗体
var ProductCategoryCommon = {
    OpenComonPage: function (ids,callback) {
       var index= layer.open({
            type: 2,
           area: ['450px', '600px'],
           btn: ['确定', '取消'],
           success: function (layero, index) {
               var iframeWin = window[layero.find('iframe')[0]['name']];
               iframeWin.zTreeSetting.SetNodesCheckedValue(ids);
           },
           yes: function (index, layero) {
               var iframeWin = window[layero.find('iframe')[0]['name']];
               var checkNodes = iframeWin.zTreeSetting.GetCheckedItems();
               if (!checkNodes || checkNodes.length <= 0) {
                   layer.msg('请选择产品类别');
                   return;
               }
               if (checkNodes.length > 10) {
                   layer.msg('最多只能选择10个产品');
                   return;
               }
               if (callback && typeof callback === "function") {
                   callback(checkNodes);
               }
               layer.close(index);
           },
           cancel: function (index, layero) {
           },
           title:'选择优惠券产品类别',
            content: '/ProductCategory/Index'
        });
    }
};