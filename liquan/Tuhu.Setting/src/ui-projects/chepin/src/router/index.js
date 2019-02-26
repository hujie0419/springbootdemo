import Vue from 'vue'
import Router from 'vue-router'

import CarProductPriceManager from '@/views/PriceManager/CarProductPriceManager'

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
      name: '首页',
      redirect: 'CarProductPriceManager'
    },
    {
      path: '/CarProductPriceManager',
      name: 'CarProductPriceManager',
      component: CarProductPriceManager,
      meta: {
        "title": "车品价格管理",
        authpath: "/ProductPrice/GetCarProductMutliPriceByCatalog"
      }
    },
    {
      path: '/banner',
      name: 'banner',
      component: () =>
        import('@/views/banner/index'),
      meta: {
        "title": "Banner列表"
      }
    },
    {
      path: '/kengwei',
      name: 'kengwei',
      component: () =>
        import('@/views/kengwei'),
      meta: {
        "title": "坑位配置"
      }
    },
    {
      path: '/secondkill',
      name: 'secondkill',
      component: () =>
        import('@/views/secondKill'),
      meta: {
        "title": "天天秒杀配置"
      }
    },
    {
      path: '/gaizhuang',
      name: 'gaizhuang',
      component: () =>
        import('@/views/gaizhuang'),
      meta: {
        "title": "玩车改装配置"
      }
    },
    {
      path: '/floor',
      name: 'floor',
      component: () =>
        import('@/views/floor'),
      meta: {
        "title": "车品楼层配置"
      }
    },
    {
      path: '/floor/edit',
      name: 'floorEdit',
      component: () =>
        import('@/views/floor/edit'),
      meta: {
        "title": "车品楼层配置"
      }
    },
    {
      path: '/pop',
      name: 'pop',
      component: () =>
        import('@/views/pop'),
      meta: {
        "title": "弹出广告配置"
      }
    },
    {
      path: '/vendorproduct/coverarea/index',
      name: 'coverarea',
      component: () =>
        import('@/views/vendorproduct/coverarea/index'),
      meta: {
        "title": "覆盖区域配置",
        authpath: "/VPCoverAreaConfig/GetAllBrands"
      }
    },
    {
      path: '/vendorproduct/couponpricedisplay/index',
      name: 'couponpricedisplay',
      component: () =>
        import('@/views/vendorproduct/couponpricedisplay/index'),
      meta: {
        "title": "券后价配置",
        authpath: "/VPCouponPrice/GetAllBrands"
      }
    },
    page403,
    page500,
    page404
  ]
})
