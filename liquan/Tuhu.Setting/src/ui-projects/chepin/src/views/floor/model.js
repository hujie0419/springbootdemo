/*
 * @Author: 肖忠保(肖工) 
 * @Email：xiaozhongbao@tuhu.cn 
 * @Create time: 2018-11-06 14:48:46 
 * @Last Modified by: 肖忠保(肖工)
 * @Last Modified time: 2018-11-20 19:08:03
 */

var floorModel = {
    PKID: 0,
    Name: "",
    Code: "",
    Sort: 0,
    PID1: "",
    PID2: "",
    PID3: "",
    DisplayName: "",
    ThreeCount: [],
    ThreeCategory: [],
    FloorConfig: [],
    IsEnabled: false
}

var nullModel = {
    PKID: 0,
    Name: "",
    Code: "",
    Sort: 0,
    PID1: "",
    PID2: "",
    PID3: "",
    DisplayName: "",
    ThreeCount: [],
    ThreeCategory: [],
    FloorConfig: [],
    IsEnabled: false
}

let adElem = [{
        label: "名称",
        prop: "Name",
        labelWidth: 100,
        placeholder: "请输入名称，不超过20字"
    },
    {
        label: "图片",
        prop: "ImgUrl",
        labelWidth: 100,
        element: "upload",
        value: "",
        width: "width:300px",
        action: "/CarProducts/UploadImage?type=image"
    },
    {
        labelWidth: 100,
        label: "目的页类型",
        prop: "LinkType",
        element: "select",
        option: [{
                label: "H5活动页",
                value: "1"
            },
            {
                label: "车品详情页",
                value: "2"
            },
            {
                label: "搜索结果页",
                value: "3"
            }
        ]
    },
    {
        label: "链接",
        prop: "NoLink",
        labelWidth: 100,
        placeholder: "请输入跳转链接"
    },
    {
        label: "已编码链接",
        prop: "Link",
        disabled: "disabled",
        labelWidth: 100,
        placeholder: "请输入跳转链接"
    },
    {
        label: "开始时间",
        prop: "StartTime",
        labelWidth: 100,
        element: "date",
        type: "datetime",
        elemWidth: "width:300px",
        format: "yyyy-MM-dd HH:mm:ss"
    },
    {
        label: "结束时间",
        prop: "EndTime",
        labelWidth: 100,
        element: "date",
        type: "datetime",
        elemWidth: "width:300px",
        format: "yyyy-MM-dd HH:mm:ss"
    },
    {
        label: "状态",
        size: "large",
        prop: "IsEnabled",
        element: "switch",
        labelWidth: 100,
        option: [{
                label: "启用",
                slot: "open",
                value: true
            },
            {
                label: "禁用",
                slot: "close",
                value: false
            }
        ]
    }
]

let adModel = {
    PKID: 0,
    FKFloorID: 0,
    Name: "",
    ImgUrl: "",
    StarTime: "",
    EndTime: "",
    LinkType: "",
    Link: "",
    NoLink: "",
    Type: 5,
    IsEnabled: false
}

// 自定义排序验证
const validSort = (rule, value, callback) => {
    if (value === "" || value == null) {
        return callback(new Error("排序不可为空"));
    }
    if (!Number.isInteger(value)) {
        return callback(new Error("只支持数字类型"));
    }
    callback();
};
// 自定义状态验证
const validEnabled = (rule, value, callback) => {
    callback();
};

let floorRule = {
    Name: [{
        required: true,
        message: "请选择类目",
        trigger: "change"
    }],
    Sort: [{
        required: true,
        validator: validSort,
        trigger: "blur",
        type: "number"
    }],
    IsEnabled: [{
        required: true,
        validator: validEnabled,
        trigger: "change"
    }]
}

let adRule = {
    Name: [{
        required: true,
        message: "banner名称不能为空",
        trigger: "blur"
    }],
    LinkType: [{
        required: true,
        message: "请选择目的页类型",
        trigger: "change"
    }],
    NoLink: [{
        required: true,
        message: "跳转链接不能为空",
        trigger: "blur"
    }],
    ImgUrl: [{
        required: true,
        message: "图片不能为空",
        trigger: "blur"
    }],
    IsEnabled: [{
        required: true,
        validator: validEnabled,
        trigger: "change"
    }]
}
export {
    floorModel,
    adElem,
    adModel,
    floorRule,
    adRule,
    nullModel
}