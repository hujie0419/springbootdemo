import Vue from 'vue'
import Router from 'vue-router'
import index from '@/views/index'
import activitylist from '@/views/discount/activitylist'
import editactivity from '@/views/discount/editactivity'
import activityproduct from '@/views/discount/activityproduct'
import waitaudit from '@/views/discount/waitaudit'
import activityinfo from '@/views/discount/activityinfo'
import auditauthlist from '@/views/auditAuth/auditauthlist'

Vue.use(Router)

export const page404 = {
  path: '/*',
  name: 'error-404',
  meta: {
    title: '404-页面不存在'
  },
  component: () =>
    import('@/views/error-page/404.vue')
};

export const page403 = {
  path: '/403',
  meta: {
    title: '403-权限不足'
  },
  name: 'error-403',
  component: () =>
    import('@/views/error-page/403.vue')
};

export const page500 = {
  path: '/500',
  meta: {
    title: '500-服务端错误'
  },
  name: 'error-500',
  component: () =>
    import('@/views/error-page/500.vue')
};

export default new Router({
  routes: [{
      path: '/',
      name: 'index',
      component: index,
      meta: {
        "title": "首页"
      }
    },
    {
      path: '/discount/activitylist',
      name: 'activitylist',
      component: activitylist,
      meta: {
        "title": "促销活动列表"
      }
    },
    {
      path: '/discount/editactivity',
      name: 'editactivity',
      component: editactivity,
      meta: {
        "title": "新建促销活动"
      }
    },
    {
        path: '/discount/activityproduct',
        name: 'activityproduct',
        component: activityproduct,
        meta: {
          "title": "促销活动商品"
        }
      }, {
        path: '/discount/waitaudit',
        name: 'waitaudit',
        component: waitaudit,
        meta: {
          "title": "促销活动审核"
        }
      },
      {
        path: '/discount/activityinfo',
        name: 'activityinfo',
        component: activityinfo,
        meta: {
          "title": "查看促销活动"
        }
      },
      {
        path: '/auditAuth/auditauthlist',
        name: 'auditauthlist',
        component: auditauthlist,
        meta: {
          "title": "促销活动审核权限列表"
        }
      },
    page403,
    page500,
    page404
  ]
})
