<template >
  <div>
    <Breadcrumb separator=">">
      <BreadcrumbItem>价格配置</BreadcrumbItem>
      <BreadcrumbItem to="/">天天秒杀</BreadcrumbItem>
      <BreadcrumbItem>{{shortDate}}-{{schedule}}</BreadcrumbItem>
      <BreadcrumbItem>{{strStatus}}</BreadcrumbItem>
    </Breadcrumb>
    <div align="left"
         style="margin: 10px">
      <Table border
             :columns="errorcolumns"
             :data="wrongMsg"
             v-show="wrongMsg.length>0"></Table>
    </div>
    <div align="right"
         style="margin: 10px">
      <Button type="primary"
              shape="circle"
              style="margin: 5px"
              :disabled='disabled'
              @click="toCreate"
              icon="plus-round">添加</Button>
      <Button type="primary"
              shape="circle"
              style="margin: 5px"
              @click="toRefresh"
              icon="refresh">刷新缓存</Button>
      <Button type="primary"
              shape="circle"
              style="margin: 5px"
              @click="toGetLog"
              icon="ios-search">查看日志</Button>
      <a type="primary"
         shape="circle"
         :disabled='disabled'
         @click="toload"
         icon="arrow-down-c">导入模板下载</a>

      <Upload action="/Seckill/ExportProducts/"
              :before-upload="handleBeforeUpload"
              :on-success="handleSuccess">
        <Button type="primary"
                :disabled='disabled'
                shape="circle"
                padding="5px"
                icon="ios-cloud-upload-outline">上传文件</Button>
      </Upload>

    </div>
    <div align="left"
         style="margin: 10px">

    </div>
    <div style="margin: 10px">
      <Table border
             :columns="columns"
             :loading="loading"
             :data="data"></Table>
    </div>
    <!-- <div class="edittable-table-height-con">
            <can-edit-table refs="table2" v-model="editInlineData" :columns-list="editInlineColumns"></can-edit-table>
        </div> -->
    <div align="right"
         style="margin: 10px">
      <Button type="primary"
              shape="circle"
              :disabled='disabled'
              @click="toSave">保存提交</Button>
      <Button type="ghost"
              shape="circle"
              :disabled='btndisabled'
              @click="toApprovePass">审核通过</Button>
      <Button type="ghost"
              shape="circle"
              :disabled='btndisabled'
              @click="toApproveBack">审核驳回</Button>
    </div>
    <Modal v-model="logmodal.visible"
           title="操作日志"
           :width="logmodal.width">
      <Table :loading="logmodal.loading"
             :data="logmodal.data"
             :columns="logmodal.columns"
             stripe></Table>
    </Modal>
  </div>
</template>
<script>
// import canEditTable from '@/views/seckill/canEditTable.vue';
export default {
    /*     components: {
            canEditTable
        }, */
    data () {
        /* var _this = this;
          const PIDCheck = (rule, value, callback) => {
             if (value === '') {
                 callback(new Error('请输入PID'));
             } else {
                 var count = 0;
                 for (var item in _this.data) {
                     if (item.PID === value) {
                         count++;
                         break;
                     }
                 }
                 if (count > 0) {
                     callback(new Error(value + '重复配置了'));
                 } else {
                     this.fetchProductByPID(value, function (message) {
                         alert(message);
                         if (message !== '') {
                             callback(new Error(message));
                         } else {
                             callback();
                         }
                     });
                 }
             }
         }; */
        return {
            shortDate: "",
            schedule: "",
            strStatus: "",
            wrongMsg: [],
            disabled: true,
            btndisabled: false,
            loading: true,
            errorcolumns: [
                {
                    title: 'PID',
                    key: 'PID',
                    width: 100
                },
                {
                    title: "错误信息",
                    key: 'msgInfo',
                    render: (h, params) => {
                        return h('span', {
                            style: {
                                'color': 'Red'
                            },
                            domProps: {
                                innerHTML: params.row.msgInfo
                            }
                        }, params.row.msgInfo)
                    }
                }

            ],
            columns: [
                {
                    title: 'PID',
                    key: 'PID',
                    fixed: 'left',
                    width: 180,
                    render: (h, params) => {
                        return h('Input', {
                            props: {
                                type: 'text',

                                value: this.data[params.index].PID,
                                readonly: this.disabled
                            },
                            on: {
                                // input: function (event) {
                                //     console.log(event.target.value);
                                //     this.data[params.index].PID = event.target.value;
                                // },
                                'on-blur': (event) => {
                                    this.data[params.index].PID = event.target.value;
                                    this.PIDcheck2(params.index)
                                }
                            }
                        })
                    }
                },
                {
                    title: '排序',
                    key: 'Position',

                    render: (h, params) => {
                        return h('Input', {
                            props: {
                                value: params.row.Position,
                                readOnly: this.disabled
                            },
                            on: {
                                'on-blur': (event) => {
                                    this.data[params.index].Position = event.target.value;
                                }
                            }
                        })
                    }
                },
                // {
                //     title: '图片',
                //     key: 'ImageUrl',
                //     render: (h, params) => {
                //         return h('img', {
                //             domProps: {
                //                 src: params.row.ImageUrl
                //             }
                //         })
                //     }

                // },
                {
                    title: '秒杀标题',
                    key: 'ProductName',
                    width: 200,
                    render: (h, params) => {
                        return h('Input', {
                            props: {
                                type: 'text',
                                value: params.row.ProductName,
                                readonly: this.disabled
                            },
                            on: {
                                'on-blur': (event) => {
                                    this.data[params.index].ProductName = event.target.value;
                                }
                            }
                        })
                    }
                },
                {
                    title: '成本价',
                    key: 'CostPrice',
                    align: 'center'
                },
                {
                    title: '产品价',
                    key: 'OriginalPrice',
                    align: 'center'
                },
                {
                    title: '秒杀价',
                    key: 'Price',
                    render: (h, params) => {
                        return h('Input', {
                            props: {
                                value: params.row.Price,
                                readonly: this.disabled
                            },
                            on: {
                                'on-blur': (event) => {
                                    this.data[params.index].Price = event.target.value;
                                    this.checkPrice(params.index)
                                }
                            }
                        })
                    }
                },
                {
                    title: '伪原价',
                    key: 'FalseOriginalPrice',
                    render: (h, params) => {
                        return h('Input', {
                            props: {
                                value: params.row.FalseOriginalPrice,
                                readonly: this.disabled
                            },
                            on: {
                                'on-blur': (event) => {
                                    this.data[params.index].FalseOriginalPrice = event.target.value;
                                }
                            }
                        })
                    }
                },
                {
                    title: '降价幅度',
                    key: 'DecreaseDegree',
                    align: 'center',
                    render: (h, params) => {
                        var color = this.getdegreecolor(params.index);
                        return h('span', {
                            style: color
                        }, params.row.DecreaseDegree)
                    }
                },
                {
                    title: '每人限购',
                    key: 'MaxQuantity',
                    render: (h, params) => {
                        return h('Input', {
                            props: {
                                value: params.row.MaxQuantity,
                                readonly: this.disabled
                            },
                            on: {
                                'on-blur': (event) => {
                                    this.data[params.index].MaxQuantity = event.target.value;
                                }
                            }
                        })
                    }
                },
                {
                    title: '秒杀库存',
                    key: 'TotalQuantity',
                    render: (h, params) => {
                        return h('Input', {
                            props: {
                                value: params.row.TotalQuantity,
                                readonly: this.disabled
                            },
                            on: {
                                'on-blur': (event) => {
                                    this.data[params.index].TotalQuantity = event.target.value;
                                }
                            }
                        })
                    }
                },
                {
                    title: '优惠券',
                    key: 'IsUsePCode',
                    render: (h, params) => {
                        return h("i-switch", {
                            props: {
                                type: "primary",
                                size: "large",
                                value: params.row.IsUsePCode,
                                disabled: this.disabled
                            },
                            on: {
                                'on-change': value => {
                                    this.UpdateIsUsePCode(params.index,
                                        value);
                                }
                            }
                        });
                    }
                },
                {
                    title: '显示',
                    key: 'IsShow',
                    render: (h, params) => {
                        return h("i-switch", {
                            props: {
                                type: "primary",
                                size: "large",
                                value: params.row.IsShow,
                                disabled: this.disabled
                            },
                            on: {
                                'on-change': value => {
                                    this.UpdateIsShow(
                                        params.index,
                                        value
                                    );
                                }
                            }
                        });
                    }

                },
                {
                    title: '销量',
                    key: 'SaleOutQuantity',
                    align: 'center'
                },
                {
                    title: '错误信息',
                    key: 'Error',
                    render: (h, params) => {
                        return h('span', {
                            style: {
                                'color': 'Red'
                            }
                        }, params.row.Error)
                    }
                },
                {
                    title: '操作',
                    key: 'action',
                    fixed: 'right',
                    width: 80,
                    align: 'center',
                    render: (h, params) => {
                        return h('div', [
                            h('Button', {
                                props: {
                                    type: 'error',
                                    size: 'small',
                                    disabled: this.disabled
                                },
                                on: {
                                    click: () => {
                                        this.remove(params.index)
                                    }
                                }
                            }, '删除')
                        ]);
                    }
                }
            ],
            editInlineColumns: [
                {
                    title: 'PID',
                    key: 'PID',
                    align: 'center',
                    render: (h, params) => {
                        return h('Input', {
                            props: {
                                value: params.row.PID
                            },
                            on: {
                                input: (val) => {
                                    this.editInlineData[params.index].PID = val;
                                    // params.row.index.PID = val;
                                    console.log(val);
                                },
                                'on-blur': () => {
                                    this.PIDcheck2(params.index)
                                }
                            }
                        })
                    }
                },
                {
                    title: '排序',
                    key: 'Position',
                    align: 'center',
                    editable: true
                },
                {
                    title: '秒杀标题',
                    key: 'ProductName',
                    align: 'center',
                    editable: true
                },
                {
                    title: '图片',
                    key: 'ImageUrl',
                    align: 'center',
                    render: (h, params) => {
                        return h('img', {
                            domProps: {
                                src: params.row.ImageUrl
                            }
                        })
                    }

                },

                {
                    title: '成本价',
                    key: 'CostPrice',
                    render: (h, params) => {
                        return ('div', params.row.CostPrice)
                    }
                },
                {
                    title: '产品价',
                    key: 'OriginalPrice',
                    render: (h, params) => {
                        return ('div', params.row.OriginalPrice)
                    }
                },
                {
                    title: '*秒杀价',
                    key: 'Price',
                    align: 'center',
                    editable: true
                },
                {
                    title: '伪原价',
                    key: 'FalseOriginalPrice',
                    align: 'center',
                    editable: true
                },
                {
                    title: '降价幅度',
                    key: 'DecreaseDegree',
                    align: 'center'
                },
                {
                    title: '*每人限购',
                    key: 'MaxQuantity',
                    align: 'center',
                    editable: true
                },
                {
                    title: '*秒杀库存',
                    key: 'TotalQuantity',
                    align: 'center',
                    editable: true
                },
                {
                    title: '*优惠券',
                    key: 'IsUsePCode',
                    align: 'center',
                    editable: true

                },
                {
                    title: '*显示',
                    key: 'IsShow',
                    align: 'center'

                },
                {
                    title: '*销量',
                    key: 'SaleOutQuantity',
                    align: 'center'
                },
                {
                    title: '操作',
                    align: 'center',
                    width: 200,
                    key: 'handle',
                    handle: ['edit', 'delete']
                }
            ],
            editInlineData: [
            ],
            data: [],
            logmodal: {
                loading: true,
                visible: false,
                width: 800,
                data: [],
                columns: [
                    {
                        title: "时间",
                        width: 200,
                        key: "CreateDateTime",
                        align: "center",
                        fixed: "left",
                        render: (h, params) => {
                            return h("span", this.formatDate(params.row.CreateDateTime));
                        }
                    },
                    {
                        title: "操作人",
                        width: 100,
                        key: "Name",
                        align: "center"
                    },
                    {
                        title: "操作类型",
                        width: 150,
                        key: "Title",
                        align: "center",
                        fixed: "left"
                    },

                    {
                        title: "修改备注",
                        width: 300,
                        key: "Remark",
                        align: "center",
                        fixed: "left",
                        render: (h, params) => {
                            return h('span', {
                                style: {
                                    'color': 'Red'
                                },
                                domProps: {
                                    innerHTML: params.row.Remark
                                }
                            }, params.row.Remark)
                        }
                    }
                ]
            },
            origiondata: [],
            modal: {
                visible: false,
                width: 500,
                title: '',
                editType: '',
                formValidate: {

                    PID: '',
                    Sort: '',
                    ImageUrl: '',
                    ProductName: '',
                    CostPrice: "",
                    OriginalPrice: "",
                    Price: "",
                    FalseOriginalPrice: '',
                    DecreaseDegree: '',
                    MaxQuantity: '',
                    TotalQuantity: '',
                    IsUsePCode: '',
                    IsShow: '',
                    SaleOutQuantity: ""
                }

                /*                 ruleValidate: {
                                    PID: [
                                        { validator: PIDCheck, trigger: 'blur' }
                                    ]
                
                                } */
            }
        }
    },

    created () {
        this.loadEditData()
    },
    computed: {
        DecreaseDegree: function () {
            if (this.OriginalPrice.length && this.Price.length) {
                return (parseFloat(Math.abs(parseFloat(this.OriginalPrice) - parseFloat(this.Price)) / parseFloat(this.OriginalPrice)).toFixed(2) * 100).toString("0%");
            }
        }
    },
    watch: {
        Price: function (newvar) {
            if (this.OriginalPrice.length) {
                this.DecreaseDegree = (parseFloat(Math.abs(parseFloat(this.OriginalPrice) - parseFloat(this.Price)) / parseFloat(this.OriginalPrice)).toFixed(2) * 100).toString("0%");
            }
        },
        OriginalPrice: function (newvar) {
            if (this.Price.length) {
                this.DecreaseDegree = (parseFloat(Math.abs(parseFloat(this.OriginalPrice) - parseFloat(this.Price)) / parseFloat(this.OriginalPrice)).toFixed(2) * 100).toString("0%");
            }
        }

    },
    methods: {
        loadEditData () {
            this.activityId = this.$route.query.activityId;
            this.schedule = this.$route.query.schedule;
            this.shortDate = this.$route.query.shortDate;
            this.isDefault = this.$route.query.isDefault;
            this.strStatus = this.$route.query.strStatus;
            if (this.strStatus === "新建" || this.strStatus === "已发布" || (this.strStatus === "售卖中" && this.isDefault === "false") || this.strStatus === "已驳回" || this.strStatus === "待审核") {
                this.disabled = false;
                this.btndisabled = this.strStatus !== "待审核";
            }
            if (this.strStatus === "已结束") {
                this.disabled = true;
                this.btndisabled = true;
            }
            if (this.strStatus === "售卖中") {
                this.disabled = this.isDefault === "true";
                this.btndisabled = true;
            }
            this.data = [];
            if (this.activityId != null) {
                let params = {
                    activityId: this.activityId,
                    isDefault: this.isDefault,
                    strStatus: this.strStatus
                };
                var _this = this;
                this.ajax
                    .get("/Seckill/GetEditDatasByActivityId", {
                        params: params
                    })
                    .then(response => {
                        _this.data = response.data;
                        _this.origiondata = JSON.parse(JSON.stringify(response.data))
                        _this.data.forEach(function (item, index, arr) {
                            if (item.CostPrice === null) {
                                arr[index].CostPrice = "无";
                            }
                        })
                        this.loading = false;
                    });
            } else {
                this.loading = false;
            }
        },
        formatPrice (price) {
            return '￥' + price;
        },
        update (index) {

        },
        getdegreecolor (index) {
            if ((this.data[index].CostPrice !== "无") && this.data[index].Price < this.data[index].CostPrice) {
                return { "color": "red" };
            } else {
                return { "color": "black" };
            }
        },
        remove (index) {
            debugger;
            this.data.splice(index, 1);
        },
        toGetLog () {
            this.logmodal.loading = true;
            this.ajax
                .post("/Seckill/GetLogList", {
                    activityId: this.activityId
                })
                .then(response => {
                    this.logmodal.data = response.data;
                    this.logmodal.visible = true;
                    this.logmodal.loading = false;
                });
        },
        handleBeforeUpload () {
            this.loading = true;
        },
        handleSuccess (res, file) {
            var _this = this;
            if (res.code === 0) { alert("导入失败!!") } else if (res.code === -1) { alert("请确定Excel中有数据(PID未填认为所在行无数据)"); } else if (res.error.length) {
                var wrongMsg = '';
                res.error.forEach(function (v, i) {
                    wrongMsg += (v + "\n");
                });
                alert(wrongMsg);
            } else if (res.data.length) {
                res.data.forEach(function (e, i) {
                    _this.data.push({
                        PID: e.PID,
                        Position: e.Position,
                        ImageUrl: '',
                        ProductName: e.ProductName,
                        CostPrice: '',
                        OriginalPrice: '',
                        Price: e.Price,
                        FalseOriginalPrice: e.FalseOriginalPrice,
                        DecreaseDegree: '',
                        MaxQuantity: e.MaxQuantity,
                        TotalQuantity: e.TotalQuantity,
                        IsUsePCode: e.IsUsePCode,
                        IsShow: e.IsShow,
                        SaleOutQuantity: 0,
                        Channel: 'all',
                        IsJoinPlace: 0,
                        InstallService: e.InstallService,
                        Error: ''
                    })
                    var returnVaule = '';
                    var index = 0;
                    var count = 0;
                    _this.data.forEach(function (e1, i1) {
                        if (e1.PID === e.PID) {
                            index = i1;
                            count++;
                        }
                    });
                    if (count > 1) {
                        _this.data[index].Error = '产品配置重复了'
                    } else {
                        _this.data[index].Error = '';
                        _this.ajax.get("/Seckill/FetchProductWithCostPriceByPID", {
                            params: { PID: _this.data[index].PID, activityId: _this.activityId }
                        })
                            .then(response => {
                                if (response.data.PID === null) {
                                    returnVaule = '无此产品编号';
                                }
                                if (response.data.CaseSensitive) {
                                    returnVaule = '请跟产品库配置大小写一致';
                                }
                                if (!response.data.OnSale || response.data.Stockout) {
                                    returnVaule = '此产品下架或缺货'
                                }
                                if (returnVaule.length) {
                                    _this.data[index].Error = returnVaule;
                                }
                                if (!(returnVaule.length)) {
                                    if (!_this.data[index].ProductName.length > 1) {
                                        _this.data[index].ProductName = response.data.DisplayName;
                                    }
                                    _this.data[index].InstallService = response.data.InstallService;
                                    _this.data[index].ImageUrl = response.data.Image;
                                    _this.data[index].CostPrice = response.data.CostPrice == null ? "无" : response.data.CostPrice;

                                    _this.data[index].OriginalPrice = response.data.cy_list_price;

                                    if (!parseFloat(_this.data[index].FalseOriginalPrice) > 0) {
                                        _this.data[index].FalseOriginalPrice = response.data.cy_list_price;
                                    }
                                    _this.data[index].DecreaseDegree = '';
                                    var price = _this.data[index].Price;
                                    var originPrice = _this.data[index].OriginalPrice;
                                    if (isNaN(price)) {
                                        _this.data[index].Error = "秒杀价请输入正确的数字格式";
                                    }
                                    if (originPrice > 0) {
                                        if (parseFloat(price) <= 0 || parseFloat(price) > parseFloat(originPrice)) {
                                            _this.data[index].Error += "秒杀价不能为0或者高于原价，请查看是否配置错误";
                                        }
                                        var degree = parseFloat(Math.abs(parseFloat(originPrice) - parseFloat(price)) / parseFloat(originPrice)).toFixed(2);
                                        _this.data[index].DecreaseDegree = _this.toPercent(degree);
                                    }
                                }
                            });
                    }
                });
            }
            _this.loading = false;
        },
        updateValue: function (value) {
            this.$emit('input', value)
        },
        toload () {
            window.location.href = '/Content/QiangGou/ExportSeckillExcel.xlsx';
        },
        toupload () {

        },
        toCreate () {
            // this.modal.visible = true;
            // this.modal.editType = 1;
            // this.modal.title = '新建'

            this.data.push({
                PID: '',
                Position: '',
                ImageUrl: '',
                ProductName: '',
                CostPrice: '',
                OriginalPrice: '',
                Price: '',
                FalseOriginalPrice: '',
                DecreaseDegree: '',
                MaxQuantity: '',
                TotalQuantity: '',
                IsUsePCode: false,
                IsShow: true,
                SaleOutQuantity: 0,
                Channel: 'all',
                IsJoinPlace: 0,
                InstallService: '',
                Error: ''
            })
        },
        handleSubmit (name) {
            this.$refs[name].validate((valid) => {
                if (valid) {
                    this.data.push(this.modal.formValidate)
                    this.$refs[name].resetFields();
                    this.$Message.success('Success!');
                } else {
                    this.$Message.error('Fail!');
                }
            })
        },
        handleReset (name) {
            this.$refs[name].resetFields();
        },
        fetchProductByPID (PID, callback) {
            var returnVaule = '';
            this.ajax.get("/Seckill/FetchProductByPID", {
                async: false,
                params: { PID: PID, activityId: '' }
            })
                .then(response => {
                    if (response.data.PID === null) {
                        returnVaule = '无此产品编号';
                    }
                    if (response.data.CaseSensitive) {
                        returnVaule = '请跟产品库配置大小写一致';
                    }
                    if (response.data.OnSale || response.data.Stockout) {
                        returnVaule = '此产品下架或缺货'
                    }
                    if (returnVaule === '') {
                        this.modal.formValidate.ProductName = response.data.DisplayName;
                        this.modal.formValidate.ImageUrl = response.data.ImageUrl;
                        this.modal.formValidate.CostPrice = 100;
                        this.modal.formValidate.OriginalPrice = response.data.cy_list_price;
                    }
                    callback(returnVaule);
                });
        },
        UpdateIsShow (index, isShow) {
            this.data[index].IsShow = isShow;
        },
        UpdateIsUsePCode (index, isUsePCode) {
            this.data[index].IsUsePCode = isUsePCode;
        },
        isInteger (obj) {
            return parseInt(obj, 10) === obj
        },
        toSave () {
            var NeedExam = false;
            var _this = this;
            var count1 = 0;
            if (_this.strStatus === "已驳回" || _this.strStatus === "待审核") {
                NeedExam = true;
            }
            if (!_this.activityId === null) {
                NeedExam = true;
            }
            if (_this.origiondata.length !== _this.data.length) {
                NeedExam = true;
            }
            if (!NeedExam) {
                _this.origiondata.forEach(element => {
                    _this.data.forEach(e => {
                        if (element.PID === e.PID) {
                            count1 = count1 + 1;
                            if (parseFloat(element.Price) > parseFloat(e.Price)) {
                                console.log("price" + count1)
                                count1 = count1 - 1;
                            }
                            if (element.IsUsePCode !== e.IsUsePCode) {
                                console.log("isUsePCode" + count1)
                                count1 = count1 - 1;
                            }
                        }
                    })
                });
                if (count1 !== _this.origiondata.length) {
                    NeedExam = true;
                }
                console.log(count1);
            }
            _this.wrongMsg = [];
            _this.data.forEach(function (e, i) {
                if (_this.data[i].CostPrice === "无") {
                    _this.data[i].CostPrice = "";
                }

                if (e.Error !== undefined && e.Error !== "") {
                    _this.wrongMsg.push({
                        PID: e.PID,
                        msgInfo: e.Error
                    })
                }
            })
            if (_this.wrongMsg.length === 0) {
                if (!_this.data.length) {
                    _this.wrongMsg.push({
                        PID: '',
                        msgInfo: "请添加商品"
                    })
                } else {
                    _this.data.forEach(function (e, i) {
                        console.log("i: ", i, e)
                        var msgInfo = "";
                        if (e.ProductName.trim() == null || e.ProductName.trim() === '') { msgInfo += "秒杀标题不能为空<br/>"; }
                        if (e.Position != null && e.Position.length && !(parseInt(e.Position) === parseFloat(e.Position))) { msgInfo += "排序填写不规范<br/>"; }
                        if (!e.Price.toString().length) {
                            msgInfo += '秒杀价不允许为空<br/>';
                        } else if (isNaN(e.Price)) { msgInfo += "秒杀价请输入正确的数字格式<br/>"; } else if (parseFloat(e.Price) <= 0 || parseFloat(e.Price) > parseFloat(e.OriginalPrice)) { msgInfo += "秒杀价不能为0或者高于原价，请查看是否配置错误<br/>"; }
                        if (!e.FalseOriginalPrice.toString().length) { msgInfo += '伪原价不允许为空<br/>'; } else if (isNaN(e.FalseOriginalPrice)) { msgInfo += "伪原价请输入正确的数字格式<br/>"; } else if (parseFloat(e.FalseOriginalPrice) <= 0 || parseFloat(e.FalseOriginalPrice) < parseFloat(e.Price)) { msgInfo += "伪原价不能为0或者低于产品价，请查看是否配置错误<br/>"; }
                        if (e.MaxQuantity.length && !(parseInt(e.MaxQuantity) === parseFloat(e.MaxQuantity))) { msgInfo += "个人限购数量请输入正确的数字格式<br/>"; }
                        if (!e.TotalQuantity.toString().length) { msgInfo += "秒杀库存不允许为空<br/>"; }
                        if (e.TotalQuantity.length && !(parseInt(e.TotalQuantity) === parseFloat(e.TotalQuantity))) { msgInfo += "秒杀库存请输入正确的数字格式<br/>"; }
                        if (parseInt(e.TotalQuantity) < parseInt(e.MaxQuantity)) { msgInfo += "每人限购的数量高于秒杀库存，请确认<br/>"; }
                        if (msgInfo.length) {
                            _this.wrongMsg.push({
                                PID: e.PID,
                                msgInfo: msgInfo
                            })
                            return false;
                        }
                    });
                    if (_this.wrongMsg.length === 0) {
                        var model = {
                            ActivityID: this.activityId,
                            ActivityName: this.schedule,
                            IsDefault: this.isDefault,
                            ShortDate: this.shortDate,
                            NeedExam: NeedExam,
                            strStatus: this.strStatus
                        };
                        this.ajax.post('/Seckill/Save', {
                            strModel: JSON.stringify(model),
                            products: JSON.stringify(_this.data),
                            originProducts: JSON.stringify(_this.origiondata)
                        }).then(response => {
                            if (response.data.Status === 1) {
                                this.$Message.success(response.data.Message);
                                this.$router.push({ 'name': 'seckillindex' });
                            } else {
                                this.$Message.error(response.data.Message);
                            }
                        })
                    }
                }
            }
        },
        toApprovePass () {
            var _this = this;
            this.ajax.post('/Seckill/ExamActivity', {
                aid: _this.activityId
            }).then(response => {
                if (response.data.code === 1) {
                    this.$Message.success('审核成功');
                    this.$router.push({ 'name': 'seckillindex' });
                } else {
                    this.$Message.error('审核失败');
                }
            })
        },
        toApproveBack () {
            var _this = this;
            this.ajax.post('/Seckill/ApproveBack', {
                aid: _this.activityId
            }).then(response => {
                if (response.data.code === 1) {
                    this.$Message.success('驳回成功');
                    this.$router.push({ 'name': 'seckillindex' });
                } else {
                    this.$Message.error('驳回失败');
                }
            })
        },
        toPercent (point) {
            var str = Number(point * 100).toFixed(2);
            str += "%";
            return str;
        },
        toRefresh () {
            var _this = this;
            this.ajax.post('/Seckill/RefreshCache', {
                activityId: _this.activityId
            }).then(response => {
                if (response.data.code === "1") {
                    this.$Message.success(response.data.msg);
                } else {
                    this.$Message.error(response.data.msg);
                }
            })
        },
        checkPrice (index) {
            this.data[index].Error = '';
            this.data[index].DecreaseDegree = '';
            var price = this.data[index].Price;
            var originPrice = this.data[index].OriginalPrice;
            if (isNaN(price)) { this.data[index].Error = "秒杀价请输入正确的数字格式"; }
            if (originPrice > 0) {
                if (parseFloat(price) <= 0 || parseFloat(price) > parseFloat(originPrice)) {
                    this.data[index].Error = "秒杀价不能为0或者高于原价，请查看是否配置错误";
                }
                var degree = parseFloat(Math.abs(parseFloat(originPrice) - parseFloat(price)) / parseFloat(originPrice)).toFixed(2);
                this.data[index].DecreaseDegree = this.toPercent(degree);
            }
        },
        checkoriginPrice (index) {
            this.data[index].Error = '';
            this.data[index].DecreaseDegree = '';
            var price = this.data[index].Price;
            var originPrice = this.data[index].OriginalPrice;
            if (isNaN(originPrice)) { this.data[index].Error = "秒杀价请输入正确的数字格式"; }
            if (price.length) {
                if (parseFloat(price) <= 0 || parseFloat(price) > parseFloat(originPrice)) {
                    this.data[index].Error = "秒杀价不能为0或者高于原价，请查看是否配置错误";
                }
                this.data[index].DecreaseDegree = parseFloat(Math.abs(parseFloat(originPrice) - parseFloat(price)) / parseFloat(originPrice)).toFixed(2).toString("0%");
            }
        },
        PIDcheck2 (index) {
            var returnVaule = '';
            var _this = this;
            _this.data[index].Error = '';
            var count = 0;
            var pid = _this.data[index].PID;
            _this.data.forEach(function (e1, i1) {
                if (e1.PID === pid) {
                    count++;
                }
            });
            if (count > 1) {
                _this.data[index].Error = '产品配置重复了'
            } else {
                this.ajax.get("/Seckill/FetchProductWithCostPriceByPID", {
                    params: { PID: _this.data[index].PID, activityId: _this.activityId }
                })
                    .then(response => {
                        if (response.data.PID === null) {
                            returnVaule = '无此产品编号';
                        }
                        if (response.data.CaseSensitive) {
                            returnVaule = '请跟产品库配置大小写一致';
                        }
                        if (!response.data.OnSale || response.data.Stockout) {
                            returnVaule = '此产品下架或缺货'
                        }
                        if (returnVaule.length) {
                            _this.data[index].Error = returnVaule;
                        } else {
                            _this.data[index].InstallService = response.data.InstallService;
                            if (!(_this.data[index].ProductName.length > 1)) {
                                _this.data[index].ProductName = response.data.DisplayName;
                            }
                            if (!(_this.data[index].CostPrice.length > 1)) {
                                _this.data[index].CostPrice = response.data.CostPrice == null ? "无" : response.data.CostPrice;
                            }
                            if (!(_this.data[index].OriginalPrice.length > 1)) {
                                _this.data[index].OriginalPrice = response.data.cy_list_price;
                            }
                            console.log(_this.data[index].CostPrice);
                            console.log(_this.data[index].OriginalPrice);
                            if (!(_this.data[index].FalseOriginalPrice.length > 1)) {
                                _this.data[index].FalseOriginalPrice = response.data.cy_list_price;
                            }
                        }
                    });
            }
        },
        handleNetConnect (state) {
            this.breakConnect = state;
        },
        handleLowSpeed (state) {
            this.lowNetSpeed = state;
        },
        getCurrentData () {
            this.showCurrentTableData = true;
        },
        handleDel (val, index) {
            this.$Message.success('删除了第' + (index + 1) + '行数据');
        },
        handleCellChange (val, index, key) {
            this.$Message.success('修改了第 ' + (index + 1) + ' 行列名为 ' + key + ' 的数据');
        },
        handleChange (val, index) {
            this.$Message.success('修改了第' + (index + 1) + '行数据');
        },
        formatDate (value) {
            if (value == null) return null;
            var time = new Date(
                parseInt(value.replace("/Date(", "").replace(")/", ""))
            );
            var year = time.getFullYear();
            var day = time.getDate();
            var month = time.getMonth() + 1;
            var hours = time.getHours();
            var minutes = time.getMinutes();
            var seconds = time.getSeconds();
            return (
                year +
                "-" +
                month +
                "-" +
                day +
                " " +
                hours +
                ":" +
                minutes +
                ":" +
                seconds
            );
        }
    }

}
</script>
<style lang="less">
.dragging-tip-enter-active {
  opacity: 1;
  transition: opacity 0.3s;
}
.dragging-tip-enter,
.dragging-tip-leave-to {
  opacity: 0;
  transition: opacity 0.3s;
}
.dragging-tip-con {
  display: block;
  text-align: center;
  width: 100%;
  height: 50px;
}
.dragging-tip-con span {
  font-size: 18px;
}
.record-tip-con {
  display: block;
  width: 100%;
  height: 292px;
  overflow: auto;
}
.record-item {
  box-sizing: content-box;
  display: block;
  overflow: hidden;
  height: 24px;
  line-height: 24px;
  padding: 8px 10px;
  border-bottom: 1px dashed gainsboro;
}
.record-tip-con span {
  font-size: 14px;
}
.edittable-test-con {
  height: 160px;
}
.edittable-table-height-con {
  height: 190px;
}
.edittable-con-1 {
  box-sizing: content-box;
  padding: 15px 0 0;
  height: 196px;
}
.edittable-table-get-currentdata-con {
  height: 190px !important;
}
.exportable-table-download-con1 {
  padding: 16px 0 16px 20px;
  border-bottom: 1px dashed #c3c3c3;
  margin-bottom: 16px;
}
.exportable-table-download-con2 {
  padding-left: 20px;
}
.show-image {
  padding: 20px 0px;
}
.show-image img {
  display: block;
  width: 100%;
  height: auto;
}
.searchable-table {
  &-con1 {
    height: 230px !important;
  }
  .margin-top-8 {
    margin-top: 8px;
  }
  .margin-top-10 {
    margin-top: 10px;
  }
  .margin-top-20 {
    margin-top: 20px;
  }
  .margin-left-10 {
    margin-left: 10px;
  }
  .margin-bottom-10 {
    margin-bottom: 10px;
  }
  .margin-bottom-100 {
    margin-bottom: 100px;
  }
  .margin-right-10 {
    margin-right: 10px;
  }
  .padding-left-6 {
    padding-left: 6px;
  }
  .padding-left-8 {
    padding-left: 5px;
  }
  .padding-left-10 {
    padding-left: 10px;
  }
  .padding-left-20 {
    padding-left: 20px;
  }
  .height-100 {
    height: 100%;
  }
  .height-120px {
    height: 100px;
  }
  .height-200px {
    height: 200px;
  }
  .height-492px {
    height: 492px;
  }
  .height-460px {
    height: 460px;
  }
  .line-gray {
    height: 0;
    border-bottom: 2px solid #dcdcdc;
  }
  .notwrap {
    word-break: keep-all;
    white-space: nowrap;
    overflow: hidden;
    text-overflow: ellipsis;
  }
  .padding-left-5 {
    padding-left: 10px;
  }
  [v-cloak] {
    display: none;
  }
}
</style>
