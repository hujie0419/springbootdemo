// The Vue build version to load with the `import` command
// (runtime-only or standalone) has been set in webpack.base.conf with an alias.
import Vue from 'vue'
import App from './App'
import iView from 'iview'
import 'iview/dist/styles/iview.css'
import './theme.less'
import UtilLib from "./framework/libs/util.js"
import store from './framework/store'
import menus from '@/router/menus'

Vue.use(iView)
Vue.config.productionTip = false
Vue.prototype.$Loading.config({
  color: '#2ca9e1',
  failedColor: '#c9171e',
  height: 2
});
const util = new UtilLib(Vue.prototype.$Message, Vue.prototype.$Loading);
util.initialize();
Vue.prototype.ajax = util.ajax;
Vue.prototype.util = util;

util.ajax.get("/auth/GetBasicInformantion").then((response) => {
  if (response.data.success) {
    store.commit("user/init", response.data.user);
    store.commit("app/init", menus);
    /* eslint-disable no-new */
    var router = require('@/router/').default;

    router.beforeEach((to, from, next) => {
      util.loadingbar.start();
      util.setTitile(to.meta.title);
      console.log("pat" + to.meta.authpath);
      if (to.meta.authpath) {
        var instance = util.getAxiosInstance();
        instance.get("/auth/haspower?path=" + to.meta.authpath).then((response) => {
          if (response.data.success && response.data.result) {
            next();
          } else {
            next({ path: "/403" });
          }
        }).catch((error) => {
          console.log(error);
          util.loadingbar.error();
          next({ path: "/500" });
        });
      } else {
        next();
      }
    });
    
    router.afterEach(route => {
      console.log("finish");
      util.loadingbar.finish();
    });

    new Vue({
      el: '#app',
      router,
      store: store,
      components: { App },
      template: '<App/>'
    });
  } else {
    var host = location.hostname.replace("yunying", "yewu").replace("setting", "yewu");
    window.location.href = location.protocol + "//" + host + "/Account/LogOn?ReturnUrl=" + location.href;
  }
});
