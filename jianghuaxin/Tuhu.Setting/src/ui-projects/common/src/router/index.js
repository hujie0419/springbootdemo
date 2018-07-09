import Vue from 'vue'
import Router from 'vue-router'
import index from '@/views/index'
import blockListConfig from '@/views/blockList/blockListConfig'

Vue.use(Router)

export const page404 = {
  path: '/*',
  name: 'error-404',
  meta: {
      title: '404-页面不存在'
  },
  component: () => import('@/views/error-page/404.vue')
};

export const page403 = {
  path: '/403',
  meta: {
      title: '403-权限不足'
  },
  name: 'error-403',
  component: () => import('@/views/error-page/403.vue')
};

export const page500 = {
  path: '/500',
  meta: {
      title: '500-服务端错误'
  },
  name: 'error-500',
  component: () => import('@/views/error-page/500.vue')
};

export default new Router({
  routes: [
    {
      path: '/',
      name: 'index',
      component: index,
      meta: {
        "title": "首页"
      }
    },
    {
      path: '/BlockList/Pintuan',
      name: 'PintuanBlockList',
      component: blockListConfig,
      meta: {
        "title": "拼团黑名单配置"
      }
    },
    {
      path: '/BlockList/:blockSystem',
      name: 'BlockList',
      component: blockListConfig,
      meta: {
        "title": "黑名单配置"
        // "authpath": "/test/api"
      }
    },
    page403,
    page500,
    page404
  ]
})
