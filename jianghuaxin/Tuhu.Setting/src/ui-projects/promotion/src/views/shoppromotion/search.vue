<template>
<div style="padding:10px;">
    <Row>
    <Col span="6">名称关键字：<Input v-model="search.keywords" style="width:150px;" placeholder="请输入关键词"></Input></Col>
    <Col span="4">面值：<Input v-model="search.discount" style="width:100px;" placeholder=""></Input></Col>
    <Col span="8">创建时间：
        <DatePicker type="date" v-model="search.filterStartDate" show-week-numbers style="width: 150px"></DatePicker> - 
    <DatePicker type="date" v-model="search.filterEndDate" show-week-numbers  style="width: 150px"></DatePicker></Col>
    <Col span="3"><Button type="primary" @click="ok" icon="ios-search">搜索</Button></Col>
    </Row>
</div>
</template>
<script>
export default {
  data () {
    return {
        search: {
            keywords: '',
            discount: '',
            filterStartDate: '',
            filterEndDate: '',
            time: 0
        }
    }
  },
  methods: {
    ok () {
        this.search.time = new Date().getTime();
        this.$emit("filter", this.search)
    }
  }
}
</script>
