/**
 * 获取视频表单的配置信息
 * @param {VM} _that vue实例
 * @returns {array} 表单的配置信息
 */
export function getConfig (_that) {
    return [{
        nameText: '视频类型',
        controlName: 'VideoType',
        type: 'radio',
        defaultValue: 'Horizontal',
        // valid: {
        //     isChecked: true,
        // },
        valid: {
            required: true
        },
        list: [{
            nameText: '横屏',
            value: 'Horizontal'
        }, {
            nameText: '竖屏',
            value: 'Vertical'
        }]
    }, {
        nameText: '移动端链接',
        controlName: 'VideoMobileLink',
        defaultValue: '',
        placeholder: '(包括移动端、App、小程序)',
        valid: {
            required: true
        }
    }, {
        nameText: 'PC链接',
        controlName: 'VideoPcLink',
        defaultValue: '',
        valid: {
            required: true
        }
    }, {
        nameText: '第一帧图片',
        controlName: 'VideoImgUrl',
        defaultValue: '',
        type: 'fileText',
        prefixList: [{
            nameText: '图片',
            type: 'img'
        }],
        readonly: true,
        valid: {
            required: true,
            requiredErr: '请上传第一帧图片'
        },
        descList: [{
            action: '',
            type: 'fileButton',
            orientation: 0, // 方向的角度（0：水平或90：垂直）
            validFile: {
                type: 'Image',
                // extension: ['png', 'jpg', 'jpeg', 'gif']
                // limitSize: 10245,
                // limitWidth: 1920,
                // limitHeight: 1080,
                limitMinWidth: 1920,
                limitMinHeight: 1080,
                limitMaxWidth: 1920,
                limitMaxHeight: 1080
                // limitScale: 1/2, // 比例

                // typeErrorMsg: '请上传正确文件格式',
                // sizeErrorMsg: '文件大小过大',
                // widthErrorMsg: '文件宽度过大',
                // heightErrorMsg: '文件高度过大'
            },
            nameText: '上传图片'
        }, {
            controlName: 'ImgSize',
            nameText: '限1920*1080'
        }]
    }];
}
