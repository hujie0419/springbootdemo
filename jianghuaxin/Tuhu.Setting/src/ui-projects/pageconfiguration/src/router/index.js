import Vue from 'vue'
import Router from 'vue-router'
// import index from '@/views/index'
// import demo from '@/views/demo'
import list from '@/views/memberpage/list'
import moreList from '@/views/memberpage/moreList'
import activityList from '@/views/memberpage/activityList'

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
      component: list,
      meta: {
        title: "个人中心",
        authpath: "MemberPage/GetMemberPageList"
      }
    },
    {
      path: '/list',
      name: 'list',
      component: list,
      meta: {
        title: "个人中心",
        authpath: "MemberPage/GetMemberPageList"
      }
    },
    {
      path: '/moreList',
      name: 'moreList',
      component: moreList,
      meta: {
        title: "更多",
        authpath: "MemberPage/GetMemberPageList"
      }
    },
    {
      path: '/activityList',
      name: 'activityList',
      component: activityList,
      meta: {
        title: "报名信息"
        // authpath: "NewActivity/GetActivityPageList"
      }
    },
    page403,
    page500,
    page404
  ]
})
