import loadingSrc from '../img/loading-lazy.png'
import MyUtils from '../commons/MyUtils'

class LazyImg extends React.Component {
	constructor(props, context) {
		super(props, context);

		const item = this.props.data.src;
		const mode = this.props.data.mode;
		let colomn = 1;

		if (mode === 3 || mode === 5) {
			colomn = 2
		}else{
			colomn = 0
		}

		this.loadingSrc = this.getImageUrl(item, colomn, 0.5);
		this.biggerSrc = this.getImageUrl(item, colomn);

		this.state = {
			loadingSrc: loadingSrc
		};

		this.wHeight = window.innerHeight;
	}

	replaceCdn(url) {
		if(url){
			 return url.replace('image.tuhu.cn', 'img' + parseInt(Math.random() * 10) + '-tuhu-cn.alikunlun.com');
		}
  }

  getImageUrl(url, columns, bigger = 2) {
	  if(url){
		const wh = this._getWH(url);
		const height = this._getImageHeight(Object.assign({columns}, wh)) * bigger;
		const width = parseInt(height / wh.height * wh.width);

		if (MyUtils.isIOS()) {
			if (/[^\s]+\.(jpg|gif|bmp)/i.test(url)) {
				url = url.replace('.webp', '.jpg');
			} else {
				url = url.replace('.webp', '.png');
			}
		}
		
		return this.replaceCdn(url.replace(/@100Q/, `@${width}w_${height}h_100Q`));
	  }
  }

  _getWH(url) {
	  if(url){
		let arr = url.match(/(\d+)w_(\d+)\h/);
        if(arr==null||arr.length<=0){
			arr=url.match(/_w(\d+)_h(\d+)/);
		}
		const width = arr[1] - 0;
		const height = arr[2] - 0;

		return {width, height};
	  }
  }

  getImageHeight(url, columns) {
	  if(url){
		return this._getImageHeight(Object.assign({columns}, this._getWH(url, columns)));
	  }
  }

  _getImageHeight({width, height, columns}) {
      let number = 1;

      if (columns === 0) {
          number = 1;
      } else if (columns === 1 || columns === 4) {
          number = 2;
      } else {
          number = 3;
      }

      return parseInt(window.innerWidth / width * height / number);
  }

	get fix() {
		return this.wHeight * 1;
	}

	handleScroll() {
		if (this.state.loadingSrc === loadingSrc) {
			const pos = this.refs.loading.getBoundingClientRect();
			const fix = this.fix;

			if (pos.top < fix + this.wHeight + pos.height && pos.bottom > -pos.height - fix) {
				this.loadImg(pos);
			}
		}
	}

	loadImg(pos) {
		if (pos.left >= 0 && pos.right <= window.innerWidth) {
			this._loadImg();
		} else {
			setTimeout(() => {
				this._loadImg();
			}, this.getRandom(1000))
		}
	}

	getRandom(mseconds) {
		return parseInt(mseconds * Math.random());
	}

	_loadImg() {
		if (this.isNext()) {
			setTimeout(() => {
				if (this.state.loadingSrc === loadingSrc) {
					this.setState({
						loadingSrc: this.loadingSrc
					});
				}
			}, 100);
		} else {
			if (this.state.loadingSrc === loadingSrc) {
				this.setState({
					loadingSrc: this.loadingSrc
				});
			}
		}
	}

	componentDidMount() {
		document.addEventListener('scroll', this.handleScroll.bind(this));

		setTimeout(() => {
			this.handleScroll();
		}, 0);
	}

	componentWillUnmount() {
		document.removeEventListener('scroll', this.handleScroll.bind(this));
	}

	handleError() {
		if (this.state.loadingSrc === this.loadingSrc || this.state.loadingSrc === this.biggerSrc) {
		}
	}

	isNext() {
		const pos = this.refs.loading.getBoundingClientRect();

		if (pos.top >= this.wHeight && pos.top < this.fix + this.wHeight + pos.height || pos.bottom > -pos.height - this.fix && pos.bottom <= -pos.height) {
			return true;
		}

		return false;
	}

	handleLoad() {
		const dom = this.refs.loading;

		if (this.state.loadingSrc !== loadingSrc && dom.className) {
			dom.className = '';
		} 
		
		if (this.state.loadingSrc === this.loadingSrc) {
			setTimeout(() => {
				this.setState({
					loadingSrc: this.biggerSrc
				});
			}, 0);
		}
	}
	render() {
		return (<img onError={this.handleError.bind(this)} onLoad={this.handleLoad.bind(this)} src={this.state.loadingSrc} ref="loading" className="loading"  width="100%" />);
	}
}

export default LazyImg