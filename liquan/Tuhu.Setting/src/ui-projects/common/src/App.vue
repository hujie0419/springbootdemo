<template>
  <div id="app">
    <settingheader :name="username" v-on:logout="logout"></settingheader>
    <settingmenu :menus="menus"></settingmenu>
    <div class="content">
    <div class="real-content"><router-view  :key="$route.fullPath" />
    </div>
    <div style="height: 50px"></div>
    <settingfooter/>
    </div>
  </div>
</template>

<script>
import settingheader from '@/framework/components/settingheader/settingheader.vue'
import settingmenu from '@/framework/components/settingmenu/settingmenu.vue'
import settingfooter from '@/framework/components/settingfooter/settingfooter.vue'

export default {
  name: 'App',
  data () {
    return {
      user: {
        name: ""
      }
    }
  },
  computed: {
    menus () {
      return this.$store.state.app.menus;
    },
    username () {
      return this.$store.state.user.name;
    }
  },
  components: {
    settingheader,
    settingmenu,
    settingfooter
  },
  methods: {
    logout: function () {
      var host = location.hostname.replace("yunying", "yewu").replace("setting", "yewu");
      window.location.href = location.protocol + "//" + host + "/Account/LogOff";
    }
  }
}
</script>

<style lang='less'>
@import "./app.less";
</style>
