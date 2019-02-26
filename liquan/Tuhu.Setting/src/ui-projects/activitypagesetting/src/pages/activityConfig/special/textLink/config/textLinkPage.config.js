/**
 * 获取新的表单配置对象
 * @param {String | Number} index 表单的index
 * @returns {Array}
 */
export function getTextLinkConfig (index) {
    const res = [{
        nameText: '文字链 ' + index,
        controlName: 'ImageUrl',
        placeholder: 'icon规格246*96',
        type: 'fileText',
        valid: {
            required: true
        },
        requiredRight: true,
        readonly: true,
        prefixList: [{
            nameText: '图片',
            type: 'img'
        }],
        descList: [{
            action: '',
            type: 'fileButton',
            validFile: {
                type: 'Image',
                // extension: ['png', 'jpg', 'jpeg', 'gif']
                // limitSize: 10245,
                // limitWidth: 80,
                // limitHeight: 80
                limitMinWidth: 246,
                limitMinHeight: 96,
                limitMaxWidth: 246,
                limitMaxHeight: 96
                // limitScale: 1/2, // 比例

                // typeErrorMsg: '请上传正确文件格式',
                // sizeErrorMsg: '文件大小过大',
                // widthErrorMsg: '文件宽度过大',
                // heightErrorMsg: '文件高度过大'
            },
            nameText: '上传图片'
        }]
    }, {
        nameText: '',
        controlName: 'MobileLink',
        defaultValue: '',
        placeholder: '输入移动端跳转链接',
        requiredRight: true
    }, {
        nameText: '',
        controlName: 'AppletsLink',
        defaultValue: '',
        placeholder: '输入小程序跳转链接',
        requiredRight: true
    }, {
        nameText: '',
        controlName: 'AppLink',
        defaultValue: '',
        placeholder: '输入App跳转链接',
        requiredRight: true
    }, {
        nameText: '',
        controlName: 'QuickAppLink',
        defaultValue: '',
        placeholder: '输入快应用跳转链接',
        requiredRight: true
    }];
    return res;
}
