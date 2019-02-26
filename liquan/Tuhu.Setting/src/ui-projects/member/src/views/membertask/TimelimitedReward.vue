<template>
 <div>
     <Breadcrumb :style="{margin: '24px 0'}">
       <BreadcrumbItem>会员任务配置</BreadcrumbItem>
       <BreadcrumbItem>限时奖励</BreadcrumbItem>
     </Breadcrumb>
       <Content :style="{padding: '5px', minHeight: '280px', background: '#fff'}">
    <div  class="tool">
        <div>
            <Button type="primary" @click="EditPage" style=" float: left;">新增</Button>
        </div>
          <Modal  v-model="modal1" title="编辑签到信息" @on-ok="Save" style="width:460px;">
              <div>
                <table class="edit-table" style="width:100%">
                    <tr>
                        <td style="width:90px;">福利类型</td>
                        <td style="width:380px;">
                          <select v-model="TaskTypeCode" style="height:32px;">
                              <option value="1" selected="selected">新手福利</option>
                              <option value="0">成长福利</option>
                              <option value="2">日常福利</option>
                          </select>
                        </td>
                    </tr>
                    <tr>
                        <td>副标题文案</td>
                        <td>
                           <input v-model="DescriptionTitle" type="text"  placeholder="必填，限13个字以内" maxlength="13" />
                        </td>
                    </tr>
                    <tr>
                        <td>倒计时设置</td>
                        <td>
                            <input v-model="RemainingDay" type="text" placeholder="必填，限正整数"  onkeyup="value=value.replace(/[^\-?\d.]/g,'')"/>天
                            <br /><span>用户在倒计时结束前完成对应福利 类型即可获得额外奖励。 </span>
                        </td>
                    </tr>
                    <tr>
                        <td>额外奖励</td>
                        <td>
                            <input v-model="RewardValue" type="text" placeholder="必填，限正整数" onkeyup="value=value.replace(/[^\-?\d.]/g,'')"/>积分
                        </td>
                    </tr>
                    <tr>
                        <td>状态</td>
                        <td>
                            <i-switch v-model="Status" size="large">
                                <span slot="open">启用</span>
                                <span slot="close">禁用</span>
                             </i-switch>
                        </td>
                    </tr>
                </table>
              </div>
               <div slot="footer">
   	                   <Button type="text" size="large" @click="ModalCancel">取消</Button>
   	                   <Button type="primary" size="large" @click="ok">确定</Button>
               </div>
         </Modal>
    </div>
     <Table :data="tableData" :columns="columns" border></Table>
       </Content >
</div>
   
</template>
<style>
.tool{
    height: 50px;
}
.show{
    color: red;
}
.edit-table tr{
    line-height:30px;
}
.edit-table tr td{
    padding-top: 5px;
}
.edit-table td input,select{
   width: 280px;
}

</style>

<script>
    export default {
        data () {
            return {
                modal1: false,
                tableData:[],
                PKID:'',
                TaskTypeCode:'',
                TaskTypeName:'',
                DescriptionTitle:'',
                RewardValue:'',
                RemainingDay:'',
                Status:'',
                StrStatus:'',
                toolong:false,
                columns: [
                    {
                        title: 'PKID',
                        key:'PKID',
                        align:'center',
                        width:80
                    },
                    {
                        title: '福利类型',
                        key: 'TaskTypeName',
                          align:'center',
                           width:200
                    },
                     {
                        title: '副标题文案',
                        key: 'DescriptionTitle',
                        align:'center',
                        width:200
                    }, 
                    {
                        title: '倒计时设置（天）',
                        key: 'RemainingDay',
                        align:'center',
                        width:200
                    }, 
                    {
                        title: '额外奖励（积分）',
                        key: 'RewardValue',
                        align:'center',
                        width:200
                    }, 
                    {
                        title: '状态',
                        key: 'StrStatus',
                        align:'center',
                        width:200
                    }, 
                     {
                        title: '最后操作人',
                        key: 'LastUpdateBy',
                        align:'center',
                       
                    }
                    ,
                     {
                        title: '操作时间',
                        key: 'StrLastUpdateDateTime',
                        align:'center',
                    },
                    {
                        title: '操作',
                        key: 'action',
                        align:'center',
                        render:(h,params)=>{
                            return h('div',[
                             h('Button',{
                                 props:{
                                    type:'primary'
                                },
                                style:{
                                    width:'50px'
                                },
                                on:{
                                    click:()=>
                                    {
                                     this.UpdateTaskRewardTypes(params.row);
                                    }
                                }
                            },'编辑'),
                            h('Button',{
                                props:{
                                    type:'primary'
                                },
                                style:{
                                    width:'50px',
                                    'margin-left':'20px'
                                },
                                on:{
                                    click:()=>
                                    {
                                     this.DeleteTaskRewardTypes(params.row.PKID);
                                    }
                                }
                            },'删除')])
                        }
                    }
                ]
            }
        },
         mounted(){
             this.GetTaskRewardTypes();
         },
        methods:{
            ModalCancel(){ 
                this.modal1 = false; 
            },
            GetTaskRewardTypes(){
                this.util.ajax.get('/TaskTypeReward/GetTaskTypeReward').then(res=>{
                    if(res.data){
                          var result=res.data.data;
                          if(result)
                          {
                              this.tableData=result;
                          }
                    }
                })
            }, 
            //页面编辑
            EditPage(){
                var result= this.tableData;
               this.modal1=true;
            }, 
            GetEditData(){
                var dataList=[];
                for(var i=1;i<=7;i++){
                    var m={};
                    m.ContinuousDays=i;
                    switch(i){
                        case 1:
                        m.RewardIntegral=this.txtReward1;
                        break;
                        case 2:
                        m.RewardIntegral=this.txtReward2;
                        break;
                        case 3:
                        m.RewardIntegral=this.txtReward3;
                        break;
                        case 4:
                        m.RewardIntegral=this.txtReward4;
                        break;
                        case 5:
                        m.RewardIntegral=this.txtReward5;
                        break;
                        case 6:
                        m.RewardIntegral=this.txtReward6;
                        break;
                        case 7:
                        m.RewardIntegral=this.txtReward7;
                        break;
                    }
                    dataList.push(m);
                    if(m.RewardIntegral===''|| m.RewardIntegral<=0){
                        break;
                    }
                }
                return dataList;
            },
        //
        ValidateEdit(){
            var editdata=this.GetEditData();
            if(!editdata || editdata.length<=0){
                return false;
            }
            for(var i=0;i<editdata.length;i++){
                var m=editdata[i];
                if(m.RewardIntegral==='' || m.RewardIntegral<=0){
                    this.$Message.warning("连续签到"+m.ContinuousDays+"天奖励不可为空！");
                    return false;
                }
            }
            return true;
        }, 
        //保存按钮
        Save () {
            if(!this.ValidateEdit()){
                return;
            }
            var eidtData=this.GetEditData();
            this.util.ajax.post('/UserDailyCheckIn/SaveAsync',{
                dataList:eidtData
            }).then(res=>{
               if(!res || !res.data){
                     this.$Message.success("操作失败！");
                     return;
               }
                if (res.data.result) {
                    this.$Message.success("操作成功！");
                    this.GetTaskRewardTypes();
                    this.modal1 = false;
                }else{
                     this.$Message.success("操作失败！");
                    this.GetTaskRewardTypes();
                }
            })
        }, 
        //更新任务奖励
        UpdateTaskRewardTypes(){

        }, 
        //删除任务奖励
        DeleteTaskRewardTypes(id){
            if(!id || id<=0){
                 this.$Message.warning("重新点击删除");
                 return;
            }
             this.util.ajax.post('/TaskTypeReward/DeleteTaskTypeReward',{id:id}).then(res=>{
                 if(!res.data){
                      this.$Message.warning("操作失败");
                     return;
                 }
                if(!res.data.data || res.data.data<=0){
                     this.$Message.warning("操作失败");
                     return;
                }
                 this.$Message.success("删除成功！");
                 this.GetTaskRewardTypes();
             });
        },
        }
    };
</script>
