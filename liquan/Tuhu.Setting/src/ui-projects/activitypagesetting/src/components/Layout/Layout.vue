<template>
    <div class="app-wrapper" :class="{hideSidebar:hideSideVal}">
        <div class="sidebar-wrapper" :class="{hoverSidebar:hoverSideVal}" @mouseenter="enterSidebar" @mouseleave="leaveSidebar">
            <side-bar class="sidebar-container" v-on:fixSideEvent="fixSide($event)"></side-bar>
        </div>
        <div class="main-container">
            <nav-bar v-on:clickAction="changeSider($event)"></nav-bar>
            <app-main></app-main>
        </div>
    </div>
</template>

<script>
    import store from 'store';
    // import router from 'router';
    export default {
        name: 'layout',
        data() {
            return {
                hideSideVal: false,
                hoverSideVal: false
            };
        },
        components: {
        },
        computed: {
            sidebar() {
                return '';
                //   return this.$store.state.app.sidebar;
            }
        },
        methods: {
            changeSider(val) {
                this.hideSideVal = !this.hideSideVal;
                this.hoverSideVal = false;
            },
            fixSide(val) {
                this.hideSideVal = false;
            },
            enterSidebar() {
                if (this.hideSideVal === true) {
                    this.hoverSideVal = true;
                }
            },
            leaveSidebar() {
                if (this.hideSideVal === true) {
                    this.hoverSideVal = false;
                }
            }
        },
        beforeRouteEnter: (to, from, next) => {
            const roles = store.getters.roles;
            if (roles.length !== 0) {
                //   next();
                //   return
            }
            store.dispatch('GetInfo').then(() => {
                permission.init({
                    roles: store.getters.roles
                    // router: router.options.routes
                });
                next();
            }).catch(err => {
                console.log(err);
            });
        }
    };
</script>
<style rel="stylesheet/scss" lang="scss" scoped>
    @import "css/common/_var.scss";
    @import "css/common/_mixin.scss";
    @import "css/common/_iconFont.scss";
    .app-wrapper {
        $zIndex: 998;
        @include clearfix;
        position: relative;
        height: 100%;
        width: 100%;
        min-width: $minWidth;

        &.hideSidebar{
            .sidebar-wrapper{
                width:$smallNavLeftWidth;
            }
            .main-container{
                margin-left:$smallNavLeftWidth;
            }

            /deep/ .navbar {
                left: $smallNavLeftWidth;
                min-width: $minWidth - $smallNavLeftWidth;
            }
            /deep/ .page-tabs{
                .el-tabs__header {
                    left: $smallNavLeftWidth + 8 +1;
                    min-width: $minWidth - $smallNavLeftWidth - 8 * 2 - 2;
                }
            }
        }
        .sidebar-wrapper {
            overflow-x: hidden;
            transition: all .28s ease-out;
            // @include scrollBar;

            &.hoverSidebar{
                width:$navLeftWidth !important;
            }
        }
        .sidebar-container {
            transition: all .28s ease-out;
        }
        .main-container {
            padding-top: $headerHeight + $headerTabHeight - 1;
            min-height: 100%;
            transition: all .28s ease-out;
            margin-left: $navLeftWidth;
        }

        // 设置右侧头部浮动
        .sidebar-wrapper {
            position: fixed;
            top: 0;
            bottom: 0;
            left: 0;
            z-index: $zIndex;
        }
        /deep/ .navbar {
            transition: all .28s ease-out;
            position: fixed;
            top: 0;
            right: 0;
            left: $navLeftWidth;
            z-index: $zIndex - 10;
            min-width: $minWidth - $navLeftWidth;
        }
        /deep/ .page-tabs{
            .el-tabs--border-card {
                border-top: none;
            }
            .el-tabs__header {
                transition: all .28s ease-out;
                position: fixed;
                top: 0;
                right: 8px + 1;
                left: $navLeftWidth + 8 + 1;
                top: $headerHeight;
                padding-top: 8px;
                z-index: $zIndex - 10;
                border-bottom: none;
                background: #F2F7FE;
                min-width: $minWidth - $navLeftWidth - 8 * 2 - 2;
            }
            .el-tabs__nav-wrap {
                background: #f5f7fa;
                // border-bottom: 1px solid #e4e7ed;
                border-radius: 4px 4px 0 0;
                box-shadow: 2px -2px 3px 1px rgba(0,0,0,.08),0 -1px 0px 0 rgba(0,0,0,.08) inset, -2px -2px 3px 1px rgba(0,0,0, 0.08);
            }
        }
    }
</style>
