var $RewardGrid = $('#gridRewardList');

var Grid = {
    init: function () {
        //初始化抽奖配置列表
        $RewardGrid.dataGrid({
            height: $(window).height() - 138,
            url: "/BigBrand/BigBrandReward/GetGridJson",
            colModel: [
                { label: "PKID", name: "PKID", width: 120, align: "center", key: true },
                { label: "名 称", name: "Title", width: 260, align: 'left' },
                { label: "抽奖次数", name: "PreTimes", width: 260, align: 'left' },
                 { label: "抽奖次数(分享后)", name: "CompletedTimes", width: 260, align: 'left' },
                 { label: "积分抽奖次数", name: "IntegralTimes", width: 260, align: 'left' },
                 { label: "单次抽奖消耗积分", name: "NeedIntegral", width: 120, align: 'left' },
                 {
                     label: "抽奖类型", name: "BigBrandType", width: 100, align: 'center',
                     formatter: function (cellValue) {
                         if (cellValue == 1) {
                             return "普通抽奖"
                         } else if (cellValue == 2) {
                             return "积分抽奖";
                         } else if (cellValue == 3) {
                             return "定人群";
                         } else
                             return "";

                     }
                 },
                {
                    label: "抽奖周期", name: "Period", width: 360, align: "center"
                },
                {
                    label: "创建时间", name: "CreateDateTime", width: 120, align: "center",
                    formatter: "date", formatoptions: { srcformat: 'Y-m-d H:i:s', newformat: 'Y-m-d H:i:s' }
                },
                {
                    label: "更新时间", name: "LastUpdateDateTime", width: 120, align: "center",
                    formatter: "date", formatoptions: { srcformat: 'Y-m-d H:i:s', newformat: 'Y-m-d H:i:s' }
                }
            ],
            viewrecords: true,
            pager: "#rewardPager",
            sortname: "PKID desc,CreateDateTime desc",
            rowNum: 20,
            rowList: [10, 20, 30],
        });
    },
    initBtn: function () {
        $('#btnAddReward').on('click', function () {
            $.modalOpen({
                id: 'RewardForm',
                url: "/BigBrand/BigBrandReward/Form?keyValue=",
                title: "新增抽奖配置",
                width: "600px",
                height: "520px",
                callBack: function (iframeId) {
                    var iframe = $.currentWindow();
                    top.frames[iframeId].submitForm(iframe);
                }
            });
        });
    }
};

$(function () {
    Grid.init();
    Grid.initBtn();
});