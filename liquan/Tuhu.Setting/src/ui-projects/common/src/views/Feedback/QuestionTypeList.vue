<template>
 <div>
     <Breadcrumb :style="{margin: '24px 0'}">
       <BreadcrumbItem> 问题反馈管理</BreadcrumbItem>
       <BreadcrumbItem>问题类型列表</BreadcrumbItem>
     </Breadcrumb>
       <Content :style="{padding: '24px', minHeight: '280px', background: '#fff'}">
    <div  class="tool">
        <div>
            <Button type="primary" @click="modal1 = true" style=" float: right;">添加问题类型</Button>
        </div>
          <Modal
           v-model="modal1" 
           title="添加问题类型"
           @on-ok="ok">
              <div>
                  <input hidden v-model="questionId"/>
                   <span>问题类型：</span><input v-model="typeName"/><span class="show">*</span><span v-if="typeshow">问题类型不能为空</span>
                   <div style="margin-top: 10px;">
                    <span>默认描述：</span><textarea v-model="describtion" style="width: 320px;
    height: 87px;"></textarea><span  class="show">*</span><span v-if="descshow">描述类型不能为空</span><span v-if="toolong">描述不能超过500个字符</span>
                   </div>
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
</style>

<script>
    export default {
        data () {
            return {
                modal1: false,
                tableData: [],
                questionId: 0,
                typeName: '',
                typeshow: false,
                describtion: '',
                descshow: false,
                toolong: false,
                columns: [
                    {
                        title: '序号',
                        type: 'index',
                        align: 'center',
                        width: 80
                    },
                    {
                        title: '问题类型',
                        key: 'TypeName',
                          align: 'center',
                           width: 220
                    },
                     {
                        title: '默认描述',
                        key: 'Description',
                        align: 'center',
                        width: 770
                    },
                    {
                        title: '操作',
                        key: 'action',
                        align: 'center',
                        render: (h, params) => {
                            return h('div', [
                             h('Button', {
                                 props: {
                                    type: 'primary'
                                },
                                style: {
                                    width: '50px'
                                },
                                on: {
                                    click: () => {
                                     this.UpdateQuestionTypes(params.row);
                                    }
                                }
                            }, '编辑'),
                            h('Button', {
                                props: {
                                    type: 'primary'
                                },
                                style: {
                                    width: '50px',
                                    'margin-left': '20px'
                                },
                                on: {
                                    click: () => {
                                     this.DeleteQuestionTypes(params.row.Id);
                                    }
                                }
                            }, '删除')])
                        }
                    }
                ]
            }
        },
         mounted () {
             this.GetQuestionTypes();
         },
        methods: {
            ModalCancel () {
                this.modal1 = false
                this.typeName = "";
                this.describtion = "";
                this.questionId = 0
            },
            GetQuestionTypes () {
                this.util.ajax.get('/FeedBack/GetQuestionTypeList').then(res => {
                    if (res.data) {
                          var result = res.data.data;
                          if (result) {
                              this.tableData = result;
                          }
                    }
                })
            },
            DeleteQuestionTypes (id) {
             if (id === 0 || id === null || id === undefined) { this.$Message.success("重新点击删除"); } else { 
this.util.ajax.get('/FeedBack/DeleteQuestionType?id=' + id).then(res => {
                    if (res.data) {
                          var result = res.data.Id;
                          if (result) {
                             if (result > 0) {
                                   this.$Message.success("删除成功！");
                                   this.GetQuestionTypes();
                             } else {
                                    this.$Message.warning("删除失败！");
                               }
                          }
                    }
                 })
 }
            },
             ok () {
                  if (this.typeName == null || this.typeName === "") {
                  this.typeshow = true;
                  return;
                }
                if (this.describtion == null || this.describtion === "") {
                  this.descshow = true;
                  return;
                }
                 if (this.describtion !== "" && this.describtion.length > 500) {
                  this.toolong = true;
                  return;
                }
                    this.typeshow = false;
                     this.descshow = false;
                    this.util.ajax.post('/FeedBack/CreateQuestionType', {
                      'TypeName': this.typeName,
                      'Describtion': this.describtion,
                      'Id': this.questionId
                    }).then(res => {
                      if (res.data) {
                          var result = res.data;
                          if (result) {
                              if (result.Message) {
                                    this.$Message.error(result.Message);
                               } else if (result.Id > 0) {
                                            if (this.questionId === 0) { this.$Message.success("添加成功！"); } else { this.$Message.success("更新成功！"); }
                                            this.typeName = "";
                                            this.describtion = "";
                                            this.questionId = 0
                                            this.GetQuestionTypes();
                                            this.modal1 = false;
                                        } else {
                                               this.$Message.warning("添加失败！");
                                                }
                           }
                    }
                })
           },
           UpdateQuestionTypes (row) {
               this.modal1 = true
               this.questionId = row.Id
               this.typeName = row.TypeName
               this.describtion = row.Description
           }
        }
    };
</script>
