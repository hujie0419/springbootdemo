<template>
  <div>
    <h1>{{msg}}</h1>
    <div>
      <Table border :content="self" :columns="activityHeaders" :data="activityTbList"></Table>
    </div>
    <div>
      <Modal v-model="modal.visible" title="活动报名">
        <Form :rules="modal.rules" ref="modal" :model="modal" :label-width="200">
          <FormItem label="用户" prop="chooseUserID">
            <i-select v-model="modal.chooseUserID" style="width:150px">
              <i-option
                v-for="item in userList"
                :key="item.PKID"
                :value="item.PKID"
              >{{ item.UserName }}</i-option>
            </i-select>
          </FormItem>
          <FormItem label="活动名：">{{modal.activityName}}</FormItem>
          <FormItem label="活动描述：">{{modal.activityDesc}}</FormItem>
          <FormItem label="活动开始时间：">{{modal.activityStartTimeStr}}</FormItem>
          <FormItem label="活动结束时间：">{{modal.activityEndTimeStr}}</FormItem>
          <FormItem label="申请备注：" prop="remark">
            <Input v-model="modal.remark" placeholder="备注" style="width:150px" />
          </FormItem>
        </Form>
        <div slot="footer">
          <Button @click="cancelModal">取消</Button>
          <Button type="primary" @click="addActivityApply('modal')">报名</Button>
        </div>
      </Modal>
    </div>
  </div>
</template>

<script>
export default {
  name: "Activity",
  data() {
    return {
      msg: "活动列表",
      modal: {
        visible: false,
        chooseUserID: "",
        activityName: "",
        activityDesc: "",
        activityStartTimeStr: "",
        activityEndTimeStr: "",
        remark: "",
        rules: {
          chooseUserID: [
            {
              required: true,
              validator: (rule, value, callback) => {
                console.log(value);
                if (!value) {
                  callback(new Error("请选择用户"));
                } else {
                  callback();
                }
              }
            }
          ],
          remark: [
            { required: true, message: "请填写备注", trigger: "blur" }
          ]
        }
      },
      addActivityId: 0,
      self: this,
      activityHeaders: [
        { title: "活动名称", key: "Name" },
        { title: "描述", key: "Description" },
        { title: "开始时间", key: "StartTimeStr" },
        { title: "结束时间", key: "EndTimeStr" },
        { title: "状态", key: "StatusName" },
        {
          title: "操作",
          key: "action",
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
                      this.show(params.index);
                    }
                  }
                },
                "报名"
              )
            ]);
          }
        }
      ],
      activityTbList: [],

      userList: []
    };
  },
  created() {
    this.getAllActivityList();
    this.getUserList();
  },
  methods: {
    getUserList() {
      var that = this;
      let paraData = {
        PageSize: 100,
        CurrentPage: 1,
        IsOnlyUser: true
      };
      this.util
        .ajax({
          method: "POST",
          contentType: "application/json",
          url: `/api/Promotion/Activity/GetUserList`,
          data: paraData,
          headers: {
            RequestID: "799385b17308425a9d02e2237fc57501",
            RemoteName: "Tuhu.Service.Promotion.Server",
            RemoteEndpoint: "TH201969521"
          }
        })
        .then(function(response) {
          console.log(response);
          if (response.data && response.data.Success) {
            that.userList = response.data.Result.Source;
          }
        });
    },
    getAllActivityList() {
      var that = this;
      let paraData = {
        PageSize: 10,
        CurrentPage: 1
      };

      this.util
        .ajax({
          method: "POST",
          contentType: "application/json",
          url: `/api/Promotion/Activity/GetActivityList`,
          data: paraData,
          headers: {
            RequestID: "799385b17308425a9d02e2237fc57501",
            RemoteName: "Tuhu.Service.Promotion.Server",
            RemoteEndpoint: "TH201969521"
          }
        })
        .then(function(response) {
          if (response.data && response.data.Success) {
            that.activityTbList = response.data.Result.Source;
          }
        });
    },
    show(index) {
      this.modal.visible = true;
      this.modal.chooseUserID = 0;
      this.modal.remark="";
      this.modal.activityName = this.activityTbList[index].Name;
      this.modal.activityDesc = this.activityTbList[index].Description;
      this.modal.activityStartTimeStr = this.activityTbList[index].StartTimeStr;
      this.modal.activityEndTimeStr = this.activityTbList[index].EndTimeStr;
      this.addActivityId = this.activityTbList[index].PKID;
    },
    cancelModal() {
      this.modal.visible = false;
    },
    addActivityApply(name) {
      this.$refs[name].validate(valid => {
        if (valid) {
          var that = this;
          if (!that.modal.chooseUserID) {
            that.$Message.info("请选择报名用户");
            return;
          }
          let paraData = {
            UserId: that.modal.chooseUserID,
            ActivityId: that.addActivityId,
            Remark: that.modal.remark,
            CreateUser: "testman"
          };
          this.util
            .ajax({
              method: "POST",
              contentType: "application/json",
              url: `/api/Promotion/Activity/CreateUserActivityApply`,
              data: paraData,
              headers: {
                RequestID: "799385b17308425a9d02e2237fc57501",
                RemoteName: "Tuhu.Service.Promotion.Server",
                RemoteEndpoint: "TH201969521"
              }
            })
            .then(function(response) {
              if (response.data && response.data.Success) {
                that.$Message.info("报名成功");
                that.modal.visible = false;
              } else {
                that.$Message.error(response.data.ErrorMessage);
              }
            });
        }
      });
    }
  }
};
</script>
