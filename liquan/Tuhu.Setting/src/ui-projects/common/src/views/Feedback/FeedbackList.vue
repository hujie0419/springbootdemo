<template>
<div>
     <Breadcrumb :style="{margin: '24px 0'}">
       <BreadcrumbItem> 问题反馈管理</BreadcrumbItem>
       <BreadcrumbItem>反馈列表</BreadcrumbItem>
     </Breadcrumb>
     <Content :style="{padding: '24px', minHeight: '580px', background: '#fff'}">
         <div class="conditiondiv">
            <div class="">
               <div>
                    <span class="date">问题类型:</span>
                    <div >
                       <CheckboxGroup style="margin-left:78px;"  @on-change="SelectQuestionType" v-model="selectTypes">
                            <Checkbox  label="0" key="0"  :v-text="0"> <span>全部</span></Checkbox>
                            <Checkbox  v-for="item in questionTypes" :label="item.Id" :key="item.Id"  ref="checkboxGroup"> 
                                 <span>{{item.TypeName}}</span>
                            </Checkbox>
                       </CheckboxGroup>
                    </div>
               </div>
               <div>
                   <span class="date">日期:</span>
                   <div>
                      <RadioGroup class="date m10"  @on-change="DateRangeChange" v-model="dtscope" >
                            <Radio  label="0" true-value="0" >全部</Radio >
                            <Radio  label="1" true-value="1" >一周内</Radio >
                            <Radio  label="2" true-value="2">一个月内</Radio >
                            <Radio  label="3" true-value="3" >具体时间段</Radio >
                      </RadioGroup >
                      <div class="date" style="width:36%" v-if="dateShow">
                         <Row>
                                <DatePicker @on-change="DateChange" type="daterange" confirm placement="bottom-end" placeholder="Select date" style="width: 200px" ></DatePicker>
                            
                           </Row>
                      </div>
                      
                   </div>
               </div>
            </div>
            <div class="search">
                <Button type="primary" icon="ios-search" @click="Search">搜索</Button>
                <Button type="primary" icon="ios-search" @click="Export" style="margin-left:10px">导出</Button>
            </div>
         </div>
         <div style="display:none;width:260px;height:460px;z-index:999;position:absolute" ref="imgDiv"><img :src="bigImage" style="width:100%;height:100%"></div>
          <Table border :columns="columns" :data="tableData" stripe size="small" :loading="loading"></Table> 
          <div style="margin: 10px;overflow: hidden">
              <div class="right">
                   <Page   show-total :total="total" :current="pageIndex" @on-change="ChangePage" show-sizer  @on-page-size-change="PageSizeChange"></Page>
              </div>
    </div>
      </Content>
  </div>
 </template>
 <style scoped>
   .conditiondiv{
       border: solid 10px #e8eaec;
       height: 106px;
       font-size: 14px;
       margin-bottom: 36px;
       line-height:40px;
   }
   .conditiondiv >div:first-of-type{
       width: 80%;
       float: left;
   }
   .search{
    width: 19%;
    float: left;
    line-height: 5;
   }
   .date{
       float: left;
   }
   .right{
       float: right;
   }
   .m10{
  margin-left: 16px;
   }
 </style>
 
 <script>
    export default {
        data () {
            return {
                phoneModel: [],
                versionNums: [],
                questionTypes: [],
                networkEnvironment: [],
                tableData: [],
                loading: true,
                startDate: '',
                endDate: '',
                phoneModelValue: '手机型号',
                versionNumsValue: '版本号',
                networkEnvironmentValue: '网络环境',
                dateShow: false,
                dtscope: '0',
                selectTypes: ["0"],
                pageIndex: 1,
                pageSize: 10,
                total: 0,
                bigImage: '',
                bigimgShow: false,
                columns: [
                    {
                        title: '序号',
                        type: 'index',
                        width: 50
                    },
                    {
                        title: '问题类型',
                        key: 'TypeName',
                         width: 100
                    },
                    {
                        title: '日期',
                        key: 'CreateTime',
                         width: 150
                    },
                     {
                        title: '手机号',
                        key: 'UserPhone',
                        width: 130
                    },
                    {
                        title: '反馈问题',
                        key: 'FeedbackContent',
                         width: 800
                    },
                    {
                        title: '图片',
                        key: 'images',
                        width: 200,
                        render: (h, params) => {
                            return h('div', params.row.FeedbackImgs.map(item => {
                                var imgsrc = ""
                                if (item != null) {
                                  imgsrc = item.replace('[', '').replace(']', '')
                                   return h('img', {
                                    attrs: {
                                        src: imgsrc,
                                        width: 30,
                                        height: 40

                                    },
                                    on: {
                                        'mouseover': (value) => {
                                            let src = value.toElement.src
                                            let x = value.pageX
                                            let y = value.pageY
                                            this.BigImage(src, x, y)
                                        },
                                        'mouseout': () => {
                                         this.HideBigImage()
                                        }
                                    }
                                })
                                }
                            }))
                        }
                        
                    },
                     {
                        title: '版本号',
                        key: 'VersionNum',
                           width: 130,
                        renderHeader: (h) => {
                          return h('Select', {
                              props: {
                                  'transfer': true,
                                  placeholder: this.versionNums[0]
                              },
                              on: {
                                  'on-change': (value) => {
                                    this.versionNumsValue = value;
                                    this.tableData = this.TableDate();
                                  }
                              }
                          }, this.versionNums.map((item) => {
                             return h('Option', {
                                     props: {
                                          value: item,
                                          label: item
                                     }
                                  })
                          }))
                        }
                    },
                     {
                        title: '手机型号',
                        key: 'PhoneModels',
                         width: 130,
                         renderHeader: (h) => {
                        return h('Select', {
                              props: {
                                  'transfer': true,
                                  placeholder: this.phoneModel[0]
                              },
                              on: {
                                  'on-change': (value) => {
                                    this.phoneModelValue = value;
                                    this.tableData = this.TableDate();
                                  }
                              }
                          }, this.phoneModel.map((item) => {
                             return h('Option', {
                                     props: {
                                          value: item,
                                          label: item
                                     }
                                  })
                          }))
                        }
                    },
                    {
                        title: '网络环境',
                        key: 'NetworkEnvironment',
                        width: 130,
                        renderHeader: (h) => {
                          return h('Select', {
                              props: {
                                  'transfer': true,
                                   placeholder: this.networkEnvironment[0]
                              },
                              on: {
                                  'on-change': (value) => {
                                    this.networkEnvironmentValue = value;
                                    this.tableData = this.TableDate();
                                  }
                              }
                          }, this.networkEnvironment.map((item) => {
                             return h('Option', {
                                     props: {
                                          value: item,
                                          label: item,
                                          Select: item === this.networkEnvironmentValue

                                     }
                                  })
                          }))
                        }
                    }
                ]
            }
        },
         mounted () {
             this.GetPhoneModel();
             this.GetVersionNums();
             this.GetNetworkEnvironment();
             this.TableDate();
             this.GetQuestionTypes();
         },
        methods: {
            PageSizeChange (pageSize) {
              this.pageSize = pageSize;
              this.TableDate();
            },
            ChangePage (value) {
                this.pageIndex = value
               this.TableDate();
            },
            TableDate () {
               if (this.networkEnvironmentValue === "网络环境") {
                   this.networkEnvironmentValue = "";
               }
               if (this.versionNumsValue === "版本号") {
                   this.versionNumsValue = "";
               }
               if (this.phoneModelValue === "手机型号") {
                   this.phoneModelValue = "";
               }
               var typeId = "";
               if (this.selectTypes[0] !== "0") {
                typeId = this.selectTypes.join(',');
               } 
               var params = 'pageIndex=' + this.pageIndex + "&pageSize=" + this.pageSize + "&typeIds=" + typeId + "&flag=" + this.dtscope + "&versionNum=" + this.versionNumsValue + "&phoneModel=" + this.phoneModelValue + "&networkEnvir=" + this.networkEnvironmentValue + "&startTime=" + this.startDate + "&endTime=" + this.endDate;
               this.util.ajax.get('/FeedBack/GetFeedbackListByCondition?' + params).then(res => {
                    if (res.data) {
                        var result = res.data.data;
                         if (result) {
                             this.tableData = result.FeedbackList;
                             this.total = result.total;
                             this.loading = false
                         }  
                    }
                }) 
            },
            GetVersionNums () {
                this.versionNums.push("版本号")
                this.util.ajax.get('/FeedBack/GetVersionNumList').then(res => {
                    if (res.data) {
                           res.data.data.forEach(element => {
                               this.versionNums.push(element);
                           }); 
                    }
                })
            },
             GetPhoneModel () {
                 this.phoneModel.push("手机型号")
                this.util.ajax.get('/FeedBack/GetPhoneModelList').then(res => {
                    if (res.data) {
                           res.data.data.forEach(element => {
                               this.phoneModel.push(element);
                           }); 
                    }
                })
            },
             GetNetworkEnvironment () {
                this.networkEnvironment.push("网络环境");
                this.util.ajax.get('/FeedBack/GetNetworkEnvironmentList').then(res => {
                    if (res.data) {
                           res.data.data.forEach(element => {
                               this.networkEnvironment.push(element);
                           }); 
                    }
                })
            },
            Search () {
               this.TableDate();
            },
            GetQuestionTypes () {
                this.util.ajax.get('/FeedBack/GetQuestionTypeList').then(res => {
                    if (res.data) {
                          var result = res.data.data;
                          if (result) {
                              this.questionTypes = result;
                          }
                    }
                })
            },
            DateRangeChange (item) {
              this.dtscope = item;
              if (item === "3") {
                  this.dateShow = true;
               } else {
                   this.dateShow = false;
               }
            },
            DateChange (date) {
              this.startDate = date[0];
              this.endDate = date[1];
            },
            Export () {
              if (this.networkEnvironmentValue === "网络环境") {
                   this.networkEnvironmentValue = "";
               }
               if (this.versionNumsValue === "版本号") {
                   this.versionNumsValue = "";
               }
               if (this.phoneModelValue === "手机型号") {
                   this.phoneModelValue = "";
               }
               var typeId = "";
               if (this.selectTypes[0] !== "0") {
                typeId = this.selectTypes.join(',');
               } 

                 var params = 'pageIndex=' + this.pageIndex + "&pageSize=" + this.pageSize + "&typeIds=" + typeId + "&flag=" + this.dtscope + "&versionNum=" + this.versionNumsValue + "&phoneModel=" + this.phoneModelValue + "&networkEnvir=" + this.networkEnvironmentValue + "&startTime=" + this.startDate + "&endTime=" + this.endDate;
                  window.location.href = '/FeedBack/ExportToExcel?' + params;
            },
            SelectQuestionType (data) {
                var checkboxGroup = this.$refs.checkboxGroup;
                var typeIds = data;
                if (this.ArrayContain(data, "0") && typeIds.length > 1 && typeIds[typeIds.length - 1] !== "0") {   
                     for (var i = 0; i < typeIds.length; i++) {
                            if (typeIds[i] === "0") {
                                 typeIds.splice(i, 1); break;
                            }
                        }
                } else if (typeIds.length === checkboxGroup.length || typeIds[typeIds.length - 1] === "0") {
                    typeIds = ["0"];
                 } else {
                     typeIds = data;
                 }
                 this.selectTypes = typeIds;
             },
             ArrayContain (array, id) {
                 var result = false;
               for (var i = 0; i < array.length; i++) {
                   if (array[i] === id) {
                     result = true; break;
                   }
               }
                 return result;
             },
             kf (row) {
                this.$Modal.confirm({
                title: "温馨提示",
                content: "确定客服介入?",
                onOk: () => {
                     this.util.ajax.post("/FeedBack/CreateTaskNeiBuWithDistinctAsync", {
                         'remark': row.FeedbackContent,
                         'phone': row.UserPhone,
                         'appVersion': row.VersionNum,
                         'phoneModel': row.PhoneModels,
                         'id': row.Id
                     }).then(res => {
                         if (res.data) {
                           let result = res.data.data
                           if (result !== 0) {
                                row.IsCustomerServer = 1
                                this.$Message.info("客服介入成功！")
                            }
                         }
                     })
                }
             });
            },
            BigImage (value, x, y) {
                var imgdiv = this.$refs.imgDiv
                imgdiv.style.marginLeft = (x - 200) + 'px'
                imgdiv.style.marginTop = (y - 500) + 'px'
                imgdiv.style.display = 'block'
                this.bigImage = value
            },
            HideBigImage () {
                var imgdiv = this.$refs.imgDiv
                this.bigImage = ''
                 imgdiv.style.display = 'none'
            }
        }
    }
</script>
