<template>
  <div>
      <h1 class="title">答题配置</h1>
      <div>
        <label>题目下线日期</label>
        <DatePicker type="date" v-model="filters.endTime" show-week-numbers style="width: 150px"></DatePicker>
        <lable style="margin-left:13px">竞猜结果</lable>
            <Select v-model="filters.QuestionConfirm" placeholder="竞猜结果" style="width:200px">
                <Option v-for="item in gameStatusList" :value="item.value" :key="item.value">{{ item.label }}</Option>
            </Select>  
        
      </div>
      <div style="margin-top:18px">
          <Button type="success" icon="search" @click="loadData(1)">搜索</Button>
          <Button type="success" icon="plus" style="margin-left:20px" @click="searchQuestionWithOption()">新增竞猜</Button>        
            <Button type="success" icon="load-c" style="margin-left:20px;" @click="RefreshQuestionCache">刷新缓存</Button>                         
      </div>
      <div style="margin-top:18px">
        <Table  :loading="table.loading" :data="table.data" :columns="table.columns" style="width:100%" stripe></Table>
        <div style="margin: 10px;overflow: hidden">
            <div style="float: right;">
                <Page :total="page.total" :page-size="page.pageSize" :current="page.current" :page-size-opts="[10 ,30 ,50]"  show-sizer @on-change="handlePageChange" @on-page-size-change="handlePageSizeChange"></Page>
            </div>
        </div>
      </div>
        <Modal v-model="logmodal.visible" title="操作日志" cancelText="取消" @on-cancel="cancel" scrollable="true" :width="logmodal.width">
            <Table :loading="logmodal.loading" :data="logmodal.data" :columns="logmodal.columns" stripe></Table>
        </Modal>    
    <Modal
        v-model="deleteModal.visible"
        title="删除"
        :loading="deleteModal.loading"
        @on-ok="deleteok" :mask-closable="false">
        <p>确认删除,会将当天的套题全部删除哦？</p>
    </Modal>  
      <Modal
        v-model="releaseModal.visible"
        title="发布答案"
        :loading="releaseModal.loading"
        @on-ok="releaseok" :mask-closable="false">
        <p>确认发布答案,会将当天的套题答案全部发布哦？</p>
    </Modal>
      <Modal v-model="modal.visible" :loading="modal.loading" title="答案编辑" okText="保存" :transfer="false" cancelText="取消" @on-ok="SaveQuestionConfig"   width="50%" :mask-closable="false">
        <Form ref="modal.productConfig" :model="modal.productConfig" :rules="modal.rules" :label-width="90">
          
            <FormItem label="题目内容1">
                <Input v-model="modal.QuestionAnswer1.QuestionTitle" :disabled=true />
            </FormItem>
            <FormItem label="竞猜结果">
                <Input v-model="modal.QuestionAnswer1.QuestionTextResult" />
            </FormItem>
            <FormItem>
                <Row>
                        <Col span="11">
                       <input type="radio" v-model="modal.QuestionAnswer1.AnswerYes" value=1>是                      
                        </Col>
                        <Col span="11">
                       <input type="radio" v-model="modal.QuestionAnswer1.AnswerYes" value=-1>否    
                        </Col>
                    </Row>
            </FormItem>
             <FormItem label="题目内容2">
                <Input v-model="modal.QuestionAnswer2.QuestionTitle" :disabled=true />
            </FormItem>
            <FormItem label="竞猜结果">
                <Input v-model="modal.QuestionAnswer2.QuestionTextResult"   />
            </FormItem>
            <FormItem>
                <Row>
                        <Col span="11">
                       <input type="radio" v-model="modal.QuestionAnswer2.AnswerYes" value=1>是                      
                        </Col>
                        <Col span="11">
                       <input type="radio" v-model="modal.QuestionAnswer2.AnswerYes" value=-1>否    
                        </Col>
                    </Row>
            </FormItem>
             <FormItem label="题目内容3">
                <Input v-model="modal.QuestionAnswer3.QuestionTitle" :disabled=true />
            </FormItem>
            <FormItem label="竞猜结果">
                <Input v-model="modal.QuestionAnswer3.QuestionTextResult"  />
            </FormItem>
            <FormItem>
                <Row>
                      <Col span="11">
                       <input type="radio" v-model="modal.QuestionAnswer3.AnswerYes" value=1>是                      
                        </Col>
                        <Col span="11">
                       <input type="radio" v-model="modal.QuestionAnswer3.AnswerYes" value=-1>否    
                        </Col>
                    </Row>
            </FormItem>           
              <FormItem label="答案公布日期">
                <Input v-model="modal.QuestionAnswer1.EndTime" :disabled=true />
            </FormItem>                            
        </Form>
      </Modal>
      <Modal v-model="querymodal.visible" :loading="querymodal.loading" :transfer="false" title="答案查看" width="50%" :mask-closable="false">    
           <Form ref="querymodal.productConfig" :model="querymodal.productConfig" :rules="querymodal.rules" :label-width="90">              
            <FormItem label="题目内容1">
                <Input v-model="querymodal.QuestionAnswer1.QuestionTitle" :disabled=true />
            </FormItem>
            <FormItem label="竞猜结果">
                <Input v-model="querymodal.QuestionAnswer1.QuestionTextResult" :disabled=true />
            </FormItem>
            <FormItem>
                <Row>
                        <Col span="11">
                       <input type="radio" v-model="querymodal.QuestionAnswer1.AnswerYes" value=1 :disabled=true >是                      
                        </Col>
                        <Col span="11">
                       <input type="radio" v-model="querymodal.QuestionAnswer1.AnswerYes" value=-1 :disabled=true >否    
                        </Col>
                    </Row>
            </FormItem>
             <FormItem label="题目内容2">
                <Input v-model="querymodal.QuestionAnswer2.QuestionTitle" :disabled=true />
            </FormItem>
            <FormItem label="竞猜结果">
                <Input v-model="querymodal.QuestionAnswer2.QuestionTextResult" :disabled=true />
            </FormItem>
            <FormItem>
                <Row>
                        <Col span="11">
                       <input type="radio" v-model="querymodal.QuestionAnswer2.AnswerYes" value=1 :disabled=true />是                      
                        </Col>
                        <Col span="11">
                       <input type="radio" v-model="querymodal.QuestionAnswer2.AnswerYes" value=-1 :disabled=true />否    
                        </Col>
                    </Row>
            </FormItem>
             <FormItem label="题目内容3">
                <Input v-model="querymodal.QuestionAnswer3.QuestionTitle" :disabled=true />
            </FormItem>
            <FormItem label="竞猜结果">
                <Input v-model="querymodal.QuestionAnswer3.QuestionTextResult" :disabled=true />
            </FormItem>
            <FormItem>
                <Row>
                      <Col span="11">
                       <input type="radio" v-model="querymodal.QuestionAnswer3.AnswerYes" value=1 :disabled=true />是                      
                        </Col>
                        <Col span="11">
                       <input type="radio" v-model="querymodal.QuestionAnswer3.AnswerYes" value=-1 :disabled=true />否    
                        </Col>
                    </Row>
            </FormItem>           
              <FormItem label="答案公布日期">
                <Input v-model="querymodal.QuestionAnswer1.EndTime" :disabled=true />
            </FormItem>    
            </Form>        
            <div slot="footer"></div>    
             
      </Modal>

      <Modal v-model="questionmodal.visible" :loading="questionmodal.loading" title="题目编辑" okText="保存" :transfer="false" cancelText="取消" @on-ok="SaveQuestionOptionConfig"  overflow=visible  width="50%" :mask-closable="false">
        <div>
        <label>题目上线日期区间:</label>
           <DatePicker v-model="questionmodal.Question1.StartTime" type="datetime" format="yyyy-MM-dd" transfer placeholder="上架时间"></DatePicker> -
           <Date-Picker v-model="questionmodal.Question1.EndTime" type="datetime" format="yyyy-MM-dd" transfer placeholder="下架时间"></Date-Picker>       
        </div>
     <div>
            <label>题目标题1</label>
            <Input v-model="questionmodal.Question1.QuestionTitle" placeholder="请输入题目标题" style="width: 200px" />
            <lable style="margin-left:15px">竞猜截止时间：</lable>
              <Date-Picker v-model="questionmodal.Question1.DeadLineTime" type="datetime" format="yyyy-MM-dd HH:mm:ss" transfer placeholder="截止时间" ></Date-Picker>           
           </div>
         <div>
            <label>竞猜选「是」的选项</label>
           
           </div>
        <div>
        <lable>选项A：</lable>
            <Input v-model="questionmodal.Question1.YesOptionAUseIntegral" placeholder="请在此输入下注积分，必须为数字" style="width: 150px" /> 积分           
            <Input v-model="questionmodal.Question1.YesOptionAWinCouponCount" style="width:150px;margin-left:150px" placeholder="请在此输入获得兑换券，必须为数字" /> 兑换券
        </div>
        <div>
        <lable>选项B：</lable>
            <Input v-model="questionmodal.Question1.YesOptionBUseIntegral" placeholder="请在此输入下注积分，必须为数字" style="width: 150px" /> 积分           
            <Input v-model="questionmodal.Question1.YesOptionBWinCouponCount" style="width:150px;margin-left:150px" placeholder="请在此输入获得兑换券，必须为数字" /> 兑换券
        </div>
        <div>
        <lable>选项C：</lable>
            <Input v-model="questionmodal.Question1.YesOptionCUseIntegral" placeholder="请在此输入下注积分，必须为数字" style="width: 150px" /> 积分           
            <Input v-model="questionmodal.Question1.YesOptionCWinCouponCount" style="width:150px;margin-left:150px" placeholder="请在此输入获得兑换券，必须为数字" /> 兑换券
       </div>
             
            <div>
            <label>竞猜选「否」的选项</label>
            </div>
             <div>
        <lable>选项A：</lable>
            <Input v-model="questionmodal.Question1.NoOptionAUseIntegral" placeholder="请在此输入下注积分，必须为数字" style="width: 150px" /> 积分           
            <Input v-model="questionmodal.Question1.NoOptionAWinCouponCount" style="width:150px;margin-left:150px" placeholder="请在此输入获得兑换券，必须为数字" /> 兑换券
        </div>
        <div>
        <lable>选项B：</lable>
            <Input v-model="questionmodal.Question1.NoOptionBUseIntegral" placeholder="请在此输入下注积分，必须为数字" style="width: 150px" /> 积分           
            <Input v-model="questionmodal.Question1.NoOptionBWinCouponCount" style="width:150px;margin-left:150px" placeholder="请在此输入获得兑换券，必须为数字" /> 兑换券
        </div>
        <div>
        <lable>选项C：</lable>
            <Input v-model="questionmodal.Question1.NoOptionCUseIntegral" placeholder="请在此输入下注积分，必须为数字" style="width: 150px" /> 积分           
            <Input v-model="questionmodal.Question1.NoOptionCWinCouponCount" style="width:150px;margin-left:150px" placeholder="请在此输入获得兑换券，必须为数字" /> 兑换券
        </div>
        
         <div>   
            <label>题目标题2:</label>
            <Input v-model="questionmodal.Question2.QuestionTitle" placeholder="请输入题目标题" style="width: 200px" />
            <lable style="margin-left:15px">竞猜截止时间：</lable>
             <Date-Picker v-model="questionmodal.Question2.DeadLineTime" type="datetime" format="yyyy-MM-dd HH:mm:ss" transfer placeholder="截止时间" ></Date-Picker>             
        </div>
         <div>
            <label>竞猜选「是」的选项</label>
           
           </div>
        <div>
        <lable>选项A：</lable>
            <Input v-model="questionmodal.Question2.YesOptionAUseIntegral" placeholder="请在此输入下注积分，必须为数字" style="width: 150px" /> 积分           
            <Input v-model="questionmodal.Question2.YesOptionAWinCouponCount" style="width:150px;margin-left:150px" placeholder="请在此输入获得兑换券，必须为数字" /> 兑换券
        </div>
        <div>
        <lable>选项B：</lable>
            <Input v-model="questionmodal.Question2.YesOptionBUseIntegral" placeholder="请在此输入下注积分，必须为数字" style="width: 150px" /> 积分           
            <Input v-model="questionmodal.Question2.YesOptionBWinCouponCount" style="width:150px;margin-left:150px" placeholder="请在此输入获得兑换券，必须为数字" /> 兑换券
        </div>
        <div>
        <lable>选项C：</lable>
            <Input v-model="questionmodal.Question2.YesOptionCUseIntegral" placeholder="请在此输入下注积分，必须为数字" style="width: 150px" /> 积分           
            <Input v-model="questionmodal.Question2.YesOptionCWinCouponCount" style="width:150px;margin-left:150px" placeholder="请在此输入获得兑换券，必须为数字" /> 兑换券
       </div>
             
            <div>
            <label>竞猜选「否」的选项</label>
            </div>
             <div>
        <lable>选项A：</lable>
            <Input v-model="questionmodal.Question2.NoOptionAUseIntegral" placeholder="请在此输入下注积分，必须为数字" style="width: 150px" /> 积分           
            <Input v-model="questionmodal.Question2.NoOptionAWinCouponCount" style="width:150px;margin-left:150px" placeholder="请在此输入获得兑换券，必须为数字" /> 兑换券
        </div>
        <div>
        <lable>选项B：</lable>
            <Input v-model="questionmodal.Question2.NoOptionBUseIntegral" placeholder="请在此输入下注积分，必须为数字" style="width: 150px" /> 积分           
            <Input v-model="questionmodal.Question2.NoOptionBWinCouponCount" style="width:150px;margin-left:150px" placeholder="请在此输入获得兑换券，必须为数字" /> 兑换券
        </div>
        <div>
        <lable>选项C：</lable>
            <Input v-model="questionmodal.Question2.NoOptionCUseIntegral" placeholder="请在此输入下注积分，必须为数字" style="width: 150px" /> 积分           
            <Input v-model="questionmodal.Question2.NoOptionCWinCouponCount" style="width:150px;margin-left:150px" placeholder="请在此输入获得兑换券，必须为数字" /> 兑换券
        </div>
         <div>   
            <label>题目标题3:</label>
            <Input v-model="questionmodal.Question3.QuestionTitle" placeholder="请输入题目标题" style="width: 200px" />
            <lable style="margin-left:15px">竞猜截止时间：</lable>
              <Date-Picker v-model="questionmodal.Question3.DeadLineTime" type="datetime" format="yyyy-MM-dd HH:mm:ss" transfer placeholder="截止时间" placement="bottom-end" ></Date-Picker>          
        </div>
         <div>
            <label>竞猜选「是」的选项</label>
           
           </div>
        <div>
        <lable>选项A：</lable>
            <Input v-model="questionmodal.Question3.YesOptionAUseIntegral" placeholder="请在此输入下注积分，必须为数字" style="width: 150px" /> 积分           
            <Input v-model="questionmodal.Question3.YesOptionAWinCouponCount" style="width:150px;margin-left:150px" placeholder="请在此输入获得兑换券，必须为数字" /> 兑换券
        </div>
        <div>
        <lable>选项B：</lable>
            <Input v-model="questionmodal.Question3.YesOptionBUseIntegral" placeholder="请在此输入下注积分，必须为数字" style="width: 150px" /> 积分           
            <Input v-model="questionmodal.Question3.YesOptionBWinCouponCount" style="width:150px;margin-left:150px" placeholder="请在此输入获得兑换券，必须为数字" /> 兑换券
        </div>
        <div>
        <lable>选项C：</lable>
            <Input v-model="questionmodal.Question3.YesOptionCUseIntegral" placeholder="请在此输入下注积分，必须为数字" style="width: 150px" /> 积分           
            <Input v-model="questionmodal.Question3.YesOptionCWinCouponCount" style="width:150px;margin-left:150px" placeholder="请在此输入获得兑换券，必须为数字" /> 兑换券
       </div>
             
            <div>
            <label>竞猜选「否」的选项</label>
            </div>
             <div>
        <lable>选项A：</lable>
            <Input v-model="questionmodal.Question3.NoOptionAUseIntegral" placeholder="请在此输入下注积分，必须为数字" style="width: 150px" /> 积分           
            <Input v-model="questionmodal.Question3.NoOptionAWinCouponCount" style="width:150px;margin-left:150px" placeholder="请在此输入获得兑换券，必须为数字" /> 兑换券
        </div>
        <div>
        <lable>选项B：</lable>
            <Input v-model="questionmodal.Question3.NoOptionBUseIntegral" placeholder="请在此输入下注积分，必须为数字" style="width: 150px" /> 积分           
            <Input v-model="questionmodal.Question3.NoOptionBWinCouponCount" style="width:150px;margin-left:150px" placeholder="请在此输入获得兑换券，必须为数字" /> 兑换券
        </div>
        <div>
        <lable>选项C：</lable>
            <Input v-model="questionmodal.Question3.NoOptionCUseIntegral" placeholder="请在此输入下注积分，必须为数字" style="width: 150px" /> 积分           
            <Input v-model="questionmodal.Question3.NoOptionCWinCouponCount" style="width:150px;margin-left:150px" placeholder="请在此输入获得兑换券，必须为数字" /> 兑换券
        </div>

      </Modal> 
      <Modal v-model="queryquestionmodal.visible" :loading="queryquestionmodal.loading" title="题目查看" width="50%" :mask-closable="false">
        <div>
        <label>题目上线日期区间:</label>
        <DatePicker type="date" v-model="queryquestionmodal.Question1.StartTime" style="width: 150px" disabled=true></DatePicker> - 
       <DatePicker type="date" v-model="queryquestionmodal.Question1.EndTime"   style="width: 150px" disabled=true></DatePicker>
        </div>
        <div>   
            <label>题目标题1:</label>
            <Input v-model="queryquestionmodal.Question1.QuestionTitle" placeholder="请输入题目标题" style="width: 200px" disabled=true />
            <lable style="margin-left:15px">竞猜截止时间：</lable>
              <Date-Picker v-model="queryquestionmodal.Question1.DeadLineTime" type="datetime" format="yyyy-MM-dd HH:mm:ss" style="width: 200px" disabled=true></Date-Picker>           
        </div>
         <div>
            <label>竞猜选「是」的选项</label>           
           </div>
        <div>
        <lable>选项A：</lable>
            <Input v-model="queryquestionmodal.Question1.YesOptionAUseIntegral" placeholder="请在此输入下注积分，必须为数字" style="width: 150px" disabled=true /> 积分           
            <Input v-model="queryquestionmodal.Question1.YesOptionAWinCouponCount" style="width:150px;margin-left:150px" placeholder="请在此输入获得兑换券，必须为数字" disabled=true /> 兑换券
        </div>
        <div>
        <lable>选项B：</lable>
            <Input v-model="queryquestionmodal.Question1.YesOptionBUseIntegral" placeholder="请在此输入下注积分，必须为数字" style="width: 150px" disabled=true /> 积分           
            <Input v-model="queryquestionmodal.Question1.YesOptionBWinCouponCount" style="width:150px;margin-left:150px" placeholder="请在此输入获得兑换券，必须为数字" disabled=true /> 兑换券
        </div>
        <div>
        <lable>选项C：</lable>
            <Input v-model="queryquestionmodal.Question1.YesOptionCUseIntegral" placeholder="请在此输入下注积分，必须为数字" style="width: 150px" disabled=true /> 积分           
            <Input v-model="queryquestionmodal.Question1.YesOptionCWinCouponCount" style="width:150px;margin-left:150px" placeholder="请在此输入获得兑换券，必须为数字" disabled=true /> 兑换券
       </div>             
            <div>
            <label>竞猜选「否」的选项</label>
            </div>
             <div>
        <lable>选项A：</lable>
            <Input v-model="queryquestionmodal.Question1.NoOptionAUseIntegral" placeholder="请在此输入下注积分，必须为数字" style="width: 150px" disabled=true /> 积分           
            <Input v-model="queryquestionmodal.Question1.NoOptionAWinCouponCount" style="width:150px;margin-left:150px" placeholder="请在此输入获得兑换券，必须为数字" disabled=true /> 兑换券
        </div>
        <div>
        <lable>选项B：</lable>
            <Input v-model="queryquestionmodal.Question1.NoOptionBUseIntegral" placeholder="请在此输入下注积分，必须为数字" style="width: 150px" disabled=true /> 积分           
            <Input v-model="queryquestionmodal.Question1.NoOptionBWinCouponCount" style="width:150px;margin-left:150px" placeholder="请在此输入获得兑换券，必须为数字" disabled=true /> 兑换券
        </div>
        <div>
        <lable>选项C：</lable>
            <Input v-model="queryquestionmodal.Question1.NoOptionCUseIntegral" placeholder="请在此输入下注积分，必须为数字" style="width: 150px" disabled=true /> 积分           
            <Input v-model="queryquestionmodal.Question1.NoOptionCWinCouponCount" style="width:150px;margin-left:150px" placeholder="请在此输入获得兑换券，必须为数字" disabled=true /> 兑换券
        </div>  
         <div>   
            <label>题目标题2:</label>
            <Input v-model="queryquestionmodal.Question2.QuestionTitle" placeholder="请输入题目标题" style="width: 200px" disabled=true />
            <lable style="margin-left:15px">竞猜截止时间：</lable>
              <Date-Picker v-model="queryquestionmodal.Question2.DeadLineTime" type="datetime" format="yyyy-MM-dd HH:mm:ss" style="width: 200px" disabled=true></Date-Picker>           
        </div>
         <div>
            <label>竞猜选「是」的选项</label>           
           </div>
        <div>
        <lable>选项A：</lable>
            <Input v-model="queryquestionmodal.Question2.YesOptionAUseIntegral" placeholder="请在此输入下注积分，必须为数字" style="width: 150px" disabled=true /> 积分           
            <Input v-model="queryquestionmodal.Question2.YesOptionAWinCouponCount" style="width:150px;margin-left:150px" placeholder="请在此输入获得兑换券，必须为数字" disabled=true /> 兑换券
        </div>
        <div>
        <lable>选项B：</lable>
            <Input v-model="queryquestionmodal.Question2.YesOptionBUseIntegral" placeholder="请在此输入下注积分，必须为数字" style="width: 150px" disabled=true /> 积分           
            <Input v-model="queryquestionmodal.Question2.YesOptionBWinCouponCount" style="width:150px;margin-left:150px" placeholder="请在此输入获得兑换券，必须为数字" disabled=true /> 兑换券
        </div>
        <div>
        <lable>选项C：</lable>
            <Input v-model="queryquestionmodal.Question2.YesOptionCUseIntegral" placeholder="请在此输入下注积分，必须为数字" style="width: 150px" disabled=true /> 积分           
            <Input v-model="queryquestionmodal.Question2.YesOptionCWinCouponCount" style="width:150px;margin-left:150px" placeholder="请在此输入获得兑换券，必须为数字" disabled=true /> 兑换券
       </div>             
            <div>
            <label>竞猜选「否」的选项</label>
            </div>
             <div>
        <lable>选项A：</lable>
            <Input v-model="queryquestionmodal.Question2.NoOptionAUseIntegral" placeholder="请在此输入下注积分，必须为数字" style="width: 150px" disabled=true /> 积分           
            <Input v-model="queryquestionmodal.Question2.NoOptionAWinCouponCount" style="width:150px;margin-left:150px" placeholder="请在此输入获得兑换券，必须为数字" disabled=true /> 兑换券
        </div>
        <div>
        <lable>选项B：</lable>
            <Input v-model="queryquestionmodal.Question2.NoOptionBUseIntegral" placeholder="请在此输入下注积分，必须为数字" style="width: 150px"  disabled=true /> 积分           
            <Input v-model="queryquestionmodal.Question2.NoOptionBWinCouponCount" style="width:150px;margin-left:150px" placeholder="请在此输入获得兑换券，必须为数字" disabled=true /> 兑换券
        </div>
        <div>
        <lable>选项C：</lable>
            <Input v-model="queryquestionmodal.Question2.NoOptionCUseIntegral" placeholder="请在此输入下注积分，必须为数字" style="width: 150px" disabled=true /> 积分           
            <Input v-model="queryquestionmodal.Question2.NoOptionCWinCouponCount" style="width:150px;margin-left:150px" placeholder="请在此输入获得兑换券，必须为数字"  disabled=true /> 兑换券
        </div>  

         <div>   
            <label>题目标题3:</label>
            <Input v-model="queryquestionmodal.Question3.QuestionTitle" placeholder="请输入题目标题" style="width: 200px" disabled=true />
            <lable style="margin-left:15px">竞猜截止时间：</lable>
              <Date-Picker v-model="queryquestionmodal.Question3.DeadLineTime" type="datetime" format="yyyy-MM-dd HH:mm:ss" style="width: 200px" disabled=true></Date-Picker>           
        </div>
         <div>
            <label>竞猜选「是」的选项</label>           
           </div>
        <div>
        <lable>选项A：</lable>
            <Input v-model="queryquestionmodal.Question3.YesOptionAUseIntegral" placeholder="请在此输入下注积分，必须为数字" style="width: 150px" disabled=true /> 积分           
            <Input v-model="queryquestionmodal.Question3.YesOptionAWinCouponCount" style="width:150px;margin-left:150px" placeholder="请在此输入获得兑换券，必须为数字" disabled=true /> 兑换券
        </div>
        <div>
        <lable>选项B：</lable>
            <Input v-model="queryquestionmodal.Question3.YesOptionBUseIntegral" placeholder="请在此输入下注积分，必须为数字" style="width: 150px" disabled=true /> 积分           
            <Input v-model="queryquestionmodal.Question3.YesOptionBWinCouponCount" style="width:150px;margin-left:150px" placeholder="请在此输入获得兑换券，必须为数字" disabled=true /> 兑换券
        </div>
        <div>
        <lable>选项C：</lable>
            <Input v-model="queryquestionmodal.Question3.YesOptionCUseIntegral" placeholder="请在此输入下注积分，必须为数字" style="width: 150px" disabled=true /> 积分           
            <Input v-model="queryquestionmodal.Question3.YesOptionCWinCouponCount" style="width:150px;margin-left:150px" placeholder="请在此输入获得兑换券，必须为数字" disabled=true /> 兑换券
       </div>             
            <div>
            <label>竞猜选「否」的选项</label>
            </div>
             <div>
        <lable>选项A：</lable>
            <Input v-model="queryquestionmodal.Question3.NoOptionAUseIntegral" placeholder="请在此输入下注积分，必须为数字" style="width: 150px" disabled=true /> 积分           
            <Input v-model="queryquestionmodal.Question3.NoOptionAWinCouponCount" style="width:150px;margin-left:150px" placeholder="请在此输入获得兑换券，必须为数字" disabled=true /> 兑换券
        </div>
        <div>
        <lable>选项B：</lable>
            <Input v-model="queryquestionmodal.Question3.NoOptionBUseIntegral" placeholder="请在此输入下注积分，必须为数字" style="width: 150px" disabled=true /> 积分           
            <Input v-model="queryquestionmodal.Question3.NoOptionBWinCouponCount" style="width:150px;margin-left:150px" placeholder="请在此输入获得兑换券，必须为数字" disabled=true /> 兑换券
        </div>
        <div>
        <lable>选项C：</lable>
            <Input v-model="queryquestionmodal.Question3.NoOptionCUseIntegral" placeholder="请在此输入下注积分，必须为数字" style="width: 150px" disabled=true /> 积分           
            <Input v-model="queryquestionmodal.Question3.NoOptionCWinCouponCount" style="width:150px;margin-left:150px" placeholder="请在此输入获得兑换券，必须为数字" disabled=true /> 兑换券
        </div>  
         <div slot="footer"></div>      
      </Modal>           
  </div>
</template>
<style>
body .ivu-modal .ivu-select-dropdown {
    position: fixed !important;
}
</style>
<script>
  export default {
    data () {
        return {
          index: 0,
          isAdd: 1,
          page: {
            total: 0,
            current: 1,
            pageSize: 10
          },
          logpage: {
            total: 0,
            current: 1,
            pageSize: 5
          },
          filters: {
            endTime: '',
            QuestionConfirm: -1
          },
          tablemodel: {
            loading: true,
            visible: false,
            width: 1355,
            data: []
          },
          logmodal: {
              loading: true,
              visible: false,
              width: 635,
              data: [],
              columns: [
                  {
                      title: "操作内容",
                      width: 300,
                      key: "Remark",
                      align: "center",
                      fixed: "left"
                  },
                  {
                      title: "操作人",
                      width: 150,
                      key: "Creator",
                      align: "center",
                      fixed: "left"
                  },
                  {
                      title: "时间",
                      width: 150,
                      key: "CreateDateTime",
                      render: (h, params) => {
                          return h(
                              "span",
                              this.formatDate(params.row.CreateDateTime)
                          );
                      }
                  }                  
              ]
          },
          deleteModal: {
            visible: false,
            loading: true,
            endTime: ""
        },
         releaseModal: {
            visible: false,
            loading: true,
            endTime: ""
        },
          table: {
            loading: true,
            data: [],
            columns: [
              {
                title: "#",
                width: 50,
                align: "center",
                fixed: "left",
                type: "index",
                key: "PKID"
              },
              {
                title: "题目名称",
                  align: "center",
                key: "QuestionTitle"
               
              },
              {
                title: "竞猜结果",
                 align: "center",
                key: "QuestionConfirm",
                 render: (h, params) => {
                          return h(
                              "span",
                              this.ConvertValue(params.row.QuestionConfirm)
                          );
                      }
              },
              {
                title: "题目上线日期",
                 align: "center",
                key: "StartTime"               
              },
              {
                title: "题目下线日期",
                 align: "center",
                key: "EndTime",
                 render: (h, params) => {
                          return h(
                              "span",
                              this.formatDate(params.row.EndTime)
                          );
                      }
              }, 
              {
                title: "是否删除",     
                  align: "center",          
                key: "IsDeleted",
                 render: (h, params) => {
                          return h(
                              "span",
                              this.ConvertBoolValue(params.row.IsDeleted)
                          );
                      }
              }, 
              {
                        title: "题目操作",
                        key: "action",
                        align: "center",
                        width: 200,                      
                        render: (h, params) => {
                            return h("div", [
                                h(
                                    "Button",
                                    {
                                        props: {

                                            type: "primary",
                                            size: "small"
                                        },
                                        style: {
                                            marginRight: "5px"
                                        },
                                        on: {
                                            click: () => {
                                                this.QueryQuestionWithOption(
                                                    params.row.EndTime,
                                                    params.row.IsDeleted,
                                                    "Search"
                                                );
                                            }
                                        }
                                    },
                                    "查看"
                                ),
                                h(
                                    "Button",
                                    {
                                        props: {                                            
                                            type: "primary",
                                            size: "small",
                                            disabled: new Date(params.row.StartTime) < new Date() || params.row.IsDeleted
                                        },
                                        style: {
                                            marginRight: "5px"
                                        },
                                        on: {
                                            click: () => {
                                                  if (new Date(params.row.StartTime) < new Date()) {
                                                this.$Message.warning("当前题目已上线，不支持修改");
                                                return;
                                                  }
                                                this.searchQuestionWithOption(
                                                    params.row.EndTime,
                                                    "Update"
                                                );
                                            }
                                        }
                                    },
                                    "修改"
                                ),
                                h(
                                    "Button",
                                    {
                                        props: {
                                            type: "primary",
                                            size: "small",
                                            disabled: new Date(params.row.StartTime) < new Date() || params.row.IsDeleted
                                        },
                                        on: {
                                            click: () => {
                                                if (new Date(params.row.StartTime) < new Date()) {
                                                this.$Message.warning({
                                                   content: "当前题目已上线，不支持删除",
                                                   duration: 3
                                                });
                                                this.deleteModal.visible = false;               
                                               } else {
                                                this.deleteModal.visible = true;
                                                this.deleteModal.endTime = params.row.EndTime;
                                               }
                                            }    
                                        }                                    
                                    },
                                    "删除"
                                )
                            ]);
                        }
                    },           
              {
                title: "答案操作",
                width: 300,
                align: "center",
                render: (h, params) => {
                  return h("div", [
                      h(
                          "Button",
                          {
                              props: {
                                  type: "primary",
                                  size: "small",
                                  disabled: (params.row.QuestionConfirm === 2) || (params.row.IsDeleted)
                              },
                              style: {
                                  marginRight: "5px"
                              },
                              on: {
                                  click: () => {                                     
                                      this.search(
                                          params.row.EndTime,
                                          "Search"
                                      )
                                  }
                              }
                          },
                          "编辑答案"
                      ),
                      h(
                          "Button",
                          {
                              props: {
                                  type: "primary",
                                  size: "small"                                
                              },
                              style: {
                                  marginRight: "5px"
                              },
                              on: {
                                    click: () => {                                     
                                      this.searchquerymodel(
                                          params.row.EndTime,
                                          "Search"
                                      )
                                  }                                                                  
                              }
                          },
                          "查看答案"
                      ),
                      h(
                          "Button",
                          {
                              props: {
                                  type: "primary",
                                  size: "small",
                                  disabled: (!(params.row.QuestionConfirm === 1) || params.row.IsDeleted)
                              },
                              style: {
                                  marginRight: "5px"
                              },
                              on: {
                                  click: () => {                                     
                                          this.releaseModal.visible = true;
                                         this.releaseModal.endTime = params.row.EndTime;
                                  }
                              }
                          },
                          "公布答案"
                      )
                  ])
                }
              },             
              {
                title: "日志查看",
              
                align: "center",
                render: (h, params) => {
                  return h("div", [
                      h(
                          "Button",
                          {
                              props: {
                                  type: "primary",
                                  size: "small"                                 
                              },
                              style: {
                                  marginRight: "5px"
                              },
                              on: {
                                  click: () => {                                     
                                      this.SearchLog(
                                          params.row.PKID,
                                          "Search"
                                      )
                                  }
                              }
                          },
                          "查看日志"
                      )
                  ])
                }
              }
              
            ]
          },

          gameStatusList: [
                {
                    label: "未添加答案",
                    value: 0
                },
                {
                    label: "已添加答案",
                    value: 1
                },
                {
                    label: "已公布",
                    value: 2
                },
                {
                    label: "所有状态",
                    value: -1
                }
            ],
          modal: {
            visible: false,
            loading: true,
            disableedit: true,            
            QuestionAnswer1: {
                PKID: 0,
                QuestionTitle: "",
                StartTime: "",
                EndTime: "",              
                QuestionTextResult: "",
                AnswerYes: 0,
                AnswerNo: 0,
                QuestionConfirm: 0,
                DeadLineTime: ""               
            },
             QuestionAnswer2: {
                PKID: 0,
                QuestionTitle: "",
                StartTime: "",
                EndTime: "",              
                QuestionTextResult: "",
                AnswerYes: 0,
                AnswerNo: 0,
                QuestionConfirm: 0,
                DeadLineTime: ""               
            },
             QuestionAnswer3: {
                PKID: 0,
                QuestionTitle: "",
                StartTime: "",
                EndTime: "",              
                QuestionTextResult: "",
                AnswerYes: 0,
                AnswerNo: 0,
                QuestionConfirm: 0,
                DeadLineTime: ""               
            }
          }, 
          querymodal: {
            visible: false,
            loading: true,
            disableedit: true,            
            QuestionAnswer1: {
                PKID: 0,
                QuestionTitle: "",
                StartTime: "",
                EndTime: "",              
                QuestionTextResult: "",
                AnswerYes: 0,
                AnswerNo: 0,
                QuestionConfirm: 0,
                DeadLineTime: ""               
            },
             QuestionAnswer2: {
                PKID: 0,
                QuestionTitle: "",
                StartTime: "",
                EndTime: "",              
                QuestionTextResult: "",
                AnswerYes: 0,
                AnswerNo: 0,
                QuestionConfirm: 0,
                DeadLineTime: ""               
            },
             QuestionAnswer3: {
                PKID: 0,
                QuestionTitle: "",
                StartTime: "",
                EndTime: "",              
                QuestionTextResult: "",
                AnswerYes: 0,
                AnswerNo: 0,
                QuestionConfirm: 0,
                DeadLineTime: ""               
            }
          },      
        questionmodal: {
            visible: false,
            loading: true,
            disableedit: true,            
            Question1: {
                PKID: 0,
                QuestionTitle: "",
                StartTime: "",
                EndTime: "",                             
                DeadLineTime: "",
                YesOptionAPKID: 0,
                YesOptionAUseIntegral: 0,
                YesOptionAWinCouponCount: 0,
                YesOptionBPKID: 0,
                YesOptionBUseIntegral: 0,
                YesOptionBWinCouponCount: 0,
                YesOptionCPKID: 0,
                YesOptionCUseIntegral: 0,
                YesOptionCWinCouponCount: 0,
                NoOptionAPKID: 0,
                NoOptionAUseIntegral: 0,
                NoOptionAWinCouponCount: 0,
                NoOptionBPKID: 0,
                NoOptionBUseIntegral: 0,
                NoOptionBWinCouponCount: 0,
                NoOptionCPKID: 0,
                NoOptionCUseIntegral: 0,
                NoOptionCWinCouponCount: 0
            },
            Question2: {
                PKID: 0,
                QuestionTitle: "",
                StartTime: "",
                EndTime: "",                             
                DeadLineTime: "",
                YesOptionAPKID: 0,
                YesOptionAUseIntegral: 0,
                YesOptionAWinCouponCount: 0,
                YesOptionBPKID: 0,
                YesOptionBUseIntegral: 0,
                YesOptionBWinCouponCount: 0,
                YesOptionCPKID: 0,
                YesOptionCUseIntegral: 0,
                YesOptionCWinCouponCount: 0,
                NoOptionAPKID: 0,
                NoOptionAUseIntegral: 0,
                NoOptionAWinCouponCount: 0,
                NoOptionBPKID: 0,
                NoOptionBUseIntegral: 0,
                NoOptionBWinCouponCount: 0,
                NoOptionCPKID: 0,
                NoOptionCUseIntegral: 0,
                NoOptionCWinCouponCount: 0
            },
            Question3: {
               PKID: 0,
                QuestionTitle: "",
                StartTime: "",
                EndTime: "",                             
                DeadLineTime: "",
                YesOptionAPKID: 0,
                YesOptionAUseIntegral: 0,
                YesOptionAWinCouponCount: 0,
                YesOptionBPKID: 0,
                YesOptionBUseIntegral: 0,
                YesOptionBWinCouponCount: 0,
                YesOptionCPKID: 0,
                YesOptionCUseIntegral: 0,
                YesOptionCWinCouponCount: 0,
                NoOptionAPKID: 0,
                NoOptionAUseIntegral: 0,
                NoOptionAWinCouponCount: 0,
                NoOptionBPKID: 0,
                NoOptionBUseIntegral: 0,
                NoOptionBWinCouponCount: 0,
                NoOptionCPKID: 0,
                NoOptionCUseIntegral: 0,
                NoOptionCWinCouponCount: 0
            }
          },
        queryquestionmodal: {
            visible: false,
            loading: true,
            disableedit: true,            
            Question1: {
                PKID: 0,
                QuestionTitle: "",
                StartTime: "",
                EndTime: "",                             
                DeadLineTime: "",
                YesOptionAPKID: 0,
                YesOptionAUseIntegral: 0,
                YesOptionAWinCouponCount: 0,
                YesOptionBPKID: 0,
                YesOptionBUseIntegral: 0,
                YesOptionBWinCouponCount: 0,
                YesOptionCPKID: 0,
                YesOptionCUseIntegral: 0,
                YesOptionCWinCouponCount: 0,
                NoOptionAPKID: 0,
                NoOptionAUseIntegral: 0,
                NoOptionAWinCouponCount: 0,
                NoOptionBPKID: 0,
                NoOptionBUseIntegral: 0,
                NoOptionBWinCouponCount: 0,
                NoOptionCPKID: 0,
                NoOptionCUseIntegral: 0,
                NoOptionCWinCouponCount: 0
            },
            Question2: {
                PKID: 0,
                QuestionTitle: "",
                StartTime: "",
                EndTime: "",                             
                DeadLineTime: "",
                YesOptionAPKID: 0,
                YesOptionAUseIntegral: 0,
                YesOptionAWinCouponCount: 0,
                YesOptionBPKID: 0,
                YesOptionBUseIntegral: 0,
                YesOptionBWinCouponCount: 0,
                YesOptionCPKID: 0,
                YesOptionCUseIntegral: 0,
                YesOptionCWinCouponCount: 0,
                NoOptionAPKID: 0,
                NoOptionAUseIntegral: 0,
                NoOptionAWinCouponCount: 0,
                NoOptionBPKID: 0,
                NoOptionBUseIntegral: 0,
                NoOptionBWinCouponCount: 0,
                NoOptionCPKID: 0,
                NoOptionCUseIntegral: 0,
                NoOptionCWinCouponCount: 0
            },
            Question3: {
               PKID: 0,
                QuestionTitle: "",
                StartTime: "",
                EndTime: "",                             
                DeadLineTime: "",
                YesOptionAPKID: 0,
                YesOptionAUseIntegral: 0,
                YesOptionAWinCouponCount: 0,
                YesOptionBPKID: 0,
                YesOptionBUseIntegral: 0,
                YesOptionBWinCouponCount: 0,
                YesOptionCPKID: 0,
                YesOptionCUseIntegral: 0,
                YesOptionCWinCouponCount: 0,
                NoOptionAPKID: 0,
                NoOptionAUseIntegral: 0,
                NoOptionAWinCouponCount: 0,
                NoOptionBPKID: 0,
                NoOptionBUseIntegral: 0,
                NoOptionBWinCouponCount: 0,
                NoOptionCPKID: 0,
                NoOptionCUseIntegral: 0,
                NoOptionCWinCouponCount: 0
            }
        }    
        }                  
    },
    created: function () {           
        this.$Message.config({
                top: 50,
                duration: 3
            });
      this.loadData(1);
    },
    methods: {
      loadData (pageIndex) {
        this.page.current = pageIndex;
        this.table.loading = true;
        var requestData = "?endTime=" + this.formatDate(this.filters.endTime);
           requestData += "&QuestionConfirm=" + this.filters.QuestionConfirm;
        requestData += "&pageIndex=" + this.page.current;
        requestData += "&pageSize=" + this.page.pageSize;
        this.ajax
            .get("/GuessGame/GetQuestionList" + requestData)
            .then(response => {
              var data = response.data;
              this.page.total = data.totalCount;
              this.table.data = data.data;
              this.table.loading = false;
            });
      },
      search (time) {
        if (time) {
           if (new Date(this.formatDate(time)) < new Date()) {
               this.modal.disableedit = true;
           } else {
               this.modal.disableedit = false;
           }
           
            this.ajax
          .get("/GuessGame/GetQuestionAnswerList?endTime=" + this.formatDate(time))
          .then(response => {
              console.log(response.data);
              console.log(response.data.data[0]);
              console.log(response.data.data[1]);
              if (response.data.data[0]) {
                this.modal.QuestionAnswer1 = response.data.data[0];                               
                this.modal.visible = true;
              } 
              if (response.data.data[1]) {
                this.modal.QuestionAnswer2 = response.data.data[1];                
                this.modal.visible = true;
              } 
              if (response.data.data[2]) {
                this.modal.QuestionAnswer3 = response.data.data[2];
                this.modal.visible = true;
              }
          });
        } 
      },
     RefreshQuestionCache () {
       this.ajax.get("/GuessGame/RefreshQuestion")
              .then((response) => {
                  console.log(response);
                  if (!response.data) {                  
                        this.$Message.success('刷新缓存成功');
                        this.loadData(this.page.current);                            
                  } else {                   
                    this.$Message.error('刷新缓存失败');     
                  }              
              });  
    },
      searchquerymodel (time) {
            this.ajax
          .get("/GuessGame/GetQuestionAnswerList?endTime=" + this.formatDate(time))
          .then(response => {             
              if (response.data.data[0]) {
                this.querymodal.QuestionAnswer1 = response.data.data[0];
                                               
                this.querymodal.visible = true;
                console.log(this.querymodal.QuestionAnswer1);
              } 
              if (response.data.data[1]) {
                this.querymodal.QuestionAnswer2 = response.data.data[1];                
                this.querymodal.visible = true;
              } 
              if (response.data.data[2]) {
                this.querymodal.QuestionAnswer3 = response.data.data[2];
                this.querymodal.visible = true;
              }
          });
      },
      deleteok () {        
        this.deleteModal.loading = true;
        this.ajax.get("/GuessGame/DeleteQuestionWithOptionList?endTime=" + this.formatDate(this.deleteModal.endTime)
              ).then((response) => {
                  console.log(response);
                  if (!response.data) {                  
                        this.$Message.success('删除成功');
                        this.loadData(this.page.current);
                        this.deleteModal.loading = false;
                        this.deleteModal.visible = false;
                        this.$nextTick(() => {
                            this.deleteModal.loading = true;
                        });                  
                  } else {
                      console.log(this.deleteModal);
                    this.$Message.error('删除失败');
                    this.deleteModal.loading = false;
                    this.$nextTick(() => {
                        this.deleteModal.loading = true;
                    });
                  }
              });           
    },
       releaseok () {        
        this.deleteModal.loading = true;
        this.ajax.get("/GuessGame/ReleaseQuestionList?endTime=" + this.releaseModal.endTime
              ).then((response) => {               
                  if (!response.data) {                  
                        this.$Message.success('公布答案成功');
                        this.loadData(this.page.current);
                        this.releaseModal.loading = false;
                        this.releaseModal.visible = false;
                        this.$nextTick(() => {
                            this.releaseModal.loading = true;
                        });                  
                  } else {                     
                    this.$Message.error('公布答案失败');
                    this.releaseModal.loading = false;
                    this.$nextTick(() => {
                        this.releaseModal.loading = true;
                    });
                  }
              });           
    },
      searchQuestionWithOption (time) {        
           var starttime = '';
        if (time) {          
           var that = this;
           if (new Date(this.formatDate(time)) < new Date()) {
                 that.$Message.warning("当前题目已上线，不支持修改");
                 that.questionmodal.visible = false;               
           } else {            
            this.ajax
          .get("/GuessGame/GetQuestionWithOptionList?endTime=" + this.formatDate(time))
          .then(response => {
              if (response.data.data[0]) {
                that.questionmodal.Question1 = response.data.data[0];                                                  
                that.questionmodal.visible = true;
              } 
              if (response.data.data[1]) {
                that.questionmodal.Question2 = response.data.data[1];
                that.questionmodal.visible = true;
              } 
              if (response.data.data[2]) {
                that.questionmodal.Question3 = response.data.data[2];
                that.questionmodal.visible = true;
              }
          });
        }
        } else {
              this.questionmodal.visible = true;              
                this.ajax
          .get("/GuessGame/GetLastestQustion")
          .then(response => {
              console.log(1);
              console.log(response);
             // console.log(response.data);              
              starttime = response.data;     
              console.log(starttime);
                this.questionmodal.Question1 = {
                PKID: 0,
                QuestionTitle: "",  
                StartTime: starttime,             
                EndTime: "",                             
                DeadLineTime: "",
                YesOptionAPKID: 0,
                YesOptionAUseIntegral: 10,
                YesOptionAWinCouponCount: 1,
                YesOptionBPKID: 0,
                YesOptionBUseIntegral: 20,
                YesOptionBWinCouponCount: 2,
                YesOptionCPKID: 0,
                YesOptionCUseIntegral: 30,
                YesOptionCWinCouponCount: 3,
                NoOptionAPKID: 0,
                NoOptionAUseIntegral: 10,
                NoOptionAWinCouponCount: 1,
                NoOptionBPKID: 0,
                NoOptionBUseIntegral: 20,
                NoOptionBWinCouponCount: 2,
                NoOptionCPKID: 0,
                NoOptionCUseIntegral: 30,
                NoOptionCWinCouponCount: 3
              };
              this.questionmodal.Question2 = {
                PKID: 0,
                QuestionTitle: "",
                StartTime: "",
                EndTime: "",                             
                DeadLineTime: "",
                YesOptionAPKID: 0,
                YesOptionAUseIntegral: 10,
                YesOptionAWinCouponCount: 1,
                YesOptionBPKID: 0,
                YesOptionBUseIntegral: 20,
                YesOptionBWinCouponCount: 2,
                YesOptionCPKID: 0,
                YesOptionCUseIntegral: 30,
                YesOptionCWinCouponCount: 3,
                NoOptionAPKID: 0,
                NoOptionAUseIntegral: 10,
                NoOptionAWinCouponCount: 1,
                NoOptionBPKID: 0,
                NoOptionBUseIntegral: 20,
                NoOptionBWinCouponCount: 2,
                NoOptionCPKID: 0,
                NoOptionCUseIntegral: 30,
                NoOptionCWinCouponCount: 3
              };
              this.questionmodal.Question3 = {
                PKID: 0,
                QuestionTitle: "",
                StartTime: "",
                EndTime: "",                             
                DeadLineTime: "",
                YesOptionAPKID: 0,
                YesOptionAUseIntegral: 10,
                YesOptionAWinCouponCount: 1,
                YesOptionBPKID: 0,
                YesOptionBUseIntegral: 20,
                YesOptionBWinCouponCount: 2,
                YesOptionCPKID: 0,
                YesOptionCUseIntegral: 30,
                YesOptionCWinCouponCount: 3,
                NoOptionAPKID: 0,
                NoOptionAUseIntegral: 10,
                NoOptionAWinCouponCount: 1,
                NoOptionBPKID: 0,
                NoOptionBUseIntegral: 20,
                NoOptionBWinCouponCount: 2,
                NoOptionCPKID: 0,
                NoOptionCUseIntegral: 30,
                NoOptionCWinCouponCount: 3
              };
               console.log(this.questionmodal.Question1.StartTime);
          });
        }
      },
      SearchLog (PKID) {        
        this.logmodal.loading = true;        
        this.ajax
            .post("/CommonConfigLog/GetCommonConfigLogs", {
                objectType: "WorldCupConfig",
                objectId: PKID
            })
            .then(response => {     
                if (response.data) {
                this.logmodal.data = response.data;                              
                } else {
                     this.logmodal.data = [];
                }
                 this.logmodal.visible = true;
                this.logmodal.loading = false;
            });
      },
      ReleaseQuestion () {
        var that = this;
        if (!that.modal.QuestionAnswer1.QuestionTitle || !that.modal.QuestionAnswer1.QuestionTextResult || !that.modal.QuestionAnswer2.QuestionTitle || !that.modal.QuestionAnswer2.QuestionTextResult || !that.modal.QuestionAnswer3.QuestionTitle || !that.modal.QuestionAnswer3.QuestionTextResult) {
          that.$Message.warning("三题标题内容和竞猜结果不允许为空");
          that.$nextTick(
            () => {
                    that.modal.loading = true;
                  }
          );
          that.modal.loading = false;
          return;
        }
       that.releaseModal.visible = true;
       that.releaseModal.endTime = that.modal.QuestionAnswer1.EndTime;
      },
      FetchCouponInfo (PID, index) {
        var that = this;
        if (PID) {
          that.ajax
          .post("/GroupBuyingV2/FetchCouponInfo", {
              couponId: PID
          })
          .then(response => {
            if (response.data) {
              that.modal.productConfig.Coupons[index].AvailablePeriod = response.data.AvailablePeriod;
              that.modal.productConfig.Coupons[index].CouponDescription = response.data.CouponDescription;
              that.modal.productConfig.Coupons[index].CouponId = response.data.CouponId;
              that.modal.productConfig.Coupons[index].CouponLeastPrice = response.data.CouponLeastPrice;
              that.modal.productConfig.Coupons[index].CouponName = response.data.CouponName;
              that.modal.productConfig.Coupons[index].CouponPrice = response.data.CouponPrice;
            } else {
              that.$Message.warning("优惠券 PID 无效");
            } 
          });
        }    
      },
      SaveQuestionConfig () {
        var that = this;
         if (!that.modal.QuestionAnswer1.QuestionTitle || !that.modal.QuestionAnswer1.QuestionTextResult || !that.modal.QuestionAnswer2.QuestionTitle || !that.modal.QuestionAnswer2.QuestionTextResult || !that.modal.QuestionAnswer3.QuestionTitle || !that.modal.QuestionAnswer3.QuestionTextResult) {
          that.$Message.warning("三题标题内容和竞猜结果不允许为空");
          that.$nextTick(
            () => {
                    that.modal.loading = true;
                  }
          );
          that.modal.loading = false;
          return;
        }
        that.modal.loading = true;
        that.ajax
            .post("/GuessGame/UpdateQuestionList", {
                question1: that.modal.QuestionAnswer1,
                question2: that.modal.QuestionAnswer2,
                question3: that.modal.QuestionAnswer3
            })
            .then(response => {
                if (!response.data) {
                    that.$Message.success("操作成功");
                    that.modal.visible = false;
                    that.loadData(that.page.current);
                } else {
                    that.$Message.error(response.data);
                    that.$nextTick(() => {
                        that.modal.loading = true;
                    });
                }
                that.modal.loading = false;
            });
      },
      SaveQuestionOptionConfig () {
        var that = this; 
        if (!this.questionmodal.Question1.StartTime || !this.questionmodal.Question1.EndTime || !this.questionmodal.Question1.DeadLineTime) {    
            that.$Message.warning("第一题题目结束时间,开始时间,竞猜截至时间不能为空,请设置");
             that.$nextTick(
            () => {
                    that.questionmodal.loading = true;
                  }
          );
          that.questionmodal.loading = false;
          return;
        }       
        this.questionmodal.Question1.StartTime = this.formatDate(this.questionmodal.Question1.StartTime).replace("00:00:00", "11:00:00");
        this.questionmodal.Question1.EndTime = this.formatDate(this.questionmodal.Question1.EndTime).replace("00:00:00", "11:00:00");
        this.questionmodal.Question1.DeadLineTime = this.formatDate(this.questionmodal.Question1.DeadLineTime);  
        console.log(this.questionmodal.Question1.StartTime);
        console.log(this.questionmodal.Question1.EndTime);
        console.log(this.questionmodal.Question1.DeadLineTime);
        if (!this.isLater(this.questionmodal.Question1.EndTime, this.questionmodal.Question1.StartTime)) {
             that.$Message.warning("第一题题目结束时间不能早于开始时间");
                that.$nextTick(
            () => {
                    that.questionmodal.loading = true;
                  }
          );
          that.questionmodal.loading = false;
           return;
        }
        console.log(this.isLater(this.questionmodal.Question1.EndTime, this.questionmodal.Question1.DeadLineTime));
        console.log(this.isLater(this.questionmodal.Question1.DeadLineTime, this.questionmodal.Question1.StartTime));
        if (!this.isLater(this.questionmodal.Question1.EndTime, this.questionmodal.Question1.DeadLineTime) || !this.isLater(this.questionmodal.Question1.DeadLineTime, this.questionmodal.Question1.StartTime)) {       
              that.$Message.warning("第一题题目竞猜截至时间请介于开始时间和结束时间之间");
                that.$nextTick(
            () => {
                    that.questionmodal.loading = true;
                  }
          );
          that.questionmodal.loading = false;
           return;
        }
          if (!this.questionmodal.Question1.QuestionTitle) {    
            that.$Message.warning("第一题题目内容不能为空");
             that.$nextTick(
            () => {
                    that.questionmodal.loading = true;
                  }
          );
          that.questionmodal.loading = false;
          return;
        }       
        console.log(!isNaN(this.questionmodal.Question1.YesOptionAUseIntegral));
        console.log(this.questionmodal.Question1.YesOptionAUseIntegral <= 0);
        if ((isNaN(this.questionmodal.Question1.YesOptionAUseIntegral) || this.questionmodal.Question1.YesOptionAUseIntegral <= 0) || ((isNaN(this.questionmodal.Question1.YesOptionAWinCouponCount) || this.questionmodal.Question1.YesOptionAWinCouponCount <= 0))) {       
             that.$Message.warning("第一题题目是的选项A的积分值和兑换券必须为正整数");
                  that.$nextTick(
            () => {
                    that.questionmodal.loading = true;
                  }
          );
          that.questionmodal.loading = false;
           return;
        }
         if ((isNaN(this.questionmodal.Question1.YesOptionBUseIntegral) || this.questionmodal.Question1.YesOptionBUseIntegral <= 0) || ((isNaN(this.questionmodal.Question1.YesOptionBWinCouponCount) || this.questionmodal.Question1.YesOptionBWinCouponCount <= 0))) {       
             that.$Message.warning("第一题题目是的选项B的积分值和兑换券必须为正整数");
                  that.$nextTick(
            () => {
                    that.questionmodal.loading = true;
                  }
          );
          that.questionmodal.loading = false;
           return;
        }
         if ((isNaN(this.questionmodal.Question1.YesOptionCUseIntegral) || this.questionmodal.Question1.YesOptionCUseIntegral <= 0) || ((isNaN(this.questionmodal.Question1.YesOptionCWinCouponCount) || this.questionmodal.Question1.YesOptionCWinCouponCount <= 0))) {       
             that.$Message.warning("第一题题目是的选项C的积分值和兑换券必须为正整数");
                  that.$nextTick(
            () => {
                    that.questionmodal.loading = true;
                  }
          );
          that.questionmodal.loading = false;
           return;
        }
        if ((isNaN(this.questionmodal.Question1.NoOptionAUseIntegral) || this.questionmodal.Question1.NoOptionAUseIntegral <= 0) || ((isNaN(this.questionmodal.Question1.NoOptionAWinCouponCount) || this.questionmodal.Question1.NoOptionAWinCouponCount <= 0))) {       
             that.$Message.warning("第一题题目否的选项A的积分值和兑换券必须为正整数");
                  that.$nextTick(
            () => {
                    that.questionmodal.loading = true;
                  }
          );
          that.questionmodal.loading = false;
           return;
        }
         if ((isNaN(this.questionmodal.Question1.NoOptionBUseIntegral) || this.questionmodal.Question1.NoOptionBUseIntegral <= 0) || ((isNaN(this.questionmodal.Question1.NoOptionBWinCouponCount) || this.questionmodal.Question1.NoOptionBWinCouponCount <= 0))) {       
             that.$Message.warning("第一题题目否的选项B的积分值和兑换券必须为正整数");
                  that.$nextTick(
            () => {
                    that.questionmodal.loading = true;
                  }
          );
          that.questionmodal.loading = false;
           return;
        }
         if ((isNaN(this.questionmodal.Question1.NoOptionCUseIntegral) || this.questionmodal.Question1.NoOptionCUseIntegral <= 0) || ((isNaN(this.questionmodal.Question1.NoOptionCWinCouponCount) || this.questionmodal.Question1.NoOptionCWinCouponCount <= 0))) {       
             that.$Message.warning("第一题题目否的选项C的积分值和兑换券必须为正整数");
                  that.$nextTick(
            () => {
                    that.questionmodal.loading = true;
                  }
          );
          that.questionmodal.loading = false;
           return;
        }

        if (!this.questionmodal.Question2.DeadLineTime) {    
            that.$Message.warning("第二题题目竞猜截至时间不能为空,请设置");
             that.$nextTick(
            () => {
                    that.questionmodal.loading = true;
                  }
          );
          that.questionmodal.loading = false;
          return;
        }       
        this.questionmodal.Question2.StartTime = this.formatDate(this.questionmodal.Question1.StartTime).replace("00:00:00", "11:00:00");
        this.questionmodal.Question2.EndTime = this.formatDate(this.questionmodal.Question1.EndTime).replace("00:00:00", "11:00:00");
        this.questionmodal.Question2.DeadLineTime = this.formatDate(this.questionmodal.Question2.DeadLineTime);  
        console.log(this.questionmodal.Question2.StartTime);
        console.log(this.questionmodal.Question2.EndTime);
        console.log(this.questionmodal.Question2.DeadLineTime);
       
        console.log(this.isLater(this.questionmodal.Question2.EndTime, this.questionmodal.Question2.DeadLineTime));
        console.log(this.isLater(this.questionmodal.Question2.DeadLineTime, this.questionmodal.Question2.StartTime));
        if (!this.isLater(this.questionmodal.Question2.EndTime, this.questionmodal.Question2.DeadLineTime) || !this.isLater(this.questionmodal.Question2.DeadLineTime, this.questionmodal.Question2.StartTime)) {       
              that.$Message.warning("第二题题目竞猜截至时间请介于开始时间和结束时间之间");
                that.$nextTick(
            () => {
                    that.questionmodal.loading = true;
                  }
          );
          that.questionmodal.loading = false;
           return;
        }
          if (!this.questionmodal.Question2.QuestionTitle) {    
            that.$Message.warning("第二题题目内容不能为空");
             that.$nextTick(
            () => {
                    that.questionmodal.loading = true;
                  }
          );
          that.questionmodal.loading = false;
          return;
        }       
        console.log(!isNaN(this.questionmodal.Question2.YesOptionAUseIntegral));
        console.log(this.questionmodal.Question2.YesOptionAUseIntegral <= 0);
        if ((isNaN(this.questionmodal.Question2.YesOptionAUseIntegral) || this.questionmodal.Question2.YesOptionAUseIntegral <= 0) || ((isNaN(this.questionmodal.Question2.YesOptionAWinCouponCount) || this.questionmodal.Question2.YesOptionAWinCouponCount <= 0))) {       
             that.$Message.warning("第二题题目是的选项A的积分值和兑换券必须为正整数");
                  that.$nextTick(
            () => {
                    that.questionmodal.loading = true;
                  }
          );
          that.questionmodal.loading = false;
           return;
        }
         if ((isNaN(this.questionmodal.Question2.YesOptionBUseIntegral) || this.questionmodal.Question2.YesOptionBUseIntegral <= 0) || ((isNaN(this.questionmodal.Question2.YesOptionBWinCouponCount) || this.questionmodal.Question2.YesOptionBWinCouponCount <= 0))) {       
             that.$Message.warning("第二题题目是的选项B的积分值和兑换券必须为正整数");
                  that.$nextTick(
            () => {
                    that.questionmodal.loading = true;
                  }
          );
          that.questionmodal.loading = false;
           return;
        }
         if ((isNaN(this.questionmodal.Question2.YesOptionCUseIntegral) || this.questionmodal.Question2.YesOptionCUseIntegral <= 0) || ((isNaN(this.questionmodal.Question2.YesOptionCWinCouponCount) || this.questionmodal.Question2.YesOptionCWinCouponCount <= 0))) {       
             that.$Message.warning("第二题题目是的选项C的积分值和兑换券必须为正整数");
                  that.$nextTick(
            () => {
                    that.questionmodal.loading = true;
                  }
          );
          that.questionmodal.loading = false;
           return;
        }
        if ((isNaN(this.questionmodal.Question2.NoOptionAUseIntegral) || this.questionmodal.Question2.NoOptionAUseIntegral <= 0) || ((isNaN(this.questionmodal.Question2.NoOptionAWinCouponCount) || this.questionmodal.Question2.NoOptionAWinCouponCount <= 0))) {       
             that.$Message.warning("第二题题目否的选项A的积分值和兑换券必须为正整数");
                  that.$nextTick(
            () => {
                    that.questionmodal.loading = true;
                  }
          );
          that.questionmodal.loading = false;
           return;
        }
         if ((isNaN(this.questionmodal.Question2.NoOptionBUseIntegral) || this.questionmodal.Question2.NoOptionBUseIntegral <= 0) || ((isNaN(this.questionmodal.Question2.NoOptionBWinCouponCount) || this.questionmodal.Question2.NoOptionBWinCouponCount <= 0))) {       
             that.$Message.warning("第二题题目否的选项B的积分值和兑换券必须为正整数");
                  that.$nextTick(
            () => {
                    that.questionmodal.loading = true;
                  }
          );
          that.questionmodal.loading = false;
           return;
        }
         if ((isNaN(this.questionmodal.Question2.NoOptionCUseIntegral) || this.questionmodal.Question2.NoOptionCUseIntegral <= 0) || ((isNaN(this.questionmodal.Question2.NoOptionCWinCouponCount) || this.questionmodal.Question2.NoOptionCWinCouponCount <= 0))) {       
             that.$Message.warning("第二题题目否的选项C的积分值和兑换券必须为正整数");
                  that.$nextTick(
            () => {
                    that.questionmodal.loading = true;
                  }
          );
          that.questionmodal.loading = false;
           return;
        }

        if (!this.questionmodal.Question3.DeadLineTime) {    
            that.$Message.warning("第三题题目竞猜截至时间不能为空,请设置");
             that.$nextTick(
            () => {
                    that.questionmodal.loading = true;
                  }
          );
          that.questionmodal.loading = false;
          return;
        }       
        this.questionmodal.Question3.StartTime = this.formatDate(this.questionmodal.Question1.StartTime).replace("00:00:00", "11:00:00");
        this.questionmodal.Question3.EndTime = this.formatDate(this.questionmodal.Question1.EndTime).replace("00:00:00", "11:00:00");
        this.questionmodal.Question3.DeadLineTime = this.formatDate(this.questionmodal.Question3.DeadLineTime);  
        console.log(this.questionmodal.Question3.StartTime);
        console.log(this.questionmodal.Question3.EndTime);
        console.log(this.questionmodal.Question3.DeadLineTime);
        
        console.log(this.isLater(this.questionmodal.Question3.EndTime, this.questionmodal.Question3.DeadLineTime));
        console.log(this.isLater(this.questionmodal.Question3.DeadLineTime, this.questionmodal.Question3.StartTime));
        if (!this.isLater(this.questionmodal.Question3.EndTime, this.questionmodal.Question3.DeadLineTime) || !this.isLater(this.questionmodal.Question3.DeadLineTime, this.questionmodal.Question3.StartTime)) {       
              that.$Message.warning("第三题题目竞猜截至时间请介于开始时间和结束时间之间");
                that.$nextTick(
            () => {
                    that.questionmodal.loading = true;
                  }
          );
          that.questionmodal.loading = false;
           return;
        }
          if (!this.questionmodal.Question3.QuestionTitle) {    
            that.$Message.warning("第三题题目内容不能为空");
             that.$nextTick(
            () => {
                    that.questionmodal.loading = true;
                  }
          );
          that.questionmodal.loading = false;
          return;
        }       
        console.log(!isNaN(this.questionmodal.Question3.YesOptionAUseIntegral));
        console.log(this.questionmodal.Question3.YesOptionAUseIntegral <= 0);
        if ((isNaN(this.questionmodal.Question3.YesOptionAUseIntegral) || this.questionmodal.Question3.YesOptionAUseIntegral <= 0) || ((isNaN(this.questionmodal.Question3.YesOptionAWinCouponCount) || this.questionmodal.Question3.YesOptionAWinCouponCount <= 0))) {       
             that.$Message.warning("第三题题目是的选项A的积分值和兑换券必须为正整数");
                  that.$nextTick(
            () => {
                    that.questionmodal.loading = true;
                  }
          );
          that.questionmodal.loading = false;
           return;
        }
         if ((isNaN(this.questionmodal.Question3.YesOptionBUseIntegral) || this.questionmodal.Question3.YesOptionBUseIntegral <= 0) || ((isNaN(this.questionmodal.Question3.YesOptionBWinCouponCount) || this.questionmodal.Question3.YesOptionBWinCouponCount <= 0))) {       
             that.$Message.warning("第三题题目是的选项B的积分值和兑换券必须为正整数");
                  that.$nextTick(
            () => {
                    that.questionmodal.loading = true;
                  }
          );
          that.questionmodal.loading = false;
           return;
        }
         if ((isNaN(this.questionmodal.Question3.YesOptionCUseIntegral) || this.questionmodal.Question3.YesOptionCUseIntegral <= 0) || ((isNaN(this.questionmodal.Question3.YesOptionCWinCouponCount) || this.questionmodal.Question3.YesOptionCWinCouponCount <= 0))) {       
             that.$Message.warning("第三题题目是的选项C的积分值和兑换券必须为正整数");
                  that.$nextTick(
            () => {
                    that.questionmodal.loading = true;
                  }
          );
          that.questionmodal.loading = false;
           return;
        }
        if ((isNaN(this.questionmodal.Question3.NoOptionAUseIntegral) || this.questionmodal.Question3.NoOptionAUseIntegral <= 0) || ((isNaN(this.questionmodal.Question3.NoOptionAWinCouponCount) || this.questionmodal.Question3.NoOptionAWinCouponCount <= 0))) {       
             that.$Message.warning("第三题题目否的选项A的积分值和兑换券必须为正整数");
                  that.$nextTick(
            () => {
                    that.questionmodal.loading = true;
                  }
          );
          that.questionmodal.loading = false;
           return;
        }
         if ((isNaN(this.questionmodal.Question3.NoOptionBUseIntegral) || this.questionmodal.Question3.NoOptionBUseIntegral <= 0) || ((isNaN(this.questionmodal.Question3.NoOptionBWinCouponCount) || this.questionmodal.Question3.NoOptionBWinCouponCount <= 0))) {       
             that.$Message.warning("第三题题目否的选项B的积分值和兑换券必须为正整数");
                  that.$nextTick(
            () => {
                    that.questionmodal.loading = true;
                  }
          );
          that.questionmodal.loading = false;
           return;
        }
         if ((isNaN(this.questionmodal.Question3.NoOptionCUseIntegral) || this.questionmodal.Question3.NoOptionCUseIntegral <= 0) || ((isNaN(this.questionmodal.Question3.NoOptionCWinCouponCount) || this.questionmodal.Question3.NoOptionCWinCouponCount <= 0))) {       
             that.$Message.warning("第三题题目否的选项C的积分值和兑换券必须为正整数");
                  that.$nextTick(
            () => {
                    that.questionmodal.loading = true;
                  }
          );
          that.questionmodal.loading = false;
           return;
        }

        if (that.questionmodal.Question1.PKID > 0) {
        that.ajax
            .post("/GuessGame/UpdateQuestionWithOptionList", {
                question1: that.questionmodal.Question1,
                question2: that.questionmodal.Question2,
                question3: that.questionmodal.Question3
            })
            .then(response => {
                if (!response.data) {
                    that.$Message.success("操作成功");
                    that.questionmodal.visible = false;
                    that.loadData(that.page.current);
                } else {
                    that.$Message.error(response.data);
                    that.$nextTick(() => {
                        that.questionmodal.loading = true;
                    });
                }
                that.questionmodal.loading = false;
            });
        } else {
             that.ajax
            .post("/GuessGame/SaveQuestionWithOptionList", {
                question1: that.questionmodal.Question1,
                question2: that.questionmodal.Question2,
                question3: that.questionmodal.Question3
            })
            .then(response => {
                if (!response.data) {
                    that.$Message.success("操作成功");
                    that.questionmodal.visible = false;
                    that.loadData(that.page.current);
                } else {
                    that.$Message.error(response.data);
                    that.$nextTick(() => {
                        that.questionmodal.loading = true;
                    });
                }
                that.questionmodal.loading = false;
            });
        }
      },
      FetchProductInfo (PID) {
        var that = this;
        if (PID) {
          that.ajax
          .post("/GroupBuyingV2/FetchProductInfo", {
              pid: PID
          })
          .then(response => {
            if (response.data) {
              that.modal.productConfig.PID = response.data.PID;
              that.modal.productConfig.ProductName = response.data.ProductName;
              that.modal.productConfig.ProductPrice = response.data.ProductPrice
            } else {
              that.$Message.warning("商品 PID 无效");
            } 
          });
        }    
      },
      handlePageChange (pageIndex) {
          this.loadData(pageIndex);
      },
      handlePageSizeChange (pageSize) {
          this.page.pageSize = pageSize;
          this.loadData(this.page.current);
      },
      deleteCoupon (index) {
          this.modal.productConfig.Coupons.splice(index, 1);
      },      
      handleLogPageSizeChange (pageSize) {
          var that = this;
          that.logpage.pageSize = pageSize;
          that.loadData(this.logpage.current);
      },
      formatDate (value) {
        if (value) {
            var time = new Date(
                value
            );
            var year = time.getFullYear();
            var day = time.getDate();
            var month = time.getMonth() + 1;
            var hours = time.getHours();
            var minutes = time.getMinutes();
            var seconds = time.getSeconds();
            var func = function (value, number) {
                var str = value.toString();
                while (str.length < number) {
                    str = "0" + str;
                }
                return str;
            };
            if (year === 1) {
                return "";
            } else {
                return (
                    func(year, 4) +
                    "-" +
                    func(month, 2) +
                    "-" +
                    func(day, 2) +
                    " " +
                    func(hours, 2) +
                    ":" +
                    func(minutes, 2) +
                    ":" +
                    func(seconds, 2)
                );
            }
        } else {        
            return '';
        }
      },
      isLater (str1, str2) {
      return new Date(str1) >= new Date(str2);
     },
      ConvertValue (value) {
          var result = '';
          if (value === 0) {
              result = '未添加答案';
          } else if (value === 1) {
               result = '已添加答案';
          } else if (value === 2) {
               result = '已公布';
          }
          return result;            
      },
        ConvertBoolValue (value) {
          var result = '';
          if (value === false) {
              result = '否';
          } else if (value === true) {
               result = '是';
          }
          return result;            
      },
      QueryQuestionWithOption (time, isdeleted) {      
            this.ajax
          .get("/GuessGame/GetQuestionWithOptionList?endTime=" + this.formatDate(time) + "&isdeleted=" + isdeleted)
          .then(response => {
               if (response.data.data[0]) {
                this.queryquestionmodal.Question1 = response.data.data[0];                                                  
                this.queryquestionmodal.visible = true;
              }  
              if (response.data.data[1]) {
                this.queryquestionmodal.Question2 = response.data.data[1];
                this.queryquestionmodal.visible = true;
              } 
              if (response.data.data[2]) {
                this.queryquestionmodal.Question3 = response.data.data[2];
                this.queryquestionmodal.visible = true;
              }
          });
    }
    }
  }
</script>
