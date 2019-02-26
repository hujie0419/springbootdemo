import apis from '../../../commons/apis/goods/goodsApi';
import Vue from 'vue';
const { $http } = Vue.prototype;

/**
 * 生产请求参数
 * @param {*} params 请求参数
 * @returns {object} 拼接好的请求参数对象
 */
function generateParams (params) {
    return {
        apiServer: 'apiServer',
        isLoading: true,
        data: params
    };
}

/**
 * post 请求方法
 * @param {*} url 请求的url地址
 * @param {*} params 请求的参数
 * @returns {Promise} 返回结果的promise对象
 */
function post (url, params) {
    return $http.post(url, generateParams(params));
}

/**
 * 普通商品数据源查询接口
 * @param {*} params 请求的参数
 * @returns {Promise} 返回结果的promise对象
 */
export function getProducts (params) {
    return post(apis.GetGeneralProducts, params);
}

/**
 * 根据商品类目查询品牌集合接口
 * @param {*} params 请求的参数
 * @returns {Promise} 返回结果的promise对象
 */
export function getBrandsByCategory (params) {
    return post(apis.GetProductBrandsByCategory, params);
}

/**
 * 编辑关联商品组号接口
 * @param {*} params 请求的参数
 * @returns {Promise} 返回结果的promise对象
 */
export function editProductSortGroupId (params) {
    return post(apis.EditGeneralProductSortGroupId, params);
}

/**
 * 普通商品模块关联商品查询接口
 * @param {*} params 请求的参数
 * @returns {Promise} 返回结果的promise对象
 */
export function getProductAssociations (params) {
    return post(apis.GetGeneralProductAssociations, params);
}

/**
 * 普通商品关联保存接口
 * @param {*} params 请求的参数
 * @returns {Promise} 返回结果的promise对象
 */
export function saveProductAssociations (params) {
    return post(apis.SaveGeneralProductAssociations, params);
}
