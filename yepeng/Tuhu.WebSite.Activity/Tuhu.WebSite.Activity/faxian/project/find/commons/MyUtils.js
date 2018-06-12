class MyUtils {
    constructor(props, context) {
        this.deviceTypes = ['others', 'pc', 'phone']
        this.channelTypes = ['others', 'app', 'wx']
        this.platformTypes = ['others', 'ios', 'android']
    }

    getQueryString(name) { 
        const reg = new RegExp("(\\?|\\&)"+name+"=([^\\&]+)");
        const match = location.href.match(reg);

        return match ? match[2] : '';
    }

    isPc() {
        if (this.getIsPc === undefined) {
            // this.getIsPc = !/AppleWebKit.*Mobile.*/.test(navigator.userAgent) && !this.isApp();
            this.getIsPc = !('ontouchstart' in window || window.DocumentTouch && document instanceof window.DocumentTouch) && !this.isApp();
        }

        return this.getIsPc;
    }

    isAndroid() {
        if (this.checkAndroid === undefined) {
            this.checkAndroid = /android/i.test(navigator.userAgent);
        }
        
        return this.checkAndroid;
    }

    isIOS() {
        if (this.checkIOS === undefined) {
            this.checkIOS = /iphone|ipad|ipod/i.test(navigator.userAgent);
        }
        
        return this.checkIOS;
    }

    isApp() {
        if (this.isAndroid()) {
            return navigator.userAgent.indexOf('tuhuAndroid') > -1
        }

        return navigator.userAgent.indexOf('tuhuIOS') > -1

        // return !!window.isVersion;
    }
    
    isWX() {
        return navigator.userAgent.toLowerCase().match(/MicroMessenger/i) == "micromessenger"
    }

    ga(category, action, label, value) {
        ga('send', 'event', category, action, label, value);
    }

    whatDevice() {
        if ( this.isPc() ) {
            return this.deviceTypes[1]
        }

        return this.deviceTypes[2]
    }

    whatChannel() {
        if ( this.isApp() ) {
            return this.channelTypes[1]
        } else if ( this.isWX() ) {
            return this.channelTypes[2]
        } else {
            return this.channelTypes[0]
        }
    }

    whatPlatform() {
        if ( this.isIOS() ) {
            return this.platformTypes[1]
        } else if ( this.isAndroid() ) {
            return this.platformTypes[2]
        } else {
            return this.platformTypes[0]
        }
    }

    listToIos(url) {
        return 'tuhuaction://segue#TNHBH5VViewController#{"url": "' + url + '"}'
    }

    listUrl(fromTag) {
        let tagName = ''
        if (this.isIOS()) {
            tagName = encodeURI(encodeURI(fromTag))
        } else {
            tagName = encodeURI(fromTag)
        }
        return location.href.split('?')[0] + '?title='+ tagName + '&ArticleTagId=';
    }

    isHalf() {
        return  document.body.scrollTop + window.innerHeight >=
                document.body.clientHeight - window.innerHeight
    }

    isBottom() {
        return  document.body.scrollTop + window.innerHeight ===
                document.body.clientHeight
    }

    getSearches(key, length) {
        const arrStr =  window.localStorage.getItem(key);

        if (arrStr) {
          const arr = JSON.parse(arrStr);
          if(arr.length > length){
            return arr.slice(0, length);
          }
          return arr;
        }

        return '';
    }

    setSearches(key, value) {
        if (!value) {
          return
        }

        const arrStr = window.localStorage.getItem(key);
        let arr =[];

        if (arrStr) {
          arr = JSON.parse(arrStr)
        }

        if (arr.indexOf(value)<0) {
          arr.unshift(value);
        }

        window.localStorage.setItem(key, JSON.stringify(arr))
    }

}

export default new MyUtils()