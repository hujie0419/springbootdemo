/**
 * 模板配置文件
 *
 * @export
 * @returns {Array}
 */
export function templateConfigFormconfig() {
    return [{
        nameText: 'PC背景图',
        controlName: 'PcBackImgUrl',
        type: 'filetext',
        prefixList: [{
            nameText: '图片',
            type: 'img'
        }],
        readonly: true,
        descList: [{
            action: '',
            type: 'fileButton',
            validFile: {
                type: 'Image',
                // extension: ['png', 'jpg', 'jpeg', 'gif']
                // limitSize: 10245,
                // limitWidth: 1920,
                limitMaxWidth: 1920,
                limitMinWidth: 1920
                // limitHeight: 1080,
                // limitScale: 1/2, // 比例

                // typeErrorMsg: '请上传正确文件格式',
                // sizeErrorMsg: '文件大小过大',
                // widthErrorMsg: '文件宽度过大',
                // heightErrorMsg: '文件高度过大'
            },
            nameText: '上传图片'
        }, {
            nameText: '限1920*x'
        }]
    }, {
        nameText: '背景填充颜色',
        controlName: 'BackColor',
        placeholder: '6位RGB码',
        className: 'form-control-small',
        valid: {
            format: 'vColor'
        },
        prefixList: [{
            type: 'color',
            nameText: '颜色'
        }]
    }, {
        nameText: '车型适配',
        type: 'groupSelect',
        controlName: 'groupSelect',
        defaultValue: [],
        list: [{
            nameText: '适配弹窗',
            className: 'form-control-small',
            type: 'checkBoxSelect',
            value: 'car',
            defaultValue: 'Two',
            list: [{
                nameText: '二级车型',
                value: 'Two'
            }, {
                nameText: '五级车型',
                value: 'Five'
            }]
        }, {
            nameText: '顶部适配栏　',
            type: 'checkBoxEmpty',
            value: 'column',
            descList: [{
                nameText: '修改',
                id: 'columnModify',
                type: 'link',
                status: 'column'
            }]
        }]
    }];
}
