<template>
 <div>
     <Breadcrumb :style="{margin: '24px 0'}">
       <BreadcrumbItem> 会员任务配置</BreadcrumbItem>
       <BreadcrumbItem>签到配置后台</BreadcrumbItem>
     </Breadcrumb>
       <Content :style="{padding: '5px', minHeight: '280px', background: '#fff'}">
    <div  class="tool">
        <div>
            <Button type="primary" @click="EditPage" style=" float: left;">编辑签到信息</Button>
            <span style="margin-left: 30px;display: inline-block;padding-top: 5px;font-size:  11pt;font-weight:  bold;">本后台涉及签到奖励配置影响 5.4.6（含）之后所有版本 的全局用户</span>
        </div>
          <Modal  v-model="modal1" title="编辑签到信息" @on-ok="ok" style="width:500px;">
              <div>
                <table class="edit-table" style="width:100%">
                    <tr>
                        <td style="width:120px;">连续签到天数：1</td>
                        <td style="width:380px;">
                           <span>签到奖励：</span><input v-model="txtReward1" type="text" id="txtReward1" placeholder="连续一天奖励，正整数，最高限制三位" maxlength="3" onkeyup="value=value.replace(/[^\-?\d.]/g,'')"/>
                        </td>
                    </tr>
                    <tr>
                        <td>连续签到天数：2</td>
                        <td>
                             <span>签到奖励：</span><input v-model="txtReward2" type="text" id="txtReward2" placeholder="连续二天奖励，正整数，最高限制三位" maxlength="3" onkeyup="value=value.replace(/[^\-?\d.]/g,'')"/>
                        </td>
                    </tr>
                    <tr>
                        <td>连续签到天数：3</td>
                        <td>
                             <span>签到奖励：</span><input v-model="txtReward3" type="text" id="txtReward3" placeholder="连续三天奖励，正整数，最高限制三位" maxlength="3" onkeyup="value=value.replace(/[^\-?\d.]/g,'')"/>
                        </td>
                    </tr>
                    <tr>
                        <td>连续签到天数：4</td>
                        <td>
                             <span>签到奖励：</span><input v-model="txtReward4" type="text" id="txtReward4" placeholder="连续四天奖励，正整数，最高限制三位" maxlength="3" onkeyup="value=value.replace(/[^\-?\d.]/g,'')"/>
                        </td>
                    </tr>
                    <tr>
                        <td>连续签到天数：5</td>
                        <td>
                             <span>签到奖励：</span><input v-model="txtReward5" type="text" id="txtReward5" placeholder="连续五天奖励，正整数，最高限制三位" maxlength="3" onkeyup="value=value.replace(/[^\-?\d.]/g,'')"/>
                        </td>
                    </tr>
                    <tr>
                        <td>连续签到天数：6</td>
                        <td>
                             <span>签到奖励：</span><input v-model="txtReward6" type="text" id="txtReward6" placeholder="连续六天奖励，正整数，最高限制三位" maxlength="3" onkeyup="value=value.replace(/[^\-?\d.]/g,'')"/>
                        </td>
                    </tr>
                    <tr>
                        <td>连续签到天数：7</td>
                        <td>
                             <span>签到奖励：</span><input v-model="txtReward7" type="text" id="txtReward7" placeholder="连续七天奖励，正整数，最高限制三位" maxlength="3" onkeyup="value=value.replace(/[^\-?\d.]/g,'')"/>
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
.edit-table td input{
   width: 280px;
}
</style>

<script>
    export default {
        data () {
            return {
                modal1: false,
                tableData:[],
                txtReward1:'',
                txtReward2:'',
                txtReward3:'',
                txtReward4:'',
                txtReward5:'',
                txtReward6:'',
                txtReward7:'',
                toolong:false,
                columns: [
                    {
                        title: 'PKID',
                        key:'PKID',
                        align:'center',
                        width:80
                    },
                    {
                        title: '连续签到天数（天）',
                        key: 'ContinuousDays',
                          align:'center',
                           width:200
                    },
                     {
                        title: '签到奖励（积分）',
                        key: 'RewardIntegral',
                        align:'center',
                        width:200
                    }
                    ,
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
                    }
                ]
            }
        },
         mounted(){
             this.GetQuestionTypes();
         },
        methods:{
            ModalCancel(){ 
                this.modal1 = false; 
                 this.txtReward1 = "";
                 this.txtReward2 = "";
                 this.txtReward3 = "";
                 this.txtReward4 = "";
                 this.txtReward5 = "";
                 this.txtReward6 = "";
                 this.txtReward7 = "";
            },
            GetQuestionTypes(){
                this.util.ajax.get('/UserDailyCheckIn/GetData').then(res=>{
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
                if(result){
                    for(var i=0;i<result.length;i++){
                        var m=result[i];
                        switch(m.ContinuousDays){
                            case 1:
                                this.txtReward1 = m.RewardIntegral;
                            break;
                            case 2:
                                this.txtReward2 = m.RewardIntegral;
                            break;
                            case 3:
                                this.txtReward3 = m.RewardIntegral;
                            break;
                            case 4:
                                this.txtReward4 = m.RewardIntegral;
                            break;
                            case 5:
                                this.txtReward5 = m.RewardIntegral;
                            break;
                            case 6:
                                this.txtReward6 = m.RewardIntegral;
                            break;
                            case 7:
                                this.txtReward7 = m.RewardIntegral;
                            break;  
                        }
                    }
                }
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
        ok () {
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
                    this.GetQuestionTypes();
                    this.modal1 = false;
                }else{
                     this.$Message.success("操作失败！");
                    this.GetQuestionTypes();
                }
            })
        } 

        }
    };
</script>
