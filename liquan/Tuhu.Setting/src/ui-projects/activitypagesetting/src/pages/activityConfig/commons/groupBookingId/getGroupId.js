import apis from '../apis/groupBooking/groupBookingApi.js';

/**
 * 发起请求获取groupid的option
 *
 * @param {Object} $http xhr
 * @param {string} pid pid
 * @returns {object} Promise
 */
export function getGroupId($http, pid) {
    return new Promise((resolve, reject) => {
    // 校验pid是否合法
        isPid($http, pid).then((IsProductId) => {
            if (!IsProductId) {
                // this.$$errorMsg('PID不正确', {type: 'error'});
                reject(new Error('PID不正确'));
                return false;
            }
            $http.post(apis.GetProductGroupInfoByIds, {
                apiServer: 'apiServer',
                isLoading: true,
                data: {
                    PID: pid
                }
            }).subscribe(res => {
                let _res = res && res.data;
                if (!_res.GroupInfos || _res.GroupInfos.length === 0) {
                    reject(new Error('未找到GroupId'));
                }
                // this.tempRowData = _res.GroupInfos;
                let listArray = [];
                _res.GroupInfos.forEach(item => {
                    listArray.push({
                        nameText: item.GroupId,
                        value: item.GroupId
                    });
                });
                // this.groupIdControlConfig.list = listArray;
                resolve({
                    tempRowData: _res.GroupInfos,
                    listArray: listArray
                });
            });
        });
    });
}

/**
 * 校验pid是否合法
 *
 * @param {Object} $http xhr
 * @param {string} pid pid
 * @returns {object} Promise
 */
function isPid($http, pid) {
    return new Promise((resolve) => {
        $http.post(apis.GetProductPrice, {
            apiServer: 'apiServer',
            isLoading: true,
            data: {
                PID: pid
            }
        }).subscribe(res => {
            let _res = res && res.data;
            resolve(_res.IsProductId);
        });
    });
}
