<template>
    <el-menu class="navbar">
        <div class="left">
            <!-- <Hamburger class="hamburger-container" :toggleClick="toggleSideBar" :isActive="sidebar.opened"></Hamburger> -->
            <div class="menuBtn" @click="changeSidebar()"></div>
            <level-bar></level-bar>
            <!-- <el-input placeholder="请输入内容" suffix-icon="el-icon-search"></el-input> -->
        </div>
        <div class="right">
            <ErrLog v-if="log.length>0" class="errLog-container" :logsList="log"></ErrLog>
            <div class="avatar-container" trigger="click">
                <div class="help">
                    <Icon icon-name="alert-circle" ></Icon>
                    帮助
                </div>
                <span class="line"></span>
                <div class="avatar-wrapper">
                    <!-- <img v-if="avatar" class="user-avatar" :src="avatar"> -->
                    <Icon icon-name="user" color="#fbfdff" :size="16" style="fill:currentcolor;" ></Icon>
                    用户账号：{{userInfo && userInfo.name || ''}}
                     <!-- [{{introduction}}] -->
                </div>
                <span class="line"></span>
                <div class="exit" @click="logout">
                    <Icon icon-name="exit" ></Icon>
                    退出
                </div>
                <!-- <el-dropdown-item divided><span @click="logout" style="display:block;">退出登录</span></el-dropdown-item> -->
            </div>
        </div>
    </el-menu>
</template>

<script>
    import { mapGetters } from 'vuex';
    // import Levelbar from './Levelbar';

    export default {
        components: {
        // Levelbar,
        // Hamburger,
        // ErrLog
        },
        data() {
            return {
                log: ''
            };
        },
        computed: {
            ...mapGetters([
                'userInfo'
                // 'sidebar',
                // 'name',
                // 'avatar',
                // 'introduction'
            ])
        },
        methods: {
            toggleSideBar() {
                // this.$store.dispatch('ToggleSideBar');
            },
            changeSidebar() {
                this.$emit('clickAction');
            },
            logout() {
                this.$loginOut();
                // this.$store.dispatch('LogOut').then(() => {
                //   this.$router.push({ path: '/login' });
                // });
            }
        }
    };
</script>
<style rel="stylesheet/scss" lang="scss" scoped>
  @import "css/common/_var.scss";
  @import "css/common/_mixin.scss";
  @import "css/common/_iconFont.scss";

    .navbar {
        height: 58px;
        line-height: 58px;
        border-radius: 0px !important;
        border-bottom: solid 1px #e6e6e6;
        &>div{
            height: 58px;
        }
        &:before, &:after {
            content: none;
        }
        display: flex;
        justify-content: space-between;
        .hamburger-container {
            line-height: 58px;
            height: 50px;
            float: left;
            padding: 0 10px;
        }
        .errLog-container {
            display: inline-block;
        }
        .right {
            color: #606266;
            font-size: 14px;
            display: flex;
            justify-content: space-between;
        }
        .avatar-container {
            height: 58px;
            display: inline-block;
            text-align: center;
            display: flex;
            justify-content: space-between;
            align-items: center;

            .avatar-wrapper {
                height: 100%;

                .user-avatar {
                    width: 30px;
                    height: 30px;
                    border-radius: 50%;
                    margin-right: 5px;
                }
                svg {
                    margin-right: 5px;
                }
                .el-icon-caret-bottom {
                    position: absolute;
                    right: -20px;
                    top: 25px;
                    font-size: 12px;
                }
            }
            .avatar-wrapper, .help, .exit {
                display: flex;
                justify-content: center;
                align-items: center;
            }
            .help, .exit {
                cursor:pointer;
                .icon {
                    margin-right: 3px;
                }
            }
            .exit {
                padding-right: 10px;
            }
            .line {
                border-left: 1px solid #8391a5;
                height: 15px;
                margin: 0 10px;
            }
        }
        .menuBtn{
            width:45px;
            height:40px;
            cursor: pointer;
            background:url(../../../assets/svg/menu.png) no-repeat center;
            -webkit-background-size:14px 16px;
            background-size:14px 16px;
        }
    }
</style>
<style lang="scss">
    @import "css/common/_var.scss";
    @import "css/common/_mixin.scss";
    @import "css/common/_iconFont.scss";
    .navbar{
        .left{
            display: flex;
            align-items: center;
            border:none;
            .el-input{
               height:30px;
               width:auto;
               line-height:initial;
               margin-left:20px;
               input{
                   height:30px;
                   background-color:#f2f4f6;
                   &:focus{
                     border-color:#dcdfe6;
                   }
               }
               .el-input__icon{
                   line-height:30px;
               }
            }
        }
    }
</style>


