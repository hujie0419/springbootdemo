<template>
<Tabs type="card" @on-click="onClick">
    <TabPane label="玻璃" name="tab1">
      <Couponpriceconfig productType='Glass' v-if="isLoading"></Couponpriceconfig>
    </TabPane>
  </Tabs>
</template>
<script>
import Couponpriceconfig from "./couponpriceconfig.vue";
export default {
  data () {
    return {
      isLoading: true
    };
  },
  methods: {
    onClick (name) {
      if (name === "tab1") {
        this.isLoading = true;
      }
    }
  },
  components: {
    Couponpriceconfig
  }
};
</script>
