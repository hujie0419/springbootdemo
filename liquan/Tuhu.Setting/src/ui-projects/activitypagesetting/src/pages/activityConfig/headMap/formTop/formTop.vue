<template>
    <form-group @formInit="formInit" :form-config='formConfig' :defaultData="defaultData"></form-group>
</template>
<script>
import { formConfig } from '../config/SeperateCar.config.js';
import FormGroup from '../../commons/formGroup/FormGroup';
export default {
    data() {
        return {
            formConfig: formConfig()
        };
    },
    props: {
        defaultData: {
            type: Object
        }
    },
    components: {
        FormGroup
    },
    methods: {
        formInit(formModel) {
            this.formModel = formModel;
            this.$emit('dataChange', this.formModel);
        }
    }
};
</script>
