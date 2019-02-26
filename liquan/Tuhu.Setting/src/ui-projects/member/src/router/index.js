import Vue from 'vue'
import Router from 'vue-router'
import dailycheckIn from '@/views/membertask/DailyCheckIn.vue'
import timelimitedreward from '@/views/membertask/TimelimitedReward.vue'
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
      component: dailycheckIn,
      meta: {
        title: "签到",
        authpath: "UserDailyCheckIn/GetData"
      }
    },
    {
      path: '/DailyCheckIn',
      name: 'DailyCheckIn',
      component: dailycheckIn,
      meta: {
        title: "签到",
        authpath: "UserDailyCheckIn/GetData"
      }
    },
    {
      path: '/TimelimitedReward',
      name: 'TimelimitedReward',
      component: timelimitedreward,
      meta: {
        title: "限时奖励",
        authpath: "TaskTypeReward/GetTaskTypeReward"
      }
    },
    page403,
    page500,
    page404
  ]
})
