<template>
  <Select @on-change="onchange" v-model="clientId" clearable style="width:200px" :disabled="disabled">
    <Option v-for="item in clients" :value="item.clientId" :key="item.clientName">{{ item.clientName }}</Option>
  </Select>
</template>
<script>
export default {
  props: {
    client: {
      type: Number | String
    },
    disabled: {
      type: Boolean,
      default: false
    }
  },
  data () {
    return {
      clients: [],
      clientId: 0,
      clientName: ""
    };
  },
  watch: {
    client: function (newVal) {
      this.clientId = newVal
    }
  },
  mounted: function () {
    this.clientId = this.client
    this.getclients();
  },

  methods: {
    getclients () {
      var _this = this;
      this.ajax.get("/VipCard/GetAllClients").then(response => {
        if (response.data != null && response.data.length > 0) {
          response.data.forEach(function (i, e) {
            _this.clients.push({
              clientId: i.ClientId,
              clientName: i.ClientName
            });
          });
        } else {
          this.$Notice.warning({
            title: "获取客户信息异常！",
            desc: "获取客户信息异常！"
          });
        }
      });
    },
    onchange: function (value) {
      this.$emit('on-change', value);
    }
  }
};
</script>
