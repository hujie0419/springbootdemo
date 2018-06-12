import MyUtils from '../commons/MyUtils'

export default class SearchBar extends React.Component {
	constructor(props, context){
		super(props, context);
		this.keyword=decodeURI(MyUtils.getQueryString('ArticleTitle'));		
		/*this.searches = 'searches';
		this.searchesLength = 5;
		this.histories = [];
		this.historyItems = [];
		this.state={
			historyDoms: null,
			historyStyle: {display: 'none'}
		};
		this.searchQuery = MyUtils.getQueryString('ArticleTitle') || '';*/
	}

	handleSearch() {
		this.props.handleSearch(this.refs.searchQuery.value)
		// MyUtils.setSearches(this.searches, this.refs.searchQuery.value)
	}
	/*searchHistory(item) {
		this.refs.searchQuery.value = item;
		this.handleSearch();
	}

	showHistories() {
		this.histories = MyUtils.getSearches(this.searches, this.searchesLength);
		this.historyItems = [];

		if (this.histories) {
			this.histories.forEach((item, index)=>{
				this.historyItems.push(
					<div className="historyItem" onClick={this.searchHistory.bind(this, item)}>{item}</div>
				)
			})
		}

	if (this.state.historyStyle.display == 'none') {
		this.setState({
			historyDoms: this.historyItems,
			historyStyle:{display: 'block'}
		})
	} else {
		this.setState({
			historyDoms: this.historyItems,
			historyStyle:{display: 'none'}
		})
	}
		

	}

	componentDidMount() {
		if (this.searchQuery) {
			this.refs.searchQuery.value = decodeURI(this.searchQuery);
		}

 //doms
 
	<div className="searchHistory" onClick={this.showHistories.bind(this)}>+</div>

	<div className="histories" style={this.state.historyStyle}>
  	{this.state.historyDoms}
  </div>
		
	}*/

	render(){
		return(
			<div className="searchBar">
        <input type="text" className="input" ref="searchQuery" defaultValue={this.keyword} placeholder="搜索你感兴趣的" />
        <div className="searchBtn" onClick={this.handleSearch.bind(this)}> 搜索 </div>
      </div>
			)
	}
}