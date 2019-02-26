<template>
    <page-box
        class="slide-coupon-content"
        :title="['模块类型：','特殊优惠券—滑动优惠券']"
        @submit='submit'
        @refresh="refreshData">
        <form-group-panel
            form-type='array'
            :isDefaultAddItem="true"
            :addData="addData()"
            :defaultData="defaultData || [addData()]"
            :formConfig="formList"
            @formInit="formInit"
            :get-config="formItemConfig"
            :deleteColums="deleteColums"
            :maxPanel='15'></form-group-panel>
    </page-box>
</template>
<script>
import FormList from '../commons/formList/FormList';
import TabPage from '../tabPage/TabPage';
import PageBox from '../commons/pageBox/PageBox';
import AddMinus from '../commons/formList/addMinusControl/AddMinusControl';
import FormGroupPanel from '../commons/formGroupPanel/FormGroupPanel';
import { formItemConfig } from './config/slideConpon.config';
import * as apiConfig from '../commons/apis/coupon/slideConponapi.js';
export default {
    extends: TabPage,
    data() {
        let _that = this;
        return {
            formModel: null,
            defaultData: null,
            deleteColums: [],
            formList: [formItemConfig(1, _that.$http)],
            formItemConfig: (index) => {
                return formItemConfig(index, _that.$http);
            },
            addData() {
                return Object.assign({
                    OperationType: 'Add',
                    PKID: '0'
                }, ((_that.tagOption && _that.tagOption.data)||{}));
            }
        };
    },
    components: {
        FormList, AddMinus, PageBox, FormGroupPanel
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
        //  默认查询优惠券信息
        selectDefaultData() {
            let tagOption = this.tagOption && this.tagOption.data;
            if (!tagOption || !tagOption.ActivityId || !tagOption.ModuleId) {
                return;
            }
            this.$http.post(apiConfig.GetSlidingCoupon, {
                apiServer: 'apiServer',
                isLoading: true,
                data: {
                    ActivityId: tagOption.ActivityId,
                    ModuleId: tagOption.ModuleId
                }
            }).subscribe(data => {
                let _data = data&&data.data;
                let _coupont=_data&&_data.SlidingCoupons;
                this.defaultData = (_coupont.map(item => {
                    item.OperationType = 'Edit';
                    return item;
                })) || null;
            });
        },
        formInit(formModel) {
            this.formModel = formModel;
        },

        // 提交保存优惠券信息
        submit() {
            if (!this.$$validMsg(this.formModel)) {
                let tagOption = this.tagOption && this.tagOption.data;
                if (!tagOption || !tagOption.ActivityId || !tagOption.ModuleId) {
                    return;
                }
                let _value = this.formModel.value;
                const getData={
                    ActivityId: tagOption.ActivityId,
                    ModuleId: tagOption.ModuleId,
                    SlidingCouponList: _value
                };
                this.deleteColums.forEach(item => {
                    item.OperationType = 'Delete';
                    getData.SlidingCouponList.push(item);
                });
                this.formModel.isSend = true;
                this.$http.post(apiConfig.SaveSlidingCoupon, {
                    apiServer: 'apiServer',
                    isLoading: true,
                    data: getData
                }).subscribe(res => {
                    let _res = res&&res.data;
                    let _rMessage = _res&& _res.ResponseMessage;
                    if (this.$filterResponseCode(_res)) {
                        this.$$saveMsg(_rMessage, {type: 'success'});
                        this.selectDefaultData();
                        if (this.tagOption && this.tagOption.callBackUpdate instanceof Function) {
                            this.tagOption.callBackUpdate({tabName: this.tagOption.name, activityid: tagOption.ActivityId});
                        }
                    } else {
                        this.$$errorMsg(_rMessage, {type: 'error'});
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
@import 'css/common/_var.scss';
.slide-coupon-content{
//   background: $colorf;
//   padding: 20px;
//   .title{
//     font-weight: bold;
//   }
//   .el-card{
//       margin-bottom: 10px;
//   }
//   .btn{
//     width: 100px;
//     margin: 0 auto;
//     .el-button{
//       margin: 10px auto;
//       width: 100%;
//     }
//   }
    .form-control{
        height: 40px;
        display: flex;
        .ext-fix{
            margin-left: 5px;
        }
    }
}
</style>
<style lang="scss">
@import 'css/common/_var.scss';
.form{
  padding: 30px 0 20px;
  border-top: 1px $color6 dashed;
  &:first-of-type{
    border: 0;
  }
  // position: relative;
  // &:after{
  //   content: '+';
  //   width: 30px;
  //   height: 30px;
  //   font-size: 24px;
  //   position: absolute;
  //   top: 10%;
  //   right: 15%;
  //   cursor: pointer;
  // }
}
</style>

