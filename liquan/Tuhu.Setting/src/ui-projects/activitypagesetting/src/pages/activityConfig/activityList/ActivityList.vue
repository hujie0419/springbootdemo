<template>
    <div class="list-pages">
        <query-condition @submit="changeCondition"></query-condition>
        <page-list :formModelValue.sync="formModelValue"></page-list>
    </div>
</template>

<script>
import FormGroup from '../commons/formGroup/FormGroup';
import PageList from './pageList/PageList';
import QueryCondition from './queryCondition/QueryCondition';

export default {
    components: {
        QueryCondition,
        PageList
    },
    data() {
        return {
            formModelValue: null
        };
    },
    methods: {
        changeCondition(formModel) {
            this.formModelValue = formModel;
        }
    }
};
</script>

<style lang='scss'>
    @import "css/common/_var.scss";
    @import "css/common/_mixin.scss";
    @import "css/common/_iconFont.scss";
</style>
