<template>
  <div class="mrs_avatar" @touchstart.stop="stop" @mousedown.stop="stop" @click.stop="setActive" :class="[isActive && 'mr_active',showActive && 'mr_showActive']">
    <span class="mr_avatar-text">{{userInfo.name}}</span>
    <span class="mr_avatar-icon"></span>
    <div class="mr_avatar-info">
      <ul class="mr_info-cont">
        <li class="mr_info-item" @click="loginout()"><span class="mr_item-user-name">{{userInfo.name && userInfo.name + '，' || ''}}</span><span class="mr_item-text">退出登录</span></li>
      </ul>
    </div>
  </div>
</template>

<script>
import { mapGetters } from 'vuex';
export default {
  data() {
    return {
      isActive: false,
      showActive: false
    };
  },
  computed: {
    ...mapGetters({
      userInfo: 'userInfo'
    })
  },
  watch: {
    isActive(nowval) {
      if (nowval) {
        setTimeout(() => {
          this.showActive = nowval;
        }, 20);
      }
    }
  },
  mounted() {
    window.addEventListener('resize', () => {
      this.isActive && this.setActive();
    });
    document.addEventListener('touchstart', () => {
      this.isActive && this.setActive();
    });
    document.addEventListener('mousedown', () => {
      this.isActive && this.setActive();
    });
  },
  methods: {
    stop() {},
    loginout() {
      this.$loginOut();
    },
    setActive() {
      this.$emit('setActive', !this.isActive);
      if (this.isActive) {
        this.showActive = !this.isActive;
        setTimeout(() => {
          this.isActive = false;
        }, 300);
      } else {
        this.isActive = !this.isActive;
      }
    }
  }
};
</script>

<style lang="scss" scoped>
@import "css/common/_var.scss";
@import "css/common/_mixin.scss";
@import "css/common/_iconFont.scss";

.mrs_avatar {
  color: $colorf;
  font-size: 0;
  font-weight: bold;
  white-space: nowrap;
  cursor: pointer;
  &.mr_active {
    .mr_avatar-info {
      display: block;
      transition: opacity 0.3s ease-out, transform 0.2s ease-out;
      transform: translateY(-10px) scale(0.1);
      transform-origin: 99% 10%;
      opacity: 0;
    }
    .mr_avatar-icon {
      &::after {
        transform: rotateZ(180deg);
      }
    }
  }
  &.mr_showActive {
    .mr_avatar-info {
      opacity: 1;
      transform: translateY(0) scale(1);
    }
  }
  .mr_avatar-text,
  .mr_avatar-icon {
    font-size: $defaultFontSize;
  }
  .mr_avatar-icon {
    padding-left: 0.4rem;
    &::after {
      content: "\e603";
      display: inline-block;
      transition: transform 0.2s;
      @extend %mr_icon;
    }
  }
  .mr_avatar-info {
    position: absolute;
    top: 100%;
    right: 15px;
    padding-top: 8px;
    display: none;
  }
  .mr_info-cont {
    // width: 100%;
    border-radius: 4px;
    min-width: 100px;
    padding: 8px 15px;
    font-size: 0;
    background: rgba(0, 0, 0, 0.8);
    position: relative;
    text-align: center;
    &::after {
      content: "\20";
      position: absolute;
      height: 0;
      width: 0;
      top: -5px;
      right: 10px;
      border: 5px solid transparent;
      border-bottom-color: rgba(0, 0, 0, 0.8);
      border-top: none;
    }
  }
  .mr_info-item {
    cursor: pointer;
  }
  .mr_item-user-name,
  .mr_item-text {
    display: inline-block;
    font-size: $defaultFontSize;
  }
  .mr_item-user-name {
    display: none;
  }
}
@media screen and (max-width: $maxWidth) {
  .mrs_avatar {
    &.mr_active {
      .mr_avatar-icon {
        &::after {
          transform: rotateZ(0deg);
        }
      }
    }
    .mr_avatar-text {
      display: none;
    }
    .mr_avatar-icon {
      &::after {
        content: "\e606";
        padding-left: 0;
        font-size: 2.2rem;
      }
    }
    .mr_item-user-name {
      display: inline-block;
    }
  }
}
</style>