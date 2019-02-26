<template>
  <div>
    <Row>
      <Col span=16 offset=5>
      <Steps :current="1">
        <Step title="促销活动配置" content=""></Step>
        <Step title="选择促销商品" content=""></Step>
        <Step title="审核" content=""></Step>
      </Steps>
      </Col>
    </Row>
    <br>
    <br>
    <!-- 搜索列表 -->
    <div>
      <Row>
        <Col span="3" style="background-color:#CDC9C9;text-align:center;cursor:pointer;">
        <div @click="selectSpan1" style="padding:15px;font-size:14px;"> 按条件筛选</div>
        </Col>
        <Col span="3" style="background-color:#EEEED1;text-align:center;cursor:pointer;">
        <div @click="selectSpan2" style="padding:15px;font-size:14px;"> PID导入</div>
        </Col>
      </Row>
      <div id="tabImport" style="padding:40px 0;display:none;">
        <Form>
          <Row>
            <Upload action="/salepromotionactivity/upload" :data="uoloadPidData" :on-progress="uploadTip" :format="['xlsx','xls']" :on-format-error="handleFormatError"
              :on-success="uploadSuccess" style="margin-left:25px;display: inline;float:left">
              <Button type="warning" icon="ios-cloud-upload-outline">导入商品信息</Button>
            </Upload>
            <a style="margin:18px;display: inline;" href="/SalePromotionActivity/ExportTemplate?type=pidtemplate" target="_blank">下载PID模板文件</a>
          </Row>
        </Form>
      </div>
      <!-- 搜索框 -->
      <div id="tabSearch" style="padding:10px 5px;" v-bind:class="{tabSearch:search_data.IsSelectTireBrand==false}">
        <div>
          <Form :model="search_data" :label-width="100">
            <div>
              <Row>
                <Col span="4">
                <FormItem label="产品业务线">
                  <Select v-model="search_data.ProductlineSelect" @on-change="productlineChange" style="width:100px">
                    <Option v-for="item in search_data.Productline" :value="item.value" :key="item.value">{{ item.label }}</Option>
                  </Select>
                </FormItem>
                </Col>
                <Col span="8">
                <FormItem label="商品类别">
                  <Cascader change-on-select @on-change="categoryChange" :data="search_data.CategoryList" v-model="search_data.CategorySelect"></Cascader>
                </FormItem>
                </Col>
                <Col span="6">
                <FormItem label="选择品牌">
                  <Select v-model="search_data.BrandSelect" multiple @on-change="brandChange" clearable style="width:200px">
                    <Option v-for="item in search_data.BrandList" :value="item" :key="item">{{ item }}</Option>
                  </Select>
                </FormItem>
                </Col>
                <Col span="5">
                <Button type="primary" icon="search" @click="searchProduct()" style="margin-left:28px;">查询</Button></Col>
                <Spin size="small" fix v-if="IsSpinSearchTab"></Spin>
              </Row>
              <Row v-show="search_data.IsSelectTireBrand">
                <FormItem label="尺寸">
                  <Checkbox v-model="search_data.IsAllSize" style="float:left;width:90px;margin:0;">
                    <span>不限</span>
                  </Checkbox>
                  </span>
                  <CheckboxGroup v-model="search_data.SizeSelect">
                    <span v-for="item in search_data.SizeList" :key="item">
                      <Checkbox style="float:left;width:90px;margin:0;" :label="item">
                        <span>{{item}}</span>
                      </Checkbox>
                    </span>
                  </CheckboxGroup>
                </FormItem>
                <Spin size="small" fix v-if="IsSpinSearchTab"></Spin>
              </Row>
              <Row v-show="search_data.IsSelectTireBrand">
                <FormItem label="花纹">
                  <Checkbox v-model="search_data.IsAllPattern" style="float:left;width:110px;margin:0;">
                    <span>不限</span>
                  </Checkbox>
                  </span>
                  <CheckboxGroup v-model="search_data.PatternSelect">
                    <span v-for="item in search_data.PatternList" :key="item">
                      <Checkbox style="float:left;width:110px;margin:0;" :label="item">
                        <span>{{item}}</span>
                      </Checkbox>
                    </span>
                  </CheckboxGroup>
                </FormItem>
                <Spin size="small" fix v-if="IsSpinSearchTab"></Spin>
              </Row>
            </div>
          </Form>
        </div>
      </div>
      <div style="background-color:#F5F5F5;">
        <br>
        <Form :model="activityproduct" :label-width="100">
          <Row>
            <Col span="5">
            <FormItem label="商品pid" style="width:270px;">
              <Input v-model="search_activityPro.Pid" placeholder="请输入精确PID"></Input>
            </FormItem>
            </Col>
            <Col span="6">
            <FormItem label="商品名称">
              <Input v-model="search_activityPro.ProductName" placeholder="请输入商品名称"></Input>
            </FormItem>
            </Col>
            <Col span="12">
            <FormItem label="毛利区间">
                <Input-Number v-model="search_activityPro.LowProfile" :max="9999999" style="width:100px;"
                @on-blur="changeLowProfile" placeholder="最低毛利" /> 
              ——
               <Input-Number v-model="search_activityPro.HighProfile" :max="9999999" style="width:100px;" 
               @on-blur="changeHighProfile" placeholder="最高毛利" />
              <Button type="primary" icon="search" @click="searchActivityProduct" style="margin-left:28px;">查询</Button>
              <Button type="ghost" icon="refresh" style="margin-left: 8px;" @click="handleResetForm">重置</Button>
            </FormItem>
            </Col>
          </Row>
          <Row>
            <Col style="margin-bottom:5px;">
            <span>
              <Button type="warning" style="margin-left: 8px;" @click="addPidModalShow">新增商品</Button>
              <Button type="warning" style="margin-left: 8px;" @click="setStockModalShow">批量设置库存</Button>
              <Button type="warning" style="margin-left: 8px;" @click="setImageModalShow">批量设置列表牛皮癣</Button>
              <Button type="warning" style="margin-left: 8px;" @click="setDetailImageModalShow">批量设置详情页牛皮癣</Button>
              <Button type="primary" icon="loop" style="margin-right: 8px;" @click="refreshProduct">同步商品数据</Button>
            </span>
            </Col>
          </Row>
        </Form>
        <Table stripe border :loading="products.table.loading" :columns="products.table.columns" :data="products.table.data" @on-selection-change="selectProduct"></Table>
        <div style="margin-top:15px;float:right">
          <Page show-total :total="products.page.total" :page-size="products.page.pageSize" :current="products.page.current" :page-size-opts="[10,20 ,50 ,100]"
            show-elevator show-sizer @on-change="handleProductsPageChange" @on-page-size-change="handleProductsPageSizeChange"></Page>

        </div>
      </div>
      <div style="margin:50px 5px 0 0;">
        <Row>
          <Col span="4" offset=5>
          <Button type="primary" @click="editActivity" style="">上一步</Button>
          </Col>
          <Col span="4">
          <Button v-if="activity.AuditStatus==0" type="primary" @click="waitAudit(1)" style="">提交审核</Button>
          <Button v-else-if="activity.AuditStatus==1" type="primary" @click="waitAudit(2)" style="">审 核</Button>
          </Col>
        </Row>
      </div>
      <Spin size="large" fix v-if="IsScreenSpin"></Spin>
    </div>

    <Modal width="830" title="商品操作结果" v-model="uploadResult.modal.visible" :loading="uploadResult.modal.loading" @on-ok="ok"
      footerHide>
      <Row>
        <span style="font-size:18px;"> 操作结果：
        </span>
        <span style="font-size:16px;"> 新增成功 {{uploadResult.insertCount}} 条商品，新增失败{{uploadResult.modal.data.length}}条</span>
        <span v-if="uploadResult.deleteCount>0" style="font-size:16px;">删除{{uploadResult.deleteCount}}条</span>
      </Row>
      <br>
      <Row>
        <span style="font-size:16px;">失败的商品列表</span>
        <!-- <a style="margin:18px;display:inline;"  @click="ExportUploadResult" target="_blank">导出结果</a> -->
      </Row>
      <Table :loading="uploadResult.modal.loading" :data="uploadResult.modal.data" :columns="uploadResult.modal.columns" stripe></Table>
    </Modal>
    <Modal v-model="removeModal.visible" title="撤除商品" :loading="removeModal.loading" width="20px" @on-ok="removeProduct">
      <h3>确认将该商品撤出活动？</h3>
    </Modal>
    <Modal v-model="addPidModal.visible" title="添加商品" footerHide :loading="addPidModal.loading" width="42px">
      <br>
      <Form>
             <Row v-for="(item,index) in addPidModal.AddPidList" :key="index">
          <Col>
          <div style="height:40px;">
              <span>商品PID</span>
             <Input v-model="item.pid" style="width:220px;margin-left:5px;" placeholder="请输入商品Pid"
             @on-blur="validAddPid(index)"></Input>
            <Button v-if="index==0" type="success" size="small" style="margin-left: 8px;"
             @click="addAddPidList(index)">继续添加</Button>
            <Button v-if="CanDelAddPidList" type="warning" size="small" style="margin-left: 8px;"
              @click="delAddPidList(index)">删除</Button> 
               <span  v-if="item.tip!=''" style="color:#ed3f14;margin-left:5px;">
             {{item.tip}}
            </span>
          </div>
             
          </Col>
        </Row>
      </Form>
      <Row style="margin-top:5px;">
        <Col span="19" offset=5 style="color:red;"> {{addPidModal.message}}
        </Col>
      </Row>
      <br>
      <Row style="margin-top:5px;">
           <Col span="4" offset=4>
        <Button type="ghost" @click="validAllPid" style="">检查所有PID</Button>
        </Col>
        <Col span="4" offset=2>
        <Button v-if="AddPidSaveBtnDis" disabled type="primary" @click="saveAddPid" style="">保存</Button>
        <Button v-else type="primary" @click="saveAddPid" style="">保存</Button>
        </Col>
        <Col span="4">
        <Button type="ghost" @click="addPidModal.visible=false" style="">取消</Button>
        </Col>
      </Row>
    </Modal>
    <Modal v-model="setStockModal.visible" title="批量设置商品库存" :loading="setStockModal.loading" width="30px">
      <div style="margin:5px 50px;">
        <Row>
          <Col span=5>
          <i>活动名称</i>
          </Col>
          <Col span=15>{{activity.Name}}</Col>
        </Row>
        <Row>
          <Col span=5>
          <i>促销方式</i>
          </Col>
          <Col span=15>
          <span v-if="activity.PromotionType==1">打折</span>
          </Col>
        </Row>
        <Row>
          <Col span=5>
          <i>打折规则</i>
          </Col>
          <div v-if="computeDiscountMethod==1">
            <span v-for="(item,index) in activity.DiscountContentList" :key="index" style="margin-right:20px;">
              满 {{item.Condition}} 元 打 {{item.DiscountRate/10}} 折</span>
          </div>
          <div v-else-if="computeDiscountMethod==2">
            <span v-for="(item,index) in activity.DiscountContentList" :key="index" style="margin-right:20px;">
              满 {{item.Condition}} 件 打 {{item.DiscountRate/10}} 折</span>
          </div>
        </Row>
        <Row>
          <Col span=5>
          <i>活动时间</i>
          </Col>
          <Col span=19> {{activity.StartTime}} 至 {{activity.EndTime}}</Col>
        </Row>
        <br>
      </div>
      <Form>
        <Col>
        <FormItem :label="setStockModal.title">
          <Input-Number v-model="setStockModal.stock" :precision=0 :max="9999999" :min="1" 
          @on-blur="changeStock" @on-change="changeStock" style="width:160px;" placeholder="请输入库存数" />
          <span style="color:red;"> {{setStockModal.stockTip}}</span>
        </FormItem>
        </Col>
      </Form>
      <br>
      <div slot="footer">
        <Button type="primary" @click="setAllStock" style="">确定</Button>
        <Button type="ghost" @click="setStockModal.visible=false;" style="">取消</Button>
      </div>
    </Modal>
    <!-- 列表牛皮癣 -->
    <Modal v-model="productImgModal.visible" title="批量设置商品列表牛皮癣" :loading="productImgModal.loading" width="30px">
      <div style="margin:5px 50px;">
        <Row>
          <Col span=5>
          <i>活动名称</i>
          </Col>
          <Col span=15>{{activity.Name}}</Col>
        </Row>
        <Row>
          <Col span=5>
          <i>促销方式</i>
          </Col>
          <Col span=15>
          <span v-if="activity.PromotionType==1">打折</span>
          </Col>
        </Row>
        <Row>
          <Col span=5>
          <i>打折规则</i>
          </Col>
          <div v-if="computeDiscountMethod==1">
            <span v-for="(item,index) in activity.DiscountContentList" :key="index" style="margin-right:20px;">
              满 {{item.Condition}} 元 打 {{item.DiscountRate/10}} 折</span>
          </div>
          <div v-else-if="computeDiscountMethod==2">
            <span v-for="(item,index) in activity.DiscountContentList" :key="index" style="margin-right:20px;">
              满 {{item.Condition}} 件 打 {{item.DiscountRate/10}} 折</span>
          </div>
        </Row>
        <Row>
          <Col span=5>
          <i>活动时间</i>
          </Col>
          <Col span=19> {{activity.StartTime}} 至 {{activity.EndTime}}</Col>
        </Row>
        <br>
      </div>
      <div style="height:100px;width:100%;padding-left:20px;">
        <span style="float:left;margin:5px;width:100px;height:100px;">
          <a :href="productImgUrl" target="_blank" v-show="productImgUrl!=''">
            <img :src="productImgUrl" style='width:100px;height:100px'>
          </a>
        </span>
        <span style="float:left;text-align:center;margin:5px;">
          <Upload action="/salepromotionactivity/UploadImage?type=image" :format="['jpg','jpeg','png']" :on-format-error="handleImgFormatError"
            :max-size="1000" :on-exceeded-size="handleImgMaxSize" :on-success="handleImgSuccess" :show-upload-list="false">
            <Button type="ghost" icon="ios-cloud-upload-outline">选择图片</Button>
          </Upload>
          
      <span style="margin:0 5px;color:red;">
          建议上传420*420的尺寸
      </span>
        </span>
        <span style="float:left;margin:5px;" v-show="productImgUrl!=''">
          <Button type="warning" icon="refresh" @click="productImgUrl=''">清除</Button>
        </span>
      </div>
      <div v-if="productImgUrl!=''" style="height:50px;">
        <Input readonly v-model="productImgUrl" type="textarea"></Input>
      </div>
      <br>
      <div slot="footer">
        <Button type="primary" @click="setProductImg" style="">确定</Button>
        <Button type="ghost" @click="productImgModal.visible=false" style="">取消</Button>
      </div>
    </Modal>
    <!-- 详情页牛皮癣 -->
   <Modal v-model="productDetailImgModal.visible" title="批量设置商品详情页牛皮癣" :loading="productDetailImgModal.loading" width="30px">
      <div style="margin:5px 50px;">
        <Row>
          <Col span=5>
          <i>活动名称</i>
          </Col>
          <Col span=15>{{activity.Name}}</Col>
        </Row>
        <Row>
          <Col span=5>
          <i>促销方式</i>
          </Col>
          <Col span=15>
          <span v-if="activity.PromotionType==1">打折</span>
          </Col>
        </Row>
        <Row>
          <Col span=5>
          <i>打折规则</i>
          </Col>
          <div v-if="computeDiscountMethod==1">
            <span v-for="(item,index) in activity.DiscountContentList" :key="index" style="margin-right:20px;">
              满 {{item.Condition}} 元 打 {{item.DiscountRate/10}} 折</span>
          </div>
          <div v-else-if="computeDiscountMethod==2">
            <span v-for="(item,index) in activity.DiscountContentList" :key="index" style="margin-right:20px;">
              满 {{item.Condition}} 件 打 {{item.DiscountRate/10}} 折</span>
          </div>
        </Row>
        <Row>
          <Col span=5>
          <i>活动时间</i>
          </Col>
          <Col span=19> {{activity.StartTime}} 至 {{activity.EndTime}}</Col>
        </Row>
        <br>
      </div>
      <div style="height:100px;width:100%;padding-left:20px;">
        <span style="float:left;margin:5px;width:100px;height:100px;">
          <a :href="productDetailImgUrl" target="_blank" v-show="productDetailImgUrl!=''">
            <img :src="productDetailImgUrl" style='width:100px;height:100px'>
          </a>
        </span>
        <span style="float:left;text-align:center;margin:5px;">
          <Upload action="/salepromotionactivity/UploadImage?type=image" :format="['jpg','jpeg','png']" :on-format-error="handleImgFormatError"
            :max-size="1000" :on-exceeded-size="handleImgMaxSize" :on-success="handleDetailImgSuccess" :show-upload-list="false">
            <Button type="ghost" icon="ios-cloud-upload-outline">选择图片</Button>
          </Upload>
          
      <span style="margin:0 5px;color:red;">
          建议上传750*120的尺寸
      </span>
        </span>
        <span style="float:left;margin:5px;" v-show="productDetailImgUrl!=''">
          <Button type="warning" icon="refresh" @click="productDetailImgUrl=''">清除</Button>
        </span>
      </div>
      <div v-if="productDetailImgUrl!=''" style="height:50px;">
        <Input readonly v-model="productDetailImgUrl" type="textarea"></Input>
      </div>
      <br>
      <div slot="footer">
        <Button type="primary" @click="setProductDetailImg" style="">确定</Button>
        <Button type="ghost" @click="productDetailImgModal.visible=false" style="">取消</Button>
      </div>
    </Modal>

    <!-- 库存设置结果 -->
    <Modal width="930" :title="stockResult.modal.title" v-model="stockResult.modal.visible" :loading="stockResult.modal.loading"
      footerHide>
      <Row>
        <span style="font-size:18px;"> 设置结果：
        </span>
        <span style="font-size:16px;"> 设置成功 {{stockResult.count}} 条，失败{{stockResult.modal.data.length}}条</span>
      </Row>
      <br>
      <Row>
        <span style="font-size:16px;">设置失败的商品列表</span>
        <!-- <a style="margin:18px;display:inline;" href="/SalePromotionActivity/ExportTemplate?type=pidtemplate" target="_blank">导出结果</a> -->
      </Row>
      <Table :loading="stockResult.modal.loading" :data="stockResult.modal.data" :columns="stockResult.modal.columns" stripe></Table>
    </Modal>
    <!-- 商品搜索结果 -->
    <Modal width="1030" title="搜索匹配商品" v-model="searchResult.visible" :loading="searchResult.loading" footerHide>
      <div>
        <Row style="margin-bottom:4px;">
          <!-- <span style="font-size:16px;float:left;"> 已选中 {{allCheckedPids.length}} 条商品</span> -->
          <span>
            <Page show-total style="float:right;" :total="searchResult.page.total" :page-size="searchResult.page.pageSize" :current="searchResult.page.current"
              :page-size-opts="[10,20 ,50 ,100]" show-elevator show-sizer @on-change="handleSearchPageChange" @on-page-size-change="handleSearchPageSizeChange"></Page>
          </span>
        </Row>
        <div>
        </div>
        <Table :loading="searchResult.table.loading" :data="searchResult.table.data" :columns="searchResult.table.columns" @on-selection-change="searchProductChange"
          stripe></Table>
        <Row style="margin-top:5px;">
          <Col span="4" offset=8>
          <Button type="primary" @click="saveProduct" style="">确定</Button>
          </Col>
          <Col span="4">
          <Button type="ghost" @click="searchResult.visible=false" style="">取消</Button>
          </Col>
        </Row>
      </div>
    </Modal>
    <Modal v-model="uploadTipVisible" footerHide>
      <div>正在导入请稍后...</div>
    </Modal>
  </div>
</template>
<script>
export default {
  data () {
    return {
    AddPidSaveBtnDis: true,
      IsSpinSearchTab: false,
      IsScreenSpin: false,
      uploadTipVisible: false,
      uoloadPidData: {
        activityId: this.$route.query.activityId
      },
      heightheight: 200,
      AuditStatus: "0",
      activity: {
        DiscountContentList: []
      },
      activityPids: [],
      currentCheckedPids: [],
      allCheckedPids: [],
      addPids: [],
      delPids: [],
      // 搜索框数据
      search_data: {
        // 产品线
        ProductlineSelect: "Tires",
        Productline: [
          {
            value: "Tires",
            label: "轮胎"
          },
          {
            value: "AutoProduct",
            label: "车品"
          },
          {
            value: "CPAZ",
            label: "车品安装"
          },
          {
            value: "LTFW",
            label: "轮胎服务"
          },
          {
            value: "MRAZ",
            label: "美容安装"
          },
          {
            value: "MR1",
            label: "美容"
          }
        ],
        // 类目
        CategorySelect: [],
        CategoryList: [],
        ActivityId: this.$route.query.activityId,
        BrandSelect: [],
        BrandList: [],
        IsAllSize: false,
        SizeList: [],
        SizeSelect: [],
        IsAllPattern: false,
        PatternList: [],
        PatternSelect: [],
        IsSelectTireBrand: false
      },
      searchResult: {
        loading: false,
        visible: false,
        page: {
          total: 0,
          current: 1,
          pageSize: 20
        },
        select: [],
        table: {
          loading: false,
          data: [],
          columns: [
            {
              type: "selection",
              width: 60,
              align: "center"
            },
            {
              title: "商品PID",
              key: "Pid",
              align: "center"
            },
            {
              title: "商品名称",
              key: "ProductName",
              align: "center"
            },
            {
              title: "商品总库存",
              key: "TotalStock",
              align: "center",
              width: 80,
              render: (h, params) => {
                  if (params.row.TotalStock === -999) {
                  return h(
                    "div", "不限库存"
                  );
                } else {
                  return h("div", params.row.TotalStock);
                }
              }
            },
            {
              title: "成本价",
              key: "CostPrice",
              align: "center",
              width: 80,
              render: (h, params) => {
                return h("div", Number(params.row.CostPrice).toFixed(2));
              }
            },
            {
              title: "售价",
              key: "SalePrice",
              align: "center",
              width: 80,
              render: (h, params) => {
                return h("div", Number(params.row.SalePrice).toFixed(2));
              }
            },
            {
              title: "折后毛利",
              key: "DiscountMargin",
              align: "center",
              render: (h, params) => {
                if (params.row.IsMinusProfile) {
                  return h(
                    "div",
                    {
                      style: {
                        color: "red"
                      }
                    },
                    params.row.DiscountMargin
                  );
                } else {
                  return h("div", params.row.DiscountMargin);
                }
              }
            },
            {
              title: "折后毛利率",
              key: "DiscountMarginRate",
              align: "center",
              render: (h, params) => {
                if (params.row.IsMinusProfile) {
                  return h(
                    "div",
                    {
                      style: {
                        color: "red"
                      }
                    },
                    params.row.DiscountMarginRate
                  );
                } else {
                  return h("div", params.row.DiscountMarginRate);
                }
              }
            },
            {
              title: "备注",
              key: "Remark",
              align: "center"
            }
          ]
        }
      },
      // 活动商品搜索列表
      search_activityPro: {
        Pid: "",
        ProductName: "",
        LowProfile: null, // 毛利
        HighProfile: null
      },
      activityproduct: {
        totalCount: 100
      },
      // 商品列表数据
      products: {
        page: {
          total: 0,
          current: 1,
          pageSize: 20
        },
        select: [],
        table: {
          loading: false,
          data: [],
          columns: [
            {
              type: "selection",
              width: 60,
              align: "center"
            },
            {
              title: "商品PID",
              key: "Pid",
              align: "center"
            },
            {
              title: "商品名称",
              key: "ProductName",
              align: "center"
            },
            {
              title: "列表牛皮癣",
              key: "ImageUrl",
              align: "center",
              render: (h, params) => {
                if (
                  params.row.ImageUrl !== "" &&
                  params.row.ImageUrl !== null
                ) {
                  return h(
                    "img",
                    {
                      domProps: {
                        src: params.row.ImageUrl,
                        width: "75",
                        height: "80"
                      }
                    },
                    params.row.ImageUrl
                  );
                } else {
                  return h("div", "");
                }
              }
            },
            {
              title: "详情页牛皮癣",
              key: "DetailImageUrl",
              align: "center",
              render: (h, params) => {
                if (
                  params.row.DetailImageUrl !== "" &&
                  params.row.DetailImageUrl !== null
                ) {
                  return h(
                    "img",
                    {
                      domProps: {
                        src: params.row.DetailImageUrl,
                        width: "75",
                        height: "80"
                      }
                    },
                    params.row.DetailImageUrl
                  );
                } else {
                  return h("div", "");
                }
              }
            },
            {
              title: "商品总库存",
              key: "TotalStock",
              align: "center",
               render: (h, params) => {
                  if (params.row.TotalStock === -999) {
                  return h(
                    "div", "不限库存"
                  );
                } else {
                  return h("div", params.row.TotalStock);
                }
              }
            },
            {
              title: "限购库存",
              key: "LimitQuantity",
              align: "center",
              render: (h, params) => {
                if (params.row.LimitQuantity === 0) {
return h(
                  "div",
                  {
                    attrs: {
                      contenteditable: "false"
                    },
                    style: {
                        color: 'red'
                    }
                  },
                  params.row.LimitQuantity
                );
                } else {
                    return h(
                  "div",
                  {
                    attrs: {
                      contenteditable: "false"
                    }
                  },
                  params.row.LimitQuantity
                );
                }
              }
            }, {
                title: "已售数量",
                key: "SoldQuantity",
                align: "center"
              },
              {
                title: "剩余库存",
                align: "center",
                render: (h, params) => {
                    var num = params.row.LimitQuantity - params.row.SoldQuantity;
                    if (num < 0) {
 return h('div', 0);
                    } else {
                        if (num < 10) {
return h('div', {style: {
                        color: 'red'
                      }}, num);
                        } else {
return h('div', num);
                        }
                    }
                }
              },
            {
              title: "成本价",
              key: "CostPrice",
              align: "center",
              render: (h, params) => {
                return h("div", Number(params.row.CostPrice).toFixed(2));
              }
            },
            {
              title: "售价",
              key: "SalePrice",
              align: "center",
              render: (h, params) => {
                return h("div", Number(params.row.SalePrice).toFixed(2));
              }
            },
            {
              title: "折后毛利",
              key: "DiscountMargin",
              align: "center",
              render: (h, params) => {
                if (params.row.IsMinusProfile) {
                  return h(
                    "div",
                    {
                      style: {
                        color: "red"
                      }
                    },
                    params.row.DiscountMargin
                  );
                } else {
                  return h("div", params.row.DiscountMargin);
                }
              }
            },
            {
              title: "折后毛利率",
              key: "DiscountMarginRate",
              align: "center",
              render: (h, params) => {
                if (params.row.IsMinusProfile) {
                  return h(
                    "div",
                    {
                      style: {
                        color: "red"
                      }
                    },
                    params.row.DiscountMarginRate
                  );
                } else {
                  return h("div", params.row.DiscountMarginRate);
                }
              }
            },
            {
              title: "备注",
              key: "Remark",
              align: "center"
            },
            {
              title: "操作",
              key: "action",
              align: "center ",
              render: (h, params) => {
                return h("div", [ 
                  h(
                    "Button",
                    {
                      props: {
                        type: "error",
                        size: "small"
                      },
                      style: {
                        marginRight: "5px"
                      },
                      on: {
                        click: () => {
                          this.removeModal.Pid = params.row.Pid;
                          this.removeModal.visible = true;
                        }
                      }
                    },
                    "撤出活动"
                  )
                ]);
              }
            }
          ]
        }
      },
      // 删除商品
      removeModal: {
        Pid: " ",
        visible: false,
        loading: false
      },
      addPidModal: {
        loading: false,
        visible: false,
        AddPidList: [
          {
            pid: "",
            tip: "",
            status: false
          }
        ],
        message: ""
      },
      setStockModal: {
        title: "批量设置库存",
        loading: false,
        visible: false,
        stock: null,
        stockTip: ""
      },
      productImgModal: {
        title: "批量设置商品列表牛皮癣",
        loading: false,
        visible: false
      },
      productDetailImgModal: {
        title: "批量设置商品详情页牛皮癣",
        loading: false,
        visible: false
      }, 
      productImgUrl: "",
      productDetailImgUrl: "",
      stockResult: {
        status: false,
        count: 0,
        modal: {
          title: "批量设置库存结果",
          loading: true,
          visible: false,
          data: [],
          columns: [
            {
              title: "商品PID",
              key: "Pid",
              align: "center"
              // width: 180
            },
            {
              title: "商品名称",
              key: "ProductName",
              align: "center"
              // width: 180
            },
            {
              title: "设置限购库存",
              key: "LimitQuantity",
              align: "center"
              // width: 140
            },
            {
              title: "失败原因",
              key: "FailMessage",
              align: "center",
              width: 380
            }
          ]
        }
      },
      // 上传数据
      uploads: {
        importData: {
          activityId: this.$route.query.activityId
        }
      },
      // 上传结果
      uploadResult: {
        status: false,
        insertCount: 0,
        deleteCount: 0,
        modal: {
          loading: true,
          visible: false,
          data: [],
          columns: [
            {
              title: "商品PID",
              key: "Pid",
              align: "center"
            },
            {
              title: "商品名称",
              key: "ProductName",
              align: "center",
               width: 350
            },
            {
              title: "导入限购库存",
              key: "LimitQuantity",
              align: "center",
              render: (h, params) => {
                if (params.row.FailMessage === "导入数据不正确") {
                  return h(
                    "div",
                    {
                      style: {
                        color: "red"
                      }
                    },
                    ""
                  );
                } else {
                  return h("div", params.row.LimitQuantity);
                }
              }
            },
            {
              title: "失败原因",
              key: "FailMessage",
              align: "center",
              width: 180
            }
          ]
        }
      }
    };
  },
  mounted () {
    // 加载活动商品
    this.loadData();
    // 获取活动基本信息
    this.bindActivityModel();
    // 加载搜索类目
    this.productlineChange(this.search_data.ProductlineSelect);
  },

  computed: {
    ComputeChkCount () {
      var count =
        this.addPids.length + this.activityPids.length - this.delPids.length;
      if (count >= 0) {
        return count;
      }
      return 0;
    },
    computeDiscountMethod: function () {
      if (
        this.activity.DiscountContentList == null ||
        this.activity.DiscountContentList === undefined ||
        !(this.activity.DiscountContentList.length > 0)
      ) {
        return 0;
      }
      return this.activity.DiscountContentList[0].DiscountMethod;
    },
    CanDelAddPidList: function () {
      if (
        this.addPidModal.AddPidList == null ||
        this.addPidModal.AddPidList === undefined
      ) {
        return false;
      } else {
        if (this.addPidModal.AddPidList.length > 1) {
          return true;
        } else {
          return false;
        }
      }
    }
  },
  methods: {
    changeStock () {
      this.setStockModal.stockTip = "";
      var value = this.setStockModal.stock;
      if (value != null) {
        this.setStockModal.stock = Number(parseInt(value));
      } else {
        this.setStockModal.stockTip = "请输入库存数";
      }
    },
    bindActivityModel () {
      this.ajax
        .post("/salepromotionactivity/GetActivityModel", {
          activityId: this.$route.query.activityId
        })
        .then(response => {
          if (response.data.Status) {
            this.activity = response.data.Data;
          }
        });
    },
    // tab切换
    selectSpan1 () {
      document.getElementById("tabImport").style.display = "none";
      document.getElementById("tabSearch").style.display = "block";
    },
    selectSpan2 () {
      document.getElementById("tabSearch").style.display = "none";
      document.getElementById("tabImport").style.display = "block";
    },
    searchActivityProduct () {
      this.products.page.current = 1;
      if (this.search_activityPro.LowProfile == null || this.search_activityPro.HighProfile == null) {
 this.loadData();
      } else {
var num1 = Number(this.search_activityPro.LowProfile);
      var num2 = Number(this.search_activityPro.HighProfile);
      if (isNaN(num1) || isNaN(num2)) {
        this.messageInfo("毛利区间请输入数字");
        return false;
      }
      if (num2 < num1 && this.search_activityPro.HighProfile !== null) {
        this.messageInfo("最低毛利应小于最高毛利");
        return false;
      }
       this.loadData();
      }
    },
    // 商品搜索
    // 产品线变化
    productlineChange (value) {
      this.search_data.CategoryList = [];
      this.search_data.BrandList = [];
      this.search_data.SizeList = [];
      this.search_data.PatternList = [];
      this.search_data.CategorySelect = [];
      this.search_data.BrandSelect = [];
      this.IsSpinSearchTab = true;
      this.ajax
        .post("/salepromotionactivity/getcategorybyproductline", {
          productline: value // this.search_data.ProductlineSelect
        })
        .then(response => {
          this.IsSpinSearchTab = false;
          if (response.data.Status) {
            this.search_data.CategoryList = response.data.CategotyList;
          } else {
            this.messageInfo(response.data.Msg);
          }
        });
    },
    // 类别变化
    categoryChange (value) {
      this.search_data.BrandList = [];
      this.search_data.BrandSelect = [];
      this.search_data.SizeList = [];
      this.search_data.SizeSelect = [];
      this.search_data.PatternList = [];
      this.search_data.PatternSelect = [];
      this.IsSpinSearchTab = true;
      this.ajax
        .post("/salepromotionactivity/getbrandsbycatgory", {
          categoryList: value // this.search_data.CategorySelect
        })
        .then(response => {
          this.IsSpinSearchTab = false;
          if (response.data.Status) {
            this.search_data.BrandList = response.data.BrandList;
          } else {
            this.messageInfo(response.data.Msg);
          }
        });
    },
    // 品牌变化
    brandChange (value) {
      if (value.length === 0) {
        this.search_data.IsSelectTireBrand = false;
        return;
      }
      this.IsSpinSearchTab = true;
      this.ajax
        .post("/salepromotionactivity/GetSizeAndPatternList", {
          brandList: value,
          category: this.search_data.CategorySelect
        })
        .then(response => {
          this.IsSpinSearchTab = false;
          this.search_data.SizeList = response.data.Item1;
          this.search_data.PatternList = response.data.Item2;
          if (
            this.search_data.SizeList.length > 0 &&
            this.search_data.ProductlineSelect === "Tires"
          ) {
            this.search_data.IsSelectTireBrand = true;
          }
        });
    },
    // 点击查询
    searchProduct () {
      this.addPids = [];
      this.delPids = [];
      this.IsScreenSpin = true;
      this.searchResult.page.current = 1;
      this.ajax
        .post("/salepromotionactivity/GetActivityPids", {
          activityId: this.$route.query.activityId
        })
        .then(response => {
          this.IsScreenSpin = false;
          this.activityPids = response.data;
          this.loadSearchProduct(true);
        });
    },
    // 获取查询的商品列表
    loadSearchProduct (isSpin) {
      var sizeList = this.search_data.SizeSelect;
      var patternList = this.search_data.PatternSelect;
      var category = this.search_data.ProductlineSelect;
      if (this.search_data.IsAllSize) {
        // 不限尺寸 花纹
        sizeList = [];
      }
      if (this.search_data.IsAllPattern) {
        patternList = [];
      }
      if (this.search_data.CategorySelect.length > 0) {
        // 商品类目
        category = this.search_data.CategorySelect[this.search_data.CategorySelect.length - 1];
      }
      if (isSpin) {
        this.IsScreenSpin = true;
      } else {
        this.searchResult.table.loading = true;
      }

      this.ajax
        .post("/salepromotionactivity/SearchProduct", {
          activityId: this.$route.query.activityId,
          pageIndex: this.searchResult.page.current,
          pageSize: this.searchResult.page.pageSize,
          category: category,
          brandList: this.search_data.BrandSelect,
          sizeList: sizeList,
          patternList: patternList
        })
        .then(response => {
          this.IsScreenSpin = false;
          this.searchResult.table.loading = false;
          var data = response.data;
          if (response.data.Status) {
            this.searchResult.visible = true;
            this.searchResult.page.total = data.Total;
            this.searchResult.page.current = data.Current;
            this.searchResult.table.data = data.ProductList;

            // 1.是否存在activitypids中 2是否存在addpis中 3是否存在delpids中
            this.searchResult.table.data.forEach(e => {
              // element._checked = element.IsChecked;
              if (this.checkHasValue(this.activityPids, e.Pid)) {
                e._checked = true;
              }
              if (this.checkHasValue(this.addPids, e.Pid)) {
                e._checked = true;
              }
              if (this.checkHasValue(this.delPids, e.Pid)) {
                e._checked = false;
              }
            });
          } else {
            this.messageInfo(data.Msg);
          }
        });
    },
    changeLowProfile () {
 if (this.search_activityPro.LowProfile != null) {
          var value = Number(this.search_activityPro.LowProfile).toFixed(2);
          this.search_activityPro.LowProfile = parseFloat(value);
        }
    },
        changeHighProfile () {
 if (this.search_activityPro.HighProfile != null) {
          var value = Number(this.search_activityPro.HighProfile).toFixed(2);
          this.search_activityPro.HighProfile = parseFloat(value);
        }
    },
    // 保存商品勾选结果
    saveProduct () {
      if (!(this.addPids.length + this.delPids.length > 0)) {
        this.messageInfo("无改动项");
        return false;
      }
      this.ajax
        .post("/salepromotionactivity/AddAndDelActivityProduct", {
          activityId: this.$route.query.activityId,
          stock: 0,
          addPids: this.addPids,
          delPids: this.delPids
        })
        .then(response => {
          var data = response.data;
          if (data.Status) {
            this.searchResult.visible = false;
            this.uploadResult.modal.data = data.FailList;
            this.uploadResult.modal.visible = true;
            this.uploadResult.modal.loading = false;
            this.uploadResult.insertCount = data.InsertCount;
            this.uploadResult.deleteCount = data.DeleteCount;
            this.loadData();
            this.bindActivityModel();
          } else {
            this.messageInfo(data.Msg);
          }
        });
    },
    closeSearchResultModal () {
      this.searchResult.visible = false;
    },
    searchProductChange (row) {
      // 保存当前勾选pids到addlist和allcheckedpids
      this.currentCheckedPids = [];
      row.forEach(element => {
        this.currentCheckedPids.push(element.Pid);
      });
      // 3步操作：1.验证delpids 2.验证activityPids 3.验证addlist-添加
      this.currentCheckedPids.forEach(chk => {
        var delHasChk = this.checkHasValue(this.delPids, chk);
        if (delHasChk) {
          // 从delpid移除
          this.delPids = this.spliceArr(this.delPids, chk);
        } else {
          var activityHasChk = this.checkHasValue(this.activityPids, chk);
          if (!activityHasChk) {
            if (!this.checkHasValue(this.addPids, chk)) {
              this.addPids.push(chk);
            }
            if (!this.checkHasValue(this.allCheckedPids, chk)) {
              this.allCheckedPids.push(chk);
            }
          }
        }
      });
      // 获取当前页未勾选的pids
      var isNoChk;
      //  var arrIndex;
      var noCheckedPids = []; // 当前页未勾选的pids
      this.searchResult.table.data.forEach(curr => {
        isNoChk = true;
        this.currentCheckedPids.forEach(p => {
          if (curr.Pid === p) {
            isNoChk = false;
          }
        });
        if (isNoChk) {
          noCheckedPids.push(curr.Pid);
        }
        // 1.验证addpids 2.验证activitypids 3.添加delpids
        noCheckedPids.forEach(nochk => {
          var addHasNoChk = this.checkHasValue(this.addPids, nochk);
          if (addHasNoChk) {
            this.addPids = this.spliceArr(this.addPids, nochk);
          } else {
            var activityHasNoChk = this.checkHasValue(this.activityPids, nochk);
            if (activityHasNoChk) {
              if (!this.checkHasValue(this.delPids, nochk)) {
                this.delPids.push(nochk);
              }
              if (this.checkHasValue(this.allCheckedPids, nochk)) {
                // 从allcheckedpids移除
                this.spliceArr(this.allCheckedPids, nochk);
              }
            }
          }
        });
      });
    },
    selectProduct (row) {
      this.products.select = [];
      row.forEach(element => {
        this.products.select.push(element.Pid);
      });
    },
    // 同步商品信息
    refreshProduct () {
      if (!(this.products.table.data.length > 0)) {
        this.messageInfo("活动暂无商品");
        return;
      }
      this.products.table.loading = true;
      this.ajax
        .post("/salepromotionactivity/RefreshProductInfo", {
          activityId: this.$route.query.activityId
        })
        .then(res => {
          this.products.table.loading = false;
          if (res.data.Status) {
            this.messageInfo("同步成功");
            this.loadData();
          } else {
            this.messageInfo(res.data.Msg);
          }
        });
    },
    // 从活动中单个移除商品
    removeProduct () {
      this.products.table.loading = true;
      this.ajax
        .post("/salepromotionactivity/RemoveProductFromActivity", {
          pid: this.removeModal.Pid,
          activityId: this.$route.query.activityId
        })
        .then(response => {
          this.products.table.loading = false;
          if (response.data.Status) {
            this.messageInfo("已成功撤出该商品");
            this.loadData();
            this.bindActivityModel();
          } else {
            this.util.message.warning({
              content: response.data.Msg,
              duration: 3,
              closable: true
            });
          }
        });
    },
    // 添加商品
    addAddPidList () {
      this.addPidModal.AddPidList.push({ pid: "", tip: "", status: false });
    },
    delAddPidList (index) {
      // 删除打折内容
      this.addPidModal.AddPidList.splice(index, 1);
    },
    validAddPid (index) {
        this.addPidModal.AddPidList[index].status = false;
      this.addPidModal.AddPidList[index].tip = "";
      var pid = this.addPidModal.AddPidList[index].pid;
      if (pid === "" || pid == null) {
        this.addPidModal.AddPidList[index].tip = "请输入PID";
        return false;
      }
      var len = 0;
      // 1.验证输入框中是否重复
      this.addPidModal.AddPidList.forEach(e => {
        if (e.pid === pid) {
          len++;
        }
      });
      if (len > 1) {
        this.addPidModal.AddPidList[index].status = false;
      this.addPidModal.AddPidList[index].tip = "PID输入重复";
      } else {
        this.ajax
          .post("/salepromotionactivity/ValidAddPid", {
            activityId: this.$route.query.activityId,
            pid: pid
          })
          .then(response => {
            if (!response.data.Status) {
              this.addPidModal.AddPidList[index].tip = response.data.Msg;
        this.addPidModal.AddPidList[index].status = false;
            } else {
                this.addPidModal.AddPidList[index].status = true;
            }
          });
      }
    },
    validAllPid () {
        var flag = true;
      if (this.addPidModal.AddPidList.length > 0) {
            this.addPidModal.AddPidList.forEach((e, i) => {
          if (e.status === false) {
              if (e.pid === '' || e == null) {
this.addPidModal.AddPidList[i].tip = "请输入商品PID";
this.addPidModal.AddPidList[i].status = false;
          } else {
this.validAddPid(i);
          }
          }
            });
            this.addPidModal.AddPidList.forEach(e => {
    if (!e.status) {
        flag = false;
    } 
});
if (flag) {
 this.AddPidSaveBtnDis = false;
} else {
    return false;
}
}
    },
    saveAddPid () {
        if (this.addPidModal.AddPidList.length > 0) {
            this.addPidModal.AddPidList.forEach((e, i) => {
          if (e.status === false) {
              if (e.pid === '' || e == null) {
this.addPidModal.AddPidList[i].tip = "请输入商品PID";
this.addPidModal.AddPidList[i].status = false;
          } else {
this.validAddPid(i);
          }
          }
            });
            this.addPidModal.AddPidList.forEach(e => {
    if (!e.status) {
        return false;
    } 
});
        var pidList = [];
this.addPidModal.AddPidList.forEach(e => {
    if (e.status) {
        pidList.push(e.pid);
    } 
});

if (pidList.length > 0) {
     this.addPidModal.loading = true;
              this.AddPidSaveBtnDis = true;
      this.ajax
        .post("/salepromotionactivity/AddAndDelActivityProduct", {
          activityId: this.$route.query.activityId,
          stock: 0,
          addPids: pidList,
          delPids: []
        })
        .then(response => {
              this.AddPidSaveBtnDis = false;
          this.addPidModal.loading = false;
          var data = response.data;
          if (data.Status) {
              this.addPidModal.visible = false
            this.uploadResult.modal.data = data.FailList;
            this.uploadResult.modal.visible = true;
            this.uploadResult.modal.loading = false;
            this.uploadResult.insertCount = data.InsertCount;
            this.loadData();
            this.bindActivityModel();
          } else {
            this.messageInfo(data.Msg);
          }
        });
}
} else {
      this.messageInfo("请输入商品PID");
    return false;
}
    },
    
    handleResetForm () {
      this.search_activityPro = {
        Pid: "",
        ProductName: "",
        LowProfile: null,
        HighProfile: null
      };
    },
    // 库存设置弹框
    setStockModalShow () {
      if (!this.products.table.data.length > 0) {
        this.messageInfo("请先添加活动商品");
        return;
      }
      this.setStockModal.visible = true;
      this.setStockModal.stock = null;
      if (this.products.select.length > 0) {
        this.setStockModal.title = "设置已勾选商品库存";
      } else {
        this.setStockModal.title = "设置活动下所有商品库存";
      }
    },
    setImageModalShow () {
      if (!(this.products.table.data.length > 0)) {
        this.messageInfo("请先添加活动商品");
        return;
      }
      this.productImgModal.visible = true;
      if (this.products.select.length > 0) {
        this.productImgModal.title = "设置已勾选商品列表牛皮癣";
      } else {
        this.productImgModal.title = "设置活动下所有商品列表牛皮癣";
      }
    },
    // 打开设置详情页牛皮癣窗口
    setDetailImageModalShow () {
      if (!(this.products.table.data.length > 0)) {
        this.messageInfo("请先添加活动商品");
        return;
      }
      this.productDetailImgModal.visible = true;
      if (this.products.select.length > 0) {
        this.productDetailImgModal.title = "设置已勾选商品详情页牛皮癣";
      } else {
        this.productDetailImgModal.title = "设置活动下所有商品详情页牛皮癣";
      }
    },
    
    addPidModalShow () {
      this.addPidModal.AddPidList = [
        {
          pid: "",
          tip: "",
          status: false
        }
      ];
      this.addPidModal.visible = true;
      this.AddPidSaveBtnDis = true;
      this.addPidModal.message = "";
    },
    // 设置库存
    setAllStock () {
      var value = this.setStockModal.stock;
      value = parseInt(Number(value));
      if (value <= 0) {
        this.messageInfo("请输入正确的库存数");
        this.setStockModal.stock = null;
      } else {
        var pids = [];
        if (this.products.select.length > 0) {
          pids = this.products.select;
        }
        this.ajax
          .post("/salepromotionactivity/SetProductLimitStock", {
            activityId: this.$route.query.activityId,
            stock: this.setStockModal.stock,
            pids: pids
          })
          .then(response => {
            var data = response.data;
            this.stockResult.modal.loading = false;
            if (data.Status) {
              this.setStockModal.visible = false;
              this.stockResult.status = true;
              this.stockResult.modal.data = data.FailList;
              this.stockResult.modal.visible = true;
              this.stockResult.count = data.Count;
              this.loadData();
              this.bindActivityModel();
            } else {
              this.messageInfo(data.Msg);
            }
          });
      }
    },
    // 设置商品列表牛皮癣
    setProductImg () {
      if (
        this.productImgUrl === "" ||
        this.productImgUrl == null ||
        this.productImgUrl === undefined
      ) {
        this.messageInfo("请上传图片");
        return false;
      }
      this.productImgModal.loading = true;
      this.ajax
        .post("/salepromotionactivity/SetProductImage", {
          activityId: this.$route.query.activityId,
          imgUrl: this.productImgUrl,
          pids: this.products.select
        })
        .then(response => {
          var data = response.data;
          if (data.Status) {
            this.productImgModal.loading = false;
            this.productImgUrl = "";
            this.productImgModal.visible = false;
            this.messageInfo("设置成功");
          } else {
            this.util.message.warning({
              content: data.Msg,
              duration: 3,
              closable: true
            });
          }
          this.loadData();
          this.bindActivityModel();
        });
    },
    // 设置商品详情页牛皮癣保存
    setProductDetailImg () {
      if (
        this.productDetailImgUrl === "" ||
        this.productDetailImgUrl == null ||
        this.productDetailImgUrl === undefined
      ) {
        this.messageInfo("请上传图片");
        return false;
      }
      this.productDetailImgModal.loading = true;
      this.ajax
        .post("/salepromotionactivity/SetDiscountProductDetailImg", {
          activityId: this.$route.query.activityId,
          imgUrl: this.productDetailImgUrl,
          pids: this.products.select
        })
        .then(response => {
          var data = response.data;
          if (data.Status) {
            this.productDetailImgModal.loading = false;
            this.productDetailImgUrl = "";
            this.productDetailImgModal.visible = false;
            this.messageInfo("设置成功");
          } else {
            this.util.message.warning({
              content: data.Msg,
              duration: 3,
              closable: true
            });
          }
          this.loadData();
          this.bindActivityModel();
        });
    },
    handleProductsPageChange (pageIndex) {
      this.products.page.current = pageIndex;
      this.loadData();
    },
    handleProductsPageSizeChange (pageSize) {
      this.products.page.pageSize = pageSize;
      this.loadData();
    },
    handleSearchPageChange (pageIndex) {
      this.searchResult.page.current = pageIndex;
      this.loadSearchProduct();
    },
    handleSearchPageSizeChange (pageSize) {
      this.searchResult.page.pageSize = pageSize;
      this.searchResult.page.current = 1;
      this.loadSearchProduct();
    },

    handleFormatError (file) {
      this.util.message.warning({
        content: file.name + "格式错误，必须是xls或xlsx文件",
        duration: 3,
        closable: true
      });
    },
    handleImgFormatError (file) {
      this.$Message.warning("请选择 .jpg  or .png  or .jpeg图片");
    },
    handleImgMaxSize (file) {
      this.$Message.warning("请选择不超过1000KB的图片");
    },
    handleImgSuccess (res, file) {
      if (res.Status) {
        this.productImgUrl = res.ImageUrl;
      } else {
        this.$Message.warning(res.Msg);
      }
    },
    handleDetailImgSuccess (res, file) {
      if (res.Status) {
        this.productDetailImgUrl = res.ImageUrl;
      } else {
        this.$Message.warning(res.Msg);
      }
    },
    ExportUploadResult () {
      debugger;

      window.open(
        "/SalePromotionActivity/ExportImportFailResult?failList=" +
          this.uploadResult.modal.data
      );
    },
    // 获取活动的商品列表
    loadData (pageIndex) {
      this.products.select = [];
      if (pageIndex > 0) {
        this.products.page.current = pageIndex;
      }
      this.search_activityPro.ActivityId = this.$route.query.activityId;
      this.products.table.loading = true;
      this.ajax
        .post("/salepromotionactivity/SelectProductList", {
          condition: this.search_activityPro,
          pageIndex: this.products.page.current,
          pageSize: this.products.page.pageSize
        })
        .then(response => {
          this.products.table.loading = false;
          var data = response.data;
          if (data.Status) {
            if (data.List !== null) {
              this.products.table.data = data.List;
            } else {
              this.products.table.data = [];
            }
            this.products.page.total = data.Count;
          } else {
            this.messageInfo(data.Msg);
          }
        });
    },
    handleFileFormatError (file) {
      this.util.message.warning({
        content: file.name + " 格式错误，必须是xls或xlsx文件",
        duration: 3,
        closable: true
      });
    },
    uploadTip () {
      this.uploadTipVisible = true;
    },
    uploadSuccess (res, file) {
      this.uploadTipVisible = false;
      if (res.Status) {
        this.uploadResult.modal.data = res.FailList;
        this.uploadResult.modal.visible = true;
        this.uploadResult.modal.loading = false;
        this.uploadResult.insertCount = res.InsertCount;
        this.loadData();
      } else {
        this.messageInfo(res.Msg);
      }
    },
    editActivity () {
      this.$router.push({
        path: "/discount/editactivity",
        query: {
          activityId: this.$route.query.activityId
        }
      });
    },
    waitAudit (status) {
      if (!(this.products.table.data.length > 0)) {
        this.messageInfo("请先添加活动商品");
        return;
      }
      this.IsScreenSpin = true;
      this.IsSpinSearchTab = true;
      var url = "/salepromotionactivity/SetActivityAuditStatus";
      if (status === 2) {
        this.$router.push({
          path: "/discount/waitaudit",
          query: {
            activityId: this.$route.query.activityId
          }
        });
      } else {
        this.ajax
          .post(url, {
            activityId: this.$route.query.activityId,
            auditStatus: status
          })
          .then(response => {
            this.IsScreenSpin = false;
            this.IsSpinSearchTab = false;
            if (response.data.Status) {
              this.$router.push({
                path: "/discount/waitaudit",
                query: {
                  activityId: this.$route.query.activityId
                }
              });
            } else {
            }
          });
      }
    },
    ok () {},
    // 工具函数
    messageInfo (value) {
      this.$Message.info({
        content: value,
        duration: 3,
        closable: true
      });
    },
    // 移除数组中某个元素
    spliceArr (arr, value) {
      if (arr.length === 0) {
        return [];
      }
      var newArr = [];
      arr.forEach(element => {
        if (element !== value) {
          newArr.push(element);
        }
      });
      return newArr;
    },
    // 判断集合中是否有某个元素
    checkHasValue (arr, value) {
      var has = false;
      arr.forEach(element => {
        if (element === value) {
          has = true;
        }
      });
      return has;
    }
  }
};
</script>
<style scoped>
.ivu-form-item {
  margin-bottom: 12px;
}

.ivu-tabs-tab-focused {
  background-color: red;
}

.ivu-form-item-content .ivu-select-dropdown {
  z-index: 999;
}

.tabSearch {
  height: 200px;
}
</style>
