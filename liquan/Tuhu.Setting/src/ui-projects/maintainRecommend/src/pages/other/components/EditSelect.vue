<template>
    <div class="editSelect">
      <el-select v-model="NewViscosity" clearable filterable  placeholder="请选择新粘度" popper-class="mySelect" v-if="isOil">
        <el-option
          v-for="(item,i) in Viscositylist"
          :key="i"
          :label="item"
          :value="item">
        </el-option>
      </el-select>
      <ul class="editSelectBox">
        <li v-for="(selectedItem,i) in mySelect" :key="i">
          <span class="index" >第{{i + 1}}优先级</span>
          <el-select v-if="isOil" v-model="selectedItem.Grade" filterable  placeholder="请选择机油等级" popper-class="mySelect">
            <el-option
              v-for="(item,i) in oilGradeList"
              :key="i"
              :label="item"
              :value="item">
            </el-option>
          </el-select>
          <el-select v-model="selectedItem.Brand" filterable  placeholder="请选择商品品牌" popper-class="mySelect"  @change="setProductSeriesList(i,'')">
              <el-option
                v-for="(item,i) in productBrandList"
                :key="i"
                :label="item"
                :value="item">
              </el-option>
          </el-select>
          <el-select v-model="selectedItem.Series" filterable  placeholder="请选择商品系列" popper-class="mySelect">
              <el-option
                v-for="(item,i) in productSeriesList[selectedItem.Brand]"
                :key="i"
                :label="item"
                :value="item">
              </el-option>
          </el-select>
          <span class="mr_icon" @click="delecteditSelect(i)">&#xe602;</span>
        </li>
      </ul>
      <div class="addeditSelect" @click="addeditSelect">+ 添加优先级</div>
    </div>
</template>
<script>
export default {
  props: {
    oilGradeList: {
      type: Array,
      default: () => {
        return [];
      }
    },
    productBrandList: {
      type: Array,
      default: () => {
        return [];
      }
    },
    Viscosity: {
      type: String,
      default: ''
    },
    Viscositylist: {
      type: Array,
      default: () => {
        return [];
      }
    },
    isOil: {
      type: Boolean,
      default: false // 0 不是机油，1是机油
    },
    selected: {
      type: Array,
      default: () => {
        return [];
      }
    },
    brandAndSeries: {
      type: Object,
      default: {}
    }
  },
  data() {
    return {
      productSeriesList: [],
      NewViscosity: this.Viscosity,
      mySelect: []
    };
  },
  watch: {
    selected: {
      handler(val) {
        this.updateSelect();
      },
      deep: true
    },
    Viscosity(val) {
      this.NewViscosity = val;
    }
  },
  mounted() {
    this.updateSelect();
  },
  methods: {
    delecteditSelect(index) {
      this.mySelect.splice(index, 1);
    },
    addeditSelect() {
      this.mySelect.push({});
    },
    updateSelect() {
      this.mySelect = JSON.parse(JSON.stringify(this.selected));
      this.mySelect.forEach((item, index) => {
        this.setProductSeriesList(index, item.Series);
      });
    },
    /**
     * 通过品牌获得系列-商品
     * @param{Number} index 索引
     * @param{String} Series 要设置的系列
     */
    setProductSeriesList(index, Series) {
      this.$set(this.mySelect[index], 'Series', Series);
      if (!this.mySelect[index].Brand) {
        this.$set(this.productSeriesList, this.mySelect[index].Brand, []);
        return;
      }
      let productSeriesList = JSON.parse(JSON.stringify(this.brandAndSeries[this.mySelect[index].Brand]));
      this.$set(this.productSeriesList, this.mySelect[index].Brand, productSeriesList);
      // this.$http.get('/Product/GetProductSeries', {
      //   apiServer: 'apiServer',
      //   params: {category: this.$route.query.category, brand: this.mySelect[index].Brand}
      // }).subscribe((res) => {
      //   if (res.status === true) {
      //     this.$set(this.productSeriesList, this.mySelect[index].Brand, res.data);
      //     // this.productSeriesList[this.selected[index].Brand] = res.data;
      //   }
      // });
    },
    submit() {
      let mySelect = JSON.parse(JSON.stringify(this.mySelect));
      let item = {};
      let submitSelect = [];
      for (let i = 0; i<mySelect.length; i++) {
        item = mySelect[i];
        if (item.Grade || item.Brand || item.Series) {
          if (this.isOil && !item.Grade) {
            this.$message({message: `机油等级不能为空`, type: 'error', dangerouslyUseHTMLString: true});
            return false;
          }
          if (!item.Brand) {
            this.$message({message: `品牌不能为空`, type: 'error', dangerouslyUseHTMLString: true});
            return false;
          }
          if (!item.Series) {
            this.$message({message: `系列不能为空`, type: 'error', dangerouslyUseHTMLString: true});
            return false;
          }
          submitSelect.push(item);
        }
      }
      if (!this.NewViscosity && submitSelect.length === 0) {
        if (this.isOil) {
          this.$message({message: '请添加优先级或选择新粘度', type: 'error', dangerouslyUseHTMLString: true});
        } else {
          this.$message({message: '请添加优先级', type: 'error', dangerouslyUseHTMLString: true});
        }
        return false;
      }
      return submitSelect;
    },
    getNewViscosity() {
      return this.NewViscosity;
    },
    clearNewViscosity() {
      this.NewViscosity = this.Viscosity;
    }
  }
};
</script>

<style lang="scss" scoped>
@import "css/common/_var.scss";
@import "css/common/_mixin.scss";
@import "css/common/_iconFont.scss";
  .editSelect{
    .editSelectBox{
      li{
        margin-top: 10px;
        color: $color9;
        .index{
          width: 120px;
          height: 40px;
          line-height: 40px;
          text-align:center;
          display: inline-block;
          color: $color9;
          border:1px solid  $colore;
          background-color: $colorfa;
          border-radius: 4px;
        }
        .mr_icon{
          cursor: pointer;
        }
      }
    }
    .editSelectType{
      color: $stressColor;
    }
    .addeditSelect{
      margin-top: 1rem;
      color: $color9;
      font-size: 13px;
      letter-spacing: 1px;
      width: 120px;
      height: 40px;
      line-height: 40px;
      text-align: center;
      border: 1px dashed $btnBrColor;
      cursor: pointer;
      border-radius: 4px;
      animation: showRecommendShort .3s;
    }
    .btnList{
      margin-top: 5rem;
      // overflow: hidden;
      // float: right;
      text-align: right;
      .btnItem{
        display: inline-block;
        div{
          &.btn{
            padding: .5rem 1rem;
            border:1px solid  $colore;
            text-align: center;
            margin-left: 10px;
            border-radius:4px;
            background-color:$colore;
          }
        }
        &:last-child .btn{
          color:$colorf;
          background-color:$stressColor;
          border-color:$stressColor;
        }
      }
    }
}

</style>