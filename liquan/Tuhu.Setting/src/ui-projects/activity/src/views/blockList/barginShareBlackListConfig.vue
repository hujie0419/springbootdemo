<template>    
<div>
    <blackListComponent v-bind:blockSystemModel="blockSystemModel"></blackListComponent>
</div>
</template>

<script>
import blackListComponent from '@/components/blackListComponent'
export default {
    data () {
        return {
            blockSystemModel: {
                blockSystem: "ShareBargain",
                blockSystemName: "砍价"
            }
        }
    },
    components: { 
        blackListComponent 
    }
}
</script>
