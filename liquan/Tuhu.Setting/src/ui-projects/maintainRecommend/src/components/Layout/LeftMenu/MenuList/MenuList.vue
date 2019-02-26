<template>
  <div class="mrs_menu-list">
    <ul class="mr_menu-list-cont">
      <li class="mr_menu-item"
        :class="[(selectItem === item) && 'mr_active']" @click="clickItem(item)" v-for="(item, key) in list" :key="key">
        <span class="mr_menu-text">{{fieldFilter(item)}}</span>
      </li>
    </ul>
  </div>
</template>

<script>
export default {
  props: {
    /**
         * 显示字段过滤器
         */
    fieldFilter: {
      type: Function,
      default: (item) => {
        return item && item.CategoryName;
      }
    },
    /**
         * 初始化选中项过滤器
         */
    initSelectFilter: {
      type: Function,
      default: (item, selectItem) => {
        if (item.CategoryType === selectItem.CategoryType && selectItem.CategoryName === item.CategoryName) {
          return item;
        }
      }
    },
    selectItem: { // 选中项

    },
    list: {
      default() {
        return [];
      }
    }
  },
  watch: {
    list() {
      this.initSelectItem();
    }
  },
  mounted() {
    this.initSelectItem();
  },
  methods: {
    clickItem(item) { // 点击时的事件
      this.$emit('clickItem', item);
      this.checkItem(item);
    },
    checkItem(item) {
      this.$emit('update:selectItem', item);
      this.$emit('checkItem', item);
    },

    /**
         * 设置初始化时选中项
         */
    initSelectItem() {
      let self = this;
      let list = self.list;
      let selectItem = self.selectItem;
      let res;

      if (list && list.length > 0) {
        if (selectItem) {
          list.every(item => {
            let _res;
            if (this.initSelectFilter instanceof Function) {
              _res = this.initSelectFilter(item, selectItem);
            }
            if (_res) {
              res = _res;
              return false;
            }
            return true;
          });
        }
        if (!res) {
          res = list[0];
        }
      }
      res && this.checkItem(res);
    }
  }
};
</script>

<style lang="scss" scoped>
@import "css/common/_var.scss";
@import "css/common/_mixin.scss";
@import "css/common/_iconFont.scss";
.mrs_menu-list {
  width: 100%;
  .mr_menu-list-cont {
    width: 100%;
  }
  .mr_menu-item {
    cursor: pointer;
    width: 100%;
    transition: background-color .2s ease-out;
    padding: 0 8px;
    &.mr_active{
        font-weight: bold;
    }
  }
  .mr_menu-text {
    display: block;
    width: 100%;
    white-space: nowrap;
    overflow: hidden;
    text-overflow: ellipsis;
  }
  &.mr_menu-parent {
    color: $colorf;
    .mr_menu-item {
      line-height: 50px;
      text-align: center;
      &.mr_active{
        background-color: $stressColor;
      }
    }
  }
  &.mr_menu-child {
    .mr_menu-item {
      line-height: 40px;
      text-align: left;
      padding-left: 26px;
      &.mr_active{
        background-color: $activeColor;
        color: $stressColor;
      }
      border-bottom: 1px solid $splitBrColor;
    }
  }
}
@media screen and (max-width: $maxWidth) {
    .mrs_menu-list{
      &.mr_menu-parent {
        .mr_menu-item {
          padding-left: 16px;
          line-height: 40px;
          text-align: left;
        }
      }
      &.mr_menu-child {
        .mr_menu-item {
          padding-left: 16px;
        }
      }
    }
}
</style>