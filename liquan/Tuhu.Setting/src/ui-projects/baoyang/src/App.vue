<template>
  <div id="app">
    <settingheader :name="username" v-on:logout="logout"></settingheader>
    <settingmenu :menus="menus" :shrinkedMode="shrinked" @shrink="menuShrinked"></settingmenu>
    <div v-bind:class="[shrinked ? 'shrinkedContent' : 'content']">
    <div class="real-content"><router-view/>
    </div>
    <div style="height: 50px"></div>
    <settingfooter/>
    </div>
  </div>
</template>

<script>
import settingheader from "@/framework/components/settingheader/settingheader.vue";
import settingmenu from "@/framework/components/settingmenu/settingmenu.vue";
import settingfooter from "@/framework/components/settingfooter/settingfooter.vue";

export default {
  name: "App",
  data () {
    return {
      user: {
        name: ""
      },
      shrinked: this.util.storage.get("menu_shrinked") || false
    };
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
      var host = location.hostname
        .replace("yunying", "yewu")
        .replace("setting", "yewu");
      window.location.href =
        location.protocol + "//" + host + "/Account/LogOff";
    },
    menuShrinked: function (isShrinked) {
      this.shrinked = isShrinked;
      this.util.storage.set("menu_shrinked", isShrinked);
    }
  }
};
</script>

<style lang='less'>
@import "./app.less";
</style>
