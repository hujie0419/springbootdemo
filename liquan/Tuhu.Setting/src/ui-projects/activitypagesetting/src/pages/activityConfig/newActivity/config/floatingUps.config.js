/**
 * 添加悬浮窗配置
 * @returns {Array}
 */
export function floatingUpsConfig() {
    return [{
        nameText: '悬浮窗',
        controlName: 'FloatingUpsType',
        type: 'radio',
        defaultValue: 'NoFloating',
        valid: {
            required: true
        },
        list: [{
            nameText: '关',
            value: 'NoFloating'
        }, {
            nameText: '开',
            value: 'StaticFloating'
        // }, {
        //     nameText: '静图悬浮窗',
        //     value: 'StaticFloating'
        // }, {
        //     nameText: '动图悬浮窗',
        //     value: 'DynamicFloating'
        }]
    }];
}

/**
 * 添加悬浮窗配置 -> 静图悬浮窗
 * @returns {Array}
 */
export function getStaticFloating() {
    return [{
        nameText: '悬浮窗图片',
        controlName: 'FloatingUpsImgUrl',
        type: 'fileText',
        prefixList: [{
            nameText: '图片',
            type: 'img'
        }],
        placeholder: '限200*200',
        readonly: true,
        valid: {
            required: true,
            requiredErr: '请上传悬浮窗图片'
        },
        descList: [{
            action: '',
            type: 'fileButton',
            validFile: {
                type: 'Image',
                extension: ['png', 'jpg', 'jpeg', 'gif'],
                limitMinWidth: 200,
                limitMaxWidth: 200,
                limitMinHeight: 200,
                limitMaxHeight: 200
            },
            nameText: '上传图片'
        }]
    }, {
        nameText: '悬浮窗跳转',
        controlName: 'FloatingUpsJump',
        type: 'radio',
        defaultValue: 'PopupWin',
        valid: {
            required: false
        },
        list: [{
            nameText: '弹窗',
            value: 'PopupWin'
        }, {
            nameText: '链接',
            value: 'Link'
        }]
    }];
}

/**
 * 悬浮窗跳转选项配置
 * @param {String} type 类型
 * @export
 * @returns {Array}
 */
export function getfloatingUpsJumpConfig(type) {
    let data = {
        PopupWin: [{
            nameText: '弹窗图片',
            controlName: 'PopUpsImgUrl',
            type: 'fileText',
            prefixList: [{
                nameText: '图片',
                type: 'img'
            }],
            placeholder: '限540*710',
            readonly: true,
            valid: {
                required: true,
                requiredErr: '请上传弹窗图片'
            },
            descList: [{
                action: '',
                type: 'fileButton',
                validFile: {
                    type: 'Image',
                    limitMinWidth: 540,
                    limitMaxWidth: 540,

                    limitMinHeight: 710,
                    limitMaxHeight: 710
                },
                nameText: '上传图片'
            }]
        }],
        Link: [{
            nameText: '移动端链接',
            controlName: 'MobileLink',
            className: 'form-large',
            type: 'text',
            placeholder: '',
            valid: {
                required: true
            }
        }, {
            nameText: '小程序链接',
            controlName: 'AppletsLink',
            className: 'form-large',
            type: 'text',
            placeholder: '',
            valid: {
                required: true
            }
        }, {
            nameText: 'App链接',
            controlName: 'AppLink',
            className: 'form-large',
            type: 'text',
            placeholder: ''
        }, {
            nameText: '快应用链接',
            controlName: 'QuickAppLink',
            className: 'form-large',
            type: 'text',
            placeholder: ''
        }]
    };
    return data[type] || [];
}
// /**
//  * 添加悬浮窗配置 -> 动图悬浮窗
//  */
// export function getDynamicFloating() {

// }
