<template>
  <div id="banner-edit">
    <Modal v-model="modal.visible" :title="modal.title" footer-hide>
      <VForm :key="modal.visible" :width="400" :label-width="80" :elem="moduleElem" :model="model" :rules="rule" :loading="loading.form" :btn-loading="loading.btn" :submitText="submitText" :resetText="resetText" @on-submit="handleSubmit" @on-click="modal.visible = false" button button-text="取消"></VForm>
      <!-- VForm -->
    </Modal>
  </div>
</template>
<script>
import VForm from "@/views/components/v-form/v-form.vue";
export default {
  name: "Edit",
  components: {
    VForm
  },
  props: {
    model: Object
  },
  data() {
    const validEnabled = (rule, value, callback) => {
      callback();
    };
    return {
      // 模态框属性
      modal: {
        title: "",
        visible: false
      },
      // loading 状态
      loading: {
        btn: false,
        form: false
      },
      submitText: "保存",
      resetText: "取消",
      moduleElem: [
        {
          label: "模块名称",
          prop: "ModuleName",
          labelWidth: 100,
          placeholder: "请输入模块名称，不超过20字"
        },
        {
          label: "状态",
          size: "large",
          prop: "IsEnabled",
          element: "switch",
          labelWidth: 100,
          option: [
            {
              label: "启用",
              slot: "open",
              value: true
            },
            {
              label: "禁用",
              slot: "close",
              value: false
            }
          ]
        }
      ],
      rule: {
        ModuleName: [
          {
            required: true,
            message: "模块名称不能为空",
            trigger: "blur"
          },
          {
            type: "string",
            max: 10,
            message: "不能超过10个字符",
            trigger: "blur"
          }
        ],
        IsEnabled: [
          {
            required: true,
            validator: validEnabled,
            trigger: "change"
          }
        ]
      }
    };
  },
  methods: {
    showModal(name) {
      this.loading.form = true;
      switch (name) {
        case "edit":
          name = "编辑";
          break;
        default:
          name = "添加";
      }
      this.modal = {
        title: name,
        visible: true
      };
      this.loading.form = false;
    },
    handleSubmit() {
      this.loading.btn = true;
      let para = this.model;

      this.ajax
        .post("/CarProducts/UpdateModule", {
          ...para
        })
        .then(res => {
          var result = res.data;

          if (result) {
            this.$Message.success("操作成功");
          } else {
            this.$Message.error("操作失败!");
          }
          this.$emit("on-update", false);
          this.loading.btn = false;
          this.modal.visible = false;
          this.$Modal.remove();
        })
        .catch(() => {
          this.loading.btn = false;
        });
    }
  }
};
</script>