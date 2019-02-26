<template>
    <div v-bind:class="[displayStatus ? 'nav shrinkednav' : 'nav']" @mouseover="doOver" @mouseout="doOut">
        <Input class="search" v-if="!displayStatus" v-model="searchVal" @on-enter="fliter" @on-click="fliter" icon="ios-search" placeholder="筛选一级目录" />
        <div v-on:click="hide" style="display:inline" class="wrapper"><Icon :type="icon" :size="displayStatus? 30:20" :class="[ displayStatus ? 'shrinked shrinkbutton': 'shrinkbutton']"></Icon></div>
        <Menu v-if="!displayStatus" ref="orangeMenu" :width="menuWidth" :open-names="openedmenus" :active-name="activename" @on-select="goto">
           <Submenu v-for="(item, index) in filteredMenus" :name="item.key" :key="index">
                <template slot="title">
                    <Icon :type="item.icon"></Icon>
                    {{ item.name}}
                </template>
                <MenuItem v-for="(child, childindex) in item.children" :name="child.key" 
                :key="childindex">
                {{ child.name}}
                </MenuItem>
            </Submenu>
        </Menu>
        <ul v-if="displayStatus">
            <li v-for="(item, index) in filteredMenus" :name="item.key" :key="index" class="menu">
                <Icon :type="item.icon" size="25"></Icon>
            </li>
        </ul>
    </div>
</template>

<script>
export default {
  name: "setting-menu",
  props: {
    menus: {
      type: Array
    },
    shrinkedMode: {
      type: Boolean
    }
  },
  data () {
    return {
      searchVal: "",
      filteredMenus: [],
      openedmenus: [],
      activename: "",
      navClass: "nav",
      icon: "ios-drag",
      hover: false,
      shrinked: false
    };
  },
  watch: {
    $route: function () {
      this.setActiveName(this.$router.currentRoute.name);
    }
  },
  computed: {
    displayStatus: function () {
      return this.shrinked && (this.shrinked && !this.hover);
    },
    menuWidth: function () {
      return this.displayStatus ? "50px" : "210px";
    }
  },
  mounted () {
    this.filteredMenus = this.menus;
    this.shrinked = this.shrinkedMode;
    this.setActiveName(this.$router.currentRoute.name);
  },
  methods: {
    goto: function (name) {
      this.$router.push({ name: name });
    },
    fliter: function () {
      var value = this.searchVal;
      this.filteredMenus = this.menus.filter(
        menu => menu.name.indexOf(value) >= 0
      );
      this.updateOpenedMenus(
        this.util.array.select(this.filteredMenus, item => {
          return item.key;
        })
      );
    },
    updateOpenedMenus: function (open, name) {
      this.openedmenus = open;
      if (name) {
        this.activename = name;
      }
      this.$nextTick(() => {
        if (!this.displayStatus) {
          this.$refs.orangeMenu.updateOpened();
          this.$refs.orangeMenu.updateActiveName();
        }
      });
    },
    setActiveName: function (name) {
      var openedMenu;
      this.menus.forEach(menu => {
        menu.children.forEach(child => {
          if (child.key === name) {
            openedMenu = menu.key;
          }
        });
      });
      this.updateOpenedMenus([openedMenu], name);
    },
    hide: function () {
      this.shrinked = !this.shrinked;
      if (!this.shrinked) {
        this.hover = false;
      }
      this.$emit("shrink", this.shrinked);
    },
    doOver: function () {
      if (this.shrinked) {
        this.hover = true;
      }
    },
    doOut: function () {
      if (this.shrinked) {
        this.hover = false;
      }
    }
  }
};
</script>

<style lang="less">
@import "./settingmenu.less";
</style>
