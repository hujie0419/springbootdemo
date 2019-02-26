<template>
    <div class='seperate-car-pages'>
        <page-box
            :title='title'
            :showBtn="false"
            @submit='submit'
            @refresh="refreshData">
            <el-card>
                <form-top
                    @dataChange="dataChange"
                    :defaultData="defaultData">
                </form-top>
                <div class="submit-btn">
                    <el-button type="primary" @click="submit()">保存</el-button>
                </div>
            </el-card>
            <el-card>
                <div class="carVehicle" ref="hook_carVehicle">
                    <form-group
                        class="add-carVehicle"
                        @formInit="formInit"
                        :form-config='formConfigCar'
                        @valueChange="valueChange"
                        @extendClick="addCarVehicle">
                    </form-group>
                    <form-control
                        class="batch-upload"
                        :style="{left: btnLeft}"
                        :form-model="uploadFormModel"
                        :control-config="formConfigUpload"
                        @valueChange="imgChange">
                    </form-control>
                </div>
                <seperate-box
                    :pageNum="pageNum"
                    :total="total"
                    :defaultData="defaultDataTable"
                    @update="update"
                    @updatePage="GetDefaultTable"
                    @deleteData="deleteData"
                    @save="save"
                    @select="select">
                </seperate-box>
            </el-card>
        </page-box>
    </div>
</template>

<script>
/* eslint-disable max-lines */
import FormTop from './formTop/formTop';
import SeperateBox from './seperateBox/seperateBox';
import FormControl from '../commons/formList/formControl/FormControl';
import PageBox from '../commons/pageBox/PageBox';
import FormGroup from '../commons/formGroup/FormGroup';
import TableBox from '../commons/tableBox/TableBox';
import * as apiConfig from '../commons/apis/headMap/generalPageapi.js';
import { formConfig, formConfigCar, formConfigUpload } from './config/SeperateCar.config.js';
import TabPage from '../tabPage/TabPage';
export default {
    extends: TabPage,
    components: {
        PageBox,
        FormGroup,
        TableBox,
        SeperateBox,
        FormTop,
        FormControl
    },
    data() {
        return {
            title: [
                '模块类型:',
                '头图—分车型活动页'
            ],
            isUpdating: false,
            uploadFormModel: null,
            formConfigCar: formConfigCar(),
            formConfigUpload: formConfigUpload(),
            brandList: null,
            defaultData: null,
            defaultDataTable: null,
            pageNum: 1,
            pageSize: 20,
            total: 0,
            btnLeft: '777px',
            selection: []
        };
    },
    created() {
        if (this.formConfigUpload) {
            let form = this.$$form;
            this.uploadFormModel = form.initFormData([this.formConfigUpload]);
        }
    },
    mounted() {
        this.GetDefaultTable(this.pageNum);
        this.GetAllVehicles();
        let carVehicle = this.$refs.hook_carVehicle;
        this.btnLeft = (carVehicle.clientWidth * 0.125 + 663) + 'px';
    },
    methods: {
        /**
         * 刷新数据
         */
        refreshData() {
            this.GetDefaultTable(this.pageNum);
        },
        dataChange(formModelData) {
            this.formModelData = formModelData;
        },
        /**
         * 添加车型
         * @param {Object} evt 数据
         */
        addCarVehicle(evt) {
            let tagOption = this.tagOption && this.tagOption.data;
            if (!tagOption || !tagOption.ActivityId || !tagOption.ModuleId) {
                return;
            }
            if (!this.formModelData.value.MinuteImgUrl) {
                this.$$errorMsg('请先添加默认头图');
                return;
            }
            let _that = this;
            let _value = _that.formModel.value;
            let brand = _value.brand;
            let arrcarType = _value.carType;
            if (!brand) {
                this.$$errorMsg('请先选择品牌');
                return;
            }
            if (!arrcarType.length) {
                this.$$errorMsg('请先选择车型');
                return;
            }
            if (arrcarType.length>10) {
                this.$$errorMsg('车型必须小于10条');
                this.formModel.setValue({brand: '', carType: []});
                return;
            }
            let defaultDataTable = this.defaultDataTable || [];
            // if (arrcarType.length) { // 选了车型
            let carData = arrcarType.map(item => {
                let vehicleItem = (_that.VehicleMap && _that.VehicleMap[item])||{};
                return {
                    CarTypeBrandName: brand + vehicleItem.VehicleName,
                    ImageUrl: '',
                    PKID: '',
                    VechiceleId: vehicleItem.VehicleId,
                    VechiceleName: vehicleItem.VehicleName,
                    isUpdating: false
                };
            });
            carData.map(item => {
                defaultDataTable.push(item);
                return item;
            });
            // } else { // 只选了品牌
            //     let carItem = {
            //         CarTypeBrandName: brand,
            //         ImageUrl: '',
            //         PKID: '',
            //         VechiceleId: '',
            //         VechiceleName: brand,
            //         isUpdating: false
            //     };
            //     defaultDataTable.push(carItem);
            // }
            this.defaultDataTable = defaultDataTable;
            this.formModel.setValue({brand: '', carType: []});
        },
        /**
         * 修改表格只做前端操作
         *  @param {object} index 行号
         */
        update (index) {
            this.$set(this.defaultDataTable, index, Object.assign({}, this.defaultDataTable[index], {isUpdating: true}));
            // if (cb && cb instanceof Function) {
            //     cb();
            // }
        },
        /**
         * 保存时，PKID不存在的走add接口，存在的走Edit接口
         * @param {object} index 行号
         * @param {number} row 行参数
         */
        save (index, row) {
            let tagOption = this.tagOption && this.tagOption.data;
            if (!tagOption || !tagOption.ActivityId || !tagOption.ModuleId) {
                return;
            }
            let defaultDataItem = this.defaultDataTable && this.defaultDataTable[index];
            const getData = Object.assign({
                ActivityId: tagOption.ActivityId,
                ModuleId: tagOption.ModuleId
            }, defaultDataItem);
            // const getData = {
            //     ActivityId: tagOption.ActivityId,
            //     ModuleId: tagOption.ModuleId,
            //     VehicleInfo: defaultDataItem
            // };
            if (defaultDataItem) {
                let api;
                if (defaultDataItem.PKID) {
                    api = apiConfig.EditMinuteFigure; // 编辑接口
                } else {
                    api = apiConfig.AddMinuteFigure; // 添加接口
                }
                // let api = apiConfig.SaveMinuteFigures;
                this.$http.post(api, {
                    isLoading: true,
                    data: getData
                }).subscribe(res => {
                    let data = res && res.data;
                    if (data && data.PkId && !defaultDataItem.PKID) {
                        this.$set(this.defaultDataTable[index], 'PKID', data.PkId);
                    }
                    this.$set(this.defaultDataTable[index], 'isUpdating', false);
                    // setTimeout(() => {
                    //     row.isUpdating = false;
                    // }, 200);
                });
            }
        },
        /**
         * 批量保存车型图片
         */
        batchSave() {
            let tagOption = this.tagOption && this.tagOption.data;
            let selectionData = [];
            if (!tagOption || !tagOption.ActivityId || !tagOption.ModuleId) {
                return;
            }
            this.selection && this.selection.forEach(item => {
                if (item.PKID) {
                    item.OperationType = 'Edit';
                } else {
                    item.OperationType = 'Add';
                }
                selectionData.push(item);
            });
            if (!selectionData.length) {
                return;
            }
            const getData = {
                ActivityId: tagOption.ActivityId,
                ModuleId: tagOption.ModuleId,
                VehicleInfo: selectionData
            };
            this.$http.post(apiConfig.SaveMinuteFigures, {
                isLoading: true,
                data: getData
            }).subscribe(res => {
                this.GetDefaultTable(this.pageNum);
                this.$set(this.formConfigUpload.descList[0], 'disabled', true);
            });
        },
        deleteData (index, row) {
            this.$confirm('确定【删除】此车型头图？', '提示', {
                confirmButtonText: '确定',
                cancelButtonText: '取消',
                type: 'warning'
            }).then(() => {
                let tableData = this.defaultDataTable;
                let spliceList = tableData.splice(index, 1);
                let delItem = spliceList && spliceList[0];
                if (!delItem || !delItem.PKID) {
                    return;
                }
                let tagOption = this.tagOption && this.tagOption.data;
                if (!tagOption || !tagOption.ActivityId || !tagOption.ModuleId) {
                    return;
                }
                // let defaultDataItem = this.defaultDataTable && tableData[index];
                const getData = Object.assign({
                    ActivityId: tagOption.ActivityId,
                    ModuleId: tagOption.ModuleId
                }, delItem);
                // 删除接口
                this.$http.post(apiConfig.DeleteMinuteFigure, {
                    isLoading: true,
                    data: getData
                }).subscribe(data => {
                    this.$message({ type: 'success', message: '删除成功!' });
                });
            }).catch(() => {
                this.$message({
                    type: 'info',
                    message: '已取消删除'
                });
            });
        },
        formInit(formModel) {
            this.formModel = formModel;
        },
        valueChange (con) {
            let temp = con.controlConfig.controlName;
            if (temp == 'brand') {
                this.formModel.setValue({carType: []});
                const VehicleNewList = [];
                if (this.brandList) {
                    this.brandList.map(item => {
                        if (con.value === item.BrandName) {
                            item.BrandVehicleList.map(iitem => {
                                VehicleNewList.push({'nameText': iitem.VehicleName, 'value': iitem.VehicleId});
                                let VehicleMap = this.VehicleMap = this.VehicleMap || {};
                                VehicleMap[iitem.VehicleId + ''] = iitem;
                                return iitem;
                            });
                        }
                        return item;
                    });
                }
                // this.formConfigCar[0].formControl[1].list = VehicleNewList || '';
                this.$set(this.formConfigCar[0].formControl[1], 'list', VehicleNewList);
            }
        },
        submit() {
            this.formModel.merge(this.formModelData);
            this.formModel.isSave = true;
            if (!this.$$validMsg(this.formModel)) {
                let tagOption = this.tagOption && this.tagOption.data;
                if (!tagOption || !tagOption.ActivityId || !tagOption.ModuleId) {
                    return;
                }
                let _value = this.formModel.value;
                let defaultDataTable = this.defaultDataTable;
                const getData = Object.assign({}, _value, {
                    ActivityId: tagOption.ActivityId,
                    ModuleId: tagOption.ModuleId,
                    ModuleTypeCode: tagOption.SecondaryModuleTypeCode,
                    MinuteFigureList: defaultDataTable
                });
                // 默认保存接口
                this.formModel.isSend = true;
                this.$http.post(apiConfig.SaveDefaultMinuteFigure, {
                    isLoading: true,
                    data: getData
                }).subscribe(res => {
                    let _res = res&&res.data;
                    let _rMessage = _res&& _res.ResponseMessage;
                    if (this.$filterResponseCode(_res)) {
                        this.$$saveMsg(_rMessage, {type: 'success'});
                        this.GetDefaultTable(this.pageNum);
                    }
                }, () => {}, () => {
                    setTimeout(() => {
                        this.formModel.isSend = false;
                    }, 300);
                });
            }
        },
        GetAllVehicles() {
            // 查询所有车型接口
            this.$http.post(apiConfig.GetAllVehicles, {
                isLoading: true
            }).subscribe(data => {
                let _data= data&&data.data;
                let _vehicleList = _data&&_data.VehicleList;
                this.brandList = _vehicleList;
                const brandNewList = [];
                _vehicleList.map(item => {
                    brandNewList.push({'nameText': item.BrandName, 'value': item.BrandName});
                    // this.formConfigCar[0].formControl[0].list = brandNewList || '';
                    this.$set(this.formConfigCar[0].formControl[0], 'list', brandNewList);
                    return item;
                });
            });
        },
        // 读取数据库表单
        GetDefaultTable(pageNum) {
            let tagOption = this.tagOption && this.tagOption.data;
            if (!tagOption || !tagOption.ActivityId || !tagOption.ModuleId) {
                return;
            }
            const getData={
                ActivityId: tagOption.ActivityId,
                ModuleId: tagOption.ModuleId,
                PageIndex: pageNum || 1,
                PageSize: 20
            };
            // 查询接口
            this.$http.post(apiConfig.GetMinuteFigureList, {
                isLoading: true,
                data: getData
            }).subscribe(data => {
                let _data= data&&data.data;
                let _minuteList = _data&&_data.MinuteFigureList;
                this.defaultData = _data;
                this.defaultDataTable = _minuteList && _minuteList.map(item => {
                    item = Object.assign(item, {
                        isUpdating: false
                    });
                    return item;
                });
                this.total = (_data && _data.TotalCount) || 0;
            });
        },
        /**
         * 表格点击复选框时触发
         * @param {Object} evt 选中的数据
         */
        select(evt) {
            this.selection = evt;
            // 有选中时，可以批量上传
            if (evt && evt.length) {
                this.$set(this.formConfigUpload.descList[0], 'disabled', false);
            } else {
                this.$set(this.formConfigUpload.descList[0], 'disabled', true);
            }
        },
        /**
         * 图片变化时触发
         * @param {Object} con formControl
         */
        imgChange(con) {
            const imgUrl = con && con.value;
            const selection = this.selection || [];
            if (selection && imgUrl) {
                this.$confirm('是否确认【保存】？', '提示', {
                    confirmButtonText: '确认',
                    cancelButtonText: '取消',
                    type: 'warning'
                }).then((res) => {
                    this.uploadFormModel.setValue({'fileupload': ''});
                    selection.forEach(item => {
                        this.$set(item, 'ImageUrl', imgUrl);
                        // this.$set(item, 'isUpdating', true);
                    });
                    // TODO 批量保存的接口
                    this.batchSave();
                }).catch(() => {
                    this.uploadFormModel.setValue({'fileupload': ''});
                    this.$message({
                        type: 'info',
                        message: '已取消保存'
                    });
                });
            }
        }
    }
};
</script>
<style lang='scss'>
.seperate-car-pages {
  .tables-wrap {
    .el-button {
      margin-left: 0;
    }
    .form {
      margin: 0;
      background: unset;
    }
    .form-control-nameText {
      display: none;
    }
    .form-control-filter {
      flex: 1;
      .form-control-wraps {
        display: flex;
        flex-direction: column;
        .form-control-leftPadding {
          margin-left: 0;
        }
        .el-input {
          width: 100%;
        }
        .form-extend-filters {
          margin-top: 10px;
          text-align: left;
        }
      }
    }
  }
  .el-card {
      margin-bottom: 20px;
  }
  .submit-btn {
    text-align: center;
    margin: 10px;
    .el-button {
        padding: 12px 40px;
    }
  }
  .carVehicle {
    position: relative;
    .add-carVehicle {
        .el-select.control-items {
            width: 250px;
        }
    }
    .batch-upload {
        position: absolute;
        top: 29px;
        .form-control-cont.form-control-leftPadding {
            display: none;
        }
        .el-upload .el-button {
            height: 38px;
        }
    }
  }
}
</style>
