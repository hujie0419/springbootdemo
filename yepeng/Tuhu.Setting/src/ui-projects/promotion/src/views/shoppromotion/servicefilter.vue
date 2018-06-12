<template>
  <div>
    <Row>
      <Col span="3">类目选择：</Col>
      <Col span="20">
        <CheckboxGroup v-model="filter.CatogryIDs" @on-change="checkAllGroupChange">
          <Checkbox :label="x.PKID" v-bind:key="x.PKID" v-for="x in allServiceCategory">{{x.ServersName}}</Checkbox>
        </CheckboxGroup>
      </Col>
    </Row>
    <br/>
    <Row>
      <Col span="3">价格区间：</Col>
      <Col span="2"><Input v-model="filter.StartDefaultPrice" size="small"></Input></Col>
      <Col span="2">&nbsp;元 &nbsp;~ </Col>
      <Col span="2"><Input v-model="filter.EndDefaultPrice" size="small"></Input></Col>
      <Col span="2">&nbsp;元</Col>
      <Col span="3">名称关键字：</Col>
      <Col span="4"><Input v-model="filter.ServersName" size="small"></Input></Col>
      <Col span="1">&nbsp;</Col>
      <Col span="4"><Button type="primary" @click="search" icon="ios-search">搜索</Button></Col>
    </Row>
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
