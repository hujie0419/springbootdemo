<template>
<!-- <div class="alert-box"> -->
  <div class="show-hint">{{text}}</div>
<!-- </div> -->
</template>
<script>
export default {
  props: {
    text: {
      type: String
    }
  },
  data() {
    return {};
  }
};
</script>
<style lang="scss" scoped>
@import "css/common/_var.scss";
@import "css/common/_mixin.scss";
@import "css/common/_iconFont.scss";
@keyframes showAlert {
  0%{transform: translateY(80px)}
  100%{transform: translateY(0)}
}
// .alert-box{
  // display: flex;
  // position: absolute;
  // top: 0;
  // left: 0;
  // background: transparent;
  // width: 100%;
  // height: 100%;
  // align-items: center;
  // justify-content: center;
  .show-hint{
    display: block;
    width: 30%;
    min-width: 300px;
    line-height: 2em;
    background: #fff;
    text-align: center;
    margin: 0 auto;
    padding: 20px 0;
    border-radius: 10px;
    font-size: 14px;
    letter-spacing: 1px;
    // animation: showAlert .3s;
  }
// }
</style>
