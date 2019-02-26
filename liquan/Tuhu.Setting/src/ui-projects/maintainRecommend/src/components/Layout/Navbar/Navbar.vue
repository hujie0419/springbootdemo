<template>
    <div @click="menuClick(false)" class="mrs_navbar">
        <span @click.stop="menuClick(!menuStatus)" class="mr_menu-btn mr_icon">&#xe605;</span>
        <logo></logo>
        <avatar @setActive="menuClick(false)"></avatar>
    </div>
</template>

<script>
import Logo from './Logo/Logo';
import Avatar from './Avatar/Avatar';
export default {
  props: {
    menuStatus: {
      default: false
    }
  },
  components: {
    Logo,
    Avatar
  },
  methods: {
    // 设置菜单显示状态
    menuClick(evt) {
      this.$emit('update:menuStatus', evt);
    }
  }
};
</script>

<style lang="scss" scoped>
@import "css/common/_var.scss";
@import "css/common/_mixin.scss";
@import "css/common/_iconFont.scss";
.mrs_navbar {
  height: 100%;
  width: 100%;
  background: $layoutBg;
  height: $headerHeight;
  display: flex;
  align-items: center;
  justify-content: space-between;
  padding: 0 1rem;
  .mr_menu-btn{
      color: $colorf;
      font-size: 2.2rem;
      display: none;
      cursor: pointer;
  }
}
@media screen and (max-width: $maxWidth) {
    .mrs_navbar {
        .mr_menu-btn{
            display: block;
        }
    }
}
</style>