<template>
  <el-upload
    class="extend-control-item-cont"
    action=''
    :accept='accept'
    :readonly="controlConfig.readonly"
    :disabled="controlConfig.disabled"
    :show-file-list="false"
    :http-request="httpRequest"
    :before-upload="onBeforeUpload">
      <el-button
         @click="extendClick(controlConfig)"
        :readonly="controlConfig.readonly"
        :disabled="controlConfig.disabled"
        size="small" type="primary">{{controlConfig.nameText}}</el-button>
      <!-- action="https://jsonplaceholder.typicode.com/posts/"
      :on-preview="handlePreview"
      :on-remove="handleRemove"
      :before-remove="beforeRemove"
      :on-exceed="handleExceed" -->
      <!-- <el-button size="small" type="primary">点击上传</el-button>
      <div slot="tip" class="el-upload__tip">只能上传jpg/png文件，且不超过500kb</div> -->
  </el-upload>
</template>

<script>
import { imgFileReader } from './file/imgFileReader.js';
import { inArray } from './file/inArray';
import ControlExtend from '../common/controlExtend/ControlExtend';
import apis from '../../apis/commons/commonApi';

const STRING_DOT = '.'; // 字符串点号
const STRING_EMPTY = ''; // 空字符

// 默认的文件类型列表
const DEFAULT_FILE_TYPE_LIST = ['Image', 'Video'];

// 默认信息对象
const defaultInfo = {
    DEFAULT_ERROR_MESSAGE: '请上传正确文件格式', // 文件的格式不正确的提示信息
    DEFAULT_SIZE_ERROR_MESSAGE: '文件大小过大', // 文件大小过大时的提示信息
    DEFAULT_MAXWIDTH_ERROR_MESSAGE: '图片宽度过大', // 文件宽度过大的提示信息
    DEFAULT_MAXHEIGHT_ERROR_MESSAGE: '图片高度过大', // 文件高度过大的提示信息
    DEFAULT_WIDTH_ERROR_MESSAGE: '图片宽度不符合', // 文件宽度不相等的提示信息
    DEFAULT_HEIGHT_ERROR_MESSAGE: '图片高度不符合', // 文件高度不相等的提示信息
    DEFAULT_MINWIDTH_ERROR_MESSAGE: '图片宽度不够', // 文件宽度不够的提示信息
    DEFAULT_MINHEIGHT_ERROR_MESSAGE: '图片高度不够', // 文件高度不够的提示信息
    DEFAULT_SCALE_ERROR_MESSAGE: '图片长宽比例不符合', // 文件比例不正确的提示信息
    DEFAULT_EXTENSION_LIST: {
        'Image': ['png', 'jpg', 'jpeg', 'gif'],
        'Video': ['mp4', 'ogg', 'flv', 'wmv', 'rmvb'],
        'Ae': ['zip']
    } // 默认的文件后缀
};

export default {
    extends: ControlExtend,
    data () {
        const _validFile = this.controlConfig.validFile || {};
        let type = ((_validFile.type||'')+'').toLowerCase().replace(/^[a-z]/i, txt => {
            return (txt + '').toUpperCase();
        });
        return {
            fileType: type,
            extension: null
        };
    },
    computed: {
        accept () {
            let res;
            const extension = defaultInfo.DEFAULT_EXTENSION_LIST[this.fileType];
            // const accept = [];
            // this.extension.forEach(extension => {
            //     accept.push();
            // });
            let type = (extension && this.fileType);
            if (type === 'Ae') {
                res = 'application/zip';
            } else {
                res = (type || '*') + '/*';
            }
            return res;
        }
    },
    methods: {
        onBeforeUpload (file) {
            let _that = this;
            // let loading;
            // let timeout = setTimeout(() => {
            //     if (timeout === undefined) {
            //         return;
            //     }
            //     timeout = undefined;
            let loading = _that.$loading({
                lock: true,
                text: '文件读取中，请稍候。。。',
                // spinner: 'el-icon-loading',
                background: 'rgba(0, 0, 0, 0.1)'
            });

            let _ext = file && file.name.split('.').pop();
            this.extension = _ext;
            // }, 100);
            let orientation = Math.abs((_that.controlConfig && _that.controlConfig.orientation) || 0);
            let validFile = switchMeasure(_that.controlConfig && _that.controlConfig.validFile, orientation);

            return new Promise((resolve, reject) => {
                if (!validFile) {
                    // if (typeof timeout !== 'undefined') {
                    //     timeout = undefined;
                    //     clearTimeout(timeout);
                    // }
                    loading && loading.close();
                    loading = null;
                    resolve(true);
                } else {
                    getValidFile(file).then(valid => {
                        // if (typeof timeout !== 'undefined') {
                        //     timeout = undefined;
                        //     clearTimeout(timeout);
                        // }
                        loading && loading.close();
                        loading = null;

                        if (valid) {
                            _that.$$errorMsg(valid, {type: 'error'});
                            reject(valid);
                        } else {
                            resolve(true);
                        }
                    });
                }
            });

            /**
             * 获取文件参数结果
             * @param {File} file 文件
             * @returns {Promise}
             */
            function getValidFile(file) {
                return new Promise((resolve, reject) => {
                    let res = null;
                    let extension;

                    if (typeof validFile.extension !== 'undefined' && validFile.extension.length > 0) {
                        extension = validFile.extension;
                    } else {
                        extension = defaultInfo.DEFAULT_EXTENSION_LIST[_that.fileType] || undefined;
                    }

                    // 验证文件格式
                    if (typeof extension !== 'undefined') {
                        if (extension instanceof Array) {
                            if (extension.length <= 0) {
                                extension = undefined;
                            }
                        } else {
                            extension = [extension];
                        }
                        if (extension && inArray(_ext.toLowerCase(), extension) < 0) {
                            res = validFile.typeErrorMsg || defaultInfo.DEFAULT_ERROR_MESSAGE;
                        }
                    }

                    // 验证文件大小
                    !res && hasMeasure('limitSize', Math.max(validFile.limitSize || 0, file.size / 1024), () => {
                        res = validFile.sizeErrorMsg || defaultInfo.DEFAULT_SIZE_ERROR_MESSAGE; // ('文件超过了' + validFile.limitSize + 'kb');
                    });

                    // 验证图片尺寸
                    if (!res && (typeof validFile.limitMaxWidth !== 'undefined' ||
                        typeof validFile.limitMaxHeight !== 'undefined' ||
                        typeof validFile.limitWidth !== 'undefined' ||
                        typeof validFile.limitHeight !== 'undefined' ||
                        typeof validFile.limitMinWidth !== 'undefined' ||
                        typeof validFile.limitMinHeight !== 'undefined' ||
                        typeof validFile.limitScale !== 'undefined')) {
                        // 验证图片宽高
                        imgFileReader(file, img => {
                            if (img) {
                                let _imgw = img.width;
                                let _imgh = img.height;

                                // 最大宽高
                                !res && hasMeasure('limitMaxWidth', Math.max(_imgw, validFile.limitMaxWidth), () => {
                                    res = validFile.maxWidthErrorMsg || defaultInfo.DEFAULT_MAXWIDTH_ERROR_MESSAGE;
                                });
                                !res && hasMeasure('limitMaxHeight', Math.max(_imgh, validFile.limitMaxHeight), () => {
                                    res = validFile.maxHeightErrorMsg || defaultInfo.DEFAULT_MAXHEIGHT_ERROR_MESSAGE;
                                });

                                // 相等宽高
                                !res && hasMeasure('limitWidth', _imgw, () => {
                                    res = validFile.widthErrorMsg || defaultInfo.DEFAULT_WIDTH_ERROR_MESSAGE;
                                });
                                !res && hasMeasure('limitHeight', _imgh, () => {
                                    res = validFile.heightErrorMsg || defaultInfo.DEFAULT_HEIGHT_ERROR_MESSAGE;
                                });

                                // 最小宽高
                                !res && hasMeasure('limitMinWidth', Math.min(_imgw, validFile.limitMinWidth), () => {
                                    res = validFile.minWidthErrorMsg || defaultInfo.DEFAULT_MINWIDTH_ERROR_MESSAGE;
                                });
                                !res && hasMeasure('limitMinHeight', Math.min(_imgh, validFile.limitMinHeight), () => {
                                    res = validFile.minHeightErrorMsg || defaultInfo.DEFAULT_MINHEIGHT_ERROR_MESSAGE;
                                });

                                // 比例
                                !res && hasMeasure('limitScale', _imgw/_imgh, () => {
                                    res = validFile.scaleErrorMsg || defaultInfo.DEFAULT_SCALE_ERROR_MESSAGE;
                                });
                            }
                            resolve(res);
                        });
                    } else {
                        resolve(res);
                    }
                });
                /**
                 * 适配尺寸
                 * @param {string} key 配置的key
                 * @param {number} measure 尺寸
                 * @param {Function} errCb 适配到的错误回调
                 */
                function hasMeasure(key, measure, errCb) {
                    // 最大宽高
                    if (parseInt(validFile[key], 10) >= 0 && validFile[key] + '' !== measure + '') {
                        if (errCb instanceof Function) {
                            errCb();
                        }
                    }
                }
            }
        },
        httpRequest (fileData) {
            const params = new FormData();
            let type = this.fileType || 'Image';
            type = type === 'Ae' ? 'Image' : type;
            params.append('FileType', type);
            params.append('file', fileData.file);
            params.append('Extension', STRING_DOT + this.extension);
            this.$http.post(apis.FileUpload, {
                apiServer: 'apiServer',
                isLoading: true,
                data: params
            }).subscribe(res => {
                let _res = res && res.data;
                let _url = _res && _res.ResponseLinkUrl;
                if (_url) {
                    this.myValue = _url;
                }
            });
        }
    }
};

/**
 * 切换尺寸
 * @param {string} data 数据
 * @param {Function} orientation 角度
 * @returns {Object}
 */
function switchMeasure(data, orientation) {
    let res = data;
    if (data) {
        switch (orientation + '') {
            case '90':
                res = {
                    limitMaxWidth: data.limitMaxHeight,
                    limitMaxHeight: data.limitMaxWidth,
                    limitWidth: data.limitHeight,
                    limitHeight: data.limitWidth,
                    limitMinWidth: data.limitMinHeight,
                    limitMinHeight: data.limitMinWidth,
                    limitScale: (data.limitScale && (1/data.limitScale)) || undefined
                };
                break;

            default:
                break;
        }
    }
    return res;
}
</script>
