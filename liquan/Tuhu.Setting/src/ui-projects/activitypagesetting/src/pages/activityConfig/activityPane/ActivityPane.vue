<template>
  <!-- <el-tab-pane
      :closable="item.closable"
      :label="item.title"
      :name="item.name">

    </el-tab-pane> -->
    <div class="activity-pane">
        <div class="fresh-btn" @click="fresh">&#xe60B;</div><!-- v-if="filterPageId(item) !== 'activity-list'"-->
        <div
            :tagOption="item"
            :is="filterPageId(item)"
            v-if="filterPageId(item) && !freshData">
        </div>
        <!-- <home :tagOption="item" v-if="filterPageId(item, 'home')"></home>
        <general-page :tagOption="item" v-else-if="filterPageId(item, 'generalPage')"></general-page>
        <seperate-car :tagOption="item" v-else-if="filterPageId(item, 'seperateCar')"></seperate-car>
        <goods
            :tagOption="item"
            v-else-if="filterPageId(item, 'goods') ||
            filterPageId(item, 'goodsEdit') ||
            filterPageId(item, 'groupBooking') ||
            filterPageId(item, 'sysRec') ||
            filterPageId(item, 'goodsSeckill')">
        </goods>
        <slide-coupon
            :tagOption="item" v-else-if="filterPageId(item, 'slideConpon')"></slide-coupon>
        <picture-page
            :tagOption="item" v-else-if="filterPageId(item, 'picture')"></picture-page>
        <product-page
            :tagOption="item" v-else-if="filterPageId(item, 'ProductPage')"></product-page>
        <draw-lottery
            :tagOption="item" v-else-if="filterPageId(item, 'drawLottery')"></draw-lottery>
        <activity-list :tagOption="item" v-else-if="filterPageId(item, 'activityList')"></activity-list>
        <new-activity :tagOption="item" v-else-if="filterPageId(item, 'newActivity')"></new-activity>
        <special
            :tagOption="item"
            v-else-if="filterPageId(item, 'maintainPricing') ||
            filterPageId(item, 'countDown') ||
            filterPageId(item, 'videoConfigPage') ||
            filterPageId(item, 'textLinkPage') ||
            filterPageId(item, 'footTabsPage')">
            </special> -->
    </div>
</template>

<script>
import GeneralPage from '../headMap/GeneralPage';
import SeperateCar from '../headMap/SeperateCar';
import Goods from '../goods/Goods.vue';
import Home from '../home/Home.vue';
import SlideCoupon from '../coupon/SlideConpon';
import PicturePage from '../pictureMap/PicturePage';
import ProductPage from '../pictureMap/ProductPage';
import DrawLottery from '../drawLottery/DrawLottery';
import ActivityList from '../activityList/ActivityList';
import NewActivity from '../newActivity/NewActivity';
import Special from '../special/Special';

export default {
    // mounted() {
    //     this.addTab('footTabsPage', {
    //         title: 'footTabsPage',
    //         data: {
    //             ActivityId: '6C141AB1',
    //             ModuleId: '2'
    //         }
    //     });
    // },
    data() {
        return {
            freshData: false
        };
    },
    props: {
        item: {
            type: Object
        }
    },
    methods: {
        fresh() {
            this.freshData = true;
            setTimeout(() => {
                this.freshData = false;
            }, 20);
        },
        addPanes(...arg) {
            return this.$parent.addPanes(...arg);
        },
        /**
         * 页面ID过滤器
         * @param {Object} item 当前页签项数据
         * @param {string|number} tabKey 当前名称
         * @return {boolean}
         */
        filterPageId(item) {
            // return item.name === (tabKey + (item.pageId || ''));
            let tabKey = (item.pageId && item.name.replace(item.pageId, '')) || item.name;
            return this.pageFactory(tabKey);
        },
        pageFactory(tabKey) {
            let res = '';
            switch (tabKey) {
                case 'home':
                    res = 'home';
                    break;
                case 'generalPage':
                    res = 'general-page';
                    break;
                case 'seperateCar':
                    res = 'seperate-car';
                    break;
                case 'goods':
                case 'goodsEdit':
                case 'groupBooking':
                case 'sysRec':
                case 'goodsSeckill':
                    res = 'goods';
                    break;
                case 'slideConpon':
                    res = 'slide-coupon';
                    break;
                case 'picture':
                    res = 'picture-page';
                    break;
                case 'ProductPage':
                    res = 'product-page';
                    break;
                case 'drawLottery':
                    res = 'draw-lottery';
                    break;
                case 'activityList':
                    res = 'activity-list';
                    break;
                case 'newActivity':
                    res = 'new-activity';
                    break;
                case 'maintainPricing':
                case 'countDown':
                case 'videoConfigPage':
                case 'textLinkPage':
                case 'maintenanceVehiclePage':
                case 'footTabsPage':
                    res = 'special';
                    break;

                default:
                    break;
            }
            return res;
        }
    },
    components: {
        GeneralPage,
        SeperateCar,
        SlideCoupon,
        Home,
        Goods,
        PicturePage,
        DrawLottery,
        Special,
        ProductPage,
        ActivityList,
        NewActivity
    }
};
</script>
<style lang="scss">
@import "css/common/_var.scss";
@import "css/common/_mixin.scss";
@import "css/common/_iconFont.scss";
.fresh-btn {
    @extend %as_icon;
    position: fixed;
    top: 74px;
    right: 27px;
    z-index: 999;
    font-size: 22px;
    color: #888;
    cursor: pointer;
}
</style>
