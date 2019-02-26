<template>
  <div>
      <maintain-pricing-page
        v-if="filterPageId(tagOption, 'maintainPricing')"
        :tagOption="tagOption"></maintain-pricing-page>
      <count-down-page
        v-if="filterPageId(tagOption, 'countDown')"
        :tagOption="tagOption"></count-down-page>
      <video-config-page
        v-if="filterPageId(tagOption, 'videoConfigPage')"
        :tagOption="tagOption"></video-config-page>
      <text-link-page
        v-if="filterPageId(tagOption, 'textLinkPage')"
        :tagOption="tagOption"></text-link-page>
      <foot-tabs-page
        v-if="filterPageId(tagOption, 'footTabsPage')"
        :tagOption="tagOption"></foot-tabs-page>
      <maintenance-vehicle-page
        v-if="filterPageId(tagOption, 'maintenanceVehiclePage')"
        :tagOption="tagOption"></maintenance-vehicle-page>
  </div>
</template>

<script>
import MaintainPricingPage from './maintainPricing/MaintainPricingPage';
import CountDownPage from './countDown/CountDownPage';
import VideoConfigPage from './videoConfig/VideoConfigPage';
import TextLinkPage from './textLink/TextLinkPage';
import TabPage from '../tabPage/TabPage';
import FootTabsPage from './footTabs/FootTabsPage';
import MaintenanceVehiclePage from './maintenanceVehicle/MaintenanceVehiclePage';

export default {
    extends: TabPage,
    components: {
        MaintainPricingPage,
        CountDownPage,
        VideoConfigPage,
        TextLinkPage,
        FootTabsPage,
        MaintenanceVehiclePage
    }
};
</script>

<style scoped>

</style>
