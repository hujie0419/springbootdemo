<template>
    <div class="tableBox" :class="{draggable:draggable}">
        <table class="table">
          <tr>
            <th class="th">品牌</th>
            <th class="th">系列</th>
          </tr>
          <tr
            v-for="(item, index) in tableDataTwo" 
            :key="index"
            :draggable="draggable"
            @mousemove="mousemove"
            @mouseout="mouseout"
            @drag="drag"  
            @dragstart.stop="dragstart($event,index,1)"
            @dragover.stop="dragover($event,index,1)" 
            @dragleave.stop="dragleave($event,index,1)"
            @drop.stop="drop($event,index,1)" 
            @touchstart.stop="touchstart($event ,index,1)"
            @touchend.stop="touchend($event ,index,1)"
            :class="item.className">
            <td>{{item.Brand}}</td>
            <td>
                <table class="table2">
                  <tr 
                    v-for="(seriesItem, index2) in item.child" 
                    :key="index2"
                    :draggable="draggable"
                    @mousemove="mousemove"
                    @mouseout="mouseout"
                    @drag="drag"  
                    @dragstart.stop="dragstart($event,seriesItem.Priority,2)"
                    @dragover.stop="dragover($event,seriesItem.Priority,2)" 
                    @dragleave.stop="dragleave($event,seriesItem.Priority,2)"
                    @drop.stop="drop($event ,seriesItem.Priority,2)"
                    @touchstart.stop="touchstart($event ,seriesItem.Priority,2)"
                    @touchend.stop="touchend($event ,seriesItem.Priority,2)"
                    :class="seriesItem.className">
                    <td>{{seriesItem.Series}}</td>
                  </tr>
                </table>
            </td>
          </tr>
        </table>
    </div>
</template>
<script>
export default {
  props: {
    draggable: {
      type: Boolean,
      default: false
    },
    tableData: {
      type: Array,
      default: []
    }
  },
  data () {
    return {
      app: document.getElementsByTagName('html')[0],
      tableDataOne: [], // 一维数据，实际提交的数据
      tableDataTwo: [], // 二维数据，页面上显示的数据
      moveColumn: 0, // 当前移动的是那一列
      moveIndex: -1, // 移动模块的index
      insertIndex: -1, // 插入模块的index
      touchStartTime: 0,
      time: 0,
      selectDbl: false, // 选择拖动块- 选择or 取消
      select: false // 拖动块插入位置- 选择or 取消
    };
  },
  watch: {
    tableData: {
      handler(val, oldVal) {
        this.tableDataOne = JSON.parse(JSON.stringify(val));
      },
      deep: true
    },
    tableDataOne: {
      handler(val, oldVal) {
        this.settableDataTwo();
      },
      deep: true
    },
    moveIndex(val, oldVal) { // 待移动块index
      let tableDataNow = this.moveColumn === 1 ? this.tableDataTwo: this.tableDataOne;
      oldVal > -1 && tableDataNow[oldVal] && this.$set(tableDataNow[oldVal], 'className', '');
      val > -1 && tableDataNow[val] && this.$set(tableDataNow[val], 'className', 'hover');
      val === -1 && (this.moveColumn = 0);
    },
    insertIndex(val, oldVal) { // 插入位置index
      let tableDataNow = this.moveColumn === 1 ? this.tableDataTwo: this.tableDataOne;
      oldVal > -1 && tableDataNow[oldVal] && this.$set(tableDataNow[oldVal], 'className', '');
      tableDataNow[this.moveIndex] && this.$set(tableDataNow[this.moveIndex], 'className', 'hover');
      val > -1 && tableDataNow[val] && this.$set(tableDataNow[val], 'className', val < this.moveIndex?'borderTop':'borderBottom');
    }
  },
  created() {
    this.settableDataTwo();
  },
  methods: {
    /**
     * 设置页面上显示的二维数据
     */
    settableDataTwo() {
      let index = -1;
      let Brand = '';
      this.tableDataTwo = [];
      this.tableDataOne.forEach((item, i) => {
        item.Priority= i;
        this.$set(item, 'className', item.className || '');
        if (Brand !== item.Brand) {
          index++;
          Brand = item.Brand;
          this.$set(this.tableDataTwo, index, {Brand: Brand, child: [], className: ''});
        }
        this.tableDataTwo[index].child.push(item);
      });
    },
    /**
     * 移动端 点击开始
     * @param {Event} e 事件
     * @param {Number} nowIndex 当前索引index
     * @param {Number} flag 1:第一列；2:第二列
     */
    touchstart(e, nowIndex, flag) {
      if (this.draggable) {
        this.touchStartTime = +new Date();
        let timeNow = '';
        clearInterval(this.time);
        this.time = setInterval(() => {
          timeNow = +new Date();
          if (timeNow - this.touchStartTime > 1000) { // 长按-选中拖动块
            // this.touchStartTime = 0;
            if (this.moveColumn && this.moveColumn !== flag) { // 没选择拖拽模块 || 拖拽模块位置与插入位置一致 || 列对不上
              clearInterval(this.time);
              return;
            }
            if (this.moveIndex === nowIndex) {
              this.selectDbl = !this.selectDbl;
            } else {
              this.selectDbl = true;
            }
            if (this.selectDbl) {
              this.moveColumn = flag; // 确定 选中的列
              this.moveIndex = nowIndex;
            } else {
              this.moveIndex = -1;
            }
            this.insertIndex = -1; // 清除 插入位置
            clearInterval(this.time);
          }
        });
      }
    },
    /**
     * 移动端 点击结束
     * @param {Event} e 事件
     * @param {Number} nowIndex 当前索引index
     * @param {Number} flag 1:第一列；2:第二列
     */
    touchend(e, nowIndex, flag) {
      clearInterval(this.time);
      if (!this.draggable || !this.touchStartTime) {
        return;
      }
      if (this.moveIndex < 0 || this.moveIndex === nowIndex || this.moveColumn !== flag) { // 没选择拖拽模块 || 拖拽模块位置与插入位置一致 || 列对不上
        return;
      }
      if (this.insertIndex === nowIndex) {
        this.select = !this.select;
      } else {
        this.select = true;
      }
      if (this.select) {
        this.insertIndex = nowIndex;
      } else {
        flag === 1 ? this.insertModule1(): this.insertModule2(); // 插入
        this.insertIndex = -1;
        this.moveIndex = -1;
      }
      // }
    },
    /**
     * 第一列的插入
     */
    insertModule1() {
      let item = this.tableDataTwo;
      let nowIndex = this.insertIndex;
      // 以下代码 和 pc 逻辑相似
      let upMove = nowIndex - this.moveIndex;
      let movePriority = item[this.moveIndex].child[0].Priority;
      let moveList = item[this.moveIndex].child;
      if (upMove > 0) { // 下移
        this.tableDataOne.splice(movePriority, moveList.length);
        if (item[nowIndex+1]) {
          let nowPriority = item[nowIndex+1].child[0].Priority;
          moveList.forEach((element, index) => {
            this.tableDataOne.splice(nowPriority+index-moveList.length, 0, element);
          });
        } else { // 最后一行
          moveList.forEach((element, index) => {
            this.tableDataOne.push(element);
          });
        }
      } else if (upMove < 0) { // 上移
        let nowPriority = item[nowIndex].child[0].Priority;
        this.tableDataOne.splice(movePriority, moveList.length);
        moveList.forEach((element, index) => {
          this.tableDataOne.splice(nowPriority+index, 0, element);
        });
      }
    },
    /**
     * 第二列的插入
     */
    insertModule2() {
      let nowIndex = this.insertIndex;
      // 以下代码 和 pc 逻辑相似
      this.$set(this.tableDataOne[nowIndex], 'className', ''); // 清空插入位置的样式
      let insertItem = this.tableDataOne[this.moveIndex];
      this.tableDataOne.splice(this.moveIndex, 1);
      this.tableDataOne.splice(nowIndex, 0, insertItem);
    },
    /**
     * pc 模拟hover效果
     * 因为是嵌套表格的hover，所以不能用css
     * @param {Event} e 事件
     */
    mousemove(e) {
      if (!this.touchStartTime&&this.draggable) {
        e.target.localName === 'td' ? e.target.parentNode.classList.add('hover') : e.target.classList.add('hover');
      }
    },
    /**
     * pc 模拟hover效果
     * 因为是嵌套表格的hover，所以不能用css
     * @param {Event} e 事件
     */
    mouseout(e) {
      if (!this.touchStartTime&&this.draggable) {
        e.target.classList.remove('hover');
        e.target.parentNode.classList.remove('hover');
      }
    },
    /**
     * pc 拖拽开始
     * @param {Event} e 事件
     * @param {Number} index 当前索引index
     * @param {Number} flag 1:第一列；2:第二列
     */
    dragstart (e, index, flag) {
      if (this.draggable) {
        this.moveColumn = flag;
        this.moveIndex = index;
        e.dataTransfer.setData('index', index);
      }
    },
    /**
     * pc 拖拽经过
     * @param {Event} e 事件
     * @param {Number} index 当前索引index
     * @param {Number} flag 1:第一列；2:第二列
     */
    dragover (e, index, flag) {
      if (this.draggable && this.moveColumn == flag && this.moveIndex !== index) {
        this.insertIndex = index;
      } else {
        this.insertIndex = -1;
      }
      e.preventDefault();
    },
    /**
     * pc 拖拽离开
     * @param {Event} e 事件
     * @param {Number} index 当前索引index
     * @param {Number} flag 1:第一列；2:第二列
     */
    dragleave (e, index, flag) {
      if (this.moveIndex !== index) {
        this.insertIndex = index;
      }
    },
    /**
     * pc 拖拽结束 修改数据
     * @param {Event} e 事件
     * @param {Number} nowIndex 当前索引index
     * @param {Number} flag 1:第一列；2:第二列
     */
    drop (e, nowIndex, flag) {
      this.insertIndex = nowIndex;
      if (this.draggable && this.moveColumn == flag) {
        flag === 1 ? this.insertModule1(): this.insertModule2(); // 插入
      }
      this.insertIndex = -1;
      this.moveIndex = -1;
    },
    /**
     * pc 往上拖拽时 修改滚动条
     * @param {Event} e 事件
     */
    drag (e) {
      let y = e.clientY;
      if (y < 50) {
        this.app.scrollTop -= 3;
      }
    },
    /**
     * 返回提交的数据
     *  @return {Array} 用户拖拽后未提价的值
     */
    save() {
      let saveTableData=[];
      this.tableDataOne.forEach((item) => {
        saveTableData.push({'Brand': item.Brand, 'Series': item.Series, 'Priority': item.Priority+1});
      });
      return saveTableData;
    },
    /**
     * 取消
     */
    cancel() {
      this.tableDataOne = JSON.parse(JSON.stringify(this.tableData));
      this.insertIndex = -1;
      this.moveIndex = -1;
    }
  }
};
</script>
<style scoped lang='scss'>
@import "css/common/_var.scss";
@import "css/common/_mixin.scss";
@import "css/common/_iconFont.scss";
.tableBox{
  width: 600px;
  border: 1px solid $colore;
  border-left: 0;
  color: $color3;
  margin: 20px 0 20px 20px;
  display: inline-block;
  vertical-align:top;
  font-size:14px;
  &.draggable{
    border: 1px solid $stressColor;
  }
  .table,.table2{
    width: 100%;
    border-collapse:collapse;
    border-spacing:0;
    tr{
      display: flex;
      align-items: center;
      padding: 0!important;
      border-left: 1px solid $colore;
      border-bottom: 1px solid $colore;
      &.borderTop{
        border-top: 2px dashed $stressColor;
        // border-bottom: 0
      }
      &.borderBottom{
        border-top: 0;
        border-bottom: 2px dashed $stressColor !important;
      }
      &:last-child{
        border-bottom: 0;  
      }
      td,th{
        width: 0;
        flex-grow: 1;
        min-width: 150px;
        min-height: $headerHeight;
        line-height: $headerHeight;
        text-align: center;
        background-color: #00000000;
        border: 0;
        &.th{
          background-color: $colorfa;
          &:last-child{
           border-left: 1px solid $colore;
          }
        }
      }
      &.hover{
        cursor: move;
        background: #FBEAEC;
        transition: all .3s;
        border-top: 1px solid $stressColor !important;
        border-bottom: 1px solid $stressColor !important;
        tr{
          border-color: $stressColor;
        }
      }
    }
  }
}
@media screen and (max-width: $phoneWidth) {
  .tableBox{
    width: 100% !important;
    margin : 47px 0 0 0 ;
    // .hover{
    //   tr{
    //       border-color: $colore !important;
    //     }
    // }
    td,th{
        min-height: $baseTableHeight_phone !important;
        line-height: $baseTableHeight_phone !important;
        &.th{
          color: $color6;
          min-height: $smallTableHeight_phone !important;
          line-height: $smallTableHeight_phone !important;
        }
      }
  }
}
</style>
