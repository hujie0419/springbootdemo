<template>
  <div id="banner-edit">
    <Modal v-model="modal.visible" :title="modal.title" footer-hide>
      <VForm :key="modal.visible" :width="400" :label-width="80" :elem="bannerElem" :model="model" :rules="rule" :loading="loading.form" :btn-loading="loading.btn" :submitText="submitText" :resetText="resetText" @on-submit="handleSubmit" @on-removeImg="handleRemoveImage" @on-click="modal.visible = false" button button-text="取消"></VForm>
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
    // 自定义排序验证
    const validSort = (rule, value, callback) => {
      if (!value) {
        return callback(new Error("排序不可为空"));
      }
      if (!Number.isInteger(value)) {
        return callback(new Error("只支持数字类型"));
      }
      callback();
    };
    // 自定义状态验证
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
      bannerElem: [
        {
          label: "坑位名称",
          prop: "Name",
          labelWidth: 100,
          placeholder: "请输入坑位名称，不超过10个字符"
        },
        {
          label: "坑位图片",
          prop: "ImgUrl",
          labelWidth: 100,
          element: "upload",
          value: "",
          width: "max-width:300px",
          action: "/CarProducts/UploadImage?type=image"
        },
        {
          labelWidth: 100,
          label: "目的页类型",
          prop: "LinkType",
          element: "select",
          option: [
            {
              label: "H5活动页",
              value: "1"
            },
            {
              label: "车品详情页",
              value: "2"
            },
            {
              label: "搜索结果页",
              value: "3"
            }
          ]
        },
        {
          label: "未编码链接",
          prop: "NoLink",
          labelWidth: 100,
          placeholder: "请输入跳转链接"
        },
        {
          label: "已编码链接",
          prop: "Link",
          disabled: "disabled",
          labelWidth: 100,
          placeholder: "请输入跳转链接"
        },
        {
          label: "开始时间",
          prop: "StartTime",
          labelWidth: 100,
          element: "date",
          type: "datetime",
          elemWidth: "width:300px",
          format: "yyyy-MM-dd HH:mm:ss"
        },
        {
          label: "结束时间",
          prop: "EndTime",
          labelWidth: 100,
          element: "date",
          type: "datetime",
          elemWidth: "width:300px",
          format: "yyyy-MM-dd HH:mm:ss"
        },
        {
          label: "排序",
          prop: "Sort",
          labelWidth: 100,
          number: true,
          element: "number"
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
        Name: [
          {
            required: true,
            message: "坑位名称不能为空",
            trigger: "blur"
          },
          {
            type: "string",
            max: 10,
            message: "不能超过10个字符",
            trigger: "blur"
          }
        ],
        LinkType: [
          {
            required: true,
            message: "请选择目的页类型",
            trigger: "change"
          }
        ],
        NoLink: [
          {
            required: true,
            message: "跳转链接不能为空",
            trigger: "blur"
          }
        ],
        ImgUrl: [
          {
            required: true,
            message: "图片不能为空",
            trigger: "blur"
          }
        ],
        Sort: [
          {
            required: true,
            validator: validSort,
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
    getPatch() {
      this.$nextTick(() => {
        let data = this.bannerElem;
        let total = data.length;
        
        for (let i = 0; i < total; i++) {
          if (data[i].prop === "ImgUrl") {
            data[i].value = this.model.ImgUrl;
          }
        }
      });
    },
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

    /**
     * 提交保存
     */
    handleSubmit() {
      this.loading.btn = true;
      let para = this.model;
      para.Link = encodeURI(para.NoLink);
      this.bannerElem.forEach(p => {
        if (p.prop === "ImgUrl") para.ImgUrl = p.value;
      });

      this.ajax
        .post("/CarProducts/SaveBanner", {
          ...para
        })
        .then(res => {
          var result = res.data;

          if (result.Success) {
            this.$Message.success("操作成功");
          } else {
            setTimeout(() => {
              this.$Message.error(result.Msg);
            }, 500);
          }
          this.$emit("on-update", false);
          this.loading.btn = false;
          this.modal.visible = false;
          this.$Modal.remove();
        })
        .catch(() => {
          this.loading.btn = false;
        });
    },

    /**
     * 移除图片
     */
    handleRemoveImage() {
      this.model.ImgUrl = "";
    }
  }
};
</script>