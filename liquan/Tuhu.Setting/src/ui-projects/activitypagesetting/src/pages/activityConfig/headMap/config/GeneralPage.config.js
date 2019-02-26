/**
 *
 *顶部表单
 * @export
 * @returns {Array}
 */
export function formConfig() {
    let formConfig= [{
        nameText: '图片',
        controlName: 'GeneralImgUrl',
        type: 'fileText',
        prefixList: [{
            nameText: '图片',
            type: 'img'
        }],
        readonly: true,
        valid: {
            required: true,
            requiredErr: '请上传图片'
        },
        descList: [{
            action: '',
            type: 'fileButton',
            validFile: {
                type: 'Image',
                // extension: ['png', 'jpg', 'jpeg', 'gif'],
                // limitSize: 10245,
                // limitWidth: 1080,
                limitMaxWidth: 1080,
                limitMinWidth: 1080
                // limitHeight: 1080,
                // limitScale: 1/2, // 比例

                // typeErrorMsg: '请上传正确文件格式',
                // sizeErrorMsg: '文件大小过大',
                // widthErrorMsg: '文件宽度过大',
                // heightErrorMsg: '文件高度过大'
            },
            nameText: '上传图片'
        }, {
            nameText: '限1080*x'
        }]
    }, {
        nameText: 'AE动效',
        controlName: 'GeneralDynamic',
        type: 'fileText',
        readonly: true,
        descList: [{
            action: '',
            type: 'fileButton',
            nameText: '上传AE图片',
            validFile: {
                type: 'AE',
                extension: ['zip']
            }
        }, {
            nameText: '（每个页面最多只支持1个动图）'
        }]
    }, {
        nameText: '浮层标题',
        controlName: 'GeneralTitle',
        type: 'text',
        placeholder: '例如"活动规则“、”中奖名单“',
        valid: {
            maxLength: 20
        },
        descList: [{
            nameText: '（限20个字）'
        }]
    }, {
        nameText: '浮层文案',
        type: 'textarea',
        className: 'form-control-large',
        controlName: 'GeneralCopywriting'
    }];
    return formConfig;
}
