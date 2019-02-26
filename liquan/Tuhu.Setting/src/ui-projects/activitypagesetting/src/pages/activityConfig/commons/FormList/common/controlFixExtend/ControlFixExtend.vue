<script>
import ControlExtend from '../controlExtend/ControlExtend';

export default {
    extends: ControlExtend,
    props: {
        config: { // 父配置项
            type: Object
        }
    },
    methods: {
        /**
         * 设置失去焦点主组件的值
         */
        setParentIsBlur() {
            let config = this.config;
            if (config) {
                let con = this.formModel && this.formModel.get(config.controlName);
                if (con) {
                    con.isBlur = true;
                }
            }
        }
    }
};
</script>
