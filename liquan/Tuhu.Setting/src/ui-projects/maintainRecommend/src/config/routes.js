import Vue from 'vue'
import VueRouter from 'vue-router'

const ConsoleLayout = resolve => require(['../pages/ConsoleLayout'], resolve)
// const Dashboard = resolve => require(['../pages/Dashboard'], resolve)
// const ConsoleLoading = resolve => require(['../pages/ConsoleLoading'], resolve)
// const Demo = resolve => require(['../pages/demo/index'], resolve)
// const Home = resolve => require(['../pages/home/index'], resolve)
const Content = resolve => require(['../pages/home/Content'], resolve)
const Special = resolve => require(['../pages/other/SpecialCarConfig'], resolve)
Vue.use(VueRouter)

export default function ($env) {
  let config = {
    history: true,
    hashbang: false,
    base: __dirname,
    routes: [
      // { path: '/', name: 'console', component: ConsoleLoading },
      { path: '/', redirect: '/console' },
      {
        path: '/console',
        redirect: '/console/content',
        component: ConsoleLayout,
        meta: {
          preload: ['/console/content']
        },
        children: [
          // {
          //   meta: {
          //     title: '首页'
          //   },
          //   path: 'home', name: 'home', component: Home
          // },
          { path: 'content', name: 'content', component: Content },
          { path: 'special', name: 'special', component: Special, meta: {noMenu: true} },
        ]
      }
    ]
  };
  if ($env && !$env.useHash) {
    config.mode = 'history'
  }
  return new VueRouter(config)
}
