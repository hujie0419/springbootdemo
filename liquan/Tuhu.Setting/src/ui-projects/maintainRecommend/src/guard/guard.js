import { Preload } from '../common/preload/Preload';
import Vue from 'vue';
/**
 * 路由守卫
 *
 * @export
 * @param {*} router 路由实例
 * @param {*} $http http请求服务
 * @param {*} $store vuex
 * @param {*} $title 设置标题对象
 * @param {*} redirect 登录不成功定向的回调
 * @returns {router}
 */
export function guardFactory(router, $http, $store, $title, redirect) {
  let preload = new Preload({
    routerOptions: router.options
  });
  router.beforeEach((to, from, next) => { // 设置上线ID
    let _this = new Vue();
    const loading = _this.$loading({text: 'Loading', spinner: 'el-icon-loading', background: 'rgba(0, 0, 0, 0.7)'});
    $http.get('/auth/GetBasicInformantion', {
      apiServer: 'apiServer',
      cacheData: {
        timeout: 1000 * 60 * 5 // ,
        // cacheType: 'l'
      }
    })
      .subscribe(back => {
        $store.dispatch('userInfo', back.user);
        if (back && back.success) {
          let menuList = sessionStorage.getItem('menuList');
          if (menuList) {
            next();
          } else {
            $http.get('/BaoYang/GetBaoYangCategoriesInfo', {
              apiServer: 'apiServer'
            }).subscribe(() => {
              next();
            }, () => {
              next();
            });
          }
        } else {
          if (redirect instanceof Function) {
            redirect();
          }
        }
      }, (err) => {
        let xhr = err.request;
        if (xhr && xhr.readyState === 4) {
          if (redirect instanceof Function) {
            redirect();
          }
        }
      });
  });

  // router.onError(() => {
  //     closeLoading();
  //     sLayer.toast('你的网络不好(T_T)请稍后再试!');
  // });
  router.afterEach((to, from) => {
    $title.setTitle(to.meta && to.meta.title);
    preload.load(to);
  });

  return router;
}