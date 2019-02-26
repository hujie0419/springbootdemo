import Vue from 'vue'
import Router from 'vue-router'
import index from '@/views/seckill/index'
import edit from '@/views/seckill/edit'
import Question from '@/views/config/Question'
import Prize from '@/views/config/Prize'
import blockListConfig from '@/views/blockList/blockListConfig'
import StarRatingStore from '@/views/StarRatingStore'
import MaintenancePackageOnSale from '@/views/tireActivitySetPrice/MaintenancePackageOnSale'
import TireActivity from '@/views/tireActivitySetPrice/TireActivity'
import CarFriendsGroup from '@/views/CarFriendsGroup/CarFriendsGroup'
import CarFriendsAdministrator from '@/views/CarFriendsGroup/CarFriendsAdministrator'
import barginShareBlackListConfig from '@/views/blockList/barginShareBlackListConfig'
import GoToBannerConfig from '@/views/CarFriendsGroup/GoToBannerConfig'
import LuckyCharmActivity from '@/views/LuckyCharm/LuckyCharmActivity'
import LuckyCharmUser from '@/views/LuckyCharm/LuckyCharmUser'
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
      path: '',
      redirect: 'seckill/index'
    },
    {
      path: '/seckill/index',
      name: 'seckillindex',
      component: index,
      meta: {
        "title": "首页",
        authpath: "/Seckill/GetSeckillList"

      }
    },
    {
      path: '/seckill/edit',

      name: 'seckillEdit',

      component: edit,

      meta: {
        "title": "编辑"
      }
    },
    {
      path: '/Question',
      name: 'Question',
      component: Question,
      meta: {
        title: "答题配置",
        authpath: "/GuessGame/GetQuestionList"        
      }
    },
    {
      path: '/Prize',
      name: 'Prize',
      component: Prize,
      meta: {
        title: "兑换物配置",
        authpath: "/GuessGame/GetActivityPrizeList"             
      }
    },
    {
      path: '/BlockList/Pintuan',
      name: 'PintuanBlockList',
      component: blockListConfig,
      meta: {
        "title": "拼团黑名单配置",
        "authpath": "/BlockListConfig/List"
      }
    },
    {
      path: '/BlockList/ShareBargain',
      name: 'barginShareBlackListConfig',
      component: barginShareBlackListConfig,
      meta: {
        "title": "砍价黑名单配置",
        "authpath": "/BlockListConfig/List"
      }
    },
    {
      path: '/BlockList/:blockSystem',
      name: 'BlockList',
      component: blockListConfig,
      meta: {
        "title": "黑名单配置",
        "authpath": "/BlockListConfig/List"
      }
}, 
    {
      path: '/StarRatingStore',
      name: 'StarRatingStore',
      component: StarRatingStore,
      meta: {
        title: "星级认证店",
        authpath: "/StarRatingStore/GetStarRatingStoreList"        
      }
    }, 
    {
      path: '/MaintenancePackageOnSale',
      name: 'MaintenancePackageOnSale',
      component: MaintenancePackageOnSale,
      meta: {
        title: "小保养套餐优惠价格",
        authpath: "/TireActivity/GetMaintenancePackageOnSaleList"
      }
    }, 
    {
      path: '/TireActivity',
      name: 'TireActivity',
      component: TireActivity,
      meta: {
        title: "轮胎活动配置",
        authpath: "/TireActivity/GetTireActivityList"
      }
    },
    {
      path: '/CarFriendsGroup/CarFriendsGroup',
      name: 'CarFriendsGroup',
      component: CarFriendsGroup,
      meta: {
        title: "车友群列表",
        authpath: "/CarFriendsGroup/GetCarFriendsGroupList"
      }
    },
    {
      path: '/CarFriendsAdministrator',
      name: 'CarFriendsAdministrator',
      component: CarFriendsAdministrator,
      meta: {
        title: "管理员列表"
      }
    },
    {
      path: '/GoToBannerConfig',
      name: 'GoToBannerConfig',
      component: GoToBannerConfig,
      meta: {
        title: "banner配置页"
      }
    },
    {
      path: '/LuckyCharm/LuckyCharmActivity',
      name: 'LuckyCharmActivity',
      component: LuckyCharmActivity,
      meta: {
        title: "锦鲤活动列表"
      }
    },
    {
      path: '/LuckyCharm/LuckyCharmUser',
      name: 'LuckyCharmUser',
      component: LuckyCharmUser,
      meta: {
        title: "锦鲤活动用户列表"
      }
    },
    page403,
    page500,
    page404
  ]
})
