/* eslint-disable max-lines */
/**
 * 添加导航配置
 *
 * @export
 * @returns {Array}
 */
export function getNavConfig() {
    return [{
        nameText: '导航开关',
        controlName: 'IsNavigation',
        type: 'radio',
        defaultValue: false,
        valid: {
            required: true
        },
        list: [{
            nameText: '关',
            value: false
        }, {
            nameText: '开',
            value: true
        }]
    }, {
        nameText: '导航颜色',
        controlName: 'NavigationColorStart',
        type: 'text',
        placeholder: '开始值（6位RGB码）',
        valid: {
            required: false,
            format: 'vColor'
        },
        prefixList: [{
            type: 'color',
            popperClass: 'windwoPop-select',
            nameText: ''
        }]
    }, {
        nameText: '',
        controlName: 'NavigationColorEnd',
        type: 'text',
        placeholder: '结束值（6位RGB码）',
        valid: {
            required: false,
            format: 'vColor'
        },
        prefixList: [{
            type: 'color',
            popperClass: 'windwoPop-select',
            nameText: ''
        }]
    }];
}
/**
 * 获取子列表
 * @param {string|number} id 子列表的id
 * @returns {Array}
 */
export function getChildList(id) {
    let data = {
        '2': [{
            nameText: '导航名称',
            controlName: 'NavigationName',
            className: 'input-large',
            type: 'text',
            valid: {
                required: false
            }
        }, {
            nameText: '导航栏描述',
            controlName: 'NavigationDescription',
            className: 'input-large',
            type: 'textarea',
            placeholder: '用#车型#、#轮胎#、#轮毂#表示；示列句：你的爱车：#车型#，适配规格为：#轮胎#',
            valid: {
                required: false
            }
        }],
        '3': [{
            nameText: '配置类型',
            className: 'pztype',
            controlName: 'pztype',
            valid: {
                required: true
            },
            formControl: [{
                nameText: '配置类型',
                controlName: 'pztype1',
                className: 'pztype',
                type: 'select',
                placeholder: '------请选择------',
                popperClass: 'windwoPop-select',
                valid: {
                    isChecked: true
                },
                list: [{
                    nameText: '头图',
                    value: '1'
                }, {
                    nameText: '图片/链接',
                    value: '2'
                }]
            }, {
                nameText: '配置类型',
                popperClass: 'windwoPop-select',
                valid: {
                    isChecked: true
                },
                controlName: 'pztype2',
                className: 'pztype',
                placeholder: '------请选择------',
                type: 'select',
                list: [{
                    nameText: '通用活动页',
                    value: '1'
                }, {
                    nameText: '分车型活动页',
                    value: '2'
                }]
            }]
        }],
        '5': [{
            nameText: '显示适配标签',
            controlName: 'AdapterLabel',
            className: 'fitlabel',
            type: 'radio',
            defaultValue: 'DontFit',
            valid: {
                required: true
            },
            list: [{
                nameText: '不适配',
                value: 'DontFit'
            }, {
                nameText: '适配全部（轮胎+轮毂）',
                value: 'AllFit'
            }, {
                nameText: '适配轮胎',
                value: 'Tire'
            }, {
                nameText: '适配轮毂',
                value: 'WheelHub'
            }]
        }, {
            nameText: '背景填充颜色',
            controlName: 'AdapterBackColor',
            type: 'text',
            defaultValue: '#DF3348',
            placeholder: '6位RGB码',
            valid: {
                required: true,
                format: 'vColor'
            }
        }, {
            nameText: '字体颜色',
            controlName: 'FontColor',
            type: 'text',
            defaultValue: '#FFFFFF',
            placeholder: '6位RGB码',
            valid: {
                required: true,
                format: 'vColor'
            }
        }, {
            nameText: '分割线',
            valid: {
                isChecked: true
            },
            formControl: [{
                nameText: '分割线',
                controlName: '_IsDividingLine',
                type: 'groupSelect',
                // showNameText: true,
                defaultValue: {
                    select: true,
                    value: '#FFFFFF'
                },
                valid: [{
                    isChecked: true
                }, function(con) {
                    let val = con.value;
                    if (val && val.select === true) {
                        let validate = con.validate;
                        let validFn = validate && validate.generateValidetor({
                            required: true,
                            requiredErr: '请输入分割线颜色',
                            format: 'vColor',
                            formatErr: '分割线颜色格式不正确'
                        });
                        if (validFn instanceof Function) {
                            return validFn(val);
                        }
                    }
                }],
                list: [{
                    nameText: '不显示',
                    controlName: 'Tab1',
                    type: 'radioEmpty',
                    value: false
                }, {
                    nameText: '显示，颜色',
                    controlName: 'DividingLineColor',
                    placeholder: '6位RGB码',
                    className: 'colorFit',
                    type: 'radioText',
                    value: true,
                    valid: {
                        required: true
                    }
                }]
            }]
        }, {
            nameText: '无车型提示语',
            defaultValue: '+添加我的爱车',
            controlName: 'NoVehiclePrompt',
            type: 'text',
            placeholder: '+添加我的爱车',
            valid: {
                required: true
            }
        }, {
            nameText: '无规格提示语',
            defaultValue: '请选择规格',
            controlName: 'NoSpecificationTone',
            type: 'text',
            placeholder: '请选择规格',
            valid: {
                required: true
            }
        }]
    };

    return (typeof id!=='undefined' && data[id]) || [];
}

/**
 *
 *获取一级、二级模块类型
 * @export
 * @param {Object} moduleList 返回的所有类型的字典对象
 * @param {strubg} priCode 类型
 * @returns {Array}
 */
export function getDicList(moduleList, priCode = '') {
    let itemprimary = [];
    let itemsecond = [];

    if (moduleList) {
        moduleList.map(item => {
            itemprimary.push({
                nameText: item.DictName,
                value: item.DictCode
            });
        });

        let temp = null;

        temp = moduleList.filter(item => {
            return item.DictCode === priCode;
        });
        if (temp.length >= 1) {
            itemsecond = temp[0].SecondaryModuleList.map((item) => {
                return {
                    nameText: item.DictName,
                    value: item.DictCode
                };
            });
        }
    }

    let item = [{
        nameText: '配置类型',
        className: 'pztype',
        controlName: 'pztype',
        valid: {
            required: true
        },
        formControl: [{
            nameText: '配置类型',
            controlName: 'PrimaryModule',
            className: 'pztype',
            type: 'select',
            placeholder: '------请选择------',
            popperClass: 'windwoPop-select',
            validFlowTime: 20,
            valid: {
                isChecked: true
            },
            list: itemprimary
        }, {
            nameText: '配置类型',
            popperClass: 'windwoPop-select',
            validFlowTime: 20,
            valid: {
                isChecked: true
            },
            controlName: 'SecondaryModule',
            className: 'pztype',
            placeholder: '------请选择------',
            type: 'select',
            list: itemsecond
        }]
    }];

    return item;
}
