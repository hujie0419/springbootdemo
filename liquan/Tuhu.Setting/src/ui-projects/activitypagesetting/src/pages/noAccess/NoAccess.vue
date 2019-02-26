<template>
  <div class="no-access-page">
      <div class="no-access-hint">{{hintText}}</div>
  </div>
</template>

<script>

export default {
    name: '',
    props: {},
    components: {},

    data () {
        return {
            hintText: '您没有该项目权限，请联系部门管理员配置权限'
        };
    },

    computed: {},

    watch: {},

    created () {
        if (this.$route.query.type === 'networkError') {
            this.hintText = '网络异常，请稍后再试';
        }
    },

    mounted () {},

    methods: {}

};
</script>
<style lang='scss'>
.no-access-page {
    height: 100%;
    display: flex;
    align-items: center;
    justify-content: center;
    .no-access-hint {
        text-align: center;
    }
}
</style>
