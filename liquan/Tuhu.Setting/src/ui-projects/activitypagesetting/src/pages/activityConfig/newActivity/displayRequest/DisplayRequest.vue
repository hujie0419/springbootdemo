<template>
    <div class="display-request">
        <page-card
            @submit="submit"
            :title="'展示要求'"
            class="showConfig"
            :keepHidePageContent="!Activityid"
            @changeShow="changeShow"
            :showPageContent="!!Activityid"
            @topSubmit="$emit('refresh')"><!-- :showTopButton="true" topButton="刷新" -->
            <form-group @valueChange="valueChange" :formType="'array'" @formInit="formInit" :form-config="formConfig"></form-group>
        </page-card>
    </div>
</template>

<script>
import PageCard from '../../commons/pageCard/PageCard';
import FormGroup from '../../commons/formGroup/FormGroup';
import * as newActivityApi from '../../commons/apis/newActivity/newActivityApi';
import { getChildList, getSearchList, getShareList, addItemData } from '../config/displayRequest.config';
let activityH5 = 'https://wx.tuhu.cn/vue/NaActivity/pages/home/index?id=';
let url = location.href;
if (url.indexOf('.tuhu.work') > -1 || url.indexOf('172.') > -1 || url.indexOf('localhost') > -1) {
    activityH5 = 'https://wxdev.tuhu.work/vue/vueTest/pages/home/index?_project=NaActivity&id=';
}
let activityApplets = '/packages/active/active?id=';

export default {
    components: {
        PageCard,
        FormGroup
    },
    props: {
        Activityid: {
            type: String,
            default: ''
        },
        activityInfo: {
            type: Object
        }
    },
    data() {
        // let searchList = getSearchList();
        // let shareList = getShareList();
        let formConfig = getChildList();
        // formConfig[0][0].formControl=searchList;
        // formConfig[2][0].formControl=shareList;
        // this.temp = {SearchKeyword: '', SearchImgUrl: ''};
        // this.shareTemp ={
        //     ShareDescribe: '',
        //     ShareImgUrl: '',
        //     ShareLink: '',
        //     ShareSmallImgUrl: '',
        //     ShareSmallLink: '',
        //     ShareTitle: ''
        // };
        return {
            formModel: null,
            formUpdata: null,
            // defaultData: [{
            //     // IsSearchPage: '',
            //     // SearchKeyword: '',
            //     // SearchImgUrl: ''
            // }],
            searchList: formConfig[0][0],
            shareList: formConfig[2][0],
            formConfig: formConfig,
            formDataCache: this.$$form.initFormCache() // 缓存表单数据
        };
    },
    watch: {
        activityInfo() {
            this.setData();
        }
    },
    methods: {
        setData() {
            let activityInfo = this.activityInfo;
            let dataList = [];
            if (activityInfo) {
                dataList[0] = activityInfo.SearchSettingInfo;
                dataList[1] = activityInfo.UserInfoSetting;
                dataList[2] = activityInfo.ShareSettingInfo;
            }
            if (this.formModel && dataList.length > 0) {
                this.formDataCache.setCache(dataList, ['IsSearchPage', null, 'IsShare']);
                this.formModel.setValue(dataList);
            }
        },
        formInit(formModel) {
            this.formModel=formModel;
            formModel.groups[2].setValue({
                ShareLink: activityH5 + this.Activityid,
                ShareSmallLink: activityApplets + this.Activityid
            });
            this.formConfig.forEach((item, index) => {
                let con = item[0].formControl[0];
                this.valueChange({
                    controlConfig: con,
                    groupIndex: index,
                    value: con.defaultValue
                });
            });
            setTimeout(() => {
                this.setData();
            }, 20);
        },
        changeShow() {
            if (!this.Activityid) {
                this.$$errorMsg('请优先填写并保存基本信息');
            }
        },
        /**
         * 表单项的值发生改变
         * @param {Object} con 表单项的数据
         */
        valueChange(con) {
            let _that = this;
            let store = con;
            let formModel = _that.formModel;
            let formDataCache = this.formDataCache;

            // if (store.controlConfig.controlName === 'SearchKeyword') {
            //     _that.temp.SearchKeyword = store.value;
            // } else if (store.controlConfig.controlName === 'SearchImgUrl') {
            //     _that.temp.SearchImgUrl = store.value;
            // }
            // if (store.controlConfig.controlName === 'ShareDescribe') {
            //     _that.shareTemp.ShareDescribe = store.value;
            // } else if (store.controlConfig.controlName === 'ShareImgUrl') {
            //     _that.shareTemp.ShareImgUrl = store.value;
            // } else if (store.controlConfig.controlName === 'ShareLink') {
            //     _that.shareTemp.ShareLink = store.value;
            // } else if (store.controlConfig.controlName === 'ShareSmallImgUrl') {
            //     _that.shareTemp.ShareSmallImgUrl = store.value;
            // } else if (store.controlConfig.controlName === 'ShareSmallLink') {
            //     _that.shareTemp.ShareSmallLink = store.value;
            // } else if (store.controlConfig.controlName === 'ShareTitle') {
            //     _that.shareTemp.ShareTitle = store.value;
            // }
            let defaultData;
            switch (con && con.controlConfig && con.controlConfig.controlName) {
                case 'IsSearchPage':
                    let searchList = getSearchList();
                    setItem(con, searchList, 'IsSearchPage');
                    defaultData = formDataCache.getCache(formModel.value, ['IsSearchPage', null, 'IsShare']);
                    break;
                case 'IsShare':
                    let shareList = getShareList();
                    setItem(con, shareList, 'IsShare');
                    defaultData = formDataCache.getCache(formModel.value, ['IsSearchPage', null, 'IsShare']);
                    formModel.setValue(defaultData);
                    break;
                default:
                    formDataCache.setCache(formModel.value, ['IsSearchPage', null, 'IsShare']);
                    break;
            }
            defaultData && formModel.setValue(defaultData);
            // if (_that.formModel.value[0] && _that.formModel.value[0].IsSearchPage === '2') {
            //     _that.formModel.setValue([{
            //         SearchKeyword: _that.temp.SearchKeyword,
            //         SearchImgUrl: _that.temp.SearchImgUrl
            //     }]);
            // }
            // if (_that.formModel.value[2] && _that.formModel.value[2].IsShare === '2') {
            //     _that.formModel.setValue([{
            //         ShareDescribe: _that.shareTemp.ShareDescribe,
            //         ShareImgUrl: _that.shareTemp.ShareImgUrl,
            //         ShareLink: _that.shareTemp.ShareLink,
            //         ShareSmallImgUrl: _that.shareTemp.ShareSmallImgUrl,
            //         ShareSmallLink: _that.shareTemp.ShareSmallLink,
            //         ShareTitle: _that.shareTemp.ShareTitle
            //     }]);
            // }

            /**
             * 设置选项
             * @param {number} con 需要设置的control数据
             * @param {Array} list 修改项的列表
             * @param {string} _key 修改项的列表
             */
            function setItem(con, list, _key) {
                if (con.value === true) {
                    _that.formUpdata = _that.$$form.initFormData(list.slice(1));
                    formModel.get(con.groupIndex).merge(_that.formUpdata);
                    _that.$set(_key === 'IsShare' ? _that.shareList: _that.searchList, 'formControl', list);
                } else {
                    list.forEach((item, key) => {
                        if (key > 0) {
                            formModel.get(con.groupIndex).removeItem(item.controlName);
                        }
                    });
                    _that.$set(_key === 'IsShare' ? _that.shareList: _that.searchList, 'formControl', list.slice(0, 1));
                }
            }
        },
        submit() {
            if (this.Activityid === '' && this.Activityid.length===0) {
                this.$$errorMsg('请优先填写并保存基本信息');
                return;
            }
            let result = this.formModel.value;
            this.formModel.isSave = true;
            if (!this.$$validMsg(this.formModel)) {
                this.$emit('submit', this.formModel && this.formModel.value);
                this.formModel.isSend = true;
                // 基本信息接口
                this.$http.post(newActivityApi.SAVEACTIVITYDISPLAY, {
                    apiServer: 'apiServer',
                    isLoading: true,
                    data: Object.assign({}, result[0], result[1], result[2], {
                        ActivityId: this.Activityid
                    })
                    // data: {
                    //     ActivityId: this.Activityid,
                    //     IsSearchPage: result[0].IsSearchPage,
                    //     SearchKeyword: result[0].SearchKeyword,
                    //     SearchImgUrl: result[0].SearchImgUrl,
                    //     IsGeographical: result[1].IsGeographical,
                    //     IsLogin: result[1].IsLogin,
                    //     IsWeChatInfo: result[1].IsWeChatInfo,
                    //     IsShare: result[2].IsShare,
                    //     ShareTitle: result[2].ShareTitle,
                    //     ShareLink: result[2].ShareLink,
                    //     ShareImgUrl: result[2].ShareImgUrl,
                    //     ShareDescribe: result[2].ShareDescribe,
                    //     ShareSmallLink: result[2].ShareSmallLink,
                    //     ShareSmallImgUrl: result[2].ShareSmallImgUrl
                    // }
                }).subscribe((res) => {
                    // this.formModel.isSave=true;
                    let _res = res&&res.data;
                    let _rMessage = _res&& _res.ResponseMessage;
                    if (this.$filterResponseCode(_res)) {
                        this.$$saveMsg('保存成功!', {type: 'success'});
                    }
                    // else {
                    //     this.$$saveMsg('保存失败');
                    // }
                }, () => {}, () => {
                    setTimeout(() => {
                        this.formModel.isSend = false;
                    }, 300);
                });
            }
        }
    }
};
</script>

<style lang='scss'>
    @import "css/common/_var.scss";
    @import "css/common/_mixin.scss";
    @import "css/common/_iconFont.scss";

   .display-request {
        .showConfig{
            .form{
                .el-row{
                    display: block;
                    .control-filter-wrap {
                        width:70%;
                        margin-bottom: 15px;
                        .form-control-filter{
                            display: block;
                            margin-top: 15px;
                            &:first-child{
                                margin-top: 0px;
                            }
                        }
                    }
                    // .form-control-nameText{
                    //     height: 120px;
                    // }
                }
                &:first-child{
                    .form-control-usbNameText {
                        width: 100px;
                    }
                }
                &:nth-child(2){
                    .form-control-usbNameText {
                        width: 130px;
                    }
                }
                &:last-child{
                    .form-control-usbNameText {
                        width: 100px;
                    }
                }

            }

        }
        .el-col-config{
            height: 40px;
            text-align: right;
            line-height: 40px;
            margin-right: 15px;
        }
        .form-control-wraps.input-container {
            display: inline-flex;
        }
        .extend-control-items.form-control-leftPadding.display-img {
            margin: 0 20px 0 0;
        }
    }
</style>
