<template>
  <div class="activity-config-pages">
    <el-tabs class="page-tabs" v-model="activeIndex" type="border-card" @tab-remove="removeTab">
        <el-tab-pane
            :closable="item.closable"
            :label="item.title"
            :name="item.name"
            v-for="item in tabList"
            :key="item.name">
                <activity-pane

                    :item="item">
                </activity-pane>
        </el-tab-pane>
      <!-- v-for="(item, index) in tabList"
      :key="index" -->
    </el-tabs>
  </div>
</template>

<script>
import Vue from 'vue';
// import { Popup } from 'tiger-ui';
import ActivityPane from './activityPane/ActivityPane';

import filterPrefixInt from '../../common/filter/filterPrefixInt.filter.js';
import filterNumber from '../../common/filter/filterNumber.filter.js';
import filterMoney from '../../common/filter/filterMoney.filter.js';
import filterPromotionTyple from './commons/filters/filterPromotionTyple.filter';

// Vue.use(Popup);
Vue.filter('filter_prefixInt', filterPrefixInt);
Vue.filter('filter_number', filterNumber);
Vue.filter('filter_money', filterMoney);
Vue.filter('filter_promotionTyple', filterPromotionTyple);

export default {
    provide() {
        let _that = this;
        Vue.prototype.$$tabs = {
            addTab: _that.addTab,
            removeTab: _that.removeTab
        };
        let _popup = null;
        return {
            $$Popup: {
                open(_com, options) {
                    _popup && _popup.close();
                    return new Promise((resolve, reject) => {
                        let popup = _that.$tPopup.openPopup(_com, Object.assign({
                            wrapCla: 'alignCla form_popup', // 最外层追加的Class名
                            // isShowCloseBtn: true,
                            isClickBgClose: true,
                            alignCla: 'centerMiddle', // ''|'bottom'|'top'|'fullScreen'|'centerMiddle',
                            transitionCls: 't_scale' // , // ''|'t_toUp'|'t_scale'|'t_toBottom'|'t_toLeft';
                            // showFlag: showFlag
                        }, options, () => {
                            alert(55);
                        }));
                        _popup = popup;
                        let com = popup && popup.component;
                        let _popupCom = popup.popup;
                        if (com) {
                            com.$on('confirmChange', (dataItem) => {
                                resolve(dataItem);
                                popup.closePopup();
                            });
                            com.$on('cancelChange', (dataItem) => {
                                resolve({
                                    type: 'cancel'
                                });
                                popup.closePopup();
                            });
                            _popupCom.$on('close', (dataItem) => {
                                resolve({
                                    type: 'close'
                                });
                                popup.closePopup();
                            });
                        }
                    });
                }
            }
            // $$form: _that.$form,
            // $$tabs: _that.$tabs
        };
    },
    data() {
        return {
            activeIndex: 'activityList',
            tabList: [{
                title: '活动列表',
                name: 'activityList'
            }]
            // tabIndex: 1
        };
    },
    components: {
        ActivityPane
    },
    // mounted() {
    //     this.addTab('footTabsPage', {
    //         title: 'footTabsPage',
    //         data: {
    //             ActivityId: '6C141AB1',
    //             ModuleId: '2'
    //         }
    //     });
    // },
    methods: {
        /**
         * 添加页签
         * @param {string|number} tabKey 页签对应的key
         * @param {object} tabConfig 页签对应的数据
         */
        addTab(tabKey, tabConfig) {
            // let newTabName = ++this.tabIndex + '';
            if (!tabKey) {
                console.error('请传入页签的key');
                return;
            }
            tabKey = tabKey + ((tabConfig && tabConfig.pageId) || '');

            let nowTab = this.tabList.find(item => {
                return item.name === tabKey;
            });

            if (nowTab) {
                this.activeIndex = nowTab.name;
            } else {
                this.tabList.push(Object.assign({}, {
                    closable: true,
                    title: 'New Tab'
                }, tabConfig, {
                    name: tabKey
                }));
                let tabList = this.tabList;
                this.activeIndex = tabList[tabList.length - 1].name;
            }
        },
        /**
         * 移除页签
         * @param {string|number} tabKey 页签对应的key
         */
        removeTab(tabKey) {
            let tabs = this.tabList;
            let activeName = this.activeIndex;
            if (!tabKey) {
                console.error('请传入页签的key');
                return;
            }

            let selectIndex = -1;
            // if (activeName === tabKey) {
            tabs.forEach((tab, index) => {
                if (tab.name === tabKey) {
                    selectIndex = index;
                    let nextTab = tabs[index + 1] || tabs[index - 1];
                    if (nextTab) {
                        activeName = nextTab.name;
                    }
                }
            });
            // }

            this.activeIndex = activeName;
            if (selectIndex > -1) {
                this.$delete(this.tabList, selectIndex);
            }
            // this.tabList = tabs.filter(tab => tab.name !== tabKey);
        }
    }
};
</script>
<style lang="scss">
@import "css/common/_var.scss";
@import "css/common/_mixin.scss";
@import "css/common/_iconFont.scss";

// 页签上的分隔竖线
.activity-config-pages {
    .page-tabs {
        .el-tabs__nav {
        .el-tabs__item{
            &:first-child,&.is-active {
            z-index: 2;
            &::after{
                display: none;
            }
            }
        }
        }
        .el-tabs__item {
        position: relative;
        &::after {
            content: '';
            display: block;
            position: absolute;
            left: 0;
            top: 50%;
            border-left: 1px solid $splitBrColor;
            height: 20px;
            transform: translateY(-50%);
        }
        }
    }
    .tab-page-title{
        // font-size: $largeFontSize;
        font-weight: bold;
        display: block;
        width: 100%;
        padding-left: 20px;
        line-height: 40px;
    }

    .a-link {
        color: $linkColor;
    }
}
.el-message{
    top: 130px;
}
</style>
