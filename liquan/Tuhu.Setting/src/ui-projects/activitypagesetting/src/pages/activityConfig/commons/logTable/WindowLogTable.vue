<template>
    <div class="window-logBox">
        <window-box @cancel="cancel" @submit="cancel" :title="'日志'" :showCancel="false" :submitText="'关闭'">
            <div class="window-log-wrap">
                <log-table :referId="referId"></log-table>
            </div>
        </window-box>
    </div>
</template>

<script>
import WindowBox from '../../../../components/Layout/windowBox/WindowBox';
import LogTable from './LogTable';
export default {
    props: {
        referId: {
            type: String
        }
    },
    components: {
        WindowBox,
        LogTable
    },
    mounted() {
    },
    methods: {
        cancel(evt) {
            this.$emit('cancelChange', evt);
        }
    }
};
</script>

<style lang="scss">
.ths_popup {
    &.log_popup {
        .th_content-wrap{
            width: 88%;
            min-width: 800px;
            max-width: 90%;
            min-height: 200px;
            overflow-y: auto;
        }
        .window-log-wrap{
            padding: 20px;
        }
    }
}
</style>
