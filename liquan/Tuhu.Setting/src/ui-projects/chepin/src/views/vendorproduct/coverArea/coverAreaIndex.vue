<template>
  <div>
    <h2 class="title" v-if="this.productType=='Glass'">汽车玻璃品牌及地区配置</h2>
    <h2 class="title" v-if="this.productType==='Battery'">蓄电池玻璃品牌及地区配置</h2>
    <Tabs style="margin-top:20px;" type="card" @on-click="onClick">
      <TabPane label="品牌覆盖区域配置" name="tab0">
        <PriorityConfig style="display:inline-block" :productType="productType" :configType="configType"></PriorityConfig>
        <BatteryCoverArea style="margin-top:20px;" :productType="productType"></BatteryCoverArea>
      </TabPane>
      <TabPane label="Pid覆盖区域配置" name="tab1">
        <PidCoverArea :productType="productType" v-if="isLoading"></PidCoverArea>
      </TabPane>
    </Tabs>
  </div>
</template>
<script>
import BatteryCoverArea from "./brandCoverArea.vue";
import PidCoverArea from "./pidCoverArea.vue";
import PriorityConfig from "../priorityConfig/priorityConfig.vue";
export default {
  name: "coverAreaIndex",
  props: {
    productType: {
      type: String,
      required: true
    }
  },
  data () {
    return {
      isLoading: false,
      configType: "Region"
    };
  },
  created () {
    window.CoverAreaIndexVue = this;
  },
  methods: {
    onClick (name) {
      if (name === "tab1") {
        this.isLoading = true;
      }
    }
  },
  components: {
    BatteryCoverArea,
    PidCoverArea,
    PriorityConfig
  }
};
</script>
