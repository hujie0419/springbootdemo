<template>
    <page-box
        class="footTabsPage-pages"
        :title="['模块类型：','特殊模块—底部tab']"
        @submit='submit'
        @refresh="refreshData">
        <el-row
            class="footTabsPage-bgcolor"
            :gutter="20">
            <el-col :span='20'>
                <form-control :form-model="bgColorFormModel" :control-config="formConfigBgColor[0]"></form-control>
            </el-col>
            <el-col :span='4'>
            </el-col>
        </el-row>
        <form-group-panel
            form-type='array'
            :isDefaultAddItem="true"
            :addData="addData()"
            :defaultData="defaultData"
            :formConfig="formConfig"
            :min-panel='2'
            :get-config='getFootTabsPageConfig'
            :delFormItem="delFormItem"
            @valueChange="valueChange"
            @formInit="formInit"
            :deleteColums="deleteColums"
            @extendClick="valueChange($event, true)"
            @formConfigUpdate='formConfigUpdate'>
        </form-group-panel>
    </page-box>
</template>

<script>
import PageBox from '../../commons/pageBox/PageBox';
import TabPage from '../../tabPage/TabPage';
import { getFootTabsPageConfig, getSelfLink, getBgColor } from './config/footTabsPage.config';
import FormGroupPanel from '../../commons/formGroupPanel/FormGroupPanel';
import FormControl from '../../commons/formList/formControl/FormControl';
import FormGroup from '../../commons/formGroup/FormGroup';
import apis from '../../commons/apis/special/specialApi';
import WindowForm from '../../commons/windowForm/WindowForm';

export default {
    extends: TabPage,
    components: {
        PageBox,
        FormGroupPanel,
        FormGroup,
        FormControl
    },
    data() {
        let _that = this;
        return {
            // isDefault: false,
            addData() {
                return Object.assign({
                    OperationType: 'Add',
                    PKID: '0',
                    isNew: true
                });
            },
            lock: false,
            defaultData: null,
            deleteColums: [],
            formModel: null,
            formConfig: [ getFootTabsPageConfig(1, _that.$http), getFootTabsPageConfig(2, _that.$http) ],
            formConfigdia: getSelfLink(),
            formConfigBgColor: getBgColor(),
            bgColorFormModel: null,
            getFootTabsPageConfig(index, that) {
                if (!_that.lock) {
                    _that.popupFormDataCache.addItem(index - 2); // 向缓存里增加一项数据
                }
                return getFootTabsPageConfig(index, that.$http);
            },

            delFormItem(index) {
                _that.popupFormDataCache.removeItem(index); // 从缓存里减一项数据
            },
            popupFormDataCache: this.$$form.initFormCache() // 缓存表单数据
        };
    },

    created() {
        this.selectDefaultData();
        this.bgColorFormModel = this.$$form.initFormData(this.formConfigBgColor);
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
        /**
         * 默认查询底部Tab信息
         */
        selectDefaultData() {
            let _that = this;
            let tagOption = this.tagOption && this.tagOption.data;
            if (!tagOption || !tagOption.ActivityId || !tagOption.ModuleId) {
                return;
            }
            _that.lock = true;
            let popupFormDataCache = _that.popupFormDataCache;
            this.$http.post(apis.GetBottomTabSetting, {
                apiServer: 'apiServer',
                isLoading: true,
                data: {
                    Activityid: tagOption.ActivityId,
                    ModuleId: tagOption.ModuleId
                }
            }).subscribe(data => {
                let _data = data && data.data;
                let _bottomtab = _data && _data.BottomTabSettingList;
                let bgColor = _data && _data.TabBackGroundColor;
                bgColor && this.bgColorFormModel.setValue({'TabBackGroundColor': bgColor});
                this.defaultData = (_bottomtab.map((item, index) => {
                    item.OperationType = 'Edit';
                    // _that.isDefault = (item.BottonLinkType === 'Customize');
                    let customize = { // 设置自定义链接
                        AppletsLink: item.AppletsLink,
                        MobileLink: item.MobileLink,
                        PcLink: item.PcLink,
                        AppLink: item.AppLink,
                        QuickAppLink: item.QuickAppLink
                    };
                    let channelPage = item.ChannelPageLink; // 频道页面
                    let linkActivityId = item.LinkActivityId; // 活动页ID
                    let data = [];

                    item.tabLinkType = {
                        select: item.BottonLinkType
                    };

                    switch (item.BottonLinkType) {
                        case 'Customize':
                            item.tabLinkType.value = customize;
                            data[index] = customize;
                            break;
                        case 'ChannelPage':
                            item.tabLinkType.value = channelPage;
                            data[index] = item.tabLinkType;
                            break;
                        case 'ActivityPage':
                            item.tabLinkType.value = linkActivityId;
                            data[index] = item.tabLinkType;
                            break;
                    }
                    // 缓存默认数据
                    popupFormDataCache.setCache(data, {
                        key: item.BottonLinkType
                    });
                    return item;
                })) || [this.addData(), this.addData()];
                if (this.defaultData && this.defaultData.length < 2) {
                    this.defaultData.push(this.addData());
                }
                if (this.defaultData && this.defaultData.length < 2) {
                    this.defaultData.push(this.addData());
                }
                setTimeout(() => {
                    _that.lock = false;
                }, 0);
            });
        },

        /**
         * 设置自定义链接
         * @param {Object} con 表单控件数据
         * @param {boolean} isEdit 编辑
         */
        valueChange (con, isEdit) {
            let _that = this;
            let formModel = _that.formModel;
            let nowVal = con.value && con.value.value;
            let select = con.value && con.value.select;
            let popupFormDataCache = _that.popupFormDataCache;
            let cacheData = (_that.popupFormDataCache.getCache(formModel.value, {key: select})||[])[con.groupIndex];
            let temp = [];
            switch (select) {
                case 'ActivityPage': // 活动ID
                case 'ChannelPage': // 频道页面
                    if (!nowVal && cacheData) {
                        let tabLinkType = cacheData.tabLinkType;
                        if (select === (tabLinkType && tabLinkType.select)) {
                            temp[con.groupIndex] = cacheData;
                            formModel.setValue(temp);
                        }
                    } else {
                        // 存缓存数据
                        let _temp = [];
                        // 只缓存tabLinkType一项
                        temp[con.groupIndex] = {
                            tabLinkType: formModel.value[con.groupIndex] && formModel.value[con.groupIndex].tabLinkType
                        };
                        popupFormDataCache && popupFormDataCache.setCache(temp, {key: select});
                    }
                    break;
                case 'Customize': // 自定义链接
                    if (!nowVal || isEdit) {
                        this.$$Popup.open(WindowForm, {
                            props: {
                                onsetChange: this.onsetChange
                            },
                            data: {
                                formList: _that.formConfigdia.map(item => {
                                    item.defaultValue = cacheData && cacheData[item.controlName];
                                    return item;
                                }),
                                title: '配置自定义链接'
                            },
                            wrapCla: 'alignCla form_popup', // 最外层追加的Class名
                            alignCla: 'centerMiddle', // ''|'bottom'|'top'|'fullScreen'|'centerMiddle',
                            transitionCls: 't_scale' // , // ''|'t_toUp'|'t_scale'|'t_toBottom'|'t_toLeft';
                        }).then((back) => {
                            let control= formModel.get(con.groupIndex).get(con.controlConfig.controlName);

                            if (back.type==='cancel') {
                                if (!control.value || !control.value.value) {
                                    control.value = {};
                                }
                            } else {
                                control.setValue({
                                    select: select,
                                    value: back
                                });
                                temp[con.groupIndex] = back;
                                _that.popupFormDataCache.setCache(temp, {key: 'Customize'});
                            }
                        });
                    }
                    break;

                default:
                    break;
            }
        },
        submit() {
            this.formModel.isSave = true;
            let _value = this.formModel.value;
            let bgColor = this.bgColorFormModel.value;
            if (!this.$$validMsg(this.formModel) && !this.$$validMsg(this.bgColorFormModel)) {
                _value.forEach(element => {
                    element.OperationType = element.isNew === true ? 'Add' : 'Edit';
                    element.PKID = element.PKID || '0';
                });

                let data = _value.map(item => {
                    let res = Object.assign({}, item, { // 清除默认自定义链接
                        AppletsLink: '',
                        MobileLink: '',
                        PcLink: ''
                    });
                    let tabLinkType = item && item.tabLinkType;
                    let val = tabLinkType && tabLinkType.value;
                    res.BottonLinkType = tabLinkType && tabLinkType.select;
                    switch (tabLinkType && tabLinkType.select) {
                        case 'ChannelPage':
                            res.ChannelPageLink = val;
                            break;
                        case 'ActivityPage':
                            res.LinkActivityId = val;
                            break;
                        case 'Customize':
                            res = Object.assign(res, val);

                            break;
                    }
                    delete res.tabLinkType;
                    return res;
                });

                let tagOption = this.tagOption && this.tagOption.data;
                delete data.tabLinkType;
                // 设置提交数据
                const getData = Object.assign({
                    ActivityId: (tagOption && tagOption.ActivityId) || '',
                    ModuleId: (tagOption && tagOption.ModuleId) || '',
                    BottomTabSettingList: data
                }, bgColor);

                // 设置删项的数据
                this.deleteColums.forEach(item => {
                    item.OperationType = 'Delete';
                    if (item && !item.isNew) {
                        getData.BottomTabSettingList.push(item);
                    }
                });
                this.formModel.isSend = true;
                this.$http.post(apis.SaveBottomTabSetting, {
                    data: getData
                }).subscribe((res) => {
                    let _res = res&&res.data;
                    let _rMessage = _res&& _res.ResponseMessage;
                    if (this.$filterResponseCode(_res)) {
                        this.$$saveMsg(_rMessage, {type: 'success'});
                        this.deleteColums = [];
                        this.selectDefaultData();
                        this.formModel.value.forEach((item, index) => {
                            if (item.isNew) {
                                this.formModel.get(index).get('isNew').setValue(false);
                            }
                        });
                        this.tagOption.callBackUpdate(tagOption.ActivityId);
                    }
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

<style lang="scss">
.footTabsPage-pages{
    .form-control-usbNameText{
        width: 80px;
    }
    .footTabsPage-bgcolor {
        .form-control-nameText {
            text-align: right;
        }
        .control-filter-wrap {
            margin-left: 20px;
        }
    }
    .extend-control-items.form-control-leftPadding.foottab-img {
        margin-right: 5px;
    }
}
</style>
