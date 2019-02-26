export class Title {
    constructor(options) {
        this.options = Object.assign({
            title: ''
        }, options);
        this.setTitle();
    }
    /**
   * 设置页面标题
   * @param {string} titleText 标题文本
   * @returns {string}
   */
    setTitle(titleText) {
        let _title = '';
        let options = this.options;
        if (titleText) {
            _title = titleText;
        } else {
            _title = options && options.title;
        }
        if (_title && this._title !== _title) {
            this._title = _title;
            document.title = _title;
        }
        return _title;
    }
    /**
   * 获取页面标题
   * @returns {string}
   */
    getTitle() {
        let _title = this._title || '';
        if (!_title) {
            _title = document.title;
        }
        return _title;
    }
}

/**
 * 实例化Title
 *
 * @export
 * @param {*} args 参数
 * @returns {HttpClient}
 */
export function titleFactory (...args) {
    if (!titleFactory._title) {
        titleFactory._title = new Title(...args);
    }
    return titleFactory._title;
}
