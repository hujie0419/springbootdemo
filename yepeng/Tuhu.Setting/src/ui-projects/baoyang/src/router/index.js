import Vue from 'vue'
import Router from 'vue-router'
import index from '@/views/index'
import packageconfig from '@/views/baseconfig/packageconfig'
import installtypeconfig from '@/views/baseconfig/installtypeconfig'
import levelUpIconConfig from '@/views/baseconfig/levelUpIconConfig'

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
      path: '/baseconfig/package',
      name: 'packageconfig',
      component: packageconfig,
      meta: {
        "title": "项目配置",
        "authpath": "/test/api"
      }
    },
    {
      path: '/baseconfig/installtype',
      name: 'installtypeconfig',
      component: installtypeconfig,
      meta: {
        "title": "切换服务配置",
        "authpath": ""
      }
    },
    {
      path: '/baseconfig/levelUpIconConfig',
      name: 'levelUpIconConfig',
      component: levelUpIconConfig,
      meta: {
        "title": "升级购图标配置",
        "authpath": ""
      }
    },
    page403,
    page500,
    page404
  ]
})
