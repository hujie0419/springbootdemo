<template>
    <div>
        <div class="searchItem">
            <label class="item-label">品牌 车系:</label>
            <div class="item-content">
                <Select filterable v-model="searchinfo.brand" @on-change="handelBrandChange" placeholder="请选择品牌" style="width:200px">
                    <Option v-for="item in brands" :value="item" :key="item">{{ item }}</Option>
                </Select>
                <Select filterable v-model="searchinfo.series" placeholder="请选择车系" style="width:200px; margin-left:8px;">
                    <Option v-for="item in series" :value="item" :key="item">{{ item }}</Option>
                </Select>
            </div>
        </div>
        <div class="searchItem">
            <label class="item-label">系列:</label>
            <div class="item-content">
                <Checkbox
                    :indeterminate="indeterminate"
                    :value="checkAll"
                    @click.prevent.native="handleCheckAll" style="">全部</Checkbox>
                <CheckboxGroup v-model="searchinfo.brandcategories" @on-change="handleCheckBrandCategory" style="display:inline-block">
                    <Checkbox v-for="category in brandcategories" :key="category" :label="category"></Checkbox>
                </CheckboxGroup>
            </div>
        </div>
        <div class="searchItem">
            <label class="item-label">价格区间:</label>
            <div class="item-content">
                <InputNumber v-model="searchinfo.minprice" :max="9999" :min="1" style="width: 100px"></InputNumber>
                <span>&nbsp;到&nbsp;</span>
                <InputNumber v-model="searchinfo.maxprice" :max="9999" :min="1" style="width: 100px"></InputNumber>
                <span>&nbsp;万</span>
            </div>
        </div>
        <div class="searchItem">
            <label class="item-label">品牌车型:</label>
            <div class="item-content">
                    <a href="javascript:void(0)" @click="handleBrandLetterClick(letter)" v-for="letter in brandletters" :key="letter" style="margin-right:18px">{{ letter }}</a>
            </div>
        </div>
        <div v-if="currentbrands.length > 0" class="searchItem" style="margin-top: -10px">
            <label class="item-label"></label>
            <div class="item-content">
                <CheckboxGroup v-model="searchinfo.brands" style="width:600px;line-height:25px;">
                    <Checkbox v-for="brand in currentbrands" :key="brand" :label="brand"></Checkbox>
                </CheckboxGroup>
            </div>
        </div>
        <div class="searchItem">
            <label class="item-label">VehicleID:</label>
            <div class="item-content">
                <Input v-model="searchinfo.vehicleId" style="width: 150px"/>        
            </div>
        </div>
        <div class="searchItem">
            <label class="item-label"></label>
            <div class="item-content">
                <Checkbox v-model="searchinfo.isconfiged">显示配置数据的车型</Checkbox>        
            </div>
        </div>
        <div class="searchItem">
            <label class="item-label"></label>
            <div class="item-content">
                <Button type="success" @click="search">查询</Button>    
                <Button type="warning" @click="empty">重置</Button>           
            </div>
        </div>
    </div>
</template>

<script>
export default {
   props: {
        searchdata: { type: Object }
    },
    data () {
        return {
            brands: [],
            series: [],
            brandcategories: [],
            indeterminate: false,
            checkAll: false,
            currentbrands: [],
            initdata: {},
            searchinfo: {}
        }
    },
    computed: {
        brandletters () {
            var list = [];
            this.brands.forEach(element => {
                var letter = element.charAt(0);
                if (list.indexOf(letter) < 0) {
                    list.push(letter);
                }
            });

            return list;
        }
    },
    created () {
        this.searchinfo = this.util.deepCopy(this.searchdata);
        console.log(this.searchinfo);
        this.ajax.get("/vehicle/getallbrandcategories").then((response) => {
            this.brandcategories = response.data.Data;
        });

        this.ajax.get("/vehicle/GetAllVehicleBrands").then((response) => {
            this.brands = response.data.Data;
        });
    },
    methods: {
        handleCheckAll () {
            if (this.indeterminate) {
                this.checkAll = false;
            } else {
                this.checkAll = !this.checkAll;
            }
            this.indeterminate = false;

            if (this.checkAll) {
                this.searchinfo.brandcategories = this.brandcategories;
            } else {
                this.searchinfo.brandcategories = [];
            }
        },
        handleCheckBrandCategory (data) {
            if (data.length === this.brandcategories.length) {
                this.indeterminate = false;
                this.checkAll = true;
            } else if (data.length > 0) {
                this.indeterminate = true;
                this.checkAll = false;
            } else {
                this.indeterminate = false;
                this.checkAll = false;
            }
        },
        handleBrandLetterClick (letter) {
            this.currentbrands = [];
            this.brands.forEach(element => {
                if (element.indexOf(letter) === 0) {
                    this.currentbrands.push(element);
                }
            });
        },
        handelBrandChange (brand) {
            this.ajax.get("/vehicle/GetVehicleSeriesByBrand?brand=" + brand).then((response) => {
                this.series = response.data.Data;
            });
        },
        search () {
            console.log(this.searchinfo);
            this.$emit('search', this.searchinfo);
        },
        empty () {
            this.searchinfo = this.util.deepCopy(this.searchdata);
        }
    }
}
</script>

<style lang="less">
    .searchItem {
        margin-bottom: 18px;

        .item-label {
                width: 80px;
                text-align: right;
                display: inline-block;
            }

        .item-content {
            display: inline-block;
            margin-left: 18px;
        }
    }
</style>
