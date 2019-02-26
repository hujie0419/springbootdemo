<template>
  <div class="content"  @click="hideSelect">
    <div class="top">
      <p class="title">{{partName}}</p>
      <p class="log" @click="checkLog">修改日志</p>
    </div>
    <div class="config-tab">
      <span class="active" @click="switchConfigTab">基础配置</span>
      <!-- <span @click="switchConfigTab">特殊车型配置</span> -->
    </div>
    <div class="priority" v-if="oil">
      <div class="priority-title" ref="priorityTitle">
        <span class="active" @click="switchTab">矿物油</span>
        <span @click="switchTab">半合成</span>
        <span @click="switchTab">全合成</span>
        <span></span>
      </div>
      <selected-card class="card oil-card show" :isOil="true" :maintainList.sync="maintainList1" :brandData.sync="brandData" :category.sync="category" @selectOption="selectOption" @changeRecommend="changeRecommend"></selected-card>
      <selected-card class="card oil-card" :isOil="true" :maintainList.sync="maintainList2" :brandData.sync="brandData" :category.sync="category" @selectOption="selectOption" @changeRecommend="changeRecommend"></selected-card>
      <selected-card class="card oil-card" :isOil="true" :maintainList.sync="maintainList3" :brandData.sync="brandData" :category.sync="category" @selectOption="selectOption" @changeRecommend="changeRecommend"></selected-card>
    </div>
    <selected-card v-else class="card selected-card" :maintainList.sync="maintainList" :brandData.sync="brandData" :category.sync="category" :class="{'show': !oil}" @selectOption="selectOption" @changeRecommend="changeRecommend"></selected-card>
    <div class="save-btn" :class="{gray: grayBtn}" @click="saveRecommend" ref="btn">保存</div>
    <hr>
    <div class="notes" @click="editNotesOnly(false)">
      <div class="notes-title">备注：
        <span @click.stop="editNotes(false)"></span>
      </div>
      <textarea
        ref="noteTextarea"
        spellcheck="false"
        rows="10"
        readonly
        class="notes-content"
        placeholder="请输入备注信息~"
        v-model="notesContent"></textarea>
      <div class="btn-group">
        <div class="cancle-btn btn" ref="cancleBtn" @click.stop="editNotes(false)">取消</div>
        <div class="ok-btn btn" ref="okBtn" @click.stop="changeNotes">确认</div>
      </div>
    </div>
  </div>
</template>
<script>
/* eslint-disable max-lines */
import Vue from 'vue';
import {Picker, Popup} from 'tiger-ui';

import SelectedCard from './components/SelectedCard';
import MrAlert from '../../components/MrAlert';
import Loading from '../../components/loading/loading';
[Picker, Popup].forEach(item => Vue.use(item));
export default {
  data() {
    return {
      notesContent: '',
      oldNotesContent: '',
      brandData: [],
      category: '',
      oil: false,
      partName: '',
      savedData: [],
      showAlert: false,
      alertText: '',
      grayBtn: false,
      showLoading: false,
      timer: null,
      emptyBrand: false,
      maintainList: [],
      maintainList1: [],
      maintainList2: [],
      maintainList3: []
    };
  },
  mounted() {
    if (!this.notesContent) {
      var element = this.$refs.noteTextarea;
      element.classList.add('has-border');
    }
  },
  created() {
    // 获取当前保养常规项目列表
    this.getMaintainList(this.$route.query.category);
    this.getBrandData(this.$route.query.category);
    this.getNotes(this.$route.query.category);
    // 移动端和PC端切换时，关闭各自的下拉框/picker(setTimeout防抖)
    window.onresize = () => {
      clearTimeout(this.timer);
      this.timer = setTimeout(() => {
        if (document.body.clientWidth <= 600) {
          this.hideSelect();
        } else {
          this.$tPicker.closePopup();
        }
      }, 300);
    };
  },
  watch: {
    $route(newRouter, oldRouter) {
      if (newRouter) {
        this.getBrandData(newRouter.query.category);
        this.getNotes(newRouter.query.category);
      }
    }
    // notesContent(newVal) {
    //   if (!newVal) {
    //     this.$refs.noteTextarea.classList.add('has-border');
    //   } else {
    //     this.$refs.noteTextarea.classList.remove('has-border');
    //   }
    // }
  },
  methods: {
    /**
     * 查看日志
     */
    checkLog() {
      window.open('https://parts.tuhu.cn/Log/baoyangoprlog');
    },
    /**
     * 获取当前保养常规项目列表
     *
     * @param {string} category - 保养类别.
     */
    getMaintainList(category) {
      const params = category && JSON.parse(category);
      //   this.$tPopup.popupData(Loading, {
      //     alignCla: 'fullScreen', // ''|'bottom'|'top'|'fullScreen'|'centerMiddle',
      //     transitionCls: 't_scale' // ''|'t_toUp'|'t_scale'|'t_toBottom'|'t_toLeft';
      //   });
      this.$http.get('/BaoYang/GetProductPrioritySettings', {
        apiServer: 'apiServer',
        params: {partName: category && params.PartName},
        cacheData: false
      }).subscribe((res) => {
        this.$tPopup.closePopup();
        this.maintainList = res && res.data;
        if (this.oil) {
          this.maintainList.forEach((item) => {
            switch (item.PriorityType) {
              case '矿物油':
                this.maintainList1.push(item);
                break;
              case '半合成':
                this.maintainList2.push(item);
                break;
              case '全合成':
                this.maintainList3.push(item);
                break;
            }
          });
        }
      });
    },
    /**
     * 获取品牌信息
     *
     * @param {string} category - 保养类别.
     * @returns {*}
     */
    getBrandData(category) {
      const params = category && JSON.parse(category);
      if (!params) {
        return false;
      }
      this.oil = params.Category === 'Oil';
      this.category = params.Category;
      this.partName = params.PartName;
      this.$http.get('/Product/GetProductBrand', {
        apiServer: 'apiServer',
        params: {category: this.category},
        cacheData: {
          cacheKey: '/Product/GetProductBrand/' + this.category
        }
      }).subscribe((res) => {
        this.brandData = res.data.length ? res.data : ['暂无数据'];
      });
    },
    /**
     * 获取备注信息
     *
     * @param {string} category - 保养类别.
     * @returns {*}
     */
    getNotes(category) {
      const params = category && JSON.parse(category);
      if (!params) {
        return false;
      }
      this.partName = params.PartName;
      this.$http.get('/BaoYang/GetProductPrioritySettingDescribeConfig/', {
        apiServer: 'apiServer',
        params: {partName: this.partName},
        cacheData: {
          cacheKey: '/BaoYang/GetProductPrioritySettingDescribeConfig/' + this.partName
        }
      }).subscribe((res) => {
        this.oldNotesContent = (res && res.Data && res.Data.Value) || '';
        this.notesContent = this.oldNotesContent;
      });
    },
    /**
     * 标签页切换效果
     * @param {Event} e 事件对象
     */
    switchConfigTab(e) {
      const element = [].slice.call(e.target.parentNode.children);
      element.forEach(item => {
        if (item === e.target) {
          item.classList.add('active');
        } else {
          item.classList.remove('active');
        }
      });
    },
    /**
     * 机油种类标签页切换（显示/隐藏对应的selected-card）
     * @param {Event} e 事件对象
     */
    switchTab(e) {
      const element = [].slice.call(e.target.parentNode.children);
      this.switchConfigTab(e);
      const index = element.indexOf(e.target);
      const oilCard = document.querySelectorAll('.oil-card');
      oilCard.forEach((cardItem, i) => {
        if (index === i) {
          cardItem.classList.add('show');
        } else {
          cardItem.classList.remove('show');
        }
      });
    },
    /**
     * 隐藏下拉框（点击空白处 & 切换到移动端）
     * @param {Event} e 事件对象
     */
    hideSelect(e) {
      const element = document.querySelector('.option-box.show');
      element && element.classList.remove('show');
      element && element.parentNode.children[2].classList.remove('top');
    },
    /**
     * 编辑/保存备注信息
     *
     * @param {Boolean} isChanged 是否提交更改
     */
    editNotes(isChanged) {
      isChanged = isChanged || false;
      const element = this.$refs.noteTextarea;
      const okBtn = this.$refs.okBtn;
      const cancleBtn = this.$refs.cancleBtn;
      element.getAttribute('readonly') ? element.removeAttribute('readOnly'): element.setAttribute('readOnly', 'true');
      element.classList.toggle('edit');
      okBtn.classList.toggle('show');
      cancleBtn.classList.toggle('show');
      !isChanged && (this.notesContent = this.oldNotesContent);
    },
    editNotesOnly() {
      const element = this.$refs.noteTextarea;
      const okBtn = this.$refs.okBtn;
      const cancleBtn = this.$refs.cancleBtn;
      element.removeAttribute('readOnly');
      element.classList.add('edit');
      okBtn.classList.add('show');
      cancleBtn.classList.add('show');
    },
    changeNotes() {
      this.editNotes(true);
      if (this.notesContent === this.oldNotesContent) {
        return false;
      }
      // 打开loading弹框
      this.$tPopup.popupData(Loading, {
        alignCla: 'fullScreen', // ''|'bottom'|'top'|'fullScreen'|'centerMiddle',
        transitionCls: 't_scale' // ''|'t_toUp'|'t_scale'|'t_toBottom'|'t_toLeft';
      });
      this.$http.post('/BaoYang/SaveProductPrioritySettingDescribeConfig', {
        apiServer: 'apiServer',
        data: JSON.stringify({
          PartName: this.partName,
          Value: this.notesContent
        })
      }).subscribe((res) => {
        const param = res.Status ? '提交成功' : '提交失败';
        setTimeout(() => {
          this.getNotes(this.$route.query.category);
          this.$tPopup.closePopup();
          this.showHintAlert(param);
        }, 1500);
      });
    },
    /**
     * 点击系列后，判断是否重复
     *
     * @param {HTMLElement} element - input元素
     * @param {string} selectData - 包含品牌和系列的对象
     */
    selectOption(element, selectData) {
      selectData = JSON.stringify(selectData);
      // 多个标签页时，只判断当前标签页
      const parent = this.oil ? document.querySelector('.card.show') : document.querySelector('.selected-card');
      let recommendData = this.getRecommendData(parent);
      if (recommendData.indexOf(selectData) !== recommendData.lastIndexOf(selectData)) {
        element.value='';
        this.showHintAlert('已有相同系列');
      }
    },
    /**
     * 获取配置的推荐信息
     *
     * @param {HTMLElement} parent - 要获取信息的card元素
     * @returns {object}
     */
    getRecommendData(parent) {
      this.emptyBrand = false;
      let recommendData = [];
      const itemArray = [].slice.call(parent.querySelectorAll('.recommend'));
      itemArray.forEach(element => {
        const brand = element.querySelectorAll('.select')[0].children[0].value;
        const series = element.querySelectorAll('.select')[1].children[0].value;
        const object = {brand: brand, series: series};
        if (brand && brand.length) {
          recommendData.push(JSON.stringify(object));
        } else {
          this.emptyBrand = true;
        }
      });
      return recommendData;
    },
    /**
     * 删除/添加优先级时，保存按钮的显示效果
     *
     * @param {'add'|'delete'} manager - add/delete
     */
    changeRecommend(manager) {
    //   if (manager === 'delete') {
    //     // 多个标签页时，所有标签页都没有优先级，按钮才置灰
    //     if (this.oil) {
    //       const cardList = [].slice.call(document.querySelectorAll('.oil-card'));
    //       this.grayBtn = cardList.every((item, index) => {
    //         return !item.querySelectorAll('.recommend').length;
    //       });
    //     } else {
    //       const selectedCard = document.querySelector('.selected-card');
    //       this.grayBtn = !selectedCard.querySelectorAll('.recommend').length;
    //     }
    //   } else {
    //     this.grayBtn = false;
    //   }
    },
    /**
     * 保存按钮的click事件处理程序
     * @return {boolean}
     */
    saveRecommend() {
      // 清空上次提交的优先级信息
      this.savedData = [];
      let cardList = [];
      let flag;
      // 如果有多个标签页，依次获取每个标签页的优先级信息，并格式化
      if (this.oil) {
        cardList = [].slice.call(document.querySelectorAll('.oil-card'));
        flag = cardList.every((item, index) => {
          const _flag = this.savedDataFun(item, this.$refs.priorityTitle.children[index].innerText);
          return _flag;
        });
        if (!flag) {
          return false;
        }
      } else {
        cardList = document.querySelector('.selected-card');
        flag = this.savedDataFun(cardList);
        if (!flag) {
          return false;
        }
      }
      //   if (!this.savedData.length) {
      //     this.showHintAlert('未配置优先级');
      //     return false;
      //   }
      // 打开loading弹框
      this.$tPopup.popupData(Loading, {
        alignCla: 'fullScreen', // ''|'bottom'|'top'|'fullScreen'|'centerMiddle',
        transitionCls: 't_scale' // ''|'t_toUp'|'t_scale'|'t_toBottom'|'t_toLeft';
      });
      this.$http.post('/BaoYang/SaveProductPrioritySettings', {
        apiServer: 'apiServer',
        data: this.savedData
      }).subscribe((res) => {
        this.$tPopup.closePopup();
        const param = res.status ? '提交成功' : '提交失败';
        this.showHintAlert(param);
      });
    },
    /**
     * 提示信息弹框
     * @param {string} text 提示文字
     */
    showHintAlert(text) {
      this.$tPopup.popupData(MrAlert, {
        props: {
          text: text
        },
        wrapCla: 'text-alert',
        alignCla: 'centerMiddle', // ''|'bottom'|'top'|'fullScreen'|'centerMiddle',
        transitionCls: 't_scale' // ''|'t_toUp'|'t_scale'|'t_toBottom'|'t_toLeft';
      });
      setTimeout(() => {
        this.$tPopup.closePopup();
      }, 1500);
    },
    /**
     * 格式化配置的优先级信息
     *
     * @param {HTMLElement} card - 目标card元素
     * @param {*} priorityType - 机油类型
     * @returns {Boolean}
     */
    savedDataFun(card, priorityType) {
      priorityType = priorityType || '';
      const recommendData = this.getRecommendData(card);
      let flag = recommendData.every((item, index) => {
        return index === recommendData.lastIndexOf(item);
      });
      if (!flag) {
        this.showHintAlert('品牌信息重复，请修改后提交');
        return false;
      }
      if (this.emptyBrand) {
        this.showHintAlert('信息不完整');
        return false;
      }
      recommendData.forEach((item, index) => {
        item = JSON.parse(item);
        this.savedData.push({
          'PartName': this.partName,
          'PriorityType': priorityType,
          'Brand': item.brand,
          'Series': item.series,
          'Priority': index+1
        });
      });
      return true;
    }
  },
  components: {
    'selected-card': SelectedCard,
    'mr-alert': MrAlert,
    'loading': Loading
  }
};
</script>
<style lang="scss" scoped src="./Content1.scss">
</style>
<style lang="scss">
@import "css/common/_var.scss";
@media screen and (max-width: $phoneWidth) {
.mr_main-container .app-main{
  padding: 0 !important;
}
}
</style>

