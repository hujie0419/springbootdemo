import React, { Component, PropTypes } from 'react'
import setTopSrc from '../img/list/drawable-xhdpi/zhiding.png' 
import LazyImg from '../commons/LazyImg'
import MyUtils from '../commons/MyUtils'
import * as Config from '../constants/Config'

export default class List extends Component {
  constructor(props, context) {
    super(props, context);

    this.articleTag = MyUtils.getQueryString('ArticleTagId') || null;
    this.articleAuthor = MyUtils.getQueryString('ArticleAuthor') || null;
    this.item = null;
    this.url = '';
    this.categories = ['toutiao_list', 'toutiao_tag_list', 'toutiao_author_list']
  }

  getTopspan(item) {
    if (item.CustomTag) {
      return (<span className="zhiding">
                  <img src={setTopSrc} />
                </span>);
    }

    return null;
  }

  getImageSrc(item) {
    if(item.CoverImage){
      const arr = item.CoverImage.split(';');

      if (item.CoverMode === 3||item.CoverMode === 5) {
        return [arr[1], arr[2], arr[3]];
      }

      return arr[1];
    }

    return null;
  }

  showTime(item) {
    const date = +new Date(item.PublishDateTime);
    const now = +new Date();

    const oneSecond = 1000;
    const oneMinute = 60 * oneSecond;
    const oneHour = oneMinute * 60;
    const oneDay = 24 * oneHour;

    const dis = now - date;

    const day = parseInt(dis / oneDay);
    if (day > 0) {
      return day + '天之前';
    }

    // dis = dis - day * oneDay;
    const hour = parseInt(dis / oneHour);
    if (hour > 0) {
      return hour + '小时之前';
    }

    // dis = dis - day * oneHour;
    const minute = parseInt(dis / oneMinute);
    if (minute > 0) {
      return minute + '分钟之前';
    }

    // dis = dis - day * oneMinute;
    const second = parseInt(dis / oneSecond);
    if (second > 0) {
      return second + '秒之前';
    }
  }

  getShareImg(item) {
    if(item.CoverImage){
      const arr = item.CoverImage.split(';');
      return arr[1];
    }

    return ''
  }

  androidPara() {
    const params = {};

    params.androidValue = [
      { 
        Url: this.url, 
        shareImage: this.getShareImg(this.item),
        shareUrl: this.url,
        shareDescrip: this.item.ContentDes,
        shareTitle: this.item.Title,
        isShowShareIcon: false,
        type: 3 
      }
    ]

    params.androidKey = Config.ANDROID_KEY;

    return params;
  }

  goAndroid() {
    window.WebViewJavascriptBridge.callHandler('jumpActivityBridge', encodeURIComponent(encodeURIComponent(JSON.stringify(this.androidPara()))))
  }

  goIOS() {
    window.location.href = MyUtils.listToIos(this.url);
  }

  dealCategory() {
    let category = '';

    if (this.articleTag) {
        category = this.categories[1];
      } else if (this.articleAuthor){
        category = this.categories[2];
      } else {
        category = this.categories[0];
      }

    return category;
  }

  dealClick() {   
    if (MyUtils.isAndroid()) {
      this.goAndroid()
    } else {
      this.goIOS()
    }
  }

  jumpArticle(item) {
    this.url = Config.ARTICLE_URL + item.Id+"&bgColor="
                +encodeURIComponent(encodeURIComponent("#ffffff"))
                +"&textColor="+encodeURIComponent(encodeURIComponent("#333333"));
    this.item = item;
    MyUtils.ga(this.dealCategory(), 'click_article', this.item.Id, this.item.Index);
    if(window.Ta){
      Ta.Run("home_click_点击文章", "event", "ArticleId", this.item.Id);
    }
    this.dealClick();
  }

  jumpTagList(item) {
    this.url = MyUtils.listUrl(item.FromTag) + item.FromTagId+"&bgColor="+encodeURIComponent(encodeURIComponent("#ffffff"))+"&textColor="+encodeURIComponent(encodeURIComponent("#333333"));
    this.item = item;
    MyUtils.ga(this.dealCategory(), 'click_tag', this.item.FromTag, '');
    this.dealClick();
  }

  /*getStyle(dom) {
    const styleObj = {}
    const styles = window.getComputedStyle(dom)

    styleObj.width = parseInt(styles.width)
    styleObj.height = parseInt(styles.height)

    return styleObj
  }

  resetWidth(father, child) {
    const fw = this.getStyle(father).width;
    const cw = this.getStyle(child).width;

    if (cw > fw) {
      const mrl = (cw - fw) / 2

      this.setState({
        dlRightStyle: {
          marginLeft : '-' + mrl + 'px'
        }
      })
    }
  }*/

  getOnePic(item, index) {
     return (<section key={index} className="section">
                <div className="floor0" onClick={this.jumpArticle.bind(this,item)}>
                  <dl className="content dl-content">
                    <dd className="dl-right">
                      <LazyImg data={{src:this.getImageSrc(item),mode:item.CoverMode}} />
                    </dd>
                    <dt className="dl-left">
                      <div className="dt-top">
                        <h5 className="shenglu">{item.Title}</h5>
                      </div>
                      <div className="infoLine">
                        {this.getTopspan(item)}
                        <span className="info">
                          <span>{item.Description}</span>
                        </span>
                      </div>
                    </dt>
                  </dl>
                </div>
              </section>)
  }

  getCommon(item, index) {
    return (<section key={index} className="section">
              <div className="floor0" onClick={this.jumpArticle.bind(this,item)}>
                <div className="content">
                  <h5>{item.Title}</h5>
                </div>
                <div>
                <div className="infoLine">
                  {this.getTopspan(item)}
                  <span className="info">
                    <span>{item.Description}</span>
                  </span>
                </div>
                </div>
              </div>
            </section>);
  }

  getThreePic(item, index) {
    const images = this.getImageSrc(item);

    return (<section key={index} className="section">
                <div className="floor0" onClick={this.jumpArticle.bind(this,item)}>
                  <div className="content">
                    <h5>{item.Title}</h5>
                  </div>
                  <div className="showImg">
                    <div className="three-pic">
                      <LazyImg data={{src:images[0],mode:item.CoverMode}} />
                      <span className="blank-r"></span>
                    </div>
                    <div className="three-pic">
                      <span className="blank-l"></span>
                      <LazyImg data={{src:images[1],mode:item.CoverMode}} />
                      <span className="blank-r"></span>
                    </div>
                    <div className="three-pic">
                      <span className="blank-l"></span>
                      <LazyImg data={{src:images[2],mode:item.CoverMode}} />
                    </div>
                  </div>
                  <div>
                  <div className="infoLine">
                    {this.getTopspan(item)}
                    <span className="info">
                      <span>{item.Description}</span>
                    </span>
                  </div>
                  </div>
                </div>
              </section>);
  }

  getOneBigPic(item, index) {
    return (<section key={index} className="section">
              <div className="floor0" onClick={this.jumpArticle.bind(this,item)}>
                <div className="content">
                  <h5>{item.Title}</h5>
                </div>
                <div className="showImg">
                  <div className="oneImg">
                    <LazyImg data={{src:this.getImageSrc(item),mode:item.CoverMode}} />
                  </div>
                </div>
                <div>
                <div className="infoLine">
                  {this.getTopspan(item)}
                  <span className="info">
                    <span>{item.Description}</span>
                  </span>
                </div>
                </div>
              </div>
            </section>);
  }

  getOneTopic(item, index) {
    return (<section key={index} className="section">
              <div className="floor2">
                <p className="fromTag" onClick={this.jumpTagList.bind(this,item)}>
                  来自话题：{item.FromTag}
                </p>
                <div onClick={this.jumpArticle.bind(this,item)}>
                  <div className="showImg">
                    <div className="oneBigImg">
                      <LazyImg data={{src:this.getImageSrc(item),mode:item.CoverMode}} />
                    </div>
                  </div>
                  <div className="content">
                    <h5>{item.Title}</h5>
                  </div>
                  <div className="infoLine">
                    {this.getTopspan(item)}
                    <span className="info">
                      <span>{item.Description}</span>
                    </span>
                  </div>
                </div>
              </div>
            </section>);
  }

  getOneBigLeft(item, index) {
    const images = this.getImageSrc(item);

    return (<section key={index} className="section">
              <div className="floor2">
                <p className="fromTag" onClick={this.jumpTagList.bind(this,item)}>
                  来自话题：{item.FromTag}
                </p>
                <div onClick={this.jumpArticle.bind(this,item)}>
                  <div className="OngBigLeftImg">
                    <div className="left">
                      <LazyImg data={{src:images[0],mode:item.CoverMode}} />
                    </div>
                    <div className="right">
                      <div>
                        <LazyImg data={{src:images[1],mode:item.CoverMode}} />
                      </div>
                      <div>
                        <LazyImg data={{src:images[2],mode:item.CoverMode}} />
                      </div>
                    </div>
                  </div>
                  <div className="content">
                    <h5>{item.Title}</h5>
                  </div>
                  <div className="infoLine">
                    {this.getTopspan(item)}
                    <span className="info">
                      <span>{item.Description}</span>
                    </span>
                  </div>
                </div>
              </div>
            </section>);
  }

  render() {
    const result = [];
    const items = this.props.data;

    items.forEach((item, index) => {
      if (item) {   // in case return item of undefined
        if (item.CoverMode === 0) {
          result.push(this.getCommon(item, index));
        } else if (item.CoverMode === 1) {
          result.push(this.getOnePic(item, index));
        } else if (item.CoverMode === 2) {
          result.push(this.getOneBigPic(item, index));
        } else if (item.CoverMode === 3) {
          result.push(this.getThreePic(item, index));
        } else if (item.CoverMode === 4) {
          result.push(this.getOneTopic(item, index));
        } else if (item.CoverMode === 5) {
          result.push(this.getOneBigLeft(item, index));
        }
      }
    })

    return (
      <div>
        {result}
      </div>
    )
  }
}

List.propTypes = {}

List.defaultProps = {}
