<template>
<page-box
    @submit="submit"
    :title="['模块类型：','图片/链接 - 1图3产品']"
    @refresh="refreshData">
    <form-group
        :defaultData="defaultData"
        :formType="'array'"
        :form-config='formConfig'
        @formInit="formInit"></form-group>
</page-box>
</template>

<script>
import TabPage from '../tabPage/TabPage';
import FormGroup from '../commons/formGroup/FormGroup';
import PageBox from '../commons/pageBox/PageBox';
import { getProductConfig } from './config/productPage.config';
import apis from '../commons/apis/pictureMap/pictureMapApi';

export default {
    extends: TabPage,
    components: {
        FormGroup,
        PageBox
    },
    data() {
        let _that = this;
        return {
            defaultData: null,
            formModel: null,
            formConfig: getProductConfig(_that.$http, _that)
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
        selectDefaultData() {
            let tagOption = this.tagOption && this.tagOption.data;
            if (!tagOption || !tagOption.ActivityId || !tagOption.ModuleId) {
                return;
            }
            this.$http.post(apis.GetImgageLinkProducts, {
                apiServer: 'apiServer',
                isLoading: true,
                data: {
                    Activityid: tagOption.ActivityId,
                    ModuleId: tagOption.ModuleId
                }
            }).subscribe(data => {
                let _data = data && data.data;
                let _products = _data && _data.ImageLinkProducts;
                let _url = _data && _data.ImageLinkProductUrl;

                if (_products) {
                    _products.unshift({'ImageLinkProductUrl': _url} || {});
                    this.defaultData = (_products.map(item => {
                        item.OperationType = 'Edit';
                        return item;
                    })) || null;
                }
            });
        },
        submit() {
            let tagOption = this.tagOption && this.tagOption.data;
            if (!tagOption || !tagOption.ActivityId || !tagOption.ModuleId || !tagOption.SecondaryModuleTypeCode) {
                return;
            }
            this.formModel.isSave = true;
            if (!this.$$validMsg(this.formModel)) {
                let _temvalue = this.formModel.value;
                let _value ={};
                _value = Object.assign(_value, _temvalue[0], {
                    ActivityId: tagOption.ActivityId,
                    ModuleId: tagOption.ModuleId,
                    ModuleTypeCode: tagOption.SecondaryModuleTypeCode
                });
                // _value['ActivityId'] = _temvalue[0].ActivityId;
                _value['ImageLinkProducts'] = _temvalue.splice(1, _temvalue.length);
                _value['ImageLinkProducts'] && _value['ImageLinkProducts'].forEach(element => {
                    element.PKID = element.PKID || '0';
                    element.OperationType = element.OperationType || 'Add';
                });
                this.formModel.isSend = true;
                this.$http.post(apis.SaveImgageLinkProducts, {
                    data: _value
                }).subscribe((res) => {
                    let _res = res&&res.data;
                    let _rMessage = _res&& _res.ResponseMessage;
                    if (this.$filterResponseCode(_res)) {
                        if (_res!==null) {
                            this.$$saveMsg(_rMessage, {type: 'success'});
                            this.selectDefaultData();
                        } else {
                            this.$$errorMsg(_rMessage, {type: 'error'});
                        }
                        // this.callBackUpdata({tabname: 'ProductPage', pageId: _value.activityId});
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
    }
};
</script>

<style lang='scss'>
@import 'css/common/_var.scss';
@import 'css/common/_mixin.scss';
@import 'css/common/_iconFont.scss';

.general-pages {
  .general-page-title {
    display: flex;
    align-items: center;
    padding: 0 0 20px 20px;
    font-weight: bold;
  }
  .general-page-title-left {
    flex: 0 0 65px;
    margin-right: 15px;
    text-align: right;
  }
  .general-page-title-right {
    display: flex;
    flex: 1;
    align-items: center;
  }

  .save-btn {
    text-align: center;
    margin-top: 30px;
    .el-button {
      padding: 12px 40px;
    //   background: rgba(22, 155, 213, 1);
      color: $colorf;
    }
  }
  .input-container {
    display: inline-flex;
}
}
</style>
