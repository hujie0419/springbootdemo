import Vue from 'vue'
import Router from 'vue-router'
import index from '@/views/index'
import lottery from '@/views/lottery/lottery'
import lotteryRecycleBin from '@/views/lottery/lotteryRecycleBin'
import lotteryDetail from '@/views/lottery/lotteryDetail'
import productconfig from '@/views/groupbuying/productconfig'
import Coupon from '@/views/groupbuying/Coupon'

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
      path: "/",
      name: "index",
      component: index,
      meta: {
        title: "首页"
      }
    },
    {
      path: "/Lottery",
      name: "Lottery",
      component: lottery,
      meta: {
        title: "拼团抽奖后台",
        authpath: "/GroupBuyingV2/LotteryList"
      }
    },
    {
      path: "/Lottery/:id",
      name: "LotteryDetail",
      component: lotteryDetail,
      meta: {
        title: "拼团抽奖详情",
        authpath: "/GroupBuyingV2/LotteryDetailList"
      }
    },
    {
      path: "/LotteryRecycleBIn",
      name: "LotteryRecycleBIn",
      component: lotteryRecycleBin,
      meta: {
        title: "已过期拼团抽奖",
        authpath: "/GroupBuyingV2/LotteryRecycleBinList"
      }
    },
    {
      path: "/ProductConfig",
      name: "ProductConfig",
      component: productconfig,
      meta: {
        title: "拼团产品配置",
        authpath: "/GroupBuyingV2/ProductConfig"
      }
    },
    {
      path: '/Coupon',
      name: 'Coupon',
      component: Coupon,
      meta: {
        title: "拼团优惠券配置",
        authpath: "/GroupBuyingV2/PT_GetCouponList"
      }
    },
    page403,
    page500,
    page404
  ]
});
