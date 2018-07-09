import Vue from 'vue'
import Router from 'vue-router'
import shoppromotionIndex from '@/views/shoppromotion/index'
import shoppromotionCreate from '@/views/shoppromotion/create'
import shoppromotionDetail from '@/views/shoppromotion/detail'

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
      redirect: 'shoppromotion/Index'
    },
    {
      path: '/shoppromotion/index',
      name: 'shoppromotionIndex',
      component: shoppromotionIndex,
      meta: {
        title: "门店优惠券规则",
        authpath: "shoppromotion/GetCouponList"
      }
    },
    {
      path: '/shoppromotion/create',
      name: 'shoppromotionCreate',
      component: shoppromotionCreate,
      meta: {
        title: "创建门店优惠券规则",
        authpath: "shoppromotion/GetCouponList"
      }
    },
    {
      path: '/shoppromotion/detail',
      name: 'shoppromotionDetail',
      component: shoppromotionDetail,
      meta: {
        title: "查看门店优惠券规则",
        authpath: "shoppromotion/GetCouponList"
      }
    },
    page403,
    page500,
    page404
  ]
})
