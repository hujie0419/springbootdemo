<!-- 所有推荐信息，包含n个selected组件，和一个添加按钮 -->
<template>
  <div>
      <!-- <selected :brandData.sync="brandData" :category.sync="category" @selectOption="selectOption" @deleteRecommend="$emit('changeRecommend', 'delete')"></selected> -->
      <div
        v-for="(item, index) in maintainListTemp"
        :is="item.select"
        :key="item.id"
        :defaultValue="item"
        :brandData.sync="brandData"
        :category.sync="category"
        :index = index
        @selectOption="selectOption"
        @deleteRecommend="deleteRecommend"></div>
      <div class="add-recommend" @click="addRecommend">+ 添加优先级</div>
  </div>
</template>
<script>
import Vue from 'vue';
import Selected from './Selected';
export default {
  data() {
    return {
    //   items: [],
      notesContent: '',
      partName: '机油',
      priorityType: '',
      recommendData: [],
      maintainListTemp: [{select: Selected}]
    };
  },
  props: {
    maintainList: {
      type: Array,
      default: () => {
        return [];
      }
    },
    brandData: {
      type: Array,
      required: true
    },
    category: {
      type: String,
      required: true
    },
    isOil: {
      type: Boolean
    }
  },
  watch: {
    maintainList(newVal) {
      if (newVal && newVal.length > 0) {
        this.maintainListTemp = [];
        setTimeout(() => {
          newVal.forEach(element => {
            element.select = Selected;
            this.maintainListTemp.push(element);
          });
        //   this.maintainListTemp = newVal;
        }, 10);
      } else {
        this.maintainListTemp = [{select: Selected}];
      }
    }
  },
  methods: {
    addRecommend() {
      this.maintainListTemp.push({select: Selected});
      this.$emit('changeRecommend', 'add');
    },
    selectOption(element, selectData) {
      this.$emit('selectOption', element, selectData);
    },
    deleteRecommend(type, rowIndex) {
      //   this.maintainListTemp.splice(rowIndex, 1);
      this.$emit('changeRecommend', type);
    }
  },
  components: {
    'selected': Selected
  }
};
</script>
<style lang="scss" scoped>
@import "css/common/_var.scss";
@import "css/common/_mixin.scss";
@import "css/common/_iconFont.scss";
@keyframes showRecommend {
  0%{transform: translateY(-80px)}
  100%{transform: translateY(0)}
}
@keyframes showRecommendShort {
  0%{transform: translateY(-40px)}
  100%{transform: translateY(0)}
}
.add-recommend{
  color: $color9;
  font-size: 13px;
  letter-spacing: 1px;
  width: 260px;
  height: 40px;
  line-height: 40px;
  text-align: center;
  border: 1px dashed $btnBrColor;
  margin: 0 107px;
  cursor: pointer;
  animation: showRecommendShort .3s;
}
@media screen and (max-width: $phoneWidth) {
  .add-recommend{
    margin: 0 15px;
    border: none;
    color: $stressColor;
    width: auto;
    text-align: left;
    animation: showRecommend .3s;
  }
}
</style>
