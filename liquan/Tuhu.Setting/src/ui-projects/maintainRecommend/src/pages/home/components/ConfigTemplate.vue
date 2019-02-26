<template>
  <div class="template">
    <h2 class="templateTitle">{{!configTemplateData.AreaId?'全国模板':'地区模板'+configTemplateData.AreaId}}</h2>
    <div class="templateInfo">
      <p >状态：<span class="templateType">{{configTemplateData.IsEnabled || !configTemplateData.AreaId?"启用":"禁用"}}</span></p>
       <el-popover
        placement="top"
        title=""
        width="330"
        trigger="hover"
        :content="selectArea">
        <p slot="reference">地区：{{selectArea}}</p>
      </el-popover>
    </div>
    
    <ul class="btnList" v-if="!configTemplateData.AreaId">
      <li class="btnItem">
        <router-link target="_blank" :to="'/console/special?partName='+partName +'&category='+category">编辑</router-link>
      </li>
    </ul>
    <ul class="btnList" v-if="configTemplateData.AreaId">
      <li class="btnItem" >
        <router-link target="_blank" :to="'/console/special?partName='+partName+'&areaId=' + configTemplateData.AreaId+'&category='+category">编辑优先级</router-link>
      </li>
      <li class="btnItem" @click="editArea(configTemplateData,index)">修改地区范围</li>
      <li class="btnItem" @click="setDisabled(configTemplateData)">{{configTemplateData.IsEnabled?"禁用":"启用"}}</li>
    </ul>
  </div>
</template>
<script>
export default {
  props: {
    configTemplateData: {
      type: Object,
      default: () => {
        return {};
      }
    },
    partName: {
      type: String,
      default: '机油'
    },
    category: {
      type: String,
      default: 'Oil'
    },
    index: {
      type: Number,
      default: 0
    }
  },
  data() {
    return {
    };
  },
  computed: {
    /**
     * 地区模板显示
     * @return {String}
     */
    selectArea() {
      let data = '';
      if (!this.configTemplateData.AreaId) return '全国';
      this.configTemplateData.Details && this.configTemplateData.Details.forEach(provice => {
        let cityList = [];
        provice.Citys.forEach(cityItem => {
          cityList.push(cityItem.CityName);
        });
        if (cityList.length) {
          data += provice.RegionName + '(' +cityList.join('、')+ ')、';
        } else {
          data += provice.RegionName+'、';
        }
      });
      return data.substr(0, data.length-1);
    }
  },
  methods: {
    editArea(row, index) {
      this.$emit('openAreaTemplate', row, index);
    },
    setDisabled(row) {
      let status = row.IsEnabled?'禁用':'启用';
      this.$confirm('是否'+status+'优先级配置', '操作确认', {
        confirmButtonText: '确定',
        cancelButtonText: '取消',
        type: 'info',
        closeOnClickModal: false
      }).then(() => {
        this.$emit('setDisabled', row);
      }).catch(() => {});
    }
  }
};
</script>
<style lang="scss" scoped>
@import "css/common/_var.scss";
@import "css/common/_mixin.scss";
@import "css/common/_iconFont.scss";
.template{
  width: 330px;
  border:1px solid  $colore;
  margin-bottom: $defaultHeight;
  .templateTitle{
    text-align: center;
    border-bottom:1px solid  $colore;
    margin: 0;
    line-height: $defaultHeight - 1;
    background-color: $colorfa;
    font-size: $defaultFontSize;
  }
  .templateInfo{
    font-size: $smallFontSize;
    padding:0 1rem ;
    margin:30px 0;
    line-height: 20px;
    .templateType{
      color: $stressColor;
    }
    p{
      margin:0;
      overflow: hidden;
      text-overflow:ellipsis;
      white-space: nowrap;
      &.el-popover__reference{
        height: 40px;
        white-space:normal;
        display:-webkit-box; 
        -webkit-box-orient:vertical;
        -webkit-line-clamp:2; 
      }
    }
  }
  
  .btnList{
    border-top:1px solid  $colore;
    display:flex;
    cursor: pointer;
    .btnItem{
      line-height: $defaultHeight - 1;
      flex:1;
      text-align: center;
      border-left:1px solid  $colore;
      font-size: $smallFontSize;
      &:first-child{
        border: 0;
      }
      a{
        display: inline-block;
        width: 100%;
        height: 100%;
      }
    }
  }
}
</style>