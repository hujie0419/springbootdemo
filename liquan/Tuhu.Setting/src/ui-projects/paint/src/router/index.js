import Vue from 'vue'
import Router from 'vue-router'
import index from '@/views/index'
import packageConfig from '@/views/paintDiscount/packageConfig'
import packageDetail from '@/views/paintDiscount/packageDetail'

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
      path: '/packageConfig',
      name: 'packageConfig',
      component: packageConfig,
      meta: {
        "title": "喷漆打折价格体系配置"
      }
    },
    {
      path: '/packageDetail',
      name: 'packageDetail',
      component: packageDetail,
      meta: {
        "title": "喷漆打折价格体系详情"
      }
    },
    page403,
    page500,
    page404
  ]
})
