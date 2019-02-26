import Vue from 'vue'
import Router from 'vue-router'
import index from '@/views/index'
import blockListConfig from '@/views/blockList/blockListConfig'
import MoveCarQRCode from '@/views/MoveCar/MoveCarQRCode'
import QuestionTypeList from '@/views/Feedback/QuestionTypeList'
import FeedbackList from '@/views/Feedback/FeedbackList'
import ProductLimitTire from '@/views/productLimit/tire'
import ProductLimitBaoyang from '@/views/productLimit/baoyang'
import ProductLimitCarproduct from '@/views/productLimit/carproduct'
import ProductLimitMeirong from '@/views/productLimit/meirong'

Vue.use(Router)

export const page404 = {
    path: '/*',
    name: 'error-404',
    meta: {
        title: '404-页面不存在'
    },
    component: () =>
        import ('@/views/error-page/404.vue')
};

export const page403 = {
    path: '/403',
    meta: {
        title: '403-权限不足'
    },
    name: 'error-403',
    component: () =>
        import ('@/views/error-page/403.vue')
};

export const page500 = {
    path: '/500',
    meta: {
        title: '500-服务端错误'
    },
    name: 'error-500',
    component: () =>
        import ('@/views/error-page/500.vue')
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
            path: '/BlockList/Pintuan',
            name: 'PintuanBlockList',
            component: blockListConfig,
            meta: {
                "title": "拼团黑名单配置"
            }
        },
        {
            path: '/BlockList/:blockSystem',
            name: 'BlockList',
            component: blockListConfig,
            meta: {
                "title": "黑名单配置"
                    // "authpath": "/test/api"
            }
        },
        {
            path: '/MoveCar/MoveCarQRCode',
            name: 'MoveCarQRCode',
            component: MoveCarQRCode,
            meta: {
                "title": "二维码生成记录",
                "authpath": "/MoveCarQRCode/GetMoveCarGenerationRecordsList"
            }
        },
        {
            path: '/Feedback/QuestionTypeList',
            name: 'QuestionTypeList',
            component: QuestionTypeList,
            meta: {
                "title": "问题类型",
                "authpath": "/FeedBack/GetQuestionTypeList"
            }
        },
        {
            path: '/Feedback/FeedbackList',
            name: 'FeedbackList',
            component: FeedbackList,
            meta: {
                "title": "意见反馈"
            }
        }, {
            path: '/productlimit/tire',
            name: 'tire',
            component: ProductLimitTire,
            meta: {
                "title": "产品限购管理",
                "authpath": "/ProductLimitTire/FetchCategoryLimitCount"
            }
        }, {
            path: '/productlimit/baoyang',
            name: 'baoyang',
            component: ProductLimitBaoyang,
            meta: {
                "title": "产品限购管理",
                "authpath": "/ProductLimitBaoYang/FetchCategoryLimitCount"
            }
        }, {
            path: '/productlimit/carproduct',
            name: 'carproduct',
            component: ProductLimitCarproduct,
            meta: {
                "title": "产品限购管理",
                "authpath": "/ProductLimitChePin/GetCatrgoryTree"
            }
        }, {
            path: '/productlimit/meirong',
            name: 'meirong',
            component: ProductLimitMeirong,
            meta: {
                "title": "产品限购管理",
                "authpath": "/ProductLimitMR/GetCatrgoryTree"
            }
        },
        page403,
        page500,
        page404
    ]
})