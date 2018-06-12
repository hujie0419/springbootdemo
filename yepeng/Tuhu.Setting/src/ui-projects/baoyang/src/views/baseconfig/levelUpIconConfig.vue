<template>
  <div>
    <h2 class="title">升级购图标配置</h2>
    <Row>
      <i-col span="4">
        <img v-bind:src="'//img1.tuhu.org'+imgUrl" style="width:100px;height:100px;" />
        <Upload ref="upload" style="display:inline; width:150px;" action="/Utils/UploadImage" accept='image/*' :max-size="200" :data="{'key':'1', 'path': '/by/logo/levelUp/'}"
          :on-success="handleSuccess" :on-format-error="handleFormatError" :on-exceeded-size="handleMaxSize">
          <Button type="ghost" icon="ios-cloud-upload-outline">上传升级购图片</Button>
        </Upload>
      </i-col>
    </Row>
    <Row>
      <i-col span="2">
        <Button type="primary" @click="UpsertBaoYangLevelUpIcon" :loading="uploadImgStatus">
          {{ uploadImgStatus ? '上传中' : '保存并刷新' }}</Button>
      </i-col>
    </Row>
  </div>
</template>

<script>
  export default {
    data () {
      return {
        imgUrl: "",
        uploadImgStatus: false
      };
    },
    created () {
      this.GetIcon();
    },
    methods: {
      // 获取图标
      GetIcon () {
        var vm = this;
        vm.util.ajax.post('/BaoYang/GetBaoYangLevelUpIconConfig').then(function (result) {
          result = result.data;
          if (result.Status) {
            vm.imgUrl = result.Data;
          }
        });
      },
      handleSuccess (rsp) {
        this.imgUrl = rsp.Url;
        this.$refs.upload.fileList.splice(0, this.$refs.upload.fileList.length);
      },
      handleFormatError (file) {
        this.util.message.warning("图片格式错误, 请上传图片格式");
      },
      handleMaxSize (file) {
        this.util.message.warning("图片大小不能超过50kb");
      },
      UpsertBaoYangLevelUpIcon: function () {
        var vm = this;
        if (!confirm("确认保存?")) {
          return;
        }
        vm.uploadImgStatus = true;
        vm.util.ajax.post("/BaoYang/UpsertBaoYangLevelUpIcon", {
            icon: vm.imgUrl
          })
          .then(function (rsp) {
            rsp = (rsp || []).data;
            vm.uploadImgStatus = false;
            if (rsp.Status) {
              vm.util.message.success("更新成功!" + (rsp.Msg || ""));
              setTimeout(() => {
                vm.RemoveCache();
              }, 2000);
            } else {
              vm.util.message.error({
                content: "更新图片失败!" + (rsp.Msg || ""),
                duration: 10,
                closable: true
              });
            }
          })
          .catch(function (rsp) {
            vm.uploadImgStatus = false;
            rsp = (rsp || []).data;
            vm.util.message.error({
              content: "更新图片失败!" + (rsp.Msg || ""),
              duration: 10,
              closable: true
            });
          });
      },
      RemoveCache () {
        var vm = this;
        vm.util.ajax.post("/BaoYang/RemoveBaoYangLevelUpIconCache")
          .then(function (result) {
            result = (result || []).data;
            if (result && !result.Status) {
              vm.util.message.error({
                content: "刷新缓存失败" + (result.Msg || ""),
                duration: 0,
                closable: true
              })
            }
          });
      }
    }
  };

</script>
