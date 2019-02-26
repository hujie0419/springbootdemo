
<template>
  <div>
    <div>
      <span style="margin-left:10px">时间范围&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;</span> 
      <DatePicker editable=false  v-model.trim="startTime"  type="date" placeholder="开始时间" style="width: 200px"></DatePicker>
      <span>&nbsp; ——&nbsp; </span> 
      <DatePicker editable=false v-model.trim="endTime"  type="date"  placeholder="结束时间" style="width: 200px"></DatePicker>
      <Button type="success"  @click="searchStarRatingStoreList" style="margin-left:10px">查询</Button>
      <Button type="success"  @click="createExcel" style="margin-left:10px">下载所选时间段表单信息</Button>
    </div>
    <div style="margin-top:30px;">
      <Table stripe  :loading="tableModal.loading" :columns="tableModal.columns" :data="tableModal.list" size="small" no-data-text="暂无数据"></Table>
      <Page :total="dataCount" size="small" :page-size="pageSize" :current="pageIndex" show-total show-sizer   @on-change="changepageIndex" @on-page-size-change="changePageSize" ></Page>
    </div>
    <Modal v-model="starRatigStoreModel.visible" title="查看详情" :transfer="false" width="40%">
      <div slot="footer" style="text-align:center" >
        <Button type="success" size="large" @click="starRatigStoreModelCancel('starRatigStoreModel.starModule')">关闭</Button>
      </div>
      <Form ref="starRatigStoreModel.starModule" :model="starRatigStoreModel.starModule"  :label-width="80">
        <FormItem label="姓名" prop="UserName"  label-width="200">：
          {{starRatigStoreModel.starModule.UserName}}
        </FormItem>
        <FormItem label="手机号" prop="Phone" label-width="200">：
          {{starRatigStoreModel.starModule.Phone}}
        </FormItem>
        <FormItem label="门店名称" prop="StoreName" label-width="200">：
          {{starRatigStoreModel.starModule.StoreName}}
        </FormItem>
        <FormItem label="您的职务" prop="Duty" label-width="200">：
          {{starRatigStoreModel.starModule.Duty}}
        </FormItem>
        <FormItem label="所在城市" prop="Area" label-width="200">：
          {{starRatigStoreModel.starModule.ProvinceName}}|{{starRatigStoreModel.starModule.CityName}}|{{starRatigStoreModel.starModule.DistrictName}} {{starRatigStoreModel.starModule.StoreAddress}}
        </FormItem>
        <FormItem label="门店面积（m2）" prop="StoreArea" label-width="200">：
          {{starRatigStoreModel.starModule.StoreArea}}
        </FormItem>
        <FormItem label="工位数（个）" prop="WorkPositionNum" label-width="200">：
          {{starRatigStoreModel.starModule.WorkPositionNum}}
        </FormItem>
        <FormItem label="维修资质" prop="MaintainQualification" label-width="200">：
          {{starRatigStoreModel.starModule.MaintainQualification}}
        </FormItem>
        <FormItem label="门店位置" prop="StoreLocation" label-width="200">：
          {{starRatigStoreModel.starModule.StoreLocation}}
        </FormItem>
        <FormItem label="是否同意更换途虎店招" prop="IsAgreeString" label-width="200">：
          {{starRatigStoreModel.starModule.IsAgreeString}}
        </FormItem>
      </Form>
    </Modal>
  </div>
</template>
<script>
export default {
  name: "Activity",
  data () {
    return {
      dataCount: 0,
      pageSize: 10,  
      pageIndex: 1, 
      tableModal: {
        list: [],
        loading: true,
        columns: [
          {
            title: "序号",
             width: 60,
             align: "center",
             fixed: "left",
             type: "index",
             key: "PKID"
          },
          {
            title: "门店名称",
            key: "StoreName",
            width: 250,
            align: "center"
          },
          {
            title: "用户姓名",
            key: "UserName",
            width: 150,
            align: "center"
          },
          {
            title: "职务",
            key: "Duty",
            width: 200,
            align: "center"
          },
          {
            title: "手机号码",
            key: "Phone",
            width: 200,
            align: "center"
          },
          {
            title: "门店地址",
            key: "Area",
            align: "left"
          },
          {
            title: "操作",
            key: "action",
            width: 100,
            align: "center",
            render: (h, params) => {
              let buttons = [];
              buttons.push(
                h(
                  "Button",
                  {
                    props: {
                      type: "success",
                      size: "small"
                    },
                    style: {
                      marginLeft: "10px"
                    },
                    on: {
                      click: () => {
                        this.getstarModuleInfo(params.row.PKID);
                      }
                    }
                  },
                  "查看详情"
                )
              );
              return h("div", buttons);
            }
          }
        ]
      },
      starRatigStoreModel: { 
        visible: false,
        loading: false,
        edit: false,
        starModule: {}
      },
      startTime: " ",
      endTime: " ",
      start: "",
      end: ""
     };
  },
  created () {
    this.init();
  },
  methods: {
    FormatToDate (timestamp) {
            var time = new Date(timestamp);
            var year = time.getFullYear();
            var month = time.getMonth() + 1 < 10 ? "0" + (time.getMonth() + 1) : time.getMonth() + 1;
            var date = time.getDate() < 10 ? "0" + time.getDate() : time.getDate();
            var hour = time.getHours() < 10 ? "0" + time.getHours() : time.getHours();
            var minute = time.getMinutes() < 10 ? "0" + time.getMinutes() : time.getMinutes();
            var second = time.getSeconds() < 10 ? "0" + time.getSeconds() : time.getSeconds();
            var YmdHis = year + '-' + month + '-' + date + ' ' + hour + ':' + minute + ':' + second;
            return YmdHis;
        },
    init () {
      this.tableModal.loading = false;
      this.ajax
        .get(
          `/StarRatingStore/GetStarRatingStoreList?startTime=${this.start}&endTime=${this.end}&pageIndex=${this.pageIndex}&pageSize=${this.pageSize}`)
        .then(response => {    
          this.tableModal.list = response.data.data; 
          this.dataCount = response.data.count; 
          this.pageSize = response.data.pageSize; 
          this.pageIndex = response.data.pageIndex; 
        });
    },
    searchStarRatingByPageSize () {
      this.tableModal.loading = false;
      this.ajax
        .get(
          `/StarRatingStore/GetStarRatingStoreList?startTime=${this.start}&endTime=${this.end}&pageSize=${this.pageSize}`)
        .then(response => {    
          this.tableModal.list = response.data.data; 
          this.dataCount = response.data.count; 
          this.pageSize = response.data.pageSize; 
          this.pageIndex = response.data.pageIndex; 
        });
    },
    changePageSize (value) {
      this.pageSize = value;
      this.searchStarRatingByPageSize();
    },
    changepageIndex (value) {
      this.pageIndex = value;
      this.init();
    },
    info () {
      if (this.startTime === "" || this.endTime === "") {
         this.$Message.info('请选择时间');
         return false;
      } else {
        this.start = this.FormatToDate(this.startTime);
        this.end = this.FormatToDate(this.endTime);
        if (this.end < this.start) {
          this.$Message.info('结束时间必须大于等于开始时间');
          return false;
        }
      }
    },
    searchStarRatingStoreList () {
      var isTure = this.info();
      if (isTure === false) {
        return false;
      }
      this.tableModal.loading = false;
      this.start = this.FormatToDate(this.startTime);
      this.end = this.FormatToDate(this.endTime);
       this.ajax
        .get(
          `/StarRatingStore/GetStarRatingStoreList?startTime=${this.start}&endTime=${this.end}`
        )
        .then(response => {
          this.tableModal.list = response.data.data; 
          this.dataCount = response.data.count;
          this.pageSize = response.data.pageSize; 
          this.pageIndex = response.data.pageIndex;
        });
    },
    createExcel () {
      if (this.startTime === "" || this.endTime === "") {
        this.start = "";
        this.end = "";
      } else {
        this.start = this.FormatToDate(this.startTime);
        this.end = this.FormatToDate(this.endTime);
        if (this.end < this.start) {
          this.$Message.info('结束时间必须大于等于开始时间');
          return false;
        }
      }
      this.$Modal.confirm({
                title: "温馨提示",
                content: "是否导出数据?",
                loading: true,
                onOk: () => {
                    window.location.href = "/StarRatingStore/ExportExcel?startTime=" + this.start + "&endTime=" + this.end;
                    this.$Modal.remove();
                }
            });
    },
    getstarModuleInfo (PKID) {
      this.$refs["starRatigStoreModel.starModule"].resetFields();
      this.ajax.get(`/StarRatingStore/GetStarRatingStoreModel?PKID=${PKID}`).then(response => {
        if (response.data) {
          this.starRatigStoreModel.loading = false;
          this.starRatigStoreModel.starModule = response.data.data;
          if (response.data.data.IsAgree === true) {
          this.starRatigStoreModel.starModule.IsAgreeString = "是"
          } else {
            this.starRatigStoreModel.starModule.IsAgreeString = "否"
          }
          this.starRatigStoreModel.visible = true;
        }
      });
    },
    starRatigStoreModelCancel (name) {
      this.$refs[name].resetFields();
      this.starRatigStoreModel.visible = false;
    }
  }
};
</script>
