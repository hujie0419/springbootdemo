<template>
    <div class="areaSelect">
      <ul class="areaSelectBox">
        <li v-for="(item, index) in mySelected" :key="index">
          <span class="index">省份{{index +1}}</span>
          <el-select v-model="item.RegionId" placeholder="请选择省份(单选)" popper-class="mySelect" @change="setCity(item)">
            <el-option
              v-for="(proviceItem,i) in proviceList"
              :key="i"
              :label="proviceItem.RegionName"
              :value="proviceItem.RegionId"
              :disabled="!(proviceIdList.indexOf(proviceItem.RegionId) < 0)">
            </el-option>
          </el-select>
          <el-select v-model="item.ChildRegions" multiple collapse-tags placeholder="请选择城市(多选)" popper-class="mySelect" @change="setArea(index)">
              <el-option
                v-if="getProviceById(item.RegionId,index).ChildRegions.length > 1"
                label="全选"
                value="all"
                @click.native="selestAll(index)"
                >
              </el-option>
              <el-option
                v-for="(cityItem,i) in getProviceById(item.RegionId,index).ChildRegions"
                :key="i"
                :label="cityItem.RegionName"
                :value="cityItem.RegionId">
              </el-option>
          </el-select>
          <span class="mr_icon" @click="delectAreaSelect(index)" v-if="index">&#xe602;</span>
        </li>
      </ul>
      <div class="addAreaSelect" @click="addAreaSelect">+ 添加优先级</div>
      
    </div>
</template>
<script>
export default {
  props: {
    proviceList: { // 供选择的数据
      type: Array,
      default: () => {
        return [];
      }
    },
    selected: { // 已选择数据
      type: Array,
      default: () => {
        return [];
      }
    }
  },
  data() {
    return {
      mySelected: [] // 已选数据（组件内部）
    };
  },
  computed: {
    /**
     * select 给已选择的省份 禁用
     *  @return {*}  当前已经选择的省份id 数组
     */
    proviceIdList() {
      let provice = [];
      this.mySelected.forEach(item => {
        provice.push(item.RegionId);
      });
      return provice;
    }
  },
  watch: {
    /**
     * 更新组件选中的数据
     */
    // selected: {
    //   handler(val) {
    //     this.setMySelected(val);
    //   },
    //   deep: true
    // },
    proviceList: {
      handler(val) {
        // console.log(val);
        this.setMySelected(this.selected);
      },
      deep: true
    }
  },
  created() {
    this.setMySelected(this.selected);
  },
  methods: {
    /**
     * 设置组件内部选中的数据
     *  @param {Array} selected 当前组件选择的值
     */
    setMySelected(selected) {
      this.mySelected = [];
      selected.forEach((item, index) => {
        let _proviceList = this.getProviceById(item.RegionId).ChildRegions;
        let ChildRegions = [];
        item.Citys.forEach(ChildRegionsItem => {
          ChildRegions.push(ChildRegionsItem.CityId);
        });
        if (_proviceList.length && ChildRegions.length === _proviceList.length && ChildRegions.length!==1) {
          ChildRegions.splice(0, 0, 'all');
        }
        // debugger;
        this.mySelected.push({RegionId: item.RegionId, ChildRegions: ChildRegions});
      });
      this.mySelected.push({RegionId: null, ChildRegions: []});
    },
    /**
     * 通过省份id 获得 所对应的item
     *  @param {Number} id - 当前省份id.
     *  @param {Number} index - 当前索引
     *  @return {Object} 所对应的item
     */
    getProviceById(id, index) {
      if (!id) return {RegionId: null, ChildRegions: []};
      let provice = this.proviceList.filter(item => {
        return item.RegionId === id;
      });
      if (provice[0]) {
        return JSON.parse(JSON.stringify(provice[0]));
      } else {
        return {RegionId: null, ChildRegions: []};
      }
    },
    /**
     * 当省份改变时，清空城市
     *  @param {Object} item - 当前item
     */
    setCity(item) {
      this.$set(item, 'ChildRegions', []);
    },
    /**
     * 新增选中列表
     */
    addAreaSelect() {
      this.mySelected.push({RegionId: null, ChildRegions: []});
    },
    /**
     * 删除指定选中列表其中一个
     * @param {Number} index - 当前index
     */
    delectAreaSelect(index) {
      this.mySelected.splice(index, 1);
    },
    /**
     * 返回提交的数据
     *  @return {Array} 当前组件选择的值
     */
    submit() {
      let submitList=[];
      this.mySelected.forEach(item => {
        if (!item.RegionId) {
          return;
        }
        submitList.push(this.getProviceById(item.RegionId));
        let ChildRegionsList = [];
        submitList[submitList.length-1].ChildRegions.forEach((ChildRegionsItem, index) => {
          if (item.ChildRegions.indexOf(ChildRegionsItem.RegionId) > -1) {
            ChildRegionsList.push({'CityId': ChildRegionsItem.RegionId, 'CityName': ChildRegionsItem.RegionName});
          }
        });
        submitList[submitList.length-1].Citys = ChildRegionsList;
      });
      return submitList;
      // this.$emit('changeSelected', submitList);
    },
    selestAll(index) {
      let areaList = this.mySelected[index].ChildRegions; // 已经选择的区域列表
      let ChildRegions = this.getProviceById(this.mySelected[index].RegionId).ChildRegions;// 可选择的区域列表
      if (areaList.indexOf('all') > -1) {
        let list = ['all'];
        ChildRegions.forEach(item => {
          list.push(item.RegionId);
        });
        this.$set(this.mySelected[index], 'ChildRegions', list);
      } else {
        this.$set(this.mySelected[index], 'ChildRegions', []);
      }
    },
    setArea(index) {
      let areaList = this.mySelected[index].ChildRegions; // 已经选择的区域列表
      let ChildRegions = this.getProviceById(this.mySelected[index].RegionId).ChildRegions;// 可选择的区域列表
      if (areaList.indexOf('all') > -1 && areaList.length === ChildRegions.length) {
        areaList.splice(areaList.indexOf('all'), 1);
      }
    }
  }
};
</script>

<style lang="scss" scoped>
@import "css/common/_var.scss";
@import "css/common/_mixin.scss";
@import "css/common/_iconFont.scss";
  .areaSelect{
    .areaSelectBox{
      li{
        margin-top: 10px;
        color: $color9;
        .index{
          width: 120px;
          height: 40px;
          line-height: 40px;
          text-align:center;
          display: inline-block;
          border:1px solid  $colore;
          background-color: $colorfa;
          border-radius: 4px;
        }
        &:first-child{
          margin-top: 0;
        }
      }
      .mr_icon{
        cursor: pointer;
      }
    }
    .areaSelectType{
      color: $stressColor;
    }
    .addAreaSelect{
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
      display:flex;
      .btnItem{
        flex:1;
        .btn{
          padding: .5rem 1rem;
          border:1px solid  $colore;
          float: right;
          text-align: center;
          margin-right: 1rem;
          border-radius:4px;
          background-color:$colore;
        }
        &:last-child .btn{
          float: left;
          color:$colorf;
          background-color:$stressColor;
          margin-left: 1rem;
          border-color:$stressColor;
        }
      }
    }
  }

</style>