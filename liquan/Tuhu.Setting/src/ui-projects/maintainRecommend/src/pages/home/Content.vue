<template>
  <div class="content">
    <div class="contentFixed">
      <div class="top">
        <p class="title">{{urlCategory.partName}}</p>
        <p class="log" @click="checkLog">修改日志</p>
      </div>
      <div class="config-tab">
        <span :class="{active:!tab}" @click="switchConfigTab($event,0)">基础配置</span>
        <span :class="{active:tab}" @click="switchConfigTab($event,1)">特殊车型配置</span>
      </div>
    </div>
    <!-- 基础配置 -->
    <base-config :urlCategory="urlCategory" v-if="!tab"></base-config>
    <!-- 特殊车型配置 -->
    <special-config :urlCategory="urlCategory" v-if="tab"></special-config>
    
  </div>
</template>
<script>
/* eslint-disable max-lines */
import Vue from 'vue';
import BaseConfig from './class/BaseConfig';
import SpecialConfig from './class/SpecialConfig';
import MrAlert from '../../components/MrAlert';
export default {
  data() {
    return {
      urlCategory: {
        partName: '', // 当前分类text
        category: '' // 当前分类
      },
      tab: this.$route.query.tab + '' === '1'
    };
  },
  created() {
    const category = this.$route.query.category;
    const params = category && JSON.parse(category);
    this.$set(this.urlCategory, 'category', params && params.Category);
    this.$set(this.urlCategory, 'partName', params && params.PartName);
  },
  watch: {
  },
  methods: {
    /**
     * 查看日志
     */
    checkLog() {
      window.open('https://parts.tuhu.cn/Log/baoyangoprlog');
    },
    /**
     * 标签页切换效果
     * @param {Event} e 事件对象
     * @param {Boolean} flag 事件对象
     */
    switchConfigTab(e, flag) {
      const element = [].slice.call(e.target.parentNode.children);
      element.forEach(item => {
        if (item === e.target) {
          item.classList.add('active');
        } else {
          item.classList.remove('active');
        }
      });
      this.tab = flag;
      this.$route.query.tab = flag;
      sessionStorage.setItem('tab', flag);
    }
  },
  components: {
    'base-config': BaseConfig,
    'special-config': SpecialConfig
  }
};
</script>
<style lang="scss" scoped src="./Content.scss">
</style>
<style lang="scss">
@import "css/common/_var.scss";
@media screen and (max-width: $phoneWidth) {
.mr_main-container .app-main{
  padding: 0 !important;
}
}
</style>

