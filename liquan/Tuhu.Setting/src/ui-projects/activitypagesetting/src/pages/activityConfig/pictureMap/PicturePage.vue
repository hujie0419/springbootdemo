<template>
  <!-- <div>
    <picture-map :title='title' :form-config='formConfig' @formInit="formInit" @add_Control="addFormItem"></picture-map>
  </div> -->
    <page-box
        class="picture-page"
        @submit="submit"
        v-if="formConfig"
        :title="['模块类型：','图片/链接 - 1行' + (formConfig.length || 1) + '列']"
        @refresh="refreshData">
        <form-group-card
            :formType="'array'"
            :form-config='formConfig'
            :addData="addData"
            @formInit="formInit"
            @editControl="editFormItem"
            @valueChange="formItemChange"
            @selectFocus="selectFocus">
        </form-group-card>
    </page-box>
</template>

<script>
/* eslint-disable max-lines */
import TabPage from '../tabPage/TabPage';
import PageBox from '../commons/pageBox/PageBox';
import FormGroupCard from '../commons/formGroupCard/FormGroupCard';
import { getChildList, cardItemData } from './config/picturePage.config';
import apis from '../commons/apis/pictureMap/pictureMapApi';
// import { getGroupId } from '../commons/groupBookingId/getGroupId';
import { picturePageGetGroupId } from '../commons/picturePageGetGroupId/picturePageGetGroupId';

export default {
    extends: TabPage,
    data() {
        let _that = this;
        return {
            formConfig: null,
            // defaultData: null,
            formModel: null,
            formUpdata: null,
            deleteColums: [],
            addData: {isNew: true},
            groups: null,
            // tempData: null,
            formDataCache: this.$$form.initFormCache(), // 缓存表单数据
            tempRowData: null,
            groupPID: {}
        };
    },
    components: {
        PageBox,
        FormGroupCard
    },
    created() {
        this.getExistValue();
    },
    methods: {
        /**
         * 刷新数据
         */
        refreshData() {
            this.getExistValue();
        },
        /**
         * 表单初始化完成的事件
         * @param {formModelule} formModel formModelule
         */
        formInit(formModel) {
            this.formModel = formModel;
            let len = formModel.groups && formModel.groups.length;
            for (let i = 0; i < len; i++) {
                let item = this.formConfig[i][0];
                this.formItemChange({
                    controlConfig: item,
                    groupIndex: i,
                    value: item.defaultValue
                });
            }
            this.groups = formModel.groups;
            let defauleData = this.formDataCache.getCache(formModel.value, 'LinkType');
            formModel.setValue(defauleData);
        },
        selectFocus(evt) {
            const {formConfig, formModel} = evt;
            const pid = formModel.value.LinkPid;
            // this.groupGetGroupId(pid, formConfig, formModel);
            picturePageGetGroupId(formModel, this.$http, formConfig, this, evt.groupIndex);
        },
        /**
         * control数据发生改变的事件
         * @param {Object} evt 数据
         */
        formItemChange(evt) {
            let _that = this;
            if (typeof evt.groupIndex === 'number' && evt.controlConfig.controlName === 'LinkType') {
                this.lock = true;
                let item = getChildList(evt.value, _that.$http, _that, evt.groupIndex);
                let arrConfig = _that.formConfig && _that.formConfig[evt.groupIndex];
                let group = _that.formModel.get(evt.groupIndex);
                let res = arrConfig || [];
                if (arrConfig.length > 1) {
                    let config = arrConfig[0];
                    res = config && [config];
                    let control = group.get(config.controlName);
                    let controlIsNew = group.get('isNew');
                    let controlPKID = group.get('PKID');
                    group.clear();
                    group.setItem(control, config.controlName);
                    if (controlIsNew) {
                        group.setItem(controlIsNew, 'isNew');
                    }
                    if (controlPKID) {
                        group.setItem(controlPKID, 'PKID');
                    }
                    this.$set(_that.formConfig, evt.groupIndex, res);
                }
                if (item.length > 0) {
                    if (evt.value === 'Maintenance') {
                        _that.getMaintenanceServices().then(options => {
                            this.$set(item[1], 'list', options);
                            // group.setItem('LinkMaintenanceServiceName', [options[0].value]);
                            this.$set(item[1].defaultValue, options[0].value);
                            // item[1].list = options;
                            // item[1].defaultValue = [options[0].value];
                        });
                    }
                    this.formUpdata = _that.$$form.initFormData(item);
                    group.merge(this.formUpdata);
                    // group.merge(_that.$$form.initFormData(item));
                    res = res.concat(item);
                    this.$set(_that.formConfig, evt.groupIndex, res);
                }
                setTimeout(() => {
                    let defauleData = this.formDataCache.getCache(_that.formModel.value, 'LinkType');
                    _that.formModel.setValue(defauleData);
                    this.lock = false;
                }, 20);
            } else {
                if (!this.lock) {
                    this.formDataCache.setCache(_that.formModel.value, 'LinkType');
                }
            }

            // 如果改变的是拼团的pid，把groupId和价格清空
            // if (evt.controlConfig.controlName === 'LinkPid') {
            //     // console.log('pid change');
            //     this.groupPidChange(evt.groupIndex);
            // }

            // 如果改变的是拼团的groupId，显示对应的价格
            if (evt.controlConfig.controlName === 'LinkProductGroupId' && evt.value) {
                this.tempRowData && this.tempRowData.forEach(newItem => {
                    if (newItem.GroupId === evt.value) {
                        this.formModel.groups[evt.groupIndex].setValue({LinkCommodityPrice: newItem.Price});
                    }
                });
            }
        },
        /**
         * 拼团的pid改变时，把groupId和价格清空
         *
         * @param {Number} index 改变的是第几列
         * @returns {Function} Function
         */
        // groupPidChange(index) {
        //     // return () => {
        //     clearTimeout(this.timer);
        //     this.timer = setTimeout(() => {
        //         console.log('delay');
        //         this.timer = null;
        //     }, 2000);
        //     // };
        // },
        // groupGetGroupId(pid, formConfig, formModel) {
        //     console.log(pid);
        //     getGroupId(this.$http, pid).then(res => {
        //         formConfig.formControl[1].list = res.listArray;
        //         formModel.setValue({LinkGroupId: res.tempRowData[0].GroupId});
        //         formModel.setValue({GroupPrice: res.tempRowData[0].Price});
        //         this.tempRowData = res.tempRowData;
        //     }).catch(e => {
        //         this.$$errorMsg(e.message);
        //     });
        // },
        // groupGetGroupId () {

        // },
        /**
         * 增加/减少一项
         * @param {object} options 当前参数
         * @param {number} options.index 当前Group的索引
         * @param {'add'|'minus'} options.type 当前Group的索引
         */
        editFormItem({index, type}) {
            let _that = this;
            _that.formConfig = _that.formConfig || [];
            switch (type) {
                case 'add':
                    if (_that.formConfig.length < 4) {
                        let item = cardItemData();
                        this.formDataCache.addItem(index);
                        _that.formConfig.splice(index + 1, 0, item);
                        this.formUpdata = this.$$form.initFormData(item, {isNew: true});
                        _that.formModel.setItem(this.formUpdata, index + 1);
                        _that.tempData = _that.formModel.value;
                        this.formItemChange({
                            controlConfig: item[0],
                            groupIndex: index + 1,
                            value: item[0].defaultValue
                        });
                    }
                    break;
                case 'minus':
                    if (_that.formConfig.length > 1 && _that.formConfig.length > index) {
                        this.isDel = true;
                        _that.deleteColums.push(_that.formModel.get(index).value);
                        this.formDataCache.removeItem(index);
                        _that.formConfig.splice(index, 1);
                        let value = _that.formModel.value;
                        _that.formModel.removeItem(index);
                        this.tempData = _that.formModel.value;
                    }
                    break;

                default:
                    break;
            }
        },

        /**
         * 获取已存在的数据
         */
        getExistValue() {
            this.formConfig = null;
            this.formDataCache.clear();
            let tagOption = this.tagOption && this.tagOption.data;
            if (!tagOption || !tagOption.ActivityId || !tagOption.ModuleId) {
                this.$$errorMsg('获取页面信息失败', {type: 'error'});
                return;
            }
            this.$http.post(apis.GetImgageLinkColumns, {
                apiServer: 'apiServer',
                isLoading: true,
                data: {
                    ActivityId: tagOption.ActivityId,
                    ModuleId: tagOption.ModuleId
                }
            }).subscribe(data => {
                // setTimeout(() => {
                const _that = this;
                _that.formConfig = _that.formConfig || [];
                let defaultData = [];
                // 切换tab的时候保留值
                // _that.tempData = _that.defaultData;
                let _data=data&&data.data;
                let _ImageColumns = _data&& _data.ImageLinkColumns;
                _ImageColumns.forEach((element, index) => {
                    let item = cardItemData();
                    _that.$set(item[0], 'defaultValue', element.LinkType);
                    if (element.LinkType && element.LinkType === 'GroupBuying') {
                        this.groupPID[index] = element.LinkPid || '';
                    }
                    item.push({
                        controlName: 'PKID',
                        defaultValue: element.PKID
                    });
                    element.LinkMaintenanceServiceId = ((element.LinkMaintenanceServiceId || '') + '').split(',');
                    _that.formConfig.push(item);
                    defaultData.push(element);
                });
                if (!_ImageColumns || _ImageColumns.length === 0) {
                    let config = cardItemData();
                    config.push({
                        controlName: 'isNew',
                        defaultValue: true
                    });
                    _that.formConfig = [config];
                }
                this.formDataCache.setCache(defaultData, 'LinkType');
            });
            // }, 0);
        },
        /**
         * 获取保养服务数据
         *
         * @returns {Array} options select的option
         */
        getMaintenanceServices() {
            return new Promise((resolve, reject) => {
                this.$http.post(apis.GetMaintenanceServices, {
                    apiServer: 'apiServer',
                    isLoading: true,
                    data: {}
                }).subscribe(data => {
                    let options = [];
                    const listData = data.data.MaintenanceServiceList;
                    listData.forEach(item => {
                        options.push({
                            nameText: item.MaintenanceName,
                            value: item.MaintenanceId
                        });
                        resolve(options);
                    });
                });
            });
        },
        submit() {
            let tagOption = this.tagOption && this.tagOption.data;
            this.formModel.isSave = true;
            console.log(this.formModel.value);
            if (!tagOption || !tagOption.ActivityId || !tagOption.ModuleId) {
                this.$$errorMsg('获取页面信息失败', {type: 'error'});
                return;
            }
            if (!this.$$validMsg(this.formModel)) {
                this.$emit('submit', this.formModel && this.formModel.value);
                let data = {
                    ActivityId: tagOption.ActivityId,
                    ModuleId: tagOption.ModuleId,
                    ImageLinkColumnList: []
                };
                const formData = this.formModel.value;
                formData.forEach(element => {
                    if (element.LinkMaintenanceServiceId instanceof Array) {
                        element.LinkMaintenanceServiceId = element.LinkMaintenanceServiceId.join(',');
                    }
                    element.OperationType = element.isNew === true ? 'Add' : 'Edit';
                    (element.LinkIsTitle === null) && (element.LinkIsTitle = false);
                    element.PKID = element.PKID || '0';
                    data.ImageLinkColumnList.push(element);
                });
                this.deleteColums.forEach(element => {
                    if (!element.isNew) {
                        if (element.LinkMaintenanceServiceId instanceof Array) {
                            element.LinkMaintenanceServiceId = element.LinkMaintenanceServiceId.join(',');
                        }
                        element.OperationType = 'Delete';
                        (element.LinkIsTitle === null) && (element.LinkIsTitle = false);
                        data.ImageLinkColumnList.push(element);
                    }
                });

                this.formModel.isSend = true;
                this.$http.post(apis.SaveImgageLinkColumns, {
                    apiServer: 'apiServer',
                    isLoading: true,
                    data: data
                }).subscribe(res => {
                    let _res = res&&res.data;
                    let _rMessage = _res&& _res.ResponseMessage;
                    if (this.$filterResponseCode(_res)) {
                        if (_res!==null) {
                            this.$$saveMsg(_rMessage, {type: 'success'});
                            this.deleteColums = [];
                            this.getExistValue();
                            // this.formModel.value.forEach((item, index) => {
                            //     if (item.isNew) {
                            //         this.formModel.get(index).get('isNew').setValue(false);
                            //     }
                            // });
                            this.tagOption.callBackUpdate(tagOption.ActivityId);
                        } else {
                            this.$$errorMsg(_rMessage, {type: 'error'});
                        }
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
// .picture-page{
//     .form-group-cards-row .el-col-4{
//         width: 100px;
//     }
// }
</style>

