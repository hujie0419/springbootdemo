<template>
    <div v-bind:class="{nav:!shrinked}">
        <Input class="search" v-model="searchVal" @on-enter="fliter" @on-click="fliter" icon="ios-search" placeholder="筛选一级目录" />
        <Menu ref="orangeMenu" width="210px" :open-names="openedmenus" :active-name="activename" @on-select="goto">
           <Submenu v-for="(item, index) in filteredMenus" :name="item.key" :key="index">
                <template slot="title">
                    <Icon :type="item.icon"></Icon>
                    {{item.name}}
                </template>
                <MenuItem v-for="(child, childindex) in item.children" :name="child.key" 
                :key="childindex">
                {{child.name}}
                </MenuItem>
            </Submenu>
        </Menu>
        <Button v-show="shrinked" class="hide" v-on:click="shrinked=!shrinked">收起</Button>
    </div>
</template>

<script>
export default {
    name: 'setting-menu',
    props: {
        menus: {
            type: Array
        }
    },
    data () {
        return {
            searchVal: "",
            filteredMenus: [],
            openedmenus: [],
            shrinked: false,
            activename: ""
        }
    },
    watch: {
         '$route': function () {
             this.setActiveName(this.$router.currentRoute.name);
         }
    },
    mounted () {
        this.filteredMenus = this.menus;
        this.setActiveName(this.$router.currentRoute.name);
    },
    methods: {
        goto: function (name) {
            this.$router.push({'name': name});
        },
        fliter: function () {
            var value = this.searchVal;
            this.filteredMenus = this.menus.filter(menu => menu.name.indexOf(value) >= 0);
            this.updateOpenedMenus(this.util.array.select(this.filteredMenus, (item) => { return item.key; }));   
        },
        updateOpenedMenus: function (open, name) {
            this.openedmenus = open;
            if (name) {
                this.activename = name;
            }
            this.$nextTick(function () {
                this.$refs.orangeMenu.updateOpened();
                this.$refs.orangeMenu.updateActiveName();
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
        }
    }
};
</script>

<style lang="less">
@import "./settingmenu.less";
</style>
