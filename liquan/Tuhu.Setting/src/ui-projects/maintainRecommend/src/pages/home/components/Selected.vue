<!-- 一条推荐信息，包含一个品牌和一个系列 -->
<template>
  <div class="recommend">
    <span class="title">品牌优先级</span>
    <div class="select">
      <input type="text" class="brandInput" autocomplete="off" placeholder="请选择品牌优先级" readonly="readonly" @click.stop="toggleOption" ref="brandInput">
      <div class="option-box">
        <div class="option-title"></div>
        <ul class="option" ref="brandUl">
          <li class="option-item" v-for="brand in brandData" :key="brand.id" @click="selectBrandItem">{{brand}}</li>
        </ul>
      </div>
      <span class="arrow" @click.stop="arrowClick"></span>
    </div>
    <div class="select">
      <input type="text" class="seriesInput" autocomplete="off" placeholder="请选择指定系列" readonly="readonly" @click.stop="toggleOption" ref="seriesInput">
      <div class="option-box">
        <ul class="option" ref="seriesUl">
          <li class="option-item" v-for="series in seriesData" :key="series.id" @click="selectSeriesItem">{{series}}</li>
        </ul>
      </div>
      <span class="arrow" @click.stop="arrowClick"></span>
    </div>
    <span class="delete" @click="deleteRecommend($event, index)"></span>
  </div>
</template>
<script>
export default {
  data() {
    return {
      brandName: '',
      inputBrandName: '',
      seriesName: '',
      seriesData: [],
      getSeries: null
    };
  },
  props: {
    brandData: {
      type: Array,
      required: true
    },
    category: {
      type: String,
      required: true
    },
    defaultValue: {
      type: Object,
      default: () => {
        return {};
      }
    },
    index: {
      type: Number,
      required: true
    }
  },
  mounted() {
    if (this.defaultValue && this.defaultValue.Brand) {
      this.$refs.brandInput.value = this.defaultValue.Brand;
      this.$refs.seriesInput.value = this.defaultValue.Series;
    }
  },
  destroyed() {
    // 组件销毁时，停止请求
    this.getSeries && this.getSeries.unsubscribe();
  },
  methods: {
    /**
     * input（选择品牌/系列）click事件处理程序
     * @param {Event} e 事件对象
     */
    arrowClick(e) {
      this.toggleOption({target: e.target.parentNode.children[0]});
    },
    /**
     * input（选择品牌/系列）click事件处理程序
     * @param {Event} e 事件对象
     * @returns {*}
     */
    toggleOption(e) {
      // 移动端
      if (document.body.clientWidth <= 600) {
        let pickerData = [];
        let forEachData = [];
        let pickerTitle = '';
        if (e.target.classList.contains('brandInput')) {
          forEachData = this.brandData;
          pickerTitle = '选择品牌';
        } else {
          forEachData = this.seriesData;
          pickerTitle = '选择系列';
        }
        forEachData.forEach((brandItem, index) => {
          pickerData.push({text: brandItem, value: index});
        });
        // 没有数据的时候不弹picker
        if (!pickerData.length) {
          return false;
        }
        this.$tPicker.picker({props: {dataLists: [pickerData], nameText: pickerTitle}}).then(data => {
          if (data[0].text === '暂无数据') {
            return false;
          }
          e.target.value = data[0].text;
          if (e.target.classList.contains('brandInput')) {
            // 与上一次的品牌不同，清掉系列，调获取系列的接口
            if (this.brandName !== data[0].text) {
              this.$refs.seriesInput.value = '';
              this.getSeriesItem(data[0].text);
            }
            this.brandName = data[0].text;
          } else {
            this.$emit('selectOption', e.target, {brand: this.$refs.brandInput.value, series: data[0].text});
          }
        });
        // PC端
      } else {
        const element = document.querySelector('.option-box.show');
        const parentEle = e.target.parentNode;
        const option = parentEle.children[1];
        const arrow = parentEle.children[2];
        if (e.target.classList.contains('seriesInput')) {
          this.getSeriesItem(this.$refs.brandInput.value);
        }
        // 关闭其它下拉框
        if (element && element !== option) {
          element.classList.remove('show');
          element.parentNode.children[2].classList.remove('top');
        }
        option.classList.toggle('show');
        arrow.classList.toggle('top');
      }
    },
    /**
     * 点击品牌
     * @param {Event} e 事件对象
     */
    selectBrandItem(e) {
      const element = e.target;
      this.selectItem(element);
      // 与上一次的品牌不同，清掉系列，调获取系列的接口
      if (this.brandName !== element.innerText) {
        this.$refs.seriesInput.value = '';
        this.getSeriesItem(element.innerText);
      }
      this.brandName = element.innerText;
    },
    /**
     * 点击系列
     * @param {Event} e 事件对象
     * @returns {boolean}
     */
    selectSeriesItem(e) {
      const element = e.target;
      if (element.innerText === '暂无数据') {
        return false;
      }
      const inputEle = this.$refs.seriesInput;
      this.selectItem(element);
      // this.seriesName = element.innerText
      this.$emit('selectOption', inputEle, {brand: this.$refs.brandInput.value, series: element.innerText});
    },
    /**
     * 选中的品牌/系列填到相应的input中
     * @param {HTMLElement} element - input元素
     */
    selectItem(element) {
      const parent = element.parentNode.parentNode;
      parent.parentNode.children[0].value = element.innerText;
      parent.classList.toggle('show');
      parent.parentNode.children[2].classList.toggle('top');
    },
    /**
     * 获取系列信息
     * @param {string} brandName brandName
     */
    getSeriesItem(brandName) {
      // 每次请求前，先终止上次请求
      this.getSeries && this.getSeries.unsubscribe();
      this.getSeries = this.$http.get('/Product/GetProductSeries', {
        apiServer: 'apiServer',
        params: {category: this.category, brand: brandName},
        cacheData: false
      }).subscribe((res) => {
        this.seriesData = res.data.length ? res.data : ['暂无数据'];
      });
    },
    /**
     * 删除一条推荐信息
     * @param {Event} e 事件对象
     * @param {Number} rowIndex 列index
     */
    deleteRecommend(e, rowIndex) {
      const parent = e.target.parentNode.parentNode;
      const child = e.target.parentNode;
      parent.removeChild(child);
      this.$emit('deleteRecommend', 'delete', rowIndex);
    }
  }
};
</script>
<style lang="scss" scoped>
@import "css/common/_var.scss";
@import "css/common/_mixin.scss";
@import "css/common/_iconFont.scss";
@mixin placeholder($color: $color9, $size: 13px){
  color: $color;
  font-size: $size;
  letter-spacing: 1px;
}
@keyframes showRecommend {
  0%{transform: translateY(-80px)}
  100%{transform: translateY(0)}
}
.recommend{
  position: relative;
  display: flex;
  font-size: 13px;
  color: $color3;
  letter-spacing: 1px;
  margin: 15px 0 25px 0;
  padding: 0 24px;
  animation: hideIndex .3s;
  .title{
    line-height: 2.5em;
  }
  .select{
    width: 260px;
    margin-left: 17px;
    position: relative;
    input{
      width: 100%;
      height: 40px;
      border: 1px solid $btnBrColor;
      line-height: 2.5em;
      border-radius: 4px;
      padding: 0 20px 0 10px;
      cursor: pointer;
      &::placeholder {
        @include placeholder($color9, 13px)
      }
    }
    @keyframes hideIndex{
      0%{ opacity: 0; transform: translate(0, -40px) }
      100%{opacity: 1; transform: translate(0, 0) }
    }
    .option-box{
      display: none;
      width: 100%;
      max-height: 320px;
      overflow: hidden;
      position: absolute;
      top: 40px;
      left: 0;
      border-right: 1px solid $btnBrColor;
      border-left: 1px solid $btnBrColor;
      border-bottom: 1px solid $btnBrColor;
      border-radius: 4px;
      z-index: 100;
      &.show{
        display: block;
      }
    }
    .option{
      width: 260px;
      background: $colorf;
      line-height: 2.5em;
      list-style: none;
      margin: 0;
      padding: 0;
      z-index: 100;
      max-height: 320px;
      overflow-y: auto;
      cursor: pointer;
      animation: hideIndex .3s;
      .option-item{
        border-bottom: 1px solid $btnBrColor;
        padding: 0 10px;
        height: 40px;
        line-height: 40px;
        &:last-child{
          border: none;
        }
        &:hover{
          background: $activeColor;
        }
      }
    }
    .arrow{
      position: absolute;
      top: 10px;
      right: 10px;
      cursor: pointer;
      &::after{
        content: '\e609';
        @extend %mr_icon;
        color: $color9;
        font-size: 10px;
      }
      &.top{
        &::after{
          content: '\e60a';
        }
      }
    }
  }
  .delete{
    margin: 10px 0 0 15px;
    cursor: pointer;
    &::after{
      content: '\e602';
      @extend %mr_icon;
      color: $stressColor;
      font-size: 16px;
    }
  }
}
@media screen and (max-width: $phoneWidth) {
  .recommend{
    flex-direction: column;
    background: $grayBg;
    border-radius: 10px;
    margin: 10px 5px;
    padding: 0 10px;
    animation: showRecommend .3s;
    .title{
      line-height: 3em;
      padding-left: 5px;
      font-weight: bold;
      letter-spacing: 1px;
      font-size: 1.4rem;
    }
    .select{
      width: 100%;
      margin: 0px 0 8px 0;
      .arrow{
        &::after{
          content: '\e604';
        }
      }
    }
    .delete{
      position: absolute;
      right: 7%;
      top: 3%;
      @include iconPadding;
      &::after{
        content: '\e607';
        color: $color9;
        font-size: 1.8rem;
      }
    }
  }
}
</style>
