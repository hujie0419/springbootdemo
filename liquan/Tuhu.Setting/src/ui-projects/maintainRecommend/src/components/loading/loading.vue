<template>
  <div class="ths_loading-wrap">
    <div class="th_loading">
      <svg viewBox="25 25 50 50" class="th_circular">
        <circle cx="50" cy="50" r="20" fill="none" class="th_path"></circle>
      </svg>
      <slot>
        <span class="th_text" v-if="text">{{text}}</span>
      </slot>
    </div>
  </div>
</template>

<script>
export default {
  props: {
    text: {
      default: ''
    }
  },
  data() {
    return {};
  }
};
</script>

<style lang="scss">
.ths_loading-wrap {
    position: absolute;
    z-index: 99;
    margin: 0;
    top: 0;
    right: 0;
    bottom: 0;
    left: 0;
    transition: opacity .3s;
    .th_loading {
        position: absolute;
        text-align: center;
        transform: translate(-50%, -50%);
        top: 50%;
        left: 50%;
    }
    .th_circular {
        height: 42px;
        width: 42px;
        animation: loading-rotate 2s linear infinite;
    }
    .th_path {
        animation: loading-dash 1.5s ease-in-out infinite;
        stroke-dasharray: 90,150;
        stroke-dashoffset: 0;
        stroke-width: 2;
        stroke: currentColor;
        stroke-linecap: round;
    }
    .th_text {
        display: block;
        margin: 3px 0;
        font-size: 14px;
    }
    @keyframes loading-rotate{
        100%{
            transform:rotate(360deg)
        }
    }
    @keyframes loading-dash{
        0%{
            stroke-dasharray:1,200;
            stroke-dashoffset:0;
        }
        50%{
            stroke-dasharray:90,150;
            stroke-dashoffset:-40px;
        }
        100%{
            stroke-dasharray:90,150;
            stroke-dashoffset:-120px;
        }
    }
}
</style>