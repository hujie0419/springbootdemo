/**
 * 获取子列表
 * @param {string|number} id 子列表的id
 * @returns {Array}
 */
export function getChildList() {
    let data = [[{
        nameText: '搜索页',
        valid: {
            required: true
        },
        formControl: getSearchList()
    }], [{
        nameText: '需要用户信息',
        valid: {
            required: true
        },
        formControl: [{
            nameText: '地理位置',
            showNameText: true,
            controlName: 'IsGeographical',
            type: 'radio',
            defaultValue: false,
            valid: {
                isChecked: true
            },
            list: [{
                nameText: '不要求',
                value: false
            }, {
                nameText: '要调用',
                value: true
            }]
        }, {
            nameText: '用户登录状态',
            showNameText: true,
            controlName: 'IsLogin',
            type: 'radio',
            defaultValue: false,
            valid: {
                isChecked: true
            },
            list: [{
                nameText: '不要求',
                value: false
            }, {
                nameText: '强制登录',
                value: true
            }]
        }, {
            nameText: '获取微信账户信息',
            showNameText: true,
            controlName: 'IsWeChatInfo',
            type: 'radio',
            defaultValue: false,
            valid: {
                isChecked: true
            },
            list: [{
                nameText: '不获取',
                value: false
            }, {
                nameText: '获取',
                value: true
            }]
        }]
    }], [{
        nameText: '分享参数',
        valid: {
            required: true
        },
        formControl: getShareList()
    }]];

    return data || [];
}

/**
 * 分享的配置参数
 *
 * @returns {Array}
 */
export function getShareList() {
    return [{
        nameText: '',
        controlName: 'IsShare',
        type: 'radio',
        defaultValue: true,
        valid: {
            isChecked: true
        },
        list: [{
            nameText: '关闭分享',
            value: false
        }, {
            nameText: '开启分享',
            value: true
        }]
    }, {
        showNameText: true,
        nameText: '标题',
        controlName: 'ShareTitle',
        valid: {
            required: true,
            maxLength: 50,
            maxLengthErr: '标题最多50字符!'
        }
    }, {
        showNameText: true,
        nameText: '分享链接',
        controlName: 'ShareLink',
        valid: {
            required: true
        }
    }, {
        showNameText: true,
        nameText: '分享图',
        controlName: 'ShareImgUrl',
        readonly: true,
        valid: {
            required: true,
            requiredErr: '请上传分享图'
        },
        type: 'filetext',
        prefixList: [{
            nameText: '图片',
            className: 'display-img',
            type: 'img'
        }],
        placeholder: '长宽比1:1',
        descList: [{
            action: '',
            type: 'fileButton',
            validFile: {
                type: 'Image',
                // extension: ['png', 'jpg', 'jpeg', 'gif']
                // limitSize: 10245,
                // limitWidth: 1080
                // limitHeight: 1080,
                limitScale: 1/1 // 比例

                // typeErrorMsg: '请上传正确文件格式',
                // sizeErrorMsg: '文件大小过大',
                // widthErrorMsg: '文件宽度过大',
                // heightErrorMsg: '文件高度过大'
            },
            nameText: '上传图片'
        }]
    }, {
        showNameText: true,
        nameText: '描述',
        controlName: 'ShareDescribe',
        valid: {
            required: true,
            maxLength: 80,
            maxLengthErr: '描述最多80字符!'
        }
    }, {
        showNameText: true,
        nameText: '小程序链接',
        controlName: 'ShareSmallLink',
        valid: {
            required: true
        }
    }, {
        showNameText: true,
        nameText: '小程序分享图',
        controlName: 'ShareSmallImgUrl',
        readonly: true,
        valid: {
            required: true,
            requiredErr: '请上传小程序分享图'
        },
        type: 'filetext',
        prefixList: [{
            nameText: '图片',
            className: 'display-img',
            type: 'img'
        }],
        placeholder: '长宽比5:4',
        descList: [{
            action: '',
            type: 'fileButton',
            validFile: {
                type: 'Image',
                // extension: ['png', 'jpg', 'jpeg', 'gif']
                // limitSize: 10245,
                // limitWidth: 1080
                // limitHeight: 1080,
                limitScale: 5/4 // 比例

                // typeErrorMsg: '请上传正确文件格式',
                // sizeErrorMsg: '文件大小过大',
                // widthErrorMsg: '文件宽度过大',
                // heightErrorMsg: '文件高度过大'
            },
            nameText: '上传图片'
        }]
    }, {
        showNameText: true,
        nameText: '分享类型',
        controlName: 'ShareType',
        type: 'select',
        list: [{
            nameText: '分享为小程序',
            value: 'MINIPROGRAM'
        }, {
            nameText: '分享为H5',
            value: 'H5'
        }],
        valid: {
            required: true
        }
    }];
}

/**
 * 搜索页的配置
 * @return {Array}
 */
export function getSearchList() {
    return [{
        nameText: '',
        controlName: 'IsSearchPage',
        type: 'radio',
        defaultValue: false,
        valid: {
            isChecked: true
        },
        list: [{
            nameText: '不展示',
            value: false
        }, {
            nameText: '展示',
            value: true
        }]
    },
    {
        showNameText: true,
        nameText: '搜索关键字',
        controlName: 'SearchKeyword',
        valid: {
            required: true,
            maxLength: 50
            // maxLengthErr: '搜索关键字最多50字符!'
        },
        // className: 'form-control-small',
        descList: [{
            // nameText: '用小写(半角)分号分隔关键词'
            nameText: '用小写（半角）分号分隔，示列：;轮胎;;车品;'
        }]
    }, {
        showNameText: true,
        nameText: '搜索结果图',
        controlName: 'SearchImgUrl',
        readonly: true,
        valid: {
            required: true,
            requiredErr: '请上传搜索结果图'
        },
        type: 'filetext',
        className: 'display-img',
        prefixList: [{
            nameText: '图片',
            className: 'display-img',
            type: 'img'
        }],
        descList: [{
            action: '',
            type: 'fileButton',
            validFile: {
                type: 'Image',
                // extension: ['png', 'jpg', 'jpeg', 'gif']
                // limitSize: 10245,
                // limitWidth: 1080,
                // limitHeight: 360,
                limitMinWidth: 1080,
                limitMaxWidth: 1080,
                limitMinHeight: 360,
                limitMaxHeight: 360
                // limitScale: 1/2, // 比例

                // typeErrorMsg: '请上传正确文件格式',
                // sizeErrorMsg: '文件大小过大',
                // widthErrorMsg: '文件宽度过大',
                // heightErrorMsg: '文件高度过大'
            },
            nameText: '上传图片'
            // readonly: true
        }, {
            nameText: '限1080*360'
        }]
    }];
}
