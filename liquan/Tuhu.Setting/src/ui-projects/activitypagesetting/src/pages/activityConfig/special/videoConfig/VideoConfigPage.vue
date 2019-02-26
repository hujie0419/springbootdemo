<template>
    <page-box
        class="video-config-pages"
        @submit="submit"
        :title="['模块类型：','特殊模块—视频']"
        @refresh="refreshData">
        <form-group @valueChange="valueChange" :defaultData="defaultData" :formConfig="formList" @formInit="formInit"></form-group>
        <el-row>
            <el-col :span="3">
                <div class="grid-content bg-purple-light">&nbsp;</div>
            </el-col>
            <el-col :span="18" class="upload-video">
                <a href="http://pm.tuhu.cn/videoupload" target="_blank" class="a-link">+上传视频</a>
            </el-col>
        </el-row>
    </page-box>
</template>

<script>
import PageBox from '../../commons/pageBox/PageBox';
import FormGroup from '../../commons/formGroup/FormGroup';
import TabPage from '../../tabPage/TabPage';
import { getConfig } from './config/videoConfigPage.config';
import apis from '../../commons/apis/special/specialApi';

export default {
    extends: TabPage,
    data() {
        let _that = this;
        return {
            formModel: null,
            formList: getConfig(_that),
            defaultData: null,
            formDataCache: this.$$form.initFormCache() // 缓存表单数据
        };
    },
    mounted() {
        this.getModuleData().subscribe(data => {
            this.defaultData = data.data.VideoModule || null;
            this.formDataCache.setCache([this.defaultData], 'VideoType');
        });
    },
    methods: {
        /**
         * 刷新数据
         */
        refreshData() {
            this.getModuleData().subscribe(data => {
                this.defaultData = data.data.VideoModule || null;
                this.formDataCache.setCache([this.defaultData], 'VideoType');
            });
        },
        formInit(formModel) {
            this.formModel = formModel;
        },
        valueChange(con) {
            let val = this.formModel.value;
            if (con.controlConfig && con.controlConfig.controlName === 'VideoType') {
                let formItem = this.formList && this.formList[3];
                let descList = formItem && formItem.descList;
                let item = descList && descList[1];
                let btnData = descList && descList[0];
                if (con.value == 'Vertical') {
                    // orientation
                    item.nameText = '限1080*1920';
                    btnData.orientation = 90;
                } else {
                    item.nameText = '限1920*1080';
                    btnData.orientation = 0;
                }
                let defaultData = this.formDataCache.getCache([val], {key: val.VideoType});

                this.formModel.setValue(Object.assign({
                    VideoImgUrl: ''
                }, defaultData[0]));
            } else {
                this.formDataCache.setCache([{
                    VideoImgUrl: val && val.VideoImgUrl
                }], {key: val.VideoType});
            }
        },
        submit() {
            this.formModel.isSave = true;
            let tagOption = this.tagOption && this.tagOption.data;
            if (!tagOption || !tagOption.ActivityId || !tagOption.ModuleId) {
                return;
            }
            if (!this.$$validMsg(this.formModel)) {
                let data = this.formModel.value;
                this.formModel.isSend = true;
                this.$http.post(apis.SaveVideoSetting, {
                    data: Object.assign({}, data, {
                        ActivityId: tagOption.ActivityId,
                        ModuleId: tagOption.ModuleId,
                        ModuleTypeCode: tagOption.SecondaryModuleTypeCode
                    })
                }).subscribe((res) => {
                    let _res = res&&res.data;
                    let _rMessage = _res&& _res.ResponseMessage;
                    if (this.$filterResponseCode(_res)) {
                        this.$$saveMsg(_rMessage, {type: 'success'});
                        // this.callBackUpdata({tabname: 'VideoConfigPage', pageId: _value.activityId});
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
        FormGroup
    }
};
</script>

<style lang="scss">
.video-config-pages{
    .upload-video{
        margin-left: 15px;
    }
}
</style>

