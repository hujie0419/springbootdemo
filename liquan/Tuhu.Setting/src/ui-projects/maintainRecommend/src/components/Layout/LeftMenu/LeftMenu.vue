<template>
  <div @touchstart.stop="touchstart" @click.stop="click" class="mrs_left-menu">
      <div class="mr_list-parent">
          <menu-list class="mr_menu-parent" :list="menuList" :selectItem.sync="selectItem"></menu-list>
      </div>
      <div class="mr_list-child">
          <menu-list class="mr_menu-child" :list="selectItem && selectItem.Items" @clickItem="clickItem" @checkItem="checkItem(selectChildItem)" :selectItem.sync="selectChildItem" :initSelectFilter="initChildSelectFilter" :fieldFilter="childItemFieldFilter"></menu-list>
      </div>
  </div>
</template>

<script>
import MenuList from './MenuList/MenuList';
export default {
  data() {
    let category = this.$route.query && this.$route.query.category;
    return {
      category: (category && JSON.parse(category)) || null,
      selectChildItem: null,
      selectItem: null,
      menuList: []
    };
  },
  components: {
    MenuList
  },
  created() {
    this.getBaoYangCategoryies();
  },
  watch: {
    $route(nowval) {
      let category = nowval.query && nowval.query.category;
      this.category = (category && JSON.parse(category)) || null;
      this.menuList && this.menuList.every(item => {
        return !this.setSelectItem(item);
      });
    }
  },
  methods: {
    clickItem(item) {
      this.$emit('clickItem', item);
    },
    checkItem(item) {
      let tab = sessionStorage.getItem('tab');
      item && this.$router.push({
        path: '/console/content',
        query: {
          category: JSON.stringify(item),
          tab: tab
        }
      });
    },
    initChildSelectFilter(item, selectItem) {
      if (item && selectItem && item.Category === selectItem.Category && item.PartName === selectItem.PartName) {
        return item;
      }
    },
    childItemFieldFilter(item) {
      return item && item.PartName;
    },
    touchstart(evt) {
      this.$emit('touchstart', evt);
    },
    click(evt) {
      this.$emit('clickMenuBox', evt);
    },
    /**
         * 获取菜单列表
         */
    getBaoYangCategoryies() {
      let menuList = sessionStorage.getItem('menuList');
      if (menuList) {
        this.menuList = JSON.parse(menuList);
        this.menuList.every(item => {
          return !this.setSelectItem(item);
        });
        return;
      }
      this.$http.get('/BaoYang/GetBaoYangCategoriesInfo', {
        apiServer: 'apiServer',
        cacheData: false
      })
        .subscribe(back => {
          if (back && back.status) {
            if (back.data && back.data.length > 0) {
              back.data.every(item => {
                return !this.setSelectItem(item);
              });
              sessionStorage.setItem('menuList', JSON.stringify(back.data));
              this.menuList = back.data;
            }
          }
        });
    },
    setSelectItem(pItem) {
      let res;
      if (pItem && pItem.Items) {
        res = pItem.Items.filter(item => {
          return this.initChildSelectFilter(item, this.category);
        });
        if (res && res.length > 0) {
          this.selectChildItem = res[0];
          this.selectItem = pItem;
        }
      }
      return res && res[0];
    }
  }
};
</script>

<style lang="scss" scoped>
@import "css/common/_var.scss";
@import "css/common/_mixin.scss";
@import "css/common/_iconFont.scss";
.mrs_left-menu {
  height: 100%;
  width: 100%;
  display: flex;
  .mr_list-parent{
      background: $layoutBg;
      height: 100%;
      flex: 0 0 160px;
      width: 160px;
  }
  .mr_list-child {
      background: $colorf;
      height: 100%;
      flex: 0 0 140px;
      width: 140px;
  }
}
@media screen and (max-width: $maxWidth) {
    .mrs_left-menu {
        .mr_list-parent,.mr_list-child{
            flex: 0 0 50%;
        }
    }
}
</style>