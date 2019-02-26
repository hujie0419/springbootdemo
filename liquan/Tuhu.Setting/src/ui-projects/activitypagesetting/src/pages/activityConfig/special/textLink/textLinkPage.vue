<template>
    <page-box
        class="textLink-pages"
        :title="['模块类型：','特殊模块—轮播文字链']"
        @submit='submit'
        @refresh="refreshData">
        <form-group-panel
            form-type='array'
            :addData="addData()"
            :isDefaultAddItem="true"
            :formConfig="formConfig"
            :defaultData="defaultData || [addData()]"
            :get-config='getTextLinkConfig'
            @formInit="formInit"
            :deleteColums="deleteColums"
            @formConfigUpdate='formConfigUpdate'></form-group-panel>
    </page-box>
</template>

<script>
import PageBox from '../../commons/pageBox/PageBox';
import TabPage from '../../tabPage/TabPage';
import { getTextLinkConfig } from './config/textLinkPage.config';
import FormGroupPanel from '../../commons/formGroupPanel/FormGroupPanel';
import apis from '../../commons/apis/special/specialApi';

export default {
    extends: TabPage,
    data() {
        let _that = this;
        return {
            indata: null,
            formModel: null,
            formConfig: [ getTextLinkConfig(1) ],
            getTextLinkConfig: (index) => {
                return getTextLinkConfig(index);
            },
            defaultData: null,
            deleteColums: [],
            addData() {
                return Object.assign({
                    OperationType: 'Add',
                    PKID: '0'
                }, ((_that.tagOption && _that.tagOption.data[0])||{}));
            }
        };
    },
    created() {
        this.selectDefaultData();
    },
    methods: {
        /**
         * 刷新数据
         */
        refreshData() {
            this.selectDefaultData();
        },
        formInit(formModel) {
            this.formModel = formModel;
        },
        formConfigUpdate (formConfig) {
            this.formConfig = formConfig;
        },
        selectDefaultData() {
            let tagOption = this.tagOption && this.tagOption.data;
            if (!tagOption || !tagOption.ActivityId || !tagOption.ModuleId) {
                return;
            }
            this.$http.post(apis.GetWritingts, {
                apiServer: 'apiServer',
                isLoading: true,
                data: {
                    Activityid: tagOption.ActivityId,
                    ModuleId: tagOption.ModuleId
                }
            }).subscribe(data => {
                let _data = data && data.data;
                let _writingts = _data && _data.Writingts;
                this.defaultData = (_writingts.map(item => {
                    item.OperationType = 'Edit';
                    return item;
                })) || null;
            });
        },
        submit() {
            let tagOption = this.tagOption && this.tagOption.data;
            if (!tagOption || !tagOption.ActivityId || !tagOption.ModuleId) {
                return;
            }
            this.formModel.isSave = true;
            if (!this.$$validMsg(this.formModel)) {
                let tagOption = this.tagOption && this.tagOption.data;
                if (!tagOption || !tagOption.ActivityId || !tagOption.ModuleId) {
                    return;
                }
                let _value = this.formModel.value;
                const getData={
                    ActivityId: tagOption.ActivityId,
                    ModuleId: tagOption.ModuleId,
                    Writingts: _value
                };
                this.deleteColums.forEach(item => {
                    if (item.OperationType !== 'Add') {
                        item.OperationType = 'Delete';
                        getData.Writingts.push(item);
                    }
                });
                this.formModel.isSend = true;
                this.$http.post(apis.SaveWritingts, {
                    data: getData
                }).subscribe((res) => {
                    let _res = res&&res.data;
                    let _rMessage = _res&& _res.ResponseMessage;
                    if (this.$filterResponseCode(_res)) {
                        this.$$saveMsg(_rMessage, {type: 'success'});
                        this.deleteColums = [];
                        this.selectDefaultData();
                        if (this.tagOption && this.tagOption.callBackUpdate instanceof Function) {
                            this.tagOption.callBackUpdate({tabName: this.tagOption.name, activityid: tagOption.ActivityId});
                        }
                        // this.callBackUpdata({tabname: 'textLinkPage', pageId: _value.activityId});
                    }
                    // else {
                    //     this.$$saveMsg(res.ResponseMessage, {type: 'error'});
                    // }
                }, () => {}, () => {
                    setTimeout(() => {
                        this.formModel.isSend = false;
                    }, 300);
                });
            }
        }
    },
    components: {
        PageBox,
        FormGroupPanel
    }
};
</script>

<style lang="scss">
@import 'css/common/_var.scss';
.textLink-pages{
    .form-control{
        height: 40px;
        display: flex;
        .ext-fix{
            margin-left: 5px;
        }
    }
}
</style>
