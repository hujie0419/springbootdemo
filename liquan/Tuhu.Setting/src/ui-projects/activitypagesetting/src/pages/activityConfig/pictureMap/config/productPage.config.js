
import {validPromotionProduct} from '../valid/pictureMapValid';
/**
 * 获取产品表单的配置信息
 * @param {Object} $http xhr
 * @param {Object} that this
 *
 * @returns {Object} 配置信息
 */
export function getProductConfig ($http) {
    return [getProductConfig1(), getItem($http), getItem($http), getItem($http)];
}
/**
 * 获取图片配置
 *
 * @export
 * @returns {Array}}}
 */
export function getProductConfig1 () {
    return [{
        nameText: '图片',
        controlName: 'ImageLinkProductUrl',
        readonly: true,
        valid: {
            required: true,
            requiredErr: '请上传图片'
        },
        type: 'filetext',
        prefixList: [{
            nameText: '图片',
            type: 'img'
        }],
        descList: [{
            action: '',
            type: 'fileButton',
            validFile: {
                type: 'Image',
                limitWidth: 255,
                limitHeight: 255
            },
            nameText: '上传图片'
        }, {
            nameText: '限255*255'
        }]
    }];
}

/**
 * 获取formList数据
 * @param {Object} $http xhr
 * @return {Array}
 */
function getItem($http) {
    return [{
        nameText: '商品配置',
        formControl: [{
            showNameText: true,
            nameText: 'PID',
            controlName: 'LinkPid',
            validFlowTime: 1000,
            valid: [{
                required: true
            }, (con) => {
                return validPromotionProduct(con, $http);
            }],
            className: 'form-control-small'
        }, {
            showNameText: true,
            nameText: '促销ID',
            controlName: 'LinkPromotionType',
            className: 'form-control-small',
            type: 'select',
            defaultValue: '',
            validFlowTime: 1000,
            valid: [(con) => {
                return validPromotionProduct(con, $http);
            }],
            list: [{
                nameText: '----请选择----',
                value: ''
            }, {
                nameText: '全网活动ID',
                value: 'Whole'
            }, {
                nameText: '限时ID',
                value: 'Limit'
            }, {
                nameText: '打折ID',
                value: 'Discount'
            }]
        }, {
            nameText: 'ID',
            controlName: 'LinkPromotionId',
            validFlowTime: 1000,
            valid: [(con) => {
                return validPromotionProduct(con, $http);
            }]
        }, {
            showNameText: true,
            nameText: '价格',
            disabled: true,
            controlName: 'LinkCommodityPrice',
            className: 'form-control-small'
        }]
    }];
}
