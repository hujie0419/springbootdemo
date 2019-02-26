<template>
    <div>
        <base-info
            :Activityid="myActivityid"
            @setActivityId="setActivityId"
            :tagOption="tagOption"
            :activityInfo="myActivityInfo"
            @refresh="refresh"></base-info>
        <display-request :Activityid="myActivityid" :tagOption="tagOption" :activityInfo="myActivityInfo" @refresh="refresh"></display-request>
        <template-config
            :Activityid="myActivityid"
            @updateTable="updateTable"
            :activityInfo="myActivityInfo"
            @toTabs="goNewTab"
            :tagOption="tagOption"
            @resetIsRefresh="updateTable"
            @refresh="refresh">
        </template-config>
    </div>
</template>

<script>
import BaseInfo from './baseInfo/BaseInfo';
import DisplayRequest from './displayRequest/DisplayRequest';
import TemplateConfig from './templateConfig/TemplateConfig';
import NewActivityExtend from './newActivityExtend//NewActivityExtend';
import * as newActivityApi from '../commons/apis/newActivity/newActivityApi';

export default {
    extends: NewActivityExtend,
    components: {
        BaseInfo,
        DisplayRequest,
        TemplateConfig
    },
    // watch: {
    //     myActivityid(nowVal) {

    //     }
    // },
    mounted() {
        this.initData();
    },
    methods: {
        /**
         * 刷新数据
         */
        refresh () {
            this.initData();
        },
        updateTable() {
            this.initData();
        },
        // getActivityInfo(activityid) {
        //     if (!activityid) {
        //         return;
        //     }
        //     this.$http.post(newActivityApi.GETACTIVITYINFO, {
        //         apiServer: 'apiServer',
        //         isLoading: true,
        //         data: {
        //             ActivityId: activityid || ''
        //         }
        //     }).subscribe(data => {
        //         if (cb instanceof Function) {
        //             cb(data);
        //         }
        //     });
        // },
        setActivityId(activityid) {
            this.myActivityid = activityid;
        }
    }
};
</script>

<style lang='scss'>
    @import "css/common/_var.scss";
    @import "css/common/_mixin.scss";
    @import "css/common/_iconFont.scss";

</style>
