<template>
  <div class="header">
    <a href="/pages/pintuan/">
    <div class="headerlogo">
      <img src="static/logo.png" class="header-logo-line">
      <div class="header-logo-title">
        <h3>拼团配置系统</h3>
        <h4>setting.tuhu.cn</h4>
      </div>
    </div>
    </a>
    <div class="header-aside">
      <Dropdown @on-click="logout">
        <a href="javascript:void(0)" class="header-aside-main">
            <Icon type="ios-person" class="main-icon"></Icon>
            {{name}}
            <Icon type="arrow-down-b"></Icon>
        </a>
          <DropdownMenu slot="list">
              <DropdownItem>退出登录</DropdownItem>
          </DropdownMenu>
        </Dropdown>
    </div>
  </div>
</template>

<script>
export default {
  props: {
    name: {
      type: String
    }
  },
  methods: {
    logout: function () {
      this.$emit("logout");
    }
  }
};
</script>

<style lang="less">
@import "./settingheader.less";
</style>
