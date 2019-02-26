import Vue from 'vue';
import Vuex from 'vuex';
import { StorageClient } from 'tiger-lib';

import App from './App';
// import Mixin from './mixin.js';
import routerFactory from './config/routes';
import store from './store';
import { guardFactory } from './guard/guard';
import { envFactory } from './common/envService/EnvService';
import { httpFactory } from './common/httpClient/HttpClient';
import { titleFactory } from 'common/title/Title';

import 'assets/css/index.scss';
import babelpolyfill from 'babel-polyfill';
import components from './components';
// import NProgress from "nprogress"
// import "nprogress/nprogress.css"
// NProgress.configure({ showSpinner: false })

import ElementUI from 'element-ui';
import './assets/css/common/element-variables.scss';

import {Picker} from 'tiger-ui';
Vue.use(Picker);
// import lodash from 'lodash';

let $title = titleFactory({ // 设置页面标题
  title: '保养推荐后台系统'
});
let $storage = StorageClient();
let $env = envFactory(); // 环境变量
let $http = httpFactory({}, $env, $storage);

Vue.use(Vuex); ;
Vue.use(ElementUI);

Vue.use(babelpolyfill);

// debugger;
// console.log('components',components)
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
Vue.prototype.$loginOut = () => {
  $storage.clearAll();
  $storage.clearCookie();
  var host = location.hostname.replace('yunying', 'yewu').replace('setting', 'yewu');
  host = host.replace(/(localhost|(192|172)\.[\d.]+):?\d*/, 'yewu.tuhu.work');
  // let curl = location.href.replace(/(localhost|(192|172)\.[\d.]+):?\d*/, 'setting.tuhu.work');
  let url = location.protocol + '//' + host + '/Account/LogOff'; // ?ReturnUrl=' + curl;

  window.location.href = url;
  // window.location.href = location.protocol + '//' + host + '/Account/LogOff';
  // $http.get('/Account/LogOff')
  //   .subscribe(() => {
  //     redirect();
  //   }, () => {
  //     redirect();
  //   });
};

// Directive
Vue.directive('focus', {
  inserted: function (el) {
    el.focus();
  }
});

// Vue.use(lodash);

// const moment = require('moment')
// Vue.use(require('vue-moment'), {
//   moment
// })

// vee-validate by benQ
// import VeeValidate, { Validator } from 'vee-validate'
// Vue.use(VeeValidate)
// import { dictionary } from 'veeValidate'
// Validator.updateDictionary(dictionary)
// Validator.setLocale('cn')

new Vue({
  el: '#app',
  // mixins: [Mixin],
  router: guardFactory(routerFactory($env), $http, store, $title, () => {
    redirect();
  }),
  store,
  render: h => h(App),
  data: {
    eventHub: new Vue()
  }
});
