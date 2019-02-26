<template>
  <div class="specialConfig">
    <!-- 全国模板 -->
    <ul class="areaTemplate"><li><config-template :partName="urlCategory.partName" :category="urlCategory.category"></config-template></li></ul>
    <!-- 地区模板列表 -->
    <ul class="areaTemplate">
      <li v-for="(item,i) in areaTemplate" :key="i" class="areaTemplateItem">
        <config-template 
          :configTemplateData="item" 
          :partName="urlCategory.partName"
          :category="urlCategory.category"
          :index ="i"
          @openAreaTemplate="openAreaTemplate"
          @setDisabled="setDisabled" ></config-template>
      </li>
      <li class="areaTemplateItem add" @click="openAreaTemplate('')">+</li>
    </ul>
    <!-- 地区模板优先级弹窗 -->
    <el-dialog width="600px" :title="'地区模板'+AreaId+'范围选择'" :visible.sync="outerVisible" :close-on-click-modal="false">
      <area-select 
        :proviceList='proviceList'
        :selected='selected'
        ref="areaSelect">
        </area-select>
      <div slot="footer" class="dialog-footer">
        <el-switch v-model="selectedDisabled" :width="50" active-color="#DF3348" inactive-color="#fff"></el-switch>
        {{selectedDisabled?"启用":"禁用"}}
        <el-button @click="outerVisible = false">取 消</el-button>
        <el-button type="primary" @click="submit">提交</el-button>
      </div>
    </el-dialog>
    <!-- 备注 -->
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
import ConfigTemplate from '../components/ConfigTemplate';
import AreaSelect from '../components/AreaSelect';
import { Observable } from 'rxjs';
import 'rxjs/add/observable/forkJoin';
export default {
  props: ['urlCategory'],
  data() {
    return {
      areaTemplate: [],
      proviceList: [],
      selected: [],
      outerVisible: false,
      selectedDisabled: true,
      AreaId: '',
      areaIndex: -1,
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
     * 获取当前项目的列表&备注
     * @returns {*}
     */
    getData() {
      if (!this.urlCategory.partName) {
        return false;
      }
      let isMyTab = this.$route.query.tab + '' === '1';
      let loading = this.$loading({text: 'Loading', spinner: 'el-icon-loading', background: 'rgba(0, 0, 0, 0.7)'});
      let getNotes = this.$http.get('/BaoYang/GetProductPrioritySettingDescribeConfig/', {
        apiServer: 'apiServer',
        params: {partName: this.urlCategory.partName+'_', type: 1},
        cacheData: false
      });
      let getAreaList =this.$http.get('/BaoYangRecommend/GetPriorityAreaConfig', {
        apiServer: 'apiServer',
        params: this.urlCategory,
        cacheData: false
      });
      Observable.forkJoin([getNotes, getAreaList]).subscribe((res) => {
        loading.close();
        this.oldNotesContent = (res[0] && res[0].Data && res[0].Data.Value) || '';
        this.notesContent = this.oldNotesContent;
        this.areaTemplate = res[1] && res[1].Data;
      });
    },
    /**
     * 设置地区模板的启用&禁用
     * @param {Object} item 选择的类型
     */
    setDisabled(item) {
      let data = {
        partName: this.urlCategory.partName,
        IsEnabled: !item.IsEnabled,
        AreaId: item.AreaId
      };
      const loading = this.$loading({text: '数据加载中,请稍后...', spinner: 'el-icon-loading', background: 'rgba(0, 0, 0, 0.7)'});
      this.$http.get('/BaoYangRecommend/UpdatePriorityAreaIsEnabled', {
        apiServer: 'apiServer',
        params: data,
        cacheData: false
      }).subscribe(res => {
        loading.close();
        if (res&&res.Status) {
          item.IsEnabled = !item.IsEnabled;
          this.$message({type: 'success', message: '设置状态成功'});
        } else {
          this.$message({message: res.Msg || '设置状态成功', type: 'error', dangerouslyUseHTMLString: true});
        }
      });
    },
    /**
     * 修改当前模板配置信息
     * @returns {*}
     */
    changeNotes() {
      this.editNotes(true);
      if (this.notesContent === this.oldNotesContent) {
        return false;
      }
      const loading = this.$loading({text: 'Loading', spinner: 'el-icon-loading', background: 'rgba(0, 0, 0, 0.7)'});
      this.$http.post('/BaoYang/SaveProductPrioritySettingDescribeConfig', {
        apiServer: 'apiServer',
        data: JSON.stringify({
          PartName: this.urlCategory.partName+'_',
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
      element.getAttribute('readonly') ? element.removeAttribute('readOnly'): element.setAttribute('readOnly', 'true');
      element.classList.toggle('edit');
      okBtn.classList.toggle('show');
      cancleBtn.classList.toggle('show');
      !isChanged && (this.notesContent = this.oldNotesContent);
    },
    /**
     * 备注 进入可编辑状态
     */
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
     * 地区模板编辑优先级（城市是根据模板id进行排他操作）
     * @param {Boolean} item 当前模板信息
     * @param {Boolean} index 当前模板index
     */
    openAreaTemplate(item, index) {
      this.AreaId = item.AreaId || '';
      this.areaIndex = index;
      this.selectedDisabled = item?item.IsEnabled:true;
      this.selected =item.Details || [];
      const loading = this.$loading({text: 'Loading', spinner: 'el-icon-loading', background: 'rgba(0, 0, 0, 0.7)'});
      this.$http.get('/BaoYangRecommend/GetRegion', {
        apiServer: 'apiServer',
        params: {
          PartName: this.urlCategory.partName,
          AreaId: item.AreaId || 0
        }
      }).subscribe((res) => {
        loading.close();
        if (res.Status) {
          this.proviceList = res && res.Data;
          this.outerVisible = true;
        } else {
          this.$message({message: res.Msg || '获取失败', type: 'error', dangerouslyUseHTMLString: true});
        }
      });
    },
    /**
     * 提交编辑的地区模板
     */
    submit() {
      this.$confirm('确认新增/编辑地区模板?', '确认操作?', {
        confirmButtonText: '确定',
        cancelButtonText: '取消',
        type: 'info',
        closeOnClickModal: false
      }).then(() => {
        let data = {
          AreaId: this.areaIndex > -1 ? this.areaTemplate[this.areaIndex].AreaId : 0,
          PartName: this.urlCategory.partName,
          IsEnabled: this.selectedDisabled,
          Details: this.$refs.areaSelect.submit()
        };
        const loading = this.$loading({text: 'Loading', spinner: 'el-icon-loading', background: 'rgba(0, 0, 0, 0.7)'});
        this.$http.post('/BaoYangRecommend/SavePriorityAreaConfig', {
          apiServer: 'apiServer',
          data: data
        }).subscribe((res) => {
          this.outerVisible = false;
          loading.close();
          if (res.Status) {
            if (this.areaIndex > -1) {
              this.$set(this.areaTemplate, this.areaIndex, data);
            } else {
              this.areaTemplate.push({
                'partName': this.urlCategory.partName,
                'AreaId': res.Data,
                'IsEnabled': this.selectedDisabled,
                'Details': this.$refs.areaSelect.submit()
              });
            }

            this.$message({message: '提交成功', type: 'success'});
          } else {
            this.$message({message: res.Msg || '提交失败', type: 'error', dangerouslyUseHTMLString: true});
          }
        });
      }).catch(() => {});
    }
  },
  components: {
    'config-template': ConfigTemplate,
    'area-select': AreaSelect
  }
};
</script>
<style lang="scss" scoped src="../Content.scss">
</style>
<style lang="scss" scoped>
@import "css/common/_var.scss";
@import "css/common/_mixin.scss";
@import "css/common/_iconFont.scss";
.specialConfig{
  .areaTemplate{
    display: flex;
    padding:0 20px;
    flex-wrap: wrap;
    justify-content: flex-start;
    .areaTemplateItem{
      flex: 0 0 330px;;
      margin-right: 1rem;
      &.add{
        text-align: center;
        font-size: 7rem;
        line-height: 200px;
        height: 200px;
        border: 1px solid $colore;
        color: #e5e5e6;
      }
    }
  }
}

</style>

