import * as Config from './constants/Config'
import Urls from './commons/Urls'
import 'es6-promise'
import 'fetch-ie8'

const pageIndex = 1;
const tag = getQueryString('ArticleTagId') || '';
const author = getQueryString('ArticleAuthor') || '';
const query = getQueryString('ArticleTitle') || '';

function getQueryString(name) {
    const reg = new RegExp(name + '=(.+?)(\&|$)')
    const match = location.href.match(reg);

    return match ? match[1] : '';
}

function polyFills() {
	const promises = [];

	if (!Object.assign) {
		promises.push(new Promise((resolve, reject) => {
			require.ensure([], (require) => {
				Object.assign = require('object.assign');
				resolve();
			})
		}));	
	}

	if (typeof Symbol === 'undefined') {
		promises.push(new Promise((resolve, reject) => {
			require.ensure([], (require) => {
				window.Symbol = require('Symbol');
				resolve();
			})
		}));	
	}

	if (/android/i.test(navigator.userAgent)) {
		promises.push(new Promise((resolve, reject) => {
			require.ensure([], (require) => {
				require('../../lib/WebViewJavascriptBridge.min.js');
				//connect Android
				function connectWebViewJavascriptBridge(callback) {
				    if (window.WebViewJavascriptBridge) {
				        callback(WebViewJavascriptBridge)
				    } else {
				        document.addEventListener(
				            'WebViewJavascriptBridgeReady'
				            , function () {
				                callback(WebViewJavascriptBridge)
				            },
				            false
				        );
				    }
				  }

				connectWebViewJavascriptBridge((bridge) => {
					bridge.init(function (message, responseCallback) {
					  var data = {
					      'Javascript Responds': 'Wee!'
					  };
					  responseCallback(data);
					});

					bridge.registerHandler("functionInJs", function (data, responseCallback) {
					  const responseData = "Javascript Says Right back aka!";
					  responseCallback(responseData);
					});

					resolve();
				})
			})
		}));
	}

	return promises;
}

const promises = polyFills();

window.preRequest = Urls.callApi(pageIndex, tag, author, query);
window.prePolyfill = Promise.all(promises);