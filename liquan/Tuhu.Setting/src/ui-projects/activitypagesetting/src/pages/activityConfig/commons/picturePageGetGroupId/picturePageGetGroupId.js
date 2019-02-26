// import apis from '../apis/groupBooking/groupBookingApi.js';
import {getGroupId} from '../groupBookingId/getGroupId';

/**
 * 校验Pid并获取groupId
 * @param {*} formModel formModel
 * @param {*} $http xhr
 * @param {*} formConfig formConfig
 * @param {*} context context
 * @param {Number} index 序号
 * @returns {Promise}
 */
export function picturePageGetGroupId(formModel, $http, formConfig, context, index) {
    return new Promise((resolve, reject) => {
        const pid = formModel.value.LinkPid;
        const oldPid = context.groupPID[index];
        context.groupPID[index] = pid;
        getGroupId($http, pid).then(res => {
            formConfig.formControl[1].list = res.listArray;
            if (oldPid !== pid) {
                formModel.setValue({LinkProductGroupId: res.tempRowData[0].GroupId});
                formModel.setValue({LinkCommodityPrice: res.tempRowData[0].Price});
            }
            context.tempRowData = res.tempRowData;
            resolve(null);
        }).catch(e => {
            context.$$errorMsg(e.message);
            formModel.setValue({LinkProductGroupId: null});
            formModel.setValue({LinkCommodityPrice: null});
            formConfig.formControl[1].list = [];
            context.tempRowData = [];
            resolve({
                errInfo: {
                    desc: e.message
                }
            });
            // const _res = {IsProductId: false, ProductPrice: 0, ResponseCode: '0001', ResponseMessage: e.message, ResponseRow: 0};
            // reject(_res);
        });
    });
}
