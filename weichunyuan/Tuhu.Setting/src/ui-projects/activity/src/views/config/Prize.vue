<template>
  <div>
      <h1 class="title">兑换物配置</h1>
      <div>
        <label>兑换物名称</label>
        <input v-model="filters.ActivityPrizeName"  style="width: 150px" />
        <lable style="margin-left:13px">兑换物状态</lable>
            <Select v-model="filters.OnSale" placeholder="上架状态" style="width:200px">
                <Option v-for="item in SaleStatusList" :value="item.value" :key="item.value">{{ item.label }}</Option>
            </Select>  
        
      </div>
      <div style="margin-top:18px">
          <Button type="success" icon="search" @click="loadData(1)">搜索</Button>
          <Button type="success" icon="plus" style="margin-left:20px" @click="searchActivityPrize(0)">新增兑换物</Button>         
          <Button type="success" icon="load-c" style="margin-left:20px;" @click="RefreshPrizeCache">刷新缓存</Button>        
      </div>
      <div style="margin-top:18px">
        <Table  :loading="table.loading" :data="table.data" :columns="table.columns" style="width:100%" stripe></Table>
        <div style="margin: 10px;overflow: hidden">
            <div style="float: right;">
               <Page :total="page.total" :page-size="page.pageSize" :current="page.current" :page-size-opts="[10 ,30 ,50]"  show-sizer @on-change="handlePageChange" @on-page-size-change="handlePageSizeChange"></Page>
            </div>
        </div>
      </div>  
      <Modal v-model="logmodal.visible" title="操作日志" cancelText="取消" @on-cancel="cancel" scrollable= true :width="logmodal.width">
            <Table :loading="logmodal.loading" :data="logmodal.data" :columns="logmodal.columns" stripe></Table>
        </Modal>  
       <Modal
        v-model="deleteModal.visible"
        title="删除"
        :loading="deleteModal.loading"
        @on-ok="deleteok" :mask-closable="false">
        <p>确认删除？</p>
    </Modal>   
     <Modal
        v-model="onsaleModal.visible"
        title="上架"
        :loading="onsaleModal.loading"
        @on-ok="onsaleok" :mask-closable="false">
        <p>确认上架？</p>
    </Modal>   
    <Modal
        v-model="offsaleModal.visible"
        title="下架"
        :loading="offsaleModal.loading"
        @on-ok="offsaleok" :mask-closable="false">
        <p>确认下架？</p>
    </Modal>   
     <Modal v-model="insertmodal.visible" :loading="insertmodal.loading" title="添加兑换物" okText="保存"  cancelText="取消" @on-ok="SaveActivityPrizeConfig"   width="50%" :mask-closable="false">
       <Form ref="insertmodal.activityPrize" :model="insertmodal.activityPrize" :rules="ruleValidate" :label-width="90">       
            <FormItem label="兑换物名称" >
                <Input v-model="insertmodal.activityPrize.ActivityPrizeName"  />
            </FormItem>        
          <FormItem label="上传缩略图">
                    <Col span="4" v-show="insertmodal.activityPrize.PicUrl!=''">
                    <a :href="insertmodal.activityPrize.PicUrl" target="_blank"><img :src="insertmodal.activityPrize.PicUrl" style='width:50px;height:50px'></a>
                    </Col>
                    <Col span="9">
                    <Upload action="/GroupBuyingV2/UploadImage?type=Prizeimage" :format="['jpg','jpeg','png']" :on-format-error="handleFormatError" :max-size="10000" :on-exceeded-size="handleMaxSize" :on-success="handleSuccess" :show-upload-list="false">
                        <Button type="ghost" icon="ios-cloud-upload-outline">上传图片</Button>
                    </Upload>
                    </Col>
                    <Col span="9" v-show="insertmodal.activityPrize.PicUrl!=''">
                    <Button type="warning" icon="refresh" @click="insertmodal.activityPrize.PicUrl=''">清除</Button>
                    </Col>
                </FormItem>
             <FormItem label="图片地址" >
                <Input v-model="insertmodal.activityPrize.PicUrl"  />
            </FormItem>
            <FormItem label="兑换券数量" >
                <Input v-model="insertmodal.activityPrize.CouponCount"  />
            </FormItem>                        
              <FormItem label="库存">
                <Input v-model="insertmodal.activityPrize.SumStock" />
            </FormItem>                  
            <FormItem label="是否支持兑换">
                <Row>
                      <Col span="11">
                       <input type="radio" v-model="insertmodal.activityPrize.IsDisableSale" value=false />是                      
                        </Col>
                        <Col span="11">
                       <input type="radio" v-model="insertmodal.activityPrize.IsDisableSale" value=true />否    
                        </Col>
                    </Row>
            </FormItem>
              <FormItem label="优惠券 GUID">
                <Input v-model="insertmodal.activityPrize.GetRuleId"  />
            </FormItem>           
        </Form>                                   
    </Modal>
      <Modal v-model="modal.visible" :loading="modal.loading" title="修改兑换物" okText="保存"  cancelText="取消" @on-ok="UpdateActivityPrizeStock"   width="50%" :mask-closable="false">       
       <Form ref="modal.activityPrize" :model="modal.activityPrize" :rules="ruleValidate" :label-width="90">
       <FormItem label="兑换商品id" prop="PID">
                <Input v-model="modal.activityPrize.PID" :disabled="modal.disableedit" />
            </FormItem>   
            <FormItem label="兑换物名称" prop="ActivityPrizeName">
                <Input v-model="modal.activityPrize.ActivityPrizeName" :disabled="modal.disableedit" />
            </FormItem>                 
             <FormItem label="图片地址" prop="PicUrl">
                <Input v-model="modal.activityPrize.PicUrl" :disabled=modal.disableedit />
            </FormItem>
            <FormItem label="兑换券数量" prop="CouponCount">
                <Input v-model="modal.activityPrize.CouponCount" :disabled=modal.disableedit  />
            </FormItem>            
             <FormItem label="库存改动量" prop="UpdateStock">
                <Input v-model="modal.activityPrize.UpdateStock" />
            </FormItem>  
              <FormItem label="当前库存">
                <Input v-model="modal.activityPrize.Stock" :disabled=true />
            </FormItem>  
              <FormItem label="总库存">
                <Input v-model="modal.activityPrize.SumStock" :disabled=true />
            </FormItem>                  
            <FormItem label="是否支持兑换">
                <Row>
                      <Col span="11">
                       <input type="radio" v-model="modal.activityPrize.IsDisableSale" value=false :disabled=modal.disableedit />是                      
                        </Col>
                        <Col span="11">
                       <input type="radio" v-model="modal.activityPrize.IsDisableSale" value=true :disabled=modal.disableedit />否    
                        </Col>
                    </Row>
            </FormItem>
              <FormItem label="优惠券 GUID">
                <Input v-model="modal.activityPrize.GetRuleId" :disabled=modal.disableedit  />
            </FormItem> 
        </Form>                                   
      </Modal>       
      <Modal v-model="searchmodal.visible" :loading="searchmodal.loading" title="查看兑换物"   width="50%" :mask-closable="false">       
       <Form ref="searchmodal.activityPrize" :model="searchmodal.activityPrize" :rules="ruleValidate" :label-width="90">          
            <FormItem label="兑换物名称">
                <Input v-model="searchmodal.activityPrize.ActivityPrizeName" :disabled=true />
            </FormItem>                 
             <FormItem label="图片地址">
                <Input v-model="searchmodal.activityPrize.PicUrl" :disabled=true />
            </FormItem>
            <FormItem label="兑换券数量">
                <Input v-model="searchmodal.activityPrize.CouponCount" :disabled=true  />
            </FormItem>            
             <FormItem label="当前库存">
                <Input v-model="searchmodal.activityPrize.Stock" :disabled=true />
            </FormItem>  
              <FormItem label="总库存">
                <Input v-model="searchmodal.activityPrize.SumStock" :disabled=true />
            </FormItem>         
            <FormItem label="是否支持兑换">
                <Row>
                      <Col span="11">
                       <input type="radio" v-model="searchmodal.activityPrize.IsDisableSale" value=false :disabled=true />是                      
                        </Col>
                        <Col span="11">
                       <input type="radio" v-model="searchmodal.activityPrize.IsDisableSale" value=true :disabled=true />否    
                        </Col>
                    </Row>
            </FormItem>
              <FormItem label="优惠券 GUID">
                <Input v-model="searchmodal.activityPrize.GetRuleId" :disabled=true  />
            </FormItem> 
        </Form>    
         <div slot="footer"></div>                                
      </Modal>       
  </div>
</template>
<style>
body .ivu-modal .ivu-select-dropdown {
    position: fixed !important;
}
</style>
<script>
  export default {
    data () {
        return {
          index: 0,
          isAdd: 1,
          page: {
            total: 0,
            current: 1,
            pageSize: 10
          },
          logpage: {
            total: 0,
            current: 1,
            pageSize: 5
          },
          filters: {
            ActivityPrizeName: '',
            OnSale: -1
          },
          tablemodel: {
            loading: true,
            visible: false,
            width: 1355,
            data: []
          },
                  logmodal: {
              loading: true,
              visible: false,
              width: 635,
              data: [],
              columns: [
                  {
                      title: "操作内容",
                      width: 300,
                      key: "Remark",
                      align: "center",
                      fixed: "left"
                  },
                  {
                      title: "操作人",
                      width: 150,
                      key: "Creator",
                      align: "center",
                      fixed: "left"
                  },
                  {
                      title: "时间",
                      width: 150,
                      key: "CreateDateTime",
                      render: (h, params) => {
                          return h(
                              "span",
                              this.formatDate(params.row.CreateDateTime)
                          );
                      }
                  }                  
              ]
          },
          table: {
            loading: true,
            data: [],
            columns: [
              {
                title: "#",
                width: 50,
                align: "center",
                fixed: "left",
                type: "index",
                key: "PKID"
              },
              {
                title: "兑换物名称",
                key: "ActivityPrizeName"              
              },
              {
                title: "缩略图",               
                key: "PicUrl",
                                render: (h, params) => {
                  return h('img', {
             attrs: {
                src: params.row.PicUrl
              },
              style: {
                display: 'inline-block',
                width: '100%'
              }
            })
                }
              },
              {
                title: "兑换券数量",
                 align: "center",
                key: "CouponCount"               
              },
              {
                title: "总库存",
               align: "center",
                key: "SumStock"                
              }, 
              {
                title: "当前库存",
                 align: "center",
                key: "Stock"                
              }, 
              {
                title: "是否支持兑换",
                  align: "center",
                key: "IsDisableSale",
                 render: (h, params) => {
                          return h(
                              "span",
                              this.ConvertBoolValue(params.row.IsDisableSale)
                          );
                      }
              },
               {
                title: "状态",
                  align: "center",
                key: "OnSale",
                 render: (h, params) => {
                          return h(
                              "span",
                              this.ConvertValue(params.row.OnSale)
                          );
                      }
              },
               {
                title: "是否删除",               
                key: "IsDeleted",
                  align: "center",
                 render: (h, params) => {
                          return h(
                              "span",
                              this.ConvertDeletedBoolValue(params.row.IsDeleted)
                          );
                      }
              }, 
              {
                        title: "操作",
                        key: "action",
                        align: "center",
                        // fixed: "right",
                        width: 300,
                        render: (h, params) => {
                            return h("div", [
                                h(
                                    "Button",
                                    {
                                        props: {
                                            type: "primary",
                                            size: "small",
                                            disabled: params.row.OnSale === 1 || params.row.IsDeleted
                                        },
                                        style: {
                                            marginRight: "5px"
                                        },
                                        on: {
                                            click: () => {
                                               if (params.row.OnSale === 1) {
                                                   this.$Message.warning("当前商品已上架");
                                               } else {
                                                   this.onsaleModal.visible = true;
                                                   this.onsaleModal.PKID = params.row.PKID
                                               }
                                            }
                                        }
                                    },
                                    "上架"
                                ),
                                 h(
                                    "Button",
                                    {
                                        props: {
                                            type: "primary",
                                            size: "small",
                                            disabled: params.row.OnSale === 0 || params.row.IsDeleted
                                        },
                                        style: {
                                            marginRight: "5px"
                                        },
                                        on: {
                                            click: () => {
                                               if (params.row.OnSale === 0) {
                                                   this.$Message.warning("当前商品已下架");
                                               } else {
                                                   this.offsaleModal.visible = true;
                                                   this.offsaleModal.PKID = params.row.PKID
                                               }
                                            }
                                        }
                                    },
                                    "下架"
                                ),
                                h(
                                    "Button",
                                    {
                                        props: {
                                            type: "primary",
                                            size: "small"
                                        },
                                        style: {
                                            marginRight: "5px"
                                        },
                                        on: {
                                            click: () => {
                                                this.queryActivityPrize(
                                                    params.row.PKID,
                                                    "Search"
                                                );
                                            }
                                        }
                                    },
                                    "查看"
                                ),
                                h(
                                    "Button",
                                    {
                                        props: {
                                            type: "primary",
                                            size: "small",
                                            disabled: params.row.OnSale === 1 || params.row.IsDeleted
                                        },
                                        style: {
                                            marginRight: "5px"
                                        },
                                        on: {
                                            click: () => {
                                                  if (params.row.OnSale === 1) {
                                                   this.$Message.warning("当前商品已上架，不支持修改");
                                                   this.deleteModal.visible = false;
                                               } else {
                                                this.searchActivityPrize(
                                                    params.row.PKID,
                                                    "Update"
                                                );
                                               }
                                            }
                                        }
                                    },
                                    "修改"
                                ),
                                h(
                                    "Button",
                                    {
                                        props: {
                                            type: "primary",
                                            size: "small",
                                            disabled: params.row.OnSale === 1 || params.row.IsDeleted
                                        },
                                        on: {
                                            click: () => {
                                               if (params.row.OnSale === 1) {
                                                   this.$Message.warning("当前商品已上架，不支持删除");
                                                   this.deleteModal.visible = false;
                                               } else {
                                                   this.deleteModal.visible = true;
                                                   this.deleteModal.PKID = params.row.PKID;
                                               }
                                            }
                                        }
                                    },
                                    "删除"
                                )
                            ]);
                        }
                    },
                     {
                title: "日志查看",
              
                align: "center",
                render: (h, params) => {
                  return h("div", [
                      h(
                          "Button",
                          {
                              props: {
                                  type: "primary",
                                  size: "small"                                 
                              },
                              style: {
                                  marginRight: "5px"
                              },
                              on: {
                                  click: () => {                                     
                                      this.SearchLog(
                                          params.row.PKID,
                                          "Search"
                                      )
                                  }
                              }
                          },
                          "查看日志"
                      )
                  ])
                }
              }                            
            ]
          },
         deleteModal: {
            visible: false,
            loading: true,
            PKID: 0
        },
          onsaleModal: {
            visible: false,
            loading: true,
            PKID: 0
        },
         offsaleModal: {
            visible: false,
            loading: true,
            PKID: 0
        },
        SaleStatusList: [
                {
                    label: "未上架",
                    value: 0
                },
                {
                    label: "上架",
                    value: 1
                },
                {
                    label: '所有状态',
                    value: -1
                }
            ],
        modal: {
            visible: false,
            loading: true,
            disableedit: false,
            activityPrize: {
            PKID: 0,
            ActivityPrizeName: '',
            ActivityId: 0,
            PID: '',
            PicUrl: '',
            CouponCount: 0,
            Stock: 0,
            SumStock: 0,
            UpdateStock: 0,
            OnSale: 0,
            GetRuleId: '00000000-0000-0000-0000-000000000000',
            IsDisableSale: false           
          }
        },  
        insertmodal: {
            visible: false,
            loading: true,
            disableedit: false,
            activityPrize: {
            PKID: 0,
            ActivityPrizeName: '',
            ActivityId: 0,
            PID: '',
            PicUrl: '',
            CouponCount: 0,
            Stock: 0,
            SumStock: 0,
            UpdateStock: 0,
            OnSale: 0,
            GetRuleId: '00000000-0000-0000-0000-000000000000',
            IsDisableSale: false           
          }
        },  
        searchmodal: {
            visible: false,
            loading: true,
            disableedit: false,
            activityPrize: {
            PKID: 0,
            ActivityPrizeName: '',
            ActivityId: 0,
            PID: '',
            PicUrl: '',
            CouponCount: 0,
            Stock: 0,
            SumStock: 0,
            UpdateStock: 0,
            OnSale: 0,
            GetRuleId: '00000000-0000-0000-0000-000000000000',
            IsDisableSale: false           
          }
        }, 
        ruleValidate: {
          ActivityPrizeName: [
              { 
                trigger: 'blur',
                validator: (rule, value, callback) => {
                    if (value === '') {
                        callback(new Error('商品名称不能为空'))
                    } else {
                        callback()
                    }
                } 
              }
          ],
          PID: [
              { 
                trigger: 'blur',
                validator: (rule, value, callback) => {
                    if (value === '') {
                        callback(new Error('商品编号不能为空'))
                    } else {
                        callback()
                    }
                } 
              }
          ],
           PicUrl: [
              { 
                trigger: 'blur',
                validator: (rule, value, callback) => {
                    if (value === '') {
                        callback(new Error('图片地址不能为空'))
                    } else {
                        callback()
                    }
                } 
              }
          ],
          CouponCount: [
            { 
                trigger: 'blur',
                validator: (rule, value, callback) => {
                    if (value === '') {
                        callback(new Error('兑换券数量不能为空'))
                    } else if (isNaN(Number(value))) {
                        callback(new Error("兑换券数量请输入数字"))
                    } else if (Number(value) <= 0) {
                        callback(new Error("兑换券数量必须大于0"))
                    } else {
                        callback()
                    }
                }
            }
          ],
           UpdateStock: [
            { 
                trigger: 'blur',
                validator: (rule, value, callback) => {
                    if (value === '') {
                        callback(new Error('库存改动量不能为空'))
                    } else if (isNaN(Number(value))) {
                        callback(new Error("库存改动量请输入数字"))
                    } else {
                        callback()
                    }
                }
            }
          ],
          GetRuleId: [
              { 
                trigger: 'blur',
                validator: (rule, value, callback) => {
                   var pattern = new RegExp('^[0-9a-f]{8}-[0-9a-f]{4}-[1-5][0-9a-f]{3}-[89ab][0-9a-f]{3}-[0-9a-f]{12}$', 'i');
                        if (pattern.test(value) === false) {
                              callback(new Error("请输入一个正确的GUID"))
                        } else {
                        callback()
                    }
                }
            }
          ]
        }         
        }                  
    },
    created: function () {
         this.$Message.config({
                top: 50,
                duration: 3
            });
      this.loadData(1);
    },
    methods: {
      loadData (pageIndex) {
        this.page.current = pageIndex;
        this.table.loading = true;
        var requestData = "?prizeName=" + this.filters.ActivityPrizeName;
           requestData += "&OnSale=" + this.filters.OnSale;
        requestData += "&pageIndex=" + this.page.current;
        requestData += "&pageSize=" + this.page.pageSize;
        this.ajax
            .get("/GuessGame/GetActivityPrizeList" + requestData)
            .then(response => {
              var data = response.data;
              this.page.total = data.totalCount;
              this.table.data = data.data;
              this.table.loading = false;
            });
      },
        deleteok () {        
        this.deleteModal.loading = true;
        this.ajax.get("/GuessGame/DeleteActivityPrize?pkid=" + this.deleteModal.PKID)
              .then((response) => {
                  console.log(response);
                  if (!response.data) {                  
                        this.$Message.success('删除成功');
                        this.loadData(this.page.current);
                        this.deleteModal.loading = false;
                        this.deleteModal.visible = false;
                        this.$nextTick(() => {
                            this.deleteModal.loading = true;
                        });                  
                  } else {
                      console.log(this.deleteModal);
                    this.$Message.error('删除失败');
                    this.deleteModal.loading = false;
                    this.$nextTick(() => {
                        this.deleteModal.loading = true;
                    });
                  }
              });           
    },
    RefreshPrizeCache () {
       this.ajax.get("/GuessGame/RefreshPrize")
              .then((response) => {
                  console.log(response);
                  if (!response.data) {                  
                        this.$Message.success('刷新缓存成功');
                        this.loadData(this.page.current);                            
                  } else {                   
                    this.$Message.error('刷新缓存失败');     
                  }              
              });  
    },
      onsaleok () {        
        this.onsaleModal.loading = true;
        this.ajax.get("/GuessGame/UpdateActivityPrizeSale?onsale=1&pkid=" + this.onsaleModal.PKID)
              .then((response) => {
                  console.log(response);
                  if (!response.data) {                  
                        this.$Message.success('上架成功');
                        this.loadData(this.page.current);
                        this.onsaleModal.loading = false;
                        this.onsaleModal.visible = false;
                        this.$nextTick(() => {
                            this.onsaleModal.loading = true;
                        });                  
                  } else {                     
                    this.$Message.error('上架失败');
                    this.onsaleModal.loading = false;
                    this.$nextTick(() => {
                        this.onsaleModal.loading = true;
                    });
                  }
              });           
    },
     offsaleok () {        
        this.offsaleModal.loading = true;
        this.ajax.get("/GuessGame/UpdateActivityPrizeSale?onsale=0&pkid=" + this.offsaleModal.PKID)
              .then((response) => {
                  console.log(response);
                  if (!response.data) {                  
                        this.$Message.success('下架成功');
                        this.loadData(this.page.current);
                        this.offsaleModal.loading = false;
                        this.offsaleModal.visible = false;
                        this.$nextTick(() => {
                            this.offsaleModal.loading = true;
                        });                  
                  } else {                     
                    this.$Message.error('下架失败');
                    this.offsaleModal.loading = false;
                    this.$nextTick(() => {
                        this.offsaleModal.loading = true;
                    });
                  }
              });           
    },
      search (time) {
        if (time) {
           if (new Date(this.formatDate(time)) < new Date()) {
               this.modal.disableedit = true;
           } else {
               this.modal.disableedit = false;
           }
           
            this.ajax
          .get("/GuessGame/GetQuestionAnswerList?endTime=" + this.formatDate(time))
          .then(response => {
              if (response.data.data[0]) {
                this.modal.QuestionAnswer1 = response.data.data[0];                               
                this.modal.visible = true;
              } else if (response.data.data[1]) {
                this.modal.QuestionAnswer2 = response.data.data[1];
                this.modal.visible = true;
              } else if (response.data.data[2]) {
                this.modal.QuestionAnswer3 = response.data.data[2];
                this.modal.visible = true;
              }
          });
        } 
      },
       queryActivityPrize (pkid) {         
        if (pkid > 0) {                         
            this.ajax
          .get("/GuessGame/GetActivityPrize?pkid=" + pkid)
          .then(response => {                           
                this.searchmodal.activityPrize = response.data;                                                                  
                this.searchmodal.visible = true;
                 this.searchmodal.disableedit = true;              
          });
        } else {
              this.searchmodal.visible = true;
              this.searchmodal.disableedit = false;
              this.searchmodal.activityPrize = {
                   PKID: 0,
                    ActivityPrizeName: '',
                    ActivityId: 0,
                    PID: '',
                    PicUrl: '',
                    CouponCount: 0,
                    Stock: 0,
                    SumStock: 0,
                    OnSale: 0,
                    GetRuleId: '00000000-0000-0000-0000-000000000000',
                    IsDisableSale: false                 
              }
        }
      },
      searchActivityPrize (pkid) {         
        if (pkid > 0) {                         
            this.ajax
          .get("/GuessGame/GetActivityPrize?pkid=" + pkid)
          .then(response => {                           
                this.modal.activityPrize = response.data;                                                                  
                this.modal.visible = true;
                 this.modal.disableedit = true;              
          });
        } else {
             // this.$refs['ActivityPrizeName'].resetFields();
            //  this.$refs['CouponCount'].resetFields();          
             // this.$refs[this.insertmodal.activityPrize].resetFields();    
              this.insertmodal.visible = true;
              this.insertmodal.disableedit = false;
              this.insertmodal.activityPrize = {
                   PKID: 0,
                    ActivityPrizeName: '',
                    ActivityId: 0,
                    PID: '',
                    PicUrl: '',
                    CouponCount: 0,
                    Stock: 0,
                    SumStock: 0,
                    OnSale: 0,
                    GetRuleId: '00000000-0000-0000-0000-000000000000',
                    IsDisableSale: false                
              }
        }
      },
    SearchLog (PKID) {        
        this.logmodal.loading = true;        
        this.ajax
            .post("/CommonConfigLog/GetCommonConfigLogs", {
                objectType: "WorldCupConfigPrize",
                objectId: PKID
            })
            .then(response => {     
                if (response.data) {
                this.logmodal.data = response.data;                              
                } else {
                     this.logmodal.data = [];
                }
                 this.logmodal.visible = true;
                this.logmodal.loading = false;
            });
      },     
      handleFormatError (file) {
            this.$Message.warning("请选择 .jpg  or .png  or .jpeg图片");
        },
        handleMaxSize (file) {
            this.$Message.warning("请选择不超过100KB的图片");
        },
        handleSuccess (res, file) {
            if (res.Status) {
              this.insertmodal.activityPrize.PicUrl = res.ImageUrl;
            } else {
                this.$Message.warning(res.Msg);
            }
        },
      SaveActivityPrizeConfig () {
        var that = this;
        that.insertmodal.loading = true;       
        if (!this.insertmodal.activityPrize.ActivityPrizeName) {
              this.$Message.warning("商品名称不允许为空");  
               that.$nextTick(
            () => {
                    that.insertmodal.loading = true;
                  }
          );
          that.insertmodal.loading = false;
           return;          
        }
        if (isNaN(Number(this.insertmodal.activityPrize.CouponCount))) {
              this.$Message.warning("兑换券数量必须为数字型");  
               that.$nextTick(
            () => {
                    that.insertmodal.loading = true;
                  }
          );
          that.insertmodal.loading = false;
           return;          
        }
        if (this.insertmodal.activityPrize.IsDisableSale === false) {
             if (Number(this.insertmodal.activityPrize.CouponCount) <= 0) {
                 this.$Message.warning("可兑换商品的兑换券数量必须为正整数");  
               that.$nextTick(
            () => {
                    that.insertmodal.loading = true;
                  }
          );
          that.insertmodal.loading = false;
           return;      
             }             
        } else {
            if (Number(this.insertmodal.activityPrize.CouponCount) < 0) {
                 this.$Message.warning("不可兑换商品的兑换券数量必须为非负数");  
               that.$nextTick(
            () => {
                    that.insertmodal.loading = true;
                  }
          );
          that.insertmodal.loading = false;
           return;     
            }
        }        
        if (!this.insertmodal.activityPrize.PicUrl) {
              this.$Message.warning("请上传商品图片");    
               that.$nextTick(
            () => {
                    that.insertmodal.loading = true;
                  }
          );
          that.insertmodal.loading = false;
           return;        
        }
        if (!this.insertmodal.activityPrize.SumStock || isNaN(Number(this.insertmodal.activityPrize.SumStock))) {
              this.$Message.warning("库存量必须为数字");    
               that.$nextTick(
            () => {
                    that.insertmodal.loading = true;
                  }
          );
          that.insertmodal.loading = false;
           return;        
        }
        console.log(this.ValidateGuidid(this.insertmodal.activityPrize.GetRuleId));
        if (this.insertmodal.activityPrize.IsDisableSale === false) {
        if (!this.insertmodal.activityPrize.GetRuleId || this.ValidateGuidid(this.insertmodal.activityPrize.GetRuleId) === false) {
               this.$Message.warning("请输入有效的GUID");    
                that.$nextTick(
            () => {
                    that.insertmodal.loading = true;
                  }
          );
          that.insertmodal.loading = false;
           return;        
        }       
        }
            that.ajax
            .post("/GuessGame/InsertActivityPrize", {
                activityPrize: that.insertmodal.activityPrize
            })
            .then(response => {
                console.log(!response.data);
                if (!response.data) {
                    that.$Message.success("保存成功");
                    that.insertmodal.visible = false;
                    that.loadData(that.page.current);
                } else {
                    that.$Message.error(response.data);
                    that.$nextTick(() => {
                        that.insertmodal.loading = true;
                    });
                }
                that.insertmodal.loading = false;
            });        
      },
      UpdateActivityPrizeStock () {
          var that = this;
              if (!this.modal.activityPrize.UpdateStock || isNaN(Number(this.modal.activityPrize.UpdateStock))) {
              this.$Message.warning("改动库存量必须为数字");    
               that.$nextTick(
            () => {
                    that.modal.loading = true;
                  }
          );
          that.modal.loading = false;
           return;        
        }
        if (this.modal.activityPrize.PKID > 0) {
        that.ajax
            .post("/GuessGame/UpdateActivityPrize", {
                activityPrize: that.modal.activityPrize
            })
            .then(response => {
                if (!response.data) {
                    that.$Message.success("操作成功");
                    that.modal.visible = false;
                    that.loadData(that.page.current);
                } else {
                    that.$Message.error(response.data);
                    that.$nextTick(() => {
                        that.modal.loading = true;
                    });
                }
                that.modal.loading = false;
            });
        }
      },
      SaveQuestionOptionConfig () {
        var that = this; 
        if (!this.questionmodal.Question1.StartTime || !this.questionmodal.Question1.EndTime || !this.questionmodal.Question1.DeadLineTime) {    
            that.$Message.warning("第一题题目结束时间,开始时间,竞猜截至时间不能为空,请设置");
             that.$nextTick(
            () => {
                    that.questionmodal.loading = true;
                  }
          );
          that.questionmodal.loading = false;
          return;
        }       
        this.questionmodal.Question1.StartTime = this.formatDate(this.questionmodal.Question1.StartTime).replace("00:00:00", "11:00:00");
        this.questionmodal.Question1.EndTime = this.formatDate(this.questionmodal.Question1.EndTime).replace("00:00:00", "11:00:00");
        this.questionmodal.Question1.DeadLineTime = this.formatDate(this.questionmodal.Question1.DeadLineTime);  
        console.log(this.questionmodal.Question1.StartTime);
        console.log(this.questionmodal.Question1.EndTime);
        console.log(this.questionmodal.Question1.DeadLineTime);
        if (!this.isLater(this.questionmodal.Question1.EndTime, this.questionmodal.Question1.StartTime)) {
             that.$Message.warning("第一题题目结束时间不能早于开始时间");
                that.$nextTick(
            () => {
                    that.questionmodal.loading = true;
                  }
          );
          that.questionmodal.loading = false;
           return;
        }
        console.log(this.isLater(this.questionmodal.Question1.EndTime, this.questionmodal.Question1.DeadLineTime));
        console.log(this.isLater(this.questionmodal.Question1.DeadLineTime, this.questionmodal.Question1.StartTime));
        if (!this.isLater(this.questionmodal.Question1.EndTime, this.questionmodal.Question1.DeadLineTime) || !this.isLater(this.questionmodal.Question1.DeadLineTime, this.questionmodal.Question1.StartTime)) {       
              that.$Message.warning("第一题题目竞猜截至时间请介于开始时间和结束时间之间");
                that.$nextTick(
            () => {
                    that.questionmodal.loading = true;
                  }
          );
          that.questionmodal.loading = false;
           return;
        }
          if (!this.questionmodal.Question1.QuestionTitle) {    
            that.$Message.warning("第一题题目内容不能为空");
             that.$nextTick(
            () => {
                    that.questionmodal.loading = true;
                  }
          );
          that.questionmodal.loading = false;
          return;
        }       
        console.log(!isNaN(this.questionmodal.Question1.YesOptionAUseIntegral));
        console.log(this.questionmodal.Question1.YesOptionAUseIntegral <= 0);
        if ((isNaN(this.questionmodal.Question1.YesOptionAUseIntegral) || this.questionmodal.Question1.YesOptionAUseIntegral <= 0) || ((isNaN(this.questionmodal.Question1.YesOptionAWinCouponCount) || this.questionmodal.Question1.YesOptionAWinCouponCount <= 0))) {       
             that.$Message.warning("第一题题目是的选项A的积分值和兑换券必须为正整数");
                  that.$nextTick(
            () => {
                    that.questionmodal.loading = true;
                  }
          );
          that.questionmodal.loading = false;
           return;
        }
         if ((isNaN(this.questionmodal.Question1.YesOptionBUseIntegral) || this.questionmodal.Question1.YesOptionBUseIntegral <= 0) || ((isNaN(this.questionmodal.Question1.YesOptionBWinCouponCount) || this.questionmodal.Question1.YesOptionBWinCouponCount <= 0))) {       
             that.$Message.warning("第一题题目是的选项B的积分值和兑换券必须为正整数");
                  that.$nextTick(
            () => {
                    that.questionmodal.loading = true;
                  }
          );
          that.questionmodal.loading = false;
           return;
        }
         if ((isNaN(this.questionmodal.Question1.YesOptionCUseIntegral) || this.questionmodal.Question1.YesOptionCUseIntegral <= 0) || ((isNaN(this.questionmodal.Question1.YesOptionCWinCouponCount) || this.questionmodal.Question1.YesOptionCWinCouponCount <= 0))) {       
             that.$Message.warning("第一题题目是的选项C的积分值和兑换券必须为正整数");
                  that.$nextTick(
            () => {
                    that.questionmodal.loading = true;
                  }
          );
          that.questionmodal.loading = false;
           return;
        }
        if ((isNaN(this.questionmodal.Question1.NoOptionAUseIntegral) || this.questionmodal.Question1.NoOptionAUseIntegral <= 0) || ((isNaN(this.questionmodal.Question1.NoOptionAWinCouponCount) || this.questionmodal.Question1.NoOptionAWinCouponCount <= 0))) {       
             that.$Message.warning("第一题题目否的选项A的积分值和兑换券必须为正整数");
                  that.$nextTick(
            () => {
                    that.questionmodal.loading = true;
                  }
          );
          that.questionmodal.loading = false;
           return;
        }
         if ((isNaN(this.questionmodal.Question1.NoOptionBUseIntegral) || this.questionmodal.Question1.NoOptionBUseIntegral <= 0) || ((isNaN(this.questionmodal.Question1.NoOptionBWinCouponCount) || this.questionmodal.Question1.NoOptionBWinCouponCount <= 0))) {       
             that.$Message.warning("第一题题目否的选项B的积分值和兑换券必须为正整数");
                  that.$nextTick(
            () => {
                    that.questionmodal.loading = true;
                  }
          );
          that.questionmodal.loading = false;
           return;
        }
         if ((isNaN(this.questionmodal.Question1.NoOptionCUseIntegral) || this.questionmodal.Question1.NoOptionCUseIntegral <= 0) || ((isNaN(this.questionmodal.Question1.NoOptionCWinCouponCount) || this.questionmodal.Question1.NoOptionCWinCouponCount <= 0))) {       
             that.$Message.warning("第一题题目否的选项C的积分值和兑换券必须为正整数");
                  that.$nextTick(
            () => {
                    that.questionmodal.loading = true;
                  }
          );
          that.questionmodal.loading = false;
           return;
        }
        if (that.questionmodal.Question1.PKID > 0) {
        that.ajax
            .post("/GuessGame/UpdateQuestionWithOptionList", {
                question1: that.questionmodal.Question1,
                question2: that.questionmodal.Question2,
                question3: that.questionmodal.Question3
            })
            .then(response => {
                if (!response.data) {
                    that.$Message.success("操作成功");
                    that.questionmodal.visible = false;
                    that.loadData(that.page.current);
                } else {
                    that.$Message.error(response.data);
                    that.$nextTick(() => {
                        that.questionmodal.loading = true;
                    });
                }
                that.questionmodal.loading = false;
            });
        } else {
             that.ajax
            .post("/GuessGame/SaveQuestionWithOptionList", {
                question1: that.questionmodal.Question1,
                question2: that.questionmodal.Question2,
                question3: that.questionmodal.Question3
            })
            .then(response => {
                if (!response.data) {
                    that.$Message.success("操作成功");
                    that.questionmodal.visible = false;
                    that.loadData(that.page.current);
                } else {
                    that.$Message.error(response.data);
                    that.$nextTick(() => {
                        that.questionmodal.loading = true;
                    });
                }
                that.questionmodal.loading = false;
            });
        }
      },
      FetchProductInfo (PID) {
        var that = this;
        if (PID) {
          that.ajax
          .post("/GroupBuyingV2/FetchProductInfo", {
              pid: PID
          })
          .then(response => {
            if (response.data) {
              that.modal.productConfig.PID = response.data.PID;
              that.modal.productConfig.ProductName = response.data.ProductName;
              that.modal.productConfig.ProductPrice = response.data.ProductPrice
            } else {
              that.$Message.warning("商品 PID 无效");
            } 
          });
        }    
      },
      handlePageChange (pageIndex) {
          this.loadData(pageIndex);
      },
      handlePageSizeChange (pageSize) {
          this.page.pageSize = pageSize;
          this.loadData(this.page.current);
      },
      deleteCoupon (index) {
          this.modal.productConfig.Coupons.splice(index, 1);
      },
     
      handleLogPageSizeChange (pageSize) {
          var that = this;
          that.logpage.pageSize = pageSize;
          that.loadData(this.logpage.current);
      },
      formatDate (value) {
        if (value) {
            var time = new Date(
                value
            );
            var year = time.getFullYear();
            var day = time.getDate();
            var month = time.getMonth() + 1;
            var hours = time.getHours();
            var minutes = time.getMinutes();
            var seconds = time.getSeconds();
            var func = function (value, number) {
                var str = value.toString();
                while (str.length < number) {
                    str = "0" + str;
                }
                return str;
            };
            if (year === 1) {
                return "";
            } else {
                return (
                    func(year, 4) +
                    "-" +
                    func(month, 2) +
                    "-" +
                    func(day, 2) +
                    " " +
                    func(hours, 2) +
                    ":" +
                    func(minutes, 2) +
                    ":" +
                    func(seconds, 2)
                );
            }
        } else {        
            return '';
        }
      },
      isLater (str1, str2) {
      return new Date(str1) >= new Date(str2);
     },
      ConvertValue (value) {
          var result = '';
          if (value === 0) {
              result = '未上架';
          } else if (value === 1) {
               result = '上架';
          }
          return result;            
      },
      ValidateGuidid (value) {
         var result = true;
          var pattern = new RegExp('^[0-9a-f]{8}-[0-9a-f]{4}-[1-5][0-9a-f]{3}-[89ab][0-9a-f]{3}-[0-9a-f]{12}$', 'i');
         if (pattern.test(value) === false) {
             result = false;
         }
        return result;             
      },
      ConvertBoolValue (value) {
          var result = '';
          if (value === true) {
              result = '否';
          } else if (value === false) {
               result = '是';
          }
          return result;            
      },
       ConvertDeletedBoolValue (value) {
          var result = '';
          if (value === false) {
              result = '否';
          } else if (value === true) {
               result = '是';
          }
          return result;            
      },
      QueryQuestionWithOption (time) {      
            this.ajax
          .get("/GuessGame/GetQuestionWithOptionList?endTime=" + this.formatDate(time))
          .then(response => {
               if (response.data.data[0]) {
                this.queryquestionmodal.Question1 = response.data.data[0];                                                  
                this.queryquestionmodal.visible = true;
              } else if (response.data.data[1]) {
                this.queryquestionmodal.Question2 = response.data.data[1];
                this.queryquestionmodal.visible = true;
              } else if (response.data.data[2]) {
                this.queryquestionmodal.QuestionAnswer3 = response.data.data[2];
                this.queryquestionmodal.visible = true;
              }
          });
    }
    }
  }
</script>
