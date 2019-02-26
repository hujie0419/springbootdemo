<template>
  <div class="page-loading">
    <loading :text="'数据加载中,请稍后...'"></loading>
  </div>
  <!-- <div class="loading" v-loading="isLoading" element-loading-text="数据加载中,请稍后..."></div> -->
</template>

<script>
// import {
//   logoutUrl
// } from '../config/config.js'
import Loading from '../components/loading/loading';
export default {
  name: '',
  components: {
    Loading
  },
  data() {
    return {
      isLoading: true
    };
  },
  methods: {
    getMenus() {
      // $http.get('/auth/GetBasicInformantion', {
      //   apiServer: 'apiServer',
      //   cacheData: true
      // }).subscribe(data => {
      this.$router.push('/console/content');
      // })
    }
    // logout() {
    //   location.href = this.tmsLogout();
    // },
    // tmsLogout: function() {
    //   return logoutUrl + '?redirect=' + document.URL
    // },
  },
  mounted() {
    this.getMenus();
  }

};
</script>

<style>
</style>
