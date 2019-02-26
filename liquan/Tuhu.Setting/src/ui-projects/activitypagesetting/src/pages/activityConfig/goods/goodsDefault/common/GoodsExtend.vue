<script>
import TabPage from '../../../tabPage/TabPage';
import { getBrandsByCategory } from './GoodsApi';

export default {
    extends: TabPage,
    data () {
        return {
            brandConfig: null
        };
    },
    methods: {
        /**
         * 根据分类查询商品品牌
         * @param {String} categoryId 商品类目id
         * @param {Function} cb 完成后的回调方法
         */
        getBrand (categoryId, cb) {
            const params = {
                categoryId: categoryId.split('|')[0]
            };
            getBrandsByCategory(params)
                .subscribe(res => {
                    const brandList = [{
                        nameText: '请选择',
                        value: ''
                    }];
                    const _res = res && res.data && res.data.BrandNames;
                    _res && _res.forEach(item => {
                        brandList.push({
                            nameText: item || '',
                            value: item || ''
                        });
                    });
                    if (cb && cb instanceof Function) {
                        cb(brandList);
                    }
                });
        },
        /**
         * 获取品牌列表
         * @param {String} categoryControlName 商品类目的controlName
         * @param {String} brandName 品牌
         * @param {Function} cb 回调函数
         * @param {Boolean} isNolimited 是否要设置品牌为不限
         */
        getBrandList (categoryControlName, brandName = 'ProductBrand', cb, isNolimited = false) {
            const _that = this;
            let formModel = _that.formModel;

            const categoryId = formModel && formModel.get(categoryControlName);
            _that.brandConfig && _that.$set(_that.brandConfig, 'list', [{
                nameText: '不限',
                value: ''
            }]);
            if (isNolimited) {
                let defaultCon = formModel.get(brandName);
                defaultCon.setValue('');
            }
            if (categoryId && categoryId.value) {
                _that.getBrand(categoryId.value, (brandList) => {
                    _that.brandConfig && _that.$set(_that.brandConfig, 'list', brandList);
                    if (cb instanceof Function) {
                        cb();
                    } else {
                        let con = formModel.get(brandName);
                        con.setValue((brandList && brandList[0] && brandList[0].value) || '');
                    }
                });
            }
        }
    }
};
</script>
