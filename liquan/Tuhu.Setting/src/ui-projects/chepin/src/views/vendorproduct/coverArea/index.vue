<template>
<Tabs type="card" @on-click="onClick">
    <TabPane label="玻璃" name="tab1">
      <coverAreaIndex productType='Glass' v-if="isLoading"></coverAreaIndex>
    </TabPane>
  </Tabs>
</template>
<script>
import coverAreaIndex from "./coverAreaIndex.vue";
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
    coverAreaIndex
  }
};
</script>
