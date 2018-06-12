import React, { Component, PropTypes } from 'react'
import { connect } from 'react-redux'
import { browserHistory } from 'react-router'

import MyUtils from '../commons/MyUtils'
import { loadList } from '../actions'
import List from '../components/List'
import LoadingBar from '../commons/LoadingBar'
import SearchBar from '../components/SearchBar'

class FirstLine extends Component {
  constructor(props, context) {
    super(props, context);

    this.pageIndex = 2;
    this.tag = MyUtils.getQueryString('ArticleTagId') || '';
    this.articleAuthor = MyUtils.getQueryString('ArticleAuthor') || '';
    this.articleTitle = MyUtils.getQueryString('ArticleTitle') || '';

    this.preData = [];

    this.state = {
      loadingBarDisplay: {
        display: 'none'
      },
      noList: null
    }
    this.plat = MyUtils.whatDevice() + '_' + MyUtils.whatChannel() + '_' + MyUtils.whatPlatform();
  } 

  handleScroll() {
    if (!this.props.isSuccess) {
      this.props.loadList(this.pageIndex, this.tag, this.articleAuthor, this.articleTitle);
      return;
    }

    if (document.body.clientHeight > window.innerHeight && 
        this.state.loadingBarDisplay.display == 'none') {
        this.setState({
             loadingBarDisplay: {
               display: 'block'
              },
              noList: null
        });
    }

    if (MyUtils.isHalf() || MyUtils.isBottom()) {
      if(this.pageIndex === this.props.totalIndex) {
        this.setState({
          loadingBarDisplay: {
            display: 'none'
          },
           noList: <div className="noList">没有更多内容了</div>
        })

        return;
      }
      this.pageIndex++;
      this.props.loadList(this.pageIndex, this.tag, this.articleAuthor, this.articleTitle);
    }
  }


  initLoad() {
    document.title = decodeURI(MyUtils.getQueryString('ArticleAuthor')) 
                      ||decodeURI(MyUtils.getQueryString('title'))
                       '';
    
    if (window.ga) {
      ga('set', 'dimension1', this.plat);
      ga("send","pageview")
    }
    if(window.Ta){
      Ta.Run("pageview", "pv");
    }
  }

  componentWillMount() {

    this.initLoad();
    
  }

  componentDidMount() {
     this.setState({
        noList: <div className="noList">正在加载中...</div>
      })
    preRequest
    .then((res)=>{
      this.preData = res.data;
    })
    .then(()=>{
      this.preData.forEach((item, index)=>{
        this.props.data.push(item)
      })
    })
    .then(()=>{
      this.forceUpdate();
       this.setState({
        noList: <div className="noList">没有更多内容了</div>
      })
    })
    .then(()=>{
      this.stopRefresh();
    })

    window.document.addEventListener('scroll', this.handleScroll.bind(this));
  }

  stopRefresh() {
    if (!MyUtils.isApp()) {
      return
    }

    if (MyUtils.isAndroid()) {
      window.WebViewJavascriptBridge.callHandler.bind(this)('onRefreshingStop');
    }

    if(MyUtils.isIOS()){
      window.location.href = 'tuhuaction://setRefresh#1';
    }
  }

  handleSearch(val) {
    let path = ''

    if(this.tag){
      path = '/firstline?ArticleTagId=' + this.tag + '&ArticleTitle=' + val+"&title="+decodeURI(MyUtils.getQueryString("title"));
    } else {
      path = '/firstline?ArticleTitle=' + val+"&ArticleAuthor="+this.articleAuthor+"&title="+decodeURI(MyUtils.getQueryString("title"));
    }
    if(window.Ta){
      Ta.Run("home_click_搜索", "event", "ArticleTitle", decodeURI(decodeURI(val)));
    }
    browserHistory.push(path);

    this.setState({
         loadingBarDisplay: {
           display: 'none'
          }
    })

    window.location.reload();
  }

  render() {
    return (
      <div>
        <SearchBar handleSearch={this.handleSearch.bind(this)} />
        <List loadList={this.props.loadList} data={this.props.data} />
        <div style={this.state.loadingBarDisplay}>
          <LoadingBar />
        </div>
        {this.state.noList}
      </div>
    )
  }
}

FirstLine.propTypes = {
  
}

function mapStateToProps(state, ownProps) {
  return {
    data: state.entities.data,
    totalIndex: state.entities.totalIndex,
    isSuccess: state.entities.isSuccess
  }
}

export default connect(mapStateToProps, {
  loadList
})(FirstLine)
