<template>
  <div>
    <div class="logo-container">
      <!-- <img src="../../../assets/svg/huTIpv1.png" /> -->
      <img src="https://res.tuhu.org/managementUI/navigationUI/mnimg/logo.png" />
      <span class="logo">{{$env.systemName}}</span>
    </div>
    <el-menu :unique-opened='true' mode="vertical"  :default-active="$route.path" class="sidebars">
      <template v-for="(item, index) in permissionRoutes" v-if="!item.hidden" >
        <el-submenu :index="item.name" v-if="!item.noDropdown" :key="index">
          <template slot="title">
            <i class="el-icon-location"></i>
            <!-- <Icon :icon-name="item.icon" :size="18"></Icon> -->
            <span>{{item.name}}</span>
          </template>
          <router-link v-for="child in item.children" :key="child.path" v-if="!child.hidden" class="title-link" :to="item.path+'/'+child.path"  @click.native="fixSidebar()">
            <el-menu-item :index="item.path+'/'+child.path"><span>{{child.name}}</span>
            </el-menu-item>
          </router-link>
        </el-submenu>
        <router-link v-if="item.noDropdown&&item.children.length>0"  :to="item.path+'/'+item.children[0].path" :key="index">
            <el-menu-item :index="item.path+'/'+item.children[0].path">
                <Icon :icon-name="item.icon" :size="14"></Icon>{{item.children[0].name}}
            </el-menu-item>
        </router-link>
      </template>
    </el-menu>
  </div>
</template>

<script>
// import permissionRoutes from "store/permission";
export default {
    name: 'Sidebar',
    data() {
        return {
            permissionRoutes: [
                {
                    name: '活动页配置',
                    path: '/activity',
                    children: [{ name: '活动列表', path: 'activityConfig' }]
                },
                {
                    name: '活动页看板',
                    path: '/activity',
                    children: [{ name: '活动页看板', path: 'activityBoard' }]
                }
            ]
        };
    },
    methods: {
        fixSidebar() {
            this.$emit('fixSideEvent');
        }
    }
};
</script>

<style rel="stylesheet/scss" lang="scss" scoped>
@import "css/common/_var.scss";
@import "css/common/_mixin.scss";
@import "css/common/_iconFont.scss";

// .icon {
//   margin: 0 12px 0 1px;
// }
// .hideSidebar .title-link {
//   display: inline-block;
//   padding-left: 10px;
// }
.sidebar-container{
  display: flex;
  flex-direction: column;
  height: 100%;
  background:#343a40;
  .logo-container{
    display: flex;
    justify-content: center;
    align-items: center;
    height:58px;
    border-bottom:1px solid #4b545c;
    .logo{
      color:#ffffff;
      font-size:16px;
      white-space: nowrap;
    }
  }
  .el-menu {
    flex: 1;
    overflow-y:auto;
    padding:8px 8px 0;
  }
}
</style>
<style lang="scss">
@import "css/common/_var.scss";
@import "css/common/_mixin.scss";
@import "css/common/_iconFont.scss";
.el-menu-item,
.el-submenu__title {
  display: flex;
  align-items: center;
}
.hideSidebar{
    .sidebar-wrapper{
      width:65px;
      .el-submenu__icon-arrow{
        right:-20px;
      }
      .el-submenu__title{
        span{
          opacity:0 !important;
        }
      }
      .el-menu-item{
        span{
          opacity:0 !important;
        }
      }
      &.hoverSidebar{
        width:230px !important;
        .el-submenu__icon-arrow{
          right:20px;
        }
        .el-submenu__title{
          span{
            opacity:1 !important;
          }
        }
        .el-menu-item{
          span{
            opacity:1 !important;
          }
        }
      }
    }
}
.sidebar-wrapper{
  width:230px;
  .el-submenu__icon-arrow{
    transition: all .28s ease-out;
    right:20px;
  }
}
.sidebars{
  background:#343a40;
  border-right:none;
  .el-submenu{
    .el-submenu__title{
      padding-left:16px !important;
      padding-right:16px;
      height:40px;
      line-height:initial;
      margin-bottom:4px;
      border-radius:4px;
      color:#c2c7d0;
      font-size:16px;
      span{
        flex: 1;
        opacity:1;
        line-height:40px;
        transition: all .28s ease-out;
        margin-right:30px;
        white-space: nowrap;
        overflow: hidden;
        text-overflow:ellipsis;
      }
      i{
        color:#c2c7d0;
      }
      .el-submenu__icon-arrow{
        font-weight:bold;
        margin-top:-6px;
      }
      &:hover{
        background-color:rgba(255,255,255,.1);
        color:#ffffff;
        i{
          color:#ffffff;
        }
        span{
          color:#ffffff;
        }
      }
    }
    .el-menu--inline{
      background:#343a40;
      .title-link{
        .el-menu-item{
          font-size:16px;
          background:#343a40;
          height:40px;
          padding:0;
          padding-left:45px !important;
          padding-right:0;
          border-radius:4px;
          color:#c2c7d0;
          min-width:auto;
          span{
            flex: 1;
            line-height:40px;
            opacity:1;
            transition: all .28s ease-out;
            margin-right:30px;
            white-space: nowrap;
            overflow: hidden;
            text-overflow:ellipsis;
          }
          &:hover{
            background-color:rgba(255,255,255,.1);
            color:#ffffff;
          }
          &.is-active{
            background-color:rgba(255,255,255,.9);
          }
        }
      }
    }
    &.is-opened{
      .el-submenu__title{
        background-color:rgba(255,255,255,.1);
        color:#ffffff;
        i{
          color:#ffffff;
        }
      }
    }
    &.is-active{
      .el-submenu__title{
        background-color:#007bff;
        color:#ffffff;
        i{
          color:#ffffff;
        }
      }
      .el-menu--inline{
        .is-active{
          color:#343a40;
          span{
            color:#343a40;
          }
          &:hover{
            color:#343a40;
          }
        }
      }
    }
  }
}
// .logo-container {
//   height: 50px;
//   background: #424a57;
//   color: #fff;
//   width: 100%;
//   @include flex;
//   @include flex-justify-center;
//   @include flex-align-center;
//   .logo {
//     overflow: hidden;
//     transition: all 0.5s linear;
//   }
// }
// .hideSidebar .logo-container .logo {
//   width: 0;
// }
// .sidebar-wrapper:hover .logo {
//   width: auto;
// }
</style>

