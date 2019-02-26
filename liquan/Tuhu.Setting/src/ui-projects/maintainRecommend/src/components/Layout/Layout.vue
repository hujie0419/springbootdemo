<template>
    <div class="mrs_app-wrapper">
        <div class="mr_herder-wrapper">
            <nav-bar :menuStatus.sync="menuStatus" v-on:clickAction="changeSider($event)"></nav-bar>
        </div>
        <div class="mr_cont-wrap" v-if="!$route.meta.noMenu">
            <div class="mr_left-menu-wrapper"
            @touchstart="menuStatus = false" 
            @click="menuStatus = false"
            :class="[menuShow && 'mr_menu-active', menuTransition && 'mr_menu-transition']">
                <left-menu class="t_toRight" @clickItem="menuStatus = false"></left-menu>
                <!-- <side-bar></side-bar> -->
            </div>
            <div class="mr_main-container">
                <app-main></app-main>
            </div>
        </div>
        <div class="mr_main-container" v-else :class="{noMenu:$route.meta.noMenu}">
            <app-main></app-main>
        </div>
    </div>
</template>

<script>
// import router from 'router';
export default {
  name: 'layout',
  data() {
    return {
      menuStatus: false,
      menuTransition: false,
      menuShow: false,
      timeout: null
    };
  },
  watch: {
    menuStatus(nowval) {
      this.timeout !== null && clearTimeout(this.timeout);
      if (nowval) {
        this.menuShow = nowval;
      } else {
        this.timeout = setTimeout(() => {
          this.menuShow = nowval;
        }, 200);
      }
      setTimeout(() => {
        this.menuTransition = nowval;
      }, 10);
    }
  },
  mounted() {
    this.menuTransition = this.menuStatus;
    this.menuShow = this.menuStatus;
  },
  components: {
  },
  computed: {
  },
  methods: {
  }
};
</script>
<style rel="stylesheet/scss" lang="scss" scoped>
    @import "css/common/_var.scss";
    @import "css/common/_mixin.scss";
    @import "css/common/_iconFont.scss";

    $leftWidth: $navLeftWidth + $navLeftChildWidth;
    .mrs_app-wrapper {
        @include clearFloat;
        position: relative;
        height: 100%;
        width: 100%;
        .mr_herder-wrapper {
            height: $headerHeight;
            position: fixed;
            top: 0;
            width: 100%;
            // overflow: hidden;
            z-index: 999;
        }
        .mr_cont-wrap {
            // width: 100%;
            margin-left: $leftWidth;
            transition: all .28s ease-out;
            padding-top: $headerHeight;
        }
        .mr_left-menu-wrapper {
            position: fixed;
            top: $headerHeight;
            bottom: 0;
            left: 0;
            z-index: 9;
            overflow: hidden;
            width: $leftWidth;
        }
        .mr_main-container {
            min-height: 100%;
            &.noMenu{
                padding-top: $headerHeight;
            }
        }
    }
    @media screen and (max-width: $maxWidth) {
        $time: .2s;// 动画时间
        .mrs_app-wrapper {
            .mr_cont-wrap {
                margin-left: 0;
            }
            .mr_left-menu-wrapper{
                display: none;
                .t_toRight {
                    transition: all $time ease-out;
                    transform: translateX(-100%);
                }
                &.mr_menu-active{
                    display: block;
                    width: 100%;
                    padding-right: 70px;
                    background-color: rgba(0,0,0,0);
                    transition: background-color $time ease-out;
                }
                &.mr_menu-transition {
                    background-color: rgba(0,0,0,0.5);
                    .t_toRight {
                        transform: translateX(0);
                    }
                }
            }
        }
    }
</style>
