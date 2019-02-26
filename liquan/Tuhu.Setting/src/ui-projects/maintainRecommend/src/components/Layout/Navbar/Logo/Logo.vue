<template>
  <div class="mrs_logo">
    <span class="mr_logo-icon mr_icon">&#xe608;</span>
    <div class="mr_logo-name-wrap">
        <span class="mr_logo-name mr_icon">途虎养车</span>
        <span class="mr_logo-split"></span>
        <span class="mr_logo-subName">保养推荐系统</span>
    </div>
  </div>
</template>

<script>
export default {

};
</script>

<style lang="scss" scoped>
@import "css/common/_var.scss";
@import "css/common/_mixin.scss";
@import "css/common/_iconFont.scss";

.mrs_logo{
    color: $colorf;
    display: inline-block;
    font-size: 0;
    vertical-align: middle;
    white-space: nowrap;
    .mr_logo-icon{
        font-size: $largeFontSize * 2;
        display: inline-block;
        vertical-align: middle;
    }
    .mr_logo-name-wrap {
        display: inline-block;
        font-size: 0;
        vertical-align: middle;
        margin-left: 12px;
    }
    .mr_logo-name{
        font-size: $largeFontSize * 1.5;
        vertical-align: middle;
    }
    .mr_logo-subName {
        position: relative;
        font-size: $largeFontSize;
        vertical-align: middle;
        margin-left: 13px;
        padding-left: 13px;
        &::after{
            content: '\20';
            display: inline-block;
            position: absolute;
            left: -1px;
            top: 0px;
            bottom: 0px;
            background: $colorf;
            width: 2px;
        }
    }
}
@media screen and (max-width: $maxWidth) {
    .mrs_logo{
        display: flex;
        align-items: center;
        margin-right: -10px;
        .mr_logo-icon{
            font-size: $largeFontSize * 2;
        }
        .mr_logo-name-wrap {
            display: flex;
            flex-direction: column;
        }
        .mr_logo-name{
            font-size: $largeFontSize;
        }
        .mr_logo-subName {
            margin: 0;
            padding: 0;
            font-size: $smallFontSize;
            transform-origin: 0 100%;
            transform: scale(0.83);
            letter-spacing: 0.1em;
            &::after{
                display: none;
            }
        }
    }
}
</style>