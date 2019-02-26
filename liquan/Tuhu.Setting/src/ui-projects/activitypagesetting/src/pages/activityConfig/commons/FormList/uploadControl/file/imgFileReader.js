import { inArray } from './inArray';
/**
 * 读取图片文件
 *
 * @param {File} file 文件对象
 * @param {Function} readerSuccessCallback 读取完后的回调
 * @returns {file|null}
 */
export function imgFileReader(file, readerSuccessCallback) {
    // let _this = this;
    // let option = _this._options;
    let _ext = file.name.split('.').pop();

    // 如果不是图片格式不支持压缩或图片大小小于100kb，则直接上传
    if (!_ext || inArray(_ext.toLowerCase(), ['jpg', 'jpeg', 'png', 'gif', 'bmp', 'jpe']) < 0) {
        if (typeof readerSuccessCallback === 'function') {
            readerSuccessCallback();
        }
        return file;
    }

    let reader = new FileReader();

    reader.onload = function () {
        let result = this.result;
        let img = new Image();
        img.src = result;
        //      图片加载完毕之后进行压缩，然后上传
        if (img.complete) {
            callback();
        } else {
            img.onload = callback;
        }

        /**
         * 图片载完成或压缩完后的回调
         *
         */
        function callback() {
            if (typeof readerSuccessCallback === 'function') {
                readerSuccessCallback(img);
            }
            img = null;
        }
    };

    reader.readAsDataURL(file);
}
