<template>
  <div>
    <h1>{{msg}}</h1>
    <div>
      <input type="text" v-model="user.username" />
      <input type="text" v-model="user.password" />
      <button type="success" @click="login">登录</button>
    </div>
  </div>
</template>

<script>
export default {
  name: "Login",
  data() {
    return {
      msg: "登录",
      user: {
        username: "",
        password: ""
      }
    };
  },
  methods: {
    login() {
      var that = this;
      this.util.ajax({
          method: "POST",
          url: `/api/Promotion/Activity/Login?username=${that.user.username}&password=${that.user.password}`,
          data: {},
          headers: {
            RequestID: "799385b17308425a9d02e2237fc57501",
            RemoteName: "Tuhu.Service.Promotion.Server",
            RemoteEndpoint: "TH201969521"
          }
        }).then(function(response) {
          console.log(response);
          if (response.data && response.data.Status > 0) {
            this.$Message.info("测试消息");
          }
        });
    }
  }
};
</script>
