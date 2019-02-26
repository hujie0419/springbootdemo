<template>
  <div class='wrapper' style="overflow:hidden">
    <layout></layout>
  </div>
</template>

<script>
import { mapGetters } from 'vuex';
export default {
    name: 'home',
    data() {
        return {
            dashboardComponent: 'dashboard',
            clientHeight: 590
        };
    },
    methods: {},
    computed: {
        ...mapGetters({
            comps: 'allComps',
            dashboardActive: 'dashboardActive'
        })
    },
    mounted() {
        this.clientHeight = document.documentElement.clientHeight - 40;
    }
};
</script>

<style scoped>
.tab-content {
  background: #eff3f7;
}

.content-wrapper {
  margin-left: 120px;
}
</style>
