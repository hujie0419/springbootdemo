import Vue from 'vue';
import VueRouter from 'vue-router';

const ConsoleLayout = resolve => require(['../pages/ConsoleLayout'], resolve);
const ConsoleLoading = resolve => require(['../pages/ConsoleLoading'], resolve);
const ActivityConfig = resolve => require(['../pages/activityConfig/ActivityConfig'], resolve);
const ActivityBoard = resolve => require(['../pages/activityBoard/ActivityBoard'], resolve);
const NoAccess = resolve => require(['../pages/noAccess/NoAccess'], resolve);

Vue.use(VueRouter);

/**
 * 路由配置
 * @param {Object} $env 环境变量
 * @return {Object}
 */
export default function ($env) {
    let config = {
        history: true,
        hashbang: false,
        base: __dirname,
        routes: [
            { path: '/', name: 'console', component: ConsoleLoading },
            {
                path: '/activity',
                component: ConsoleLayout,
                children: [
                    { path: 'activityConfig', name: 'activityConfig', component: ActivityConfig },
                    { path: 'noAccess', name: 'noAccess', component: NoAccess },
                    {
                        path: 'activityBoard',
                        name: 'activityBoard',
                        component: ActivityBoard
                    }
                ]
            }
        ]
    };
    if ($env && !$env.useHash) {
        config.mode = 'history';
    }
    return new VueRouter(config);
}
