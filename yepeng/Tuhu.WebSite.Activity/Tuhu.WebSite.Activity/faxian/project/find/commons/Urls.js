import * as Config from '../constants/Config'

class Urls {
	constructor() {
       this.apiRoot = Config.ROOT_URL;
       this.size = 20;
       this.page = '';
       this.tag = '';
       this.author = '';
       this.query = '';
	}

    paramData() {
        return {
          PageSize: this.size,
          PageIndex: this.page,
          ArticleTagId: this.tag,
          ArticleAuthor: this.author,
          ArticleTitle: this.query
        }
    }

	callApi(page, tag, author, query) {
        this.page = page;
        this.tag = tag;
        this.author = author;
        this.query = query;

      const subUrl = this.urls(Config.LIST_URl)
      const fullUrl = (subUrl.indexOf(this.apiRoot) === -1) ? this.apiRoot + subUrl : subUrl
      
      return fetch(fullUrl)
        .then(response =>
          response.json()
        ).then((response) => {
          return response;
        }).catch((e) => {
          // console.log(e.message);
        });
    }

    urls(subUrl) {
      const result = [];
      const data = this.paramData();

      for (let name in data) {
        result.push(name + '=' + data[name])
      }

      if (result.length > 0) {
        return subUrl + '?' + result.join("&");
      }

      return subUrl;
    }

}

export default new Urls()