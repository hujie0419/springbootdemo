<template>
  <div>
    <Card style="margin:0 auto;width:1000px">
      <Steps :current="current">
        <Step title="选择楼层"></Step>
        <Step title="选择商品"></Step>
        <Step title="编辑广告"></Step>
      </Steps>
    </Card>
    <!-- 步骤条 -->

    <Card style="margin:0 auto;margin-top:20px;width:1000px">
      <Row v-if="firstModal">
        <Form ref="first" :model="floorModel" :rules="floorRules" :loading="loading.form" :label-width="80" style="margin:0 auto;width:80%">
          <FormItem label="楼层" prop="Name">
            <RadioGroup v-model.trim="floorModel.Name" @on-change="handleSwitch(floorModel.Name,null)">
              <Radio v-for="item in categoryData" :key="item.CategoryName" :label="(item.CategoryName)+'|'+(item.DisplayName)">
                <span>{{item.DisplayName}}</span>
              </Radio>
            </RadioGroup>
          </FormItem>
          <FormItem label="状态" prop="IsEnabled">
            <i-switch v-model="floorModel.IsEnabled" size="large">
              <span slot="open">启用</span>
              <span slot="close">禁用</span>
            </i-switch>
          </FormItem>
          <FormItem label="排序" prop="Sort">
            <InputNumber v-model.trim="floorModel.Sort" :max="9999"></InputNumber>
          </FormItem>
          <FormItem>
            <Button type="primary" @click="handleNextSecond">下一步</Button>
            <Button @click="handleReset" style="margin-left:20px">取消</Button>
          </FormItem>
        </Form>
        <Spin v-if="loading.form" size="large" fix></Spin>
      </Row>
      <!-- step one -->

      <Form ref="second" v-if="secondModal" :model="floorModel" :rules="configRule" :loading="loading.form" :label-width="100" style="margin:0 auto;width:80%">
        <code class="tips">自定义商品为选填项，自定义商品排序优先</code>
        <FormItem label="自定义商品1" prop="PID1" style="margin-top:10px">
          <Input v-model="floorModel.PID1" placeholder="请填写商品PID" style="width:200px"></Input>
        </FormItem>
        <FormItem label="自定义商品2" prop="PID2">
          <Input v-model="floorModel.PID2" placeholder="请填写商品PID" style="width:200px"></Input>
        </FormItem>
        <FormItem label="自定义商品3" prop="PID3">
          <Input v-model="floorModel.PID3" placeholder="请填写商品PID" style="width:200px"></Input>
        </FormItem>
        <FormItem label="三级车品" prop="ThreeCategory">
          <CheckboxGroup v-model="clkCategoryData" @on-change="handleSelectCheckbox" style="max-height:400px;overflow:scroll">
            <Checkbox ref="chkCode" v-for="(item, index) in ChildCategoryData" :key="item.CategoryName" :label="(item.CategoryName)+'|'+(item.DisplayName)">
              <span>
                <span>{{item.DisplayName}}</span>
                <InputNumber :id="item.CategoryName" :max="15" :min="0" size="small" :value="numbers[index]"></InputNumber>
              </span>
            </Checkbox>
          </CheckboxGroup>
        </FormItem>

        <FormItem>
          <Button type="primary" @click="handleFloorSubmit(1)">直接保存</Button>
          <Button type="success" @click="handleFloorSubmit(2)" style="margin-left:20px">设置广告</Button>
          <Button @click="handleReset" style="margin-left:20px">上一步</Button>
        </FormItem>
      </Form>
      <!-- setp two -->

      <VForm style="margin:0 auto;width:80%" v-if="thirdModal" :label-width="80" :elem="AdElem" :model="AdModel" :rules="AdRule" :loading="loading.form" :btn-loading="loading.btn" :submitText="submitText" :resetText="resetText" @on-submit="handleSubmit" @on-removeImg="handleRemoveImage" @on-click="handleReset" button button-text="取消"></VForm>
      <!-- VForm -->
    </Card>
  </div>
</template>
<script>
import VForm from "@/views/components/v-form/v-form.vue";
import {
  floorModel,
  floorRule,
  adElem,
  adModel,
  adRule,
  configRules,
  nullModel
} from "./model.js";
export default {
  name: "floorEdit",
  components: {
    VForm
  },
  data() {
    const validPid = (rule, value, callback) => {
      var that = this;
      if (value == "") return callback();

      setTimeout(() => {
        that.ajax
          .post("/CarProducts/CheckPid", {
            pid: value
          })
          .then(res => {
            if (res.data.PID === null) {
              return callback(new Error("无此产品编号"));
            }
            if (res.data.CaseSensitive) {
              return callback(new Error("请跟产品库配置大小写一致"));
            }
            if (!res.data.OnSale || res.data.Stockout) {
              return callback(new Error("此产品下架或缺货"));
            } else {
              callback();
            }
          });
      }, 100);
    };
    const validCategory = (rule, value, callback) => {
      var that = this;
      if (that.floorId > 0 && that.clkFlag) {
        that.floorModel.ThreeCategory = [];
        that.floorModel.ThreeCategory = that.clkCategoryData;
      }

      if (value.length == 0 || that.floorModel.ThreeCategory.length == 0) {
        return callback(new Error("请选择三级车品"));
      } else {
        callback();
      }
    };
    return {
      numbers: [],
      // 当前步骤
      current: 0,

      // 步骤展示
      firstModal: true,
      secondModal: false,
      thirdModal: false,

      //展示产品数量
      showPidCnt: 0,
      // 点选的三级车品
      clkFlag: false,
      clkCategoryData: [],
      // 二级车品选项数据
      categoryData: [],
      // 三级车品选项数据
      ChildCategoryData: [],

      // 编辑楼层参数
      floorId: this.$route.query.FloorId,
      floorModel: floorModel,
      floorRules: floorRule,
      configRule: {
        PID1: [
          {
            trigger: "blur",
            validator: validPid
          }
        ],
        PID2: [
          {
            trigger: "blur",
            validator: validPid
          }
        ],
        PID3: [
          {
            trigger: "blur",
            validator: validPid
          }
        ],
        ThreeCategory: [
          {
            trigger: "blur",
            validator: validCategory
          }
        ]
      },
      // loading 状态
      loading: {
        btn: false,
        form: true
      },
      submitText: "保存",
      resetText: "取消",
      AdElem: adElem,
      AdModel: adModel,
      AdRule: adRule
    };
  },
  mounted() {
    this.floorModel = Object.assign({}, nullModel);
    this.loadCategorys();
    this.loadInitData();

    /**
     * 根据val返回对应的下标
     * @param 数组中的值
     */
    Array.prototype.arrIndex = function(val) {
      for (var i = 0; i < this.length; i++) {
        if (this[i] == val) return i;
      }
      return -1;
    };
    /**
     * 根据val移除数组中的记录
     * @param 数组中的值
     */
    Array.prototype.removeCurEle = function(val) {
      var index = this.arrIndex(val);
      if (index > -1) {
        this.splice(index, 1);
      }
    };
  },
  methods: {
    /**
     * 初始化数据
     */
    loadInitData() {
      if (this.floorId != "" && !isNaN(this.floorId) && this.floorId != 0) {
        this.ajax
          .post("/CarProducts/GetCarProductsFloorContent", {
            floorId: this.floorId
          })
          .then(res => {
            this.loading.form = false;
            if (res.data != null && res.data.Floor != null) {
              this.floorModel = res.data.Floor;

              // 初始化数据后对缓存的模型数据重新赋值
              this.floorModel.PID1 = "";
              this.floorModel.PID2 = "";
              this.floorModel.PID3 = "";
              this.floorModel.ThreeCount = [];
              this.floorModel.FloorConfig = [];
              this.floorModel.ThreeCategory = [];
              this.floorModel.Name =
                this.floorModel.Code + "|" + this.floorModel.Name;

              if (res.data.Config != null && res.data.Config.length > 0) {
                // 将数据库模型转换为前端模型
                res.data.Config.forEach((item, idx) => {
                  if (item.Code != "" && item.Code != null) {
                    this.floorModel.ThreeCategory.push(
                      item.Code + "|" + item.Name
                    );
                    this.floorModel.ThreeCount.push(
                      item.Code + "|" + item.PidCount
                    );
                  }

                  if (item.PIDS != null && this.floorModel.PID1 === "") {
                    this.floorModel.PID1 = item.PIDS;
                    return;
                  }
                  if (item.PIDS != null && this.floorModel.PID2 === "") {
                    this.floorModel.PID2 = item.PIDS;
                    return;
                  }
                  if (item.PIDS != null && this.floorModel.PID3 === "") {
                    this.floorModel.PID3 = item.PIDS;
                    return;
                  }
                });

                // 加载三级车品数据
                this.handleSwitch(this.floorModel.Code, res.data.Config);
              }
            } else {
              this.$Message.error("数据不存在");
              this.$router.push({
                path: "/floor"
              });
            }
          });
      }
    },

    /**
     * 加载二级车品类目
     */
    loadCategorys() {
      this.ajax
        .post("/CarProducts/GetCatalogs", {})
        .then(res => {
          if (res.data.Success) {
            this.categoryData = res.data.Data;
          } else {
            this.$Message.warning("车品类目数据初始化失败");
          }
          this.loading.form = false;
        })
        .catch(res => {
          this.loading.form = false;
          this.$Message.warning("车品类目数据初始化失败");
        });
    },

    /**
     * 进入第二步
     */
    handleNextSecond() {
      this.$nextTick(() => {
        var that = this;
        that.$refs["first"].validate(valid => {
          if (valid) {
            // 步骤条切换
            that.current += 1;

            // 内容展示切换
            that.firstModal = false;
            that.thirdModal = false;
            that.secondModal = true;

            // 第一步保存的 Name 属性是 Code和Name 组合的数据
            that.floorModel.Code = that.floorModel.Name.split("|")[0];
            that.floorModel.Name = that.floorModel.Name.split("|")[1];
            that.floorModel.DisplayName = that.floorModel.Name;
          }
        });
      });
    },

    /**
     * 进入第三步
     */
    handleNextThird() {
      // 步骤条切换
      this.current += 1;

      // 内容展示切换
      this.firstModal = false;
      this.secondModal = false;
      this.thirdModal = true;

      // 加载banner数据
      this.loading.form = true;
      this.ajax
        .post("/CarProducts/GetFloorBanner", { floorId: this.floorId })
        .then(res => {
          this.loading.form = false;
          this.$nextTick(() => {
            if (res.data != null) {
              this.AdModel = res.data;

              this.AdModel.NoLink = decodeURI(this.AdModel.Link);
              this.AdModel.LinkType += "";

              let data = this.AdElem;
              let total = data.length;

              for (let i = 0; i < total; i++) {
                if (data[i].prop === "ImgUrl") {
                  data[i].value = this.AdModel.ImgUrl;
                }
              }
            } else {
              this.AdElem.forEach(key => {
                if (key.prop === "ImgUrl") key.value = "";
              });
            }
          });
        })
        .catch(err => {
          this.$Message.error("获取广告数据错误");
          this.loading.form = false;
        });
    },

    /**
     * 保存车品楼层配置数据
     */
    handleFloorSubmit(type) {
      this.$nextTick(() => {
        var that = this;

        this.$refs["second"].validate(valid => {
          if (valid) {
            // 重置选中个数
            that.showPidCnt = 0;
            that.floorModel.FloorConfig = [];
            // 组装三级车品Code代码
            that.floorModel.ThreeCategory.forEach((item, idx) => {
              that.floorModel.FloorConfig[idx] = {
                PIDS: "",
                IsEnabled: true,
                Code: item.split("|")[0],
                Name: item.split("|")[1]
              };
            });

            // 组装三级车品展示PID数量
            this.$refs["chkCode"].forEach((item, idx) => {
              if (
                item.$el.className != null ||
                item.$el.className != undefined
              ) {
                if (
                  item.$el.className.indexOf("ivu-checkbox-wrapper-checked") > 0
                ) {
                  that.floorModel.FloorConfig.forEach(key => {
                    if (key.Code === item.label.split("|")[0]) {
                      let cnt = item.$el.getElementsByClassName(
                        "ivu-input-number-input"
                      )[0].value;
                      key.PidCount = cnt;
                      that.showPidCnt += Number(cnt);
                    }
                  });
                }
              }
            });
            that.floorModel.FloorConfig.forEach((key, idx) => {
              if (key.PidCount === undefined) {
                that.floorModel.FloorConfig.splice(idx, 1);
              }
            });

            if (that.floorModel.PID1 != "") {
              that.showPidCnt += 1;
              that.floorModel.FloorConfig.push({
                PIDS: that.floorModel.PID1,
                IsEnabled: true,
                PidCount: 0,
                Name: "",
                Code: ""
              });
            }

            if (that.floorModel.PID2 != "") {
              that.showPidCnt += 1;
              that.floorModel.FloorConfig.push({
                PIDS: that.floorModel.PID2,
                IsEnabled: true,
                PidCount: 0,
                Name: "",
                Code: ""
              });
            }

            if (that.floorModel.PID3 != "") {
              that.showPidCnt += 1;
              that.floorModel.FloorConfig.push({
                PIDS: that.floorModel.PID3,
                IsEnabled: true,
                PidCount: 0,
                Name: "",
                Code: ""
              });
            }

            if (that.showPidCnt > 15) {
              this.$Message.error(
                `最多只能展示15个产品，当前选中${that.showPidCnt}个`
              );
              return;
            }
            this.loading.form = true;

            this.ajax
              .post("/CarProducts/SaveFloor", {
                model: JSON.stringify(this.floorModel),
                config: JSON.stringify(this.floorModel.FloorConfig)
              })
              .then(res => {
                if (res.data.Success) {
                  this.loading.form = false;
                  if (type == 1) {
                    this.$Message.success("保存成功,进入列表页");
                    this.$router.push({
                      path: "/floor"
                    });
                  } else if (type == 2) {
                    this.floorId = res.data.Id;
                    this.handleNextThird();
                  }
                } else {
                  this.$Message.error(res.data.Msg);
                  this.loading.form = false;
                }
              })
              .catch(err => {
                this.$Message.error(err);
                this.loading.form = false;
              });
          }
        });
      });
    },

    /**
     * 保存banner
     */
    handleSubmit() {
      this.loading.btn = true;
      this.loading.form = true;
      let para = this.AdModel;

      para.Link = encodeURI(para.NoLink);
      para.FKFloorID = this.floorId;

      this.ajax
        .post("/CarProducts/SaveBanner", {
          ...para
        })
        .then(res => {
          var result = res.data;

          if (result) {
            this.$Message.success("保存成功");
            setTimeout(() => {
              this.$router.push({
                path: "/floor"
              });
            }, 1000);
          } else {
            this.$Message.error("保存成失败!");
          }
          this.loading.btn = false;
          this.loading.form = false;
        })
        .catch(() => {
          this.loading.btn = false;
          this.loading.form = false;
        });
    },

    /**
     * 上一步
     */
    handleReset() {
      if (this.current == 0 || this.current == 2) {
        this.$router.push({
          path: "/floor"
        });
      }
      if (this.current == 1) {
        this.firstModal = true;
        this.secondModal = false;
        this.thirdModal = false;
        this.floorModel.Name =
          this.floorModel.Code + "|" + this.floorModel.Name;
      }
      this.current -= 1;
    },

    /**
     * 根据选择的二级品类 加载三级品类数据
     */
    handleSwitch(code, data) {
      let that = this;
      if (code == "" || code == null) {
        return this.$Message.error("请先选择二级车品");
      }
      this.ajax
        .post("/CarProducts/GetChildCatalogs", {
          code: code.indexOf("|") > 0 ? code.split("|")[0] : code
        })
        .then(res => {
          if (res.data.Success) {
            this.ChildCategoryData = res.data.Data;
          } else {
            this.$Message.warning("车品三级类目初始化失败");
          }
          this.loading.form = false;

          if (this.floorId > 0) {
            this.clkCategoryData = this.floorModel.ThreeCategory;
            this.ChildCategoryData.forEach((item, idx) => {
              let tmpCnt = 1;
              data.forEach((k, v) => {
                if (k.Code == item.CategoryName) {
                  tmpCnt = k.PidCount;
                }
              });
              this.$set(this.numbers, idx, tmpCnt);
            });
          }
        })
        .catch(res => {
          this.$Message.warning("车品三级类目初始化失败");
          this.loading.form = false;
        });
    },

    /**
     * 点选三级车品,不知为何编辑的时候点选三级车品的时候只能在原有的数据中再多绑定一个类目，多选时只保留了最后一次点选的数据
     * 因此单独加此方法将每次选中和非选中的数据记录
     */
    handleSelectCheckbox() {
      // 编辑时有效
      if (this.floorId > 0) {
        let that = this;

        this.$nextTick(() => {
          that.clkFlag = true;
          this.$refs["chkCode"].forEach((item, idx) => {
            let eleClass = item.$el.className;
            if (eleClass != null || eleClass != undefined) {
              // 三级车品选中状态
              let flag = eleClass.indexOf("ivu-checkbox-wrapper-checked") > 0;
              // 选中的三级车品存储的信息
              let label = item.$options.propsData.label;
              // 选择的三级车品在存储临时三级类目数组中的下标位置
              let idxOf = that.clkCategoryData.indexOf(label);
              if (flag) {
                // 不存在添加 存在删除
                if (idxOf < 0) {
                  that.clkCategoryData.push(label);
                }
              } else {
                that.clkCategoryData.removeCurEle(label);
              }
            }
          });
        });
      }
    },

    /**
     * 移除图片
     */
    handleRemoveImage() {
      this.AdModel.ImgUrl = "";
    }
  }
};
</script>
<style lang="less">
.tips {
  margin: 0 100px;
  color: #868686;
}
.ivu-steps {
  margin-left: 15%;
}
.ivu-steps .ivu-steps-head-inner {
  margin: 0 10px !important;
}
.ivu-steps .ivu-steps-title {
  width: 50%;
  display: block !important;
  margin-top: 5px !important;
}
.ivu-checkbox-wrapper {
  width: 80%;
}
.ivu-checkbox-wrapper .ivu-input-wrapper {
  width: auto !important;
}
.ivu-checkbox-wrapper .ivu-input-wrapper .ivu-input {
  width: 100px;
}
</style>
