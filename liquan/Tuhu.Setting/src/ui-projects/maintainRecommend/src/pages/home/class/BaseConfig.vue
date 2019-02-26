<template>
  <div class="baseConfig">
    <div class="priority" v-if="oil">
      <ul class="priority-title">
        <li @click="select($event,1)">
          <span class="title">机油等级
            <span class="select">{{oilGrade.select[0] ? '('+oilGrade.select[0].text+')':''}}</span></span>
          <div class="oilList">
            <ul >
              <li 
                v-for="(item,index) in oilGrade.list" 
                :key="index" 
                @click="setChangeOil('oilGrade',item)" 
                :class="{active:oilGrade.select[0] && oilGrade.select[0].text === item.text}">
                <div>{{item.text}}</div>
              </li>
            </ul>
          </div>
        </li>
        <li @click="select">
          <span class="title">机油粘度
            <span class="select">{{oilViscosity.select[0] ? '('+oilViscosity.select[0].text+')':''}}</span>
          </span>
          <div class="oilList">
            <ul >
              <li 
                v-for="(item,index) in oilViscosity.list" 
                :key="index" 
                @click="setChangeOil('oilViscosity',item)" 
                :class="{active:oilViscosity.select[0] && oilViscosity.select[0].text === item.text}">
                <div>{{item.text}}</div>
              </li>
            </ul>
          </div>
        </li>
      </ul>
    </div>
    <div v-if="urlCategory.category !== 'Antifreeze'">
      <sort-table ref="sortTable" :draggable="draggable" :tableData="maintainList"></sort-table>
      <ul class="save-btn">
        <li class="orderBy" v-if="!draggable" @click="draggable = true">排序</li>
        <!-- <li class="refesh" v-if="!draggable">刷新缓存</li> -->
        <li class="save" v-if="draggable" @click="save">保存</li>
        <li class="cancel" v-if="draggable" @click="cancel">取消</li>
        <li class="save phone" v-if="draggable" @click="save">保存</li>
      </ul>
    </div>
    <div v-else class="tips">
      防冻液推荐配置请查看特殊车型配置
    </div>
    
    <!-- 备注 -->
    <div class="notes" @click="editNotesOnly(false)">
      <div class="notes-title">备注：
        <span @click.stop="editNotes(false)"></span>
      </div>
      <textarea
        ref="noteTextarea"
        spellcheck="false"
        rows="10"
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
import SortTable from './../components/SortTable';
import { Observable } from 'rxjs';
import 'rxjs/add/observable/forkJoin';
export default {
  props: ['urlCategory'],
  data() {
    return {
      oil: false, // 是否为机油
      maintainList: [], // 保养项目列表
      dataList: [], // 机油 接口数据
      oilGrade: { // 机油等级数据
        title: '机油等级',
        list: [],
        select: [{}]
      },
      oilViscosity: { // 机油粘度数据
        title: '机油粘度',
        list: [],
        select: [{}]
      },
      draggable: false, // 可拖拽状态
      notesContent: '', // 备注信息
      oldNotesContent: '' // 老备注信息
    };
  },
  mounted() {
    if (!this.notesContent) {
      var element = this.$refs.noteTextarea;
      element.classList.add('has-border');
    }
  },
  created() {
    this.getData();
  },
  methods: {
    /**
     * 移动端-打开picker
     *  @param {string} e - 当前触发的dom.
     *  @param {string} flag - 0：等级；1：粘度
     */
    select(e, flag) {
      let dom = e.currentTarget.children[1];
      let style = window.getComputedStyle(dom, null);
      let selectData = flag ? this.oilGrade : this.oilViscosity;
      // 移动端
      if (style.display === 'block') {
        return;
      }
      this.$tPicker.picker({props: {dataLists: [selectData.list], nameText: '请选择'+selectData.title}}).then(data => {
        this.setChangeOil(flag ? 'oilGrade' :'oilViscosity', data.data[0]);
      });
    },
    /**
     * 获取机油保养常规项目列表
     * @param {string} data - 机油保养数据
     */
    getOilData(data) {
      this.$set(this.oilGrade, 'list', []);
      this.$set(this.oilViscosity, 'list', []);
      data.forEach((item, index) => {
        switch (item.Grade) {
          case '全合成':
            this.oilGrade.list.splice(0, 0, {text: item.Grade, value: item.Grade});
            break;
          case '半合成':
            if (this.oilGrade.list[0] && this.oilGrade.list[0].text === '全合成') {
              this.oilGrade.list.splice(1, 0, {text: item.Grade, value: item.Grade});
            } else {
              this.oilGrade.list.splice(0, 0, {text: item.Grade, value: item.Grade});
            }
            break;
          default:
            this.oilGrade.list.push({text: item.Grade, value: item.Grade});
            break;
        }

        !index && item.Viscosities && item.Viscosities.forEach((item2, index2) => {
          !index2 && (this.maintainList = item2.Detaileds);
          this.oilViscosity.list.push({text: item2.Viscosity, value: item2.Viscosity});
        });
      });
      this.$set(this.oilGrade.select, '0', this.oilGrade.list[0]);
      this.$set(this.oilViscosity.select, '0', this.oilViscosity.list[0]);
    },
    setChangeOil(type, selectItem) {
      if (this[type].select[0] === selectItem) return;
      if (!this.draggable) {
        this.changeOil(type, selectItem);
        return;
      }
      this.$confirm('您编辑的内容尚未保存，确定跳转到其他页面？', '操作确认', {
        confirmButtonText: '确定',
        cancelButtonText: '取消',
        type: 'info',
        closeOnClickModal: false
      }).then(() => {
        this.changeOil(type, selectItem);
      }).catch(() => {});
    },
    /**
     * 标签页切换效果
     *  @param {Event} type 选择的类型
     *  @param {Event} selectItem 当前选择item
     */
    changeOil(type, selectItem) {
      if (type === 'oilGrade') {
        this.$set(this['oilGrade'].select, '0', selectItem);
        let Grade = this.dataList.filter(function (item) {
          return item.Grade === selectItem.text;
        });
        this.$set(this.oilViscosity, 'list', []);
        Grade[0].Viscosities.forEach((item, index) => {
          this.oilViscosity.list.push({text: item.Viscosity, value: item.Viscosity});
          !index && (this.maintainList = item.Detaileds);
        });
        this.$set(this.oilViscosity.select, '0', this.oilViscosity.list[0]);
      } else {
        this.$set(this['oilViscosity'].select, '0', selectItem);
        this.maintainList = this.getOilMaintainList(this.oilGrade.select[0].text, this.oilViscosity.select[0].text);
      }
      document.getElementsByTagName('html')[0].scrollTop = 0;
      this.draggable = false;
    },
    /**
     * 获取当前项目的备注
     * @returns {*}
     */
    getData() {
      if (!this.urlCategory.partName) {
        return false;
      }
      this.oil = this.urlCategory.category === 'Oil';
      let isMyTab = this.$route.query.tab + '' !== '1';
      let loading =this.$loading({text: 'Loading', spinner: 'el-icon-loading', background: 'rgba(0, 0, 0, 0.7)'});
      let getNotes = this.$http.get('/BaoYang/GetProductPrioritySettingDescribeConfig/', {
        apiServer: 'apiServer',
        params: {partName: this.urlCategory.partName, type: 2},
        cacheData: false
      });
      let getMaintainList =this.$http.get('/Product/GetBaoYangProductPriority', {
        apiServer: 'apiServer',
        params: this.urlCategory,
        cacheData: false
      });
      Observable.forkJoin([getNotes, getMaintainList]).subscribe((res) => {
        loading.close();
        this.oldNotesContent = (res[0] && res[0].Data && res[0].Data.Value) || '';
        this.notesContent = this.oldNotesContent;
        let data = res[1] && res[1].Data;
        if (this.oil) {
          this.dataList = data;
          this.getOilData(this.dataList);
        } else {
          this.maintainList = data;
        }
      });
    },
    changeNotes() {
      this.editNotes(true);
      if (this.notesContent === this.oldNotesContent) {
        return false;
      }
      const loading = this.$loading({text: 'Loading', spinner: 'el-icon-loading', background: 'rgba(0, 0, 0, 0.7)'});
      this.$http.post('/BaoYang/SaveProductPrioritySettingDescribeConfig', {
        apiServer: 'apiServer',
        data: JSON.stringify({
          PartName: this.urlCategory.partName,
          Value: this.notesContent
        })
      }).subscribe((res) => {
        loading.close();
        if (res.Status) {
          this.oldNotesContent = this.notesContent;
          this.$message({message: '提交成功', type: 'success'});
        } else {
          this.$message({message: res.Msg || '提交失败', type: 'error', dangerouslyUseHTMLString: true});
        }
      });
    },
    /**
     * 编辑/保存备注信息
     * @param {Boolean} isChanged 是否提交更改
     */
    editNotes(isChanged) {
      isChanged = isChanged || false;
      const element = this.$refs.noteTextarea;
      const okBtn = this.$refs.okBtn;
      const cancleBtn = this.$refs.cancleBtn;
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
    /**
     * 标签页切换效果
     *  @param {Event} oilGradeSelect 当前选中的等级
     *  @param {Event} oilViscositySelect 当前选中的粘度
     *  @return {*}  机油所对应的项目列表
     */
    getOilMaintainList(oilGradeSelect, oilViscositySelect) {
      let Grade = this.dataList.filter(function (item) {
        return item.Grade === oilGradeSelect;
      });
      let Viscosity = Grade[0].Viscosities.filter(function (item) {
        return item.Viscosity === oilViscositySelect;
      });
      return Viscosity && Viscosity[0] && Viscosity[0].Detaileds;
    },
    /**
     * 保存排序项目列表
     */
    save() {
      let tableData = this.$refs.sortTable.save();
      const loading = this.$loading({text: 'Loading', spinner: 'el-icon-loading', background: 'rgba(0, 0, 0, 0.7)'});
      this.$http.post('/Product/SaveBaoYangProductPriority', {
        apiServer: 'apiServer',
        data: {
          'grade': this.oilGrade.select && this.oilGrade.select[0].text,
          'viscosity': this.oilViscosity.select&& this.oilViscosity.select[0].text,
          'partName': this.urlCategory.partName,
          'settings': tableData
        }
      }).subscribe((res) => {
        this.draggable = false;
        loading.close();
        if (res.status) {
          this.maintainList = tableData;
          this.$message({message: '提交成功', type: 'success'});
        } else {
          this.cancel();
          this.$message({message: res.Msg || '提交失败', type: 'error', dangerouslyUseHTMLString: true});
        }
      });
    },
    /**
     * 取消排序项目列表
     */
    cancel() {
      this.$refs.sortTable.cancel();
      this.draggable = false;
    }
  },
  components: {
    'sort-table': SortTable
  }
};
</script>
<style lang="scss" scoped src="../Content.scss">
</style>
<style lang="scss" scoped>
@import "css/common/_var.scss";
@import "css/common/_mixin.scss";
@import "css/common/_iconFont.scss";
.baseConfig{
  font-size:0;
  .priority-title{
    font-size:14px;
    color: $color3;
    &>li{
      display: flex;
      justify-content:flex-start;
      border-bottom: 1px solid $colore;
      .title{
        min-width: $topMenuWidth_pc;
        // height: $headerHeight;
        line-height: $headerHeight;
        text-align: center;
        display: inline-block;
        background-color: $colorfa;
        .select{
          display: none
        }
      }
      .oilList{
        flex-grow: 1;
        ul{
          display: flex;
          flex-wrap: wrap;
          justify-content:flex-start;
          overflow: auto;
          li{
            // max-width: $topMenuWidth_pc;
            // min-width: $topMenuChildWidth_pc;
            height: $headerHeight;
            line-height: $headerHeight;
            padding-top: $headerHeight/2-$smallTableHeight_phone/2 - 1;
            flex: 0 0 $topMenuWidth_pc;
            div{
              width: $topMenuChildWidth_pc;
              height: $smallTableHeight_phone;
              line-height: $smallTableHeight_phone;
              text-align: center;
              margin: 0 auto;
            }
            &.active{
              div{
                background-color: $stressColor;
                color: $colorf;
                border-radius:$btnBrRadius;
              }
            }
          }
        }
      }
      &:first-child{
        border-top: 1px solid $colore;
      }
    }
  }
  .save-btn{
    font-size:14px;
    margin-top: 20px;
    display: inline-block;
    vertical-align: top;
    color: $color3;
    border-top: 1px solid $colore; 
    border-bottom: 1px solid $colore; 
    border-right: 1px solid $colore;
    overflow: hidden;
    li{
      width: $saveBtnWidth;
      height: $headerHeight;
      line-height: $headerHeight;
      text-align: center;
      float: left;
      &.orderBy{
        &::after{
          content: '\e60C';
        }
      }
      &.refesh{
        border-left: 1px solid $colore;
        &::after{
          content: '';
        }
      }
      &.save{
        background-color: $stressColor;
        color: $colorf;
        &.phone{
          display: none;
        }
        &::after{
          color: $colorf;
          content: '\e60B';
        }
      }
      &::after{
        margin-left: 4px;
        content: '\e60D';
        color: $color97;
        @extend %mr_icon;
        font-size: 16px;
      }
    }
  }
  .tips{
    color:#333;
    // min-height:1rem;
    line-height:1rem;
    font-size:.7rem;
    margin:1rem;
  }
}
@media screen and (max-width: $minWidth) {
  .baseConfig{
    .tableBox{
      width: 500px;
    }
    .save-btn{
      li{
        float: none;
      }
    }
  }
}
@media screen and (max-width: $phoneWidth) {
  .baseConfig{
    .priority-title{
      width: 100%;
      display: flex;
      border-top: 1px solid #eee;
      position: fixed;
      background-color: $colorf;
      &>li{
        width: 0;
        line-height: $baseTableHeight_phone;
        flex-grow: 1;
        text-align: center;
        display: block;
        border-top: 0;
        .title{
          min-width: auto;
          background-color: $colorf;
          height: $baseTableHeight_phone;
          line-height: $baseTableHeight_phone;
          .select{
            display: inline-block
          }
        }
        .oilList{
          display: none;
        }
        &:last-child{
          border-left: 1px solid $colore;
        }
        &:after{
          content: "";
          width: 0;
          height: 0;
          border-left: 5px solid transparent;
          border-right: 5px solid transparent;
          border-top: 5px solid $colore;
          display: inline-block;
          margin-left: 5px;
          transform: translateY(-50%);
          transition: all ease-in-out 0.3s;
          position: relative;
          top: 1px;
        }
      }
    }
    .save-btn{
      width: 100%;
      display: flex;
      padding: $btnPadding;
      border-right: 0;
      position: fixed;
      bottom: 0;
      left: 0;
      z-index: 1;
      background-color: $colorf;
      li{
        flex-grow: 1;
        height: $baseTableHeight_phone;
        line-height: $baseTableHeight_phone;
        border-radius: $btnBrRadius*2;
        border: 1px solid $btnBrColor;
        &.save{
          display: none;
          &.phone{
            margin-left: $btnPadding;
            display: block;
          }
        }
        &::after{
          display: none;
        }
      }
    }
  }
}
</style>


