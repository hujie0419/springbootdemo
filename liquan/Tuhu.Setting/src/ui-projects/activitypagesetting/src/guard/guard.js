import { Preload } from '../common/preload/Preload';

/**
 * 路由守卫
 *
 * @export
 * @param {*} router 路由实例
 * @param {*} $http http请求服务
 * @param {*} $store vuex
 * @param {*} $title 设置标题对象
 * @param {*} $env 环境变量
 * @param {*} redirect 登录不成功定向的回调
 * @returns {router}
 */
export function guardFactory(router, $http, $store, $title, $env, redirect) {
    let preload = new Preload({
        routerOptions: router.options
    });

    router.beforeEach((to, from, next) => { // 设置上线ID
        if (to.name === 'noAccess') { // 无权限页面，直接next
            next();
            return;
        }
        if (($env && $env.needLogin) || to.meta.needLogin) {
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
                        // console.log(to.name);
                        if (to.name === 'activityConfig') {
                            $http.get('/auth/haspower', { // 判断权限
                                apiServer: 'apiServer',
                                params: {
                                    path: 'Activity/GetActivityList'
                                },
                                cacheData: {
                                    cacheKey: '/auth/haspower',
                                    cacheType: 'm',
                                    timeout: 1000 * 60 * 5 // 缓存5分钟
                                }
                            }).subscribe(res => {
                                if (res && res.result === true) {
                                    next();
                                } else {
                                    next({
                                        path: '/activity/noAccess?type=noAccess'
                                    });
                                }
                            }, () => {
                                next({
                                    path: '/activity/noAccess?type=networkError'
                                });
                            });
                        } else {
                            next();
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
        } else {
            next();
        }
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
