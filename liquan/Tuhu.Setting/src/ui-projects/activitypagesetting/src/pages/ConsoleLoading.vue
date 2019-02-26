<template>
<div class="loading" v-loading="isLoading" element-loading-text="数据加载中,请稍后..."></div>
</template>

<script>
// import axios from 'axios';
// import {
//   logoutUrl
// } from '../config/config.js';
export default {
    name: '',
    data() {
        return {
            isLoading: true
        };
    },
    methods: {
        getMenus() {
            this.$router.push('/activity/activityConfig');
            // this.$http.get('/api/sdsab/test')
            //   .then((response) => {
            //     this.$router.push('/console/dashboard/');
            //   }, (err) => {
            //     this.isLoading = false;
            //   });
        },
        logout() {
            // location.href = this.tmsLogout();
        }
    // tmsLogout: function() {
    //   return logoutUrl + '?redirect=' + document.URL;
    // }
    },
    created() {
        this.getMenus();
    // axios.patch('/api/sdsab/test').then((response) => {
    //   // debugger;
    //   if (response.data.items == true) {
    //     this.getMenus();
    //   } else {
    //     this.getMenus();
    //     // this.$alert('该账号无权访问,请重新登录', '错误提示', {
    //     // confirmButtonText: '确定',
    //     // callback: action => {
    //     //     this.logout();
    //     // }
    //     // });
    //   }
    // }, (err) => {
    // });
    }

};
</script>

<style>
.loading {
  margin-top: 18%;
}
</style>
