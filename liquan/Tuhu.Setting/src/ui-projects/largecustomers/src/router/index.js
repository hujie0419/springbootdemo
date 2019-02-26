import Vue from 'vue'
import Router from 'vue-router'
import index from '@/views/index'
import cscoupon from '@/views/customers/customers-coupon'
import cssetting from '@/views/customers/customers-setting'
import TPOderChannelLink from '@/views/TPOderChannel/TPOderChannelLink'
import AddTPOderChannelLink from '@/views/TPOderChannel/AddTPOderChannelLink'

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
      path: '/customers/customers-setting',
      name: 'cssetting',
      component: cssetting,
      meta: {
        "title": "企业客户专享配置"
      }
    },
    {
      path: '/customers/customers-coupon/:customersSettingId/:activityExclusiveId',
      name: 'cscoupon',
      component: cscoupon,
      meta: {
        "title": "企业客户专享券码"
      }
    },
    {
      path: '/TPOderChannel/TPOderChannelLink',
      name: 'TPOderChannelLink',
      component: TPOderChannelLink,
      meta: {
        "title": "三方渠道链接申请列表"
      }
    },
    {
      path: '/TPOderChannel/AddTPOderChannelLink',
      name: 'AddTPOderChannelLink',
      component: AddTPOderChannelLink,
      meta: {
        "title": "添加渠道链接"
      }
    },
    page403,
    page500,
    page404
  ]
})
