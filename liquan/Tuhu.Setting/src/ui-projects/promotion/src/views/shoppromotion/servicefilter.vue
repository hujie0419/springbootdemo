<template>
  <div>
    <Row>
      <Col span="2">类目选择：</Col>
      <Col span="20">
        <CheckboxGroup v-model="filter.CatogryIDs" @on-change="checkAllGroupChange">
          <Checkbox :label="x.PKID" v-bind:key="x.PKID" v-for="x in allServiceCategory">{{x.ServersName}}</Checkbox>
        </CheckboxGroup>
      </Col>
    </Row>
    <br/>
    <Row>
      <Col span="2">关键字：</Col>
      <Col span="4"><Input v-model="filter.ServersName" size="small"></Input></Col>
      <Col span="2" style="text-align:right">PID：</Col>
      <Col span="4"><Input v-model="filter.ProductID" size="small"></Input></Col>
    </Row>
    <br/>
    <Row>
      <Col span="2">价格区间：</Col>
      <Col span="3"><Input v-model="filter.StartDefaultPrice" size="small"></Input></Col>
      <Col span="2">元 &nbsp;&nbsp;&nbsp;&nbsp;&nbsp;~ </Col>
      <Col span="3"><Input v-model="filter.EndDefaultPrice" size="small" style="text-align:left" ></Input></Col>
      <Col span="2">&nbsp;元</Col>
      <Col span="1">&nbsp;</Col>
      <Col span="3"><Button type="primary" @click="search" icon="ios-search">搜索</Button></Col>
    </Row>
    <br/>
  </div>
</template>
<script>
import util from "@/framework/libs/util.js"
export default {
  props: ["filter"],
  data () {
    return {
      allServiceCategory: []
    }
  },
  created () {
    this.loadData()
  },
  methods: {
    loadData () {
      util.ajax.get("/shoppromotion/GetShopServesAllCatogry").then((response) => {
        if (response.status === 200) {
          this.allServiceCategory = response.data.data;
        }
      })
    },
    search () {
      this.$emit('search');
    },
    checkAllGroupChange () {
      this.$emit('checkAllGroupChange');
    }
  }
}
</script>
