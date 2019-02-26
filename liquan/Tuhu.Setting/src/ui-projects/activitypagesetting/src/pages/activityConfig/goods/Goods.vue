<template>
  <div>
    <goods-default-page
        v-if="filterPageId(tagOption, 'goods')"
        :tagOption="tagOption">
    </goods-default-page>
    <goods-edit-page
        v-if="filterPageId(tagOption, 'goodsEdit')"
        :tagOption="tagOption">
    </goods-edit-page>
    <seckill-page
        v-if="filterPageId(tagOption, 'goodsSeckill')"
        :tagOption="tagOption">
    </seckill-page>
    <group-booking
        v-if="filterPageId(tagOption, 'groupBooking')"
        :tagOption="tagOption">
    </group-booking>
    <sys-rec
        v-if="filterPageId(tagOption, 'sysRec')"
        :tagOption="tagOption">
    </sys-rec>
  </div>
</template>

<script>
import GoodsDefaultPage from './goodsDefault/GoodsDefaultPage';
import GoodsEditPage from './goodsDefault/GoodsEditPage';
import SeckillPage from './seckill/SeckillPage';
import GroupBooking from './groupBooking/GroupBooking';
import SysRec from './sysRec/SysRec';
import TabPage from '../tabPage/TabPage';

export default {
    extends: TabPage,
    components: {
        GoodsDefaultPage,
        GoodsEditPage,
        SeckillPage,
        GroupBooking,
        SysRec
    }
};
</script>

<style scoped>

</style>
