import Vue from 'vue';
import Vuex from 'vuex';
import { StorageClient, FormModule } from 'tiger-lib';

import App from './App';
import { Popup } from 'tiger-ui';
// import Mixin from './mixin.js'
import routerFactory from './config/routes';
import store from './store';
import { guardFactory } from './guard/guard';
import { envFactory } from './common/envService/EnvService';
import { httpFactory } from './common/httpClient/httpClient';
import { titleFactory } from 'common/title/Title';
import filterResponseCode from './common/filter/filterResponseCode.filter';

import NProgress from 'nprogress';
import 'nprogress/nprogress.css';

import ElementUI, { Loading } from 'element-ui';
import 'element-ui/lib/theme-chalk/index.css';
import 'assets/css/index.scss';

import babelpolyfill from 'babel-polyfill';

import components from './components';
import { filterStr } from './common/utils/utils';

let $storage = StorageClient();
let $env = envFactory(); // 环境变量
let $title = titleFactory({ // 设置页面标题
    title: $env.systemName
});
// 请求的loading配置
let loading; // 加载框
let loadingtimeout; // 加载定时
let loadingArr = []; // 加载队列
let errorMsg = (desc) => {
    let _that = Vue.prototype;
    _that.$message.error({
        message: desc||'出错了',
        center: true
    });
};

let $$form = FormModule({
    validate: {
        rules: {
            'vColor': { // 校验颜色
                reg: /^#([0-9a-f]{3}){1,2}$/gi,
                infoTxt: '格式不正确'
            },
            'vNumber': { // 校验大于0的整数
                reg: /^[1-9]{1}[0-9]*$/gi,
                infoTxt: '只能为大于0的整数'
            }
        }
    }
});

Vue.prototype.$$form = $$form;
Vue.prototype.$$validMsg = (formModel) => {
    let _that = Vue.prototype;
    let valid = $$form.validForm(formModel, (desc) => {
        _that.$$errorMsg(desc||'出错了');
    });
    if (!valid && formModel && formModel.isSend) {
        valid = true;
    }
    return valid;
};
Vue.prototype.$$errorMsg = (desc) => {
    let _that = Vue.prototype;
    _that.$message.error({
        message: desc||'出错了',
        center: true
    });
};
Vue.prototype.$$confirm = (text, param) => {
    let _that = Vue.prototype;
    param = Object.assign({
        title: '提示',
        confirmButtonText: '确认',
        cancelButtonText: '取消',
        type: 'warning'
    }, param);
    return _that.$confirm(text, param.title, param);
};
Vue.prototype.$$saveMsg = (msg, param) => {
    let _that = Vue.prototype;
    _that.$message(Object.assign({
        message: msg || '保存成功',
        center: true
    }, param));
};
let $http = httpFactory({
    isErrorData: true
}, {
    beforeSend(options) { // 开始请求前
        if (options && options.isLoading) {
            loadingArr.push(1);
            if (loadingArr.length === 1) {
                if (typeof loadingtimeout !== 'undefined') {
                    clearTimeout(loadingtimeout);
                    loadingtimeout = undefined;
                }
                loadingtimeout = setTimeout(() => {
                    loadingtimeout = undefined;
                    loading= Loading.service({
                        lock: true,
                        text: '数据加载中，请稍候。。。',
                        // spinner: 'el-icon-loading',
                        background: 'rgba(0, 0, 0, 0.1)'
                    });
                }, 200);
            }
        }
        if (options && options.data) {
            options.data = filterStr(options.data);
        }
        if (options && options.params) {
            options.params = filterStr(options.params);
        }
        return options;
    },
    onHttpError(err, url) { // 请求出错
        if (err) {
            let errInfo = 'err：' + err.message;
            if (err.response && err.response.status) {
                errInfo = err.message + '：\n' + url;
            }
            errorMsg(errInfo);
        }
    },
    onServiceError(data, options) { // 业务出错
        let _data = (data && data.data) || data;
        let _rMessage = _data&& _data.ResponseMessage;
        if (filterResponseCode(_data)) { // 不处理的错误码
            return data;
        } else {
            errorMsg(_rMessage);
            if (!options.isErrorData) {
                return data;
            }
        }
    },
    afterSend(options, back) { // 请求结束后
        if (options && options.isLoading) {
            loadingArr.pop();
            if (loadingArr.length <= 0) {
                if (typeof loadingtimeout !== 'undefined') {
                    clearTimeout(loadingtimeout);
                    loadingtimeout = undefined;
                }
                loading && loading.close();
                loading = undefined;
            }
        }
        if (back && back.data) {
            back.data = filterNullValue(back.data);
        }
        return back;
    }
}, $env, $storage);

/**
 * 过滤空值字段
 * @param {Object} data 需要过滤的数据
 * @return {Object}
 */
function filterNullValue (data) {
    if (data instanceof Object) {
        for (const key in data) {
            if (data.hasOwnProperty(key)) {
                const item = data[key];
                if (item === null || item === undefined) {
                    delete data[key];
                } else if ((item instanceof Object) && !(item instanceof Array)) {
                    data[key] = filterNullValue(item);
                }
            }
        }
    }
    return data;
}
// // 添加请求拦截器
// $http.axios.interceptors.request.use(function (config) {
// // 在发送请求之前做些什么
//     return config;
// }, function (error) {
// // 对请求错误做些什么
//     return Promise.reject(error);
// });

// // 添加响应拦截器
// $http.axios.interceptors.response.use(function (response) {
// // 对响应数据做点什么
//     return response;
// }, function (error) {
// // 对响应错误做点什么
//     return Promise.reject(error);
// });

Vue.use(Vuex);
Vue.use(Popup);
NProgress.configure({ showSpinner: false });
Vue.use(ElementUI);
Vue.use(babelpolyfill);
// debugger;
Vue.use(components);

/**
 * 跳转到登录页面
 *
 */
function redirect() {
    let host = location.hostname.replace('yunying', 'yewu').replace('setting', 'yewu');
    host = host.replace(/(localhost|(192|172)\.[\d.]+):?\d*/, 'yewu.tuhu.work');
    let curl = location.href.replace(/(localhost|(192|172)\.[\d.]+):?\d*/, 'setting.tuhu.work');
    let url = location.protocol + '//' + host + '/Account/LogOn?ReturnUrl=' + curl;
    window.location.replace(url);
}

// import axios from "axios"
// axios.defaults.withCredentials = true // 上送cookie
Vue.prototype.$env = $env;
Vue.prototype.$http = $http;
Vue.prototype.$filterResponseCode = filterResponseCode;
Vue.prototype.$loginOut = () => {
    $storage.clearAll();
    $storage.clearCookie();
    var host = location.hostname.replace('yunying', 'yewu').replace('setting', 'yewu');
    host = host.replace(/(localhost|(192|172)\.[\d.]+):?\d*/, 'yewu.tuhu.work');
    // let curl = location.href.replace(/(localhost|(192|172)\.[\d.]+):?\d*/, 'setting.tuhu.work');
    let url = location.protocol + '//' + host + '/Account/LogOff'; // ?ReturnUrl=' + curl;

    window.location.href = url;
};

new Vue({
    el: '#app',
    // mixins: [Mixin],
    router: guardFactory(routerFactory($env), $http, store, $title, $env, () => {
        redirect();
    }),
    store,
    render: h => h(App),
    data: {
        eventHub: new Vue()
    }
});
