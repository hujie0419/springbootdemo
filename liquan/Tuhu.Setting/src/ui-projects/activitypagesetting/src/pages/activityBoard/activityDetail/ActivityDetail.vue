<template>
  <div class="activity-detail">
      <div class="title">详情查看
          <span class="close el-icon-close" @click="closePopup"></span>
      </div>
      <div class="content">
          <div class="row el-row">
            <span class="cate-name el-col-3">类型</span>
            <span class="cate-text el-col-8">{{activity.ActivityTypeChinese}}</span>
            <span class="cate-name el-col-3">负责人</span>
            <span class="cate-text el-col-8">{{activity.Head}}</span>
          </div>

          <div class="row el-row">
            <span class="cate-name el-col-3">创建人</span>
            <span class="cate-text el-col-20">{{activity.CreateBy}}</span>
          </div>
          <div class="row el-row">
            <span class="cate-name el-col-3">活动名称</span>
            <span class="cate-text el-col-20">{{activity.ActivityId}} {{activity.ActivityName}}</span>
          </div>
          <div class="row el-row">
            <span class="cate-name el-col-3">开始日期</span>
            <span class="cate-text el-col-20">{{activity.StartTimeString}}</span>
          </div>
          <div class="row el-row">
            <span class="cate-name el-col-3">结束日期</span>
            <span class="cate-text el-col-20">{{activity.EndTimeString}}</span>
          </div>
          <div class="row el-row">
            <span class="cate-name el-col-3">h5链接</span>
            <span class="cate-text el-col-20">{{h5Url}}</span>
            <el-button type="primary" class="jump-btn" @click="jumpLink">跳转到此页面</el-button>
          </div>
          <div class="row el-row">
            <span class="cate-name el-col-3">小程序链接</span>
            <span class="cate-text el-col-20">{{appletsUrl}}</span>
          </div>
          <div class="row el-row">
            <span class="cate-name el-col-3">活动规则</span>
            <div class="rule-text el-col-20" v-html="activity.ActivityRule || '<br/>'"></div>
          </div>
      </div>
      <div class="bottom">
          <el-button type="primary" plain class="cancle-btn" @click="closePopup">取消</el-button>
      </div>
  </div>
</template>

<script>
let activityH5 = 'https://wx.tuhu.cn/vue/NaActivity/pages/home/index?id=';
let url = location.href;
if (url.indexOf('.tuhu.work') > -1 || url.indexOf('172.') > -1 || url.indexOf('localhost') > -1) {
    activityH5 = 'https://wx.tuhu.work/vue/vueTest/pages/home/index?_project=NaActivity&id=';
}
let activityApplets = '/packages/active/active?id=';

export default {
    name: '',
    props: {
        activity: {
            type: Object,
            required: false
        }
    },
    components: {},

    data () {
        return {

        };
    },

    computed: {
        h5Url() {
            return activityH5 + this.activity.ActivityId;
        },
        appletsUrl() {
            return activityApplets + this.activity.ActivityId;
        }
    },

    watch: {},

    created () {},

    mounted () {
        console.log(this.activity);
    },

    methods: {
        closePopup() {
            this.$emit('cancelChange');
        },
        jumpLink() {
            window.open(this.h5Url);
        }
    }

};
</script>
<style lang='scss'>
.acdetail {
    .th_content-wrap {
        width: 800px;
        margin: 0 auto;
        .th_content {
            border-radius: 5px;
        }
    }
}
.activity-detail {
    .title {
        padding-left: 20px;
        font-weight: bold;
        height: 50px;
        line-height: 50px;
        border-bottom: 1px solid #ddd;
        .close {
            float: right;
            line-height: 50px;
            margin-right: 20px;
            font-weight: bold;
            color: #bbb;
            cursor: pointer;
        }
    }
    .content {
        padding: 20px;
        height: 400px;
        overflow-y: scroll;
        .row {
            margin: 20px auto;
        }
    }
    .cate-name {
        display: inline-block;
        // width: 90px;
        text-align: right;
        font-weight: bold;
        height: 40px;
        line-height: 40px;
    }
    .cate-text {
        // display: inline-block;
        min-width: 100px;
        // width: 100%;
        height: 40px;
        padding: 0 5px;
        line-height: 40px;
        margin-left: 10px;
        border: #ddd 1px solid;
        border-radius: 3px;
        cursor: default;
        white-space: nowrap;
    }
    .rule-text {
        // display: inline-block;
        padding: 5px;
        line-height: 30px;
        margin-left: 10px;
        border: #ddd 1px solid;
        border-radius: 3px;
        cursor: default;
    }
    .jump-btn{
        margin: 10px 0 0 103px;
    }
    .bottom {
        .cancle-btn {
            float: right;
            margin: 10px 30px;
            padding: 10px;
        }
        border-top: 1px solid #ddd;
    }
}
</style>
