import './css/common.css'
import './css/list.css'
import './css/detail.css'

import React from 'react'
import { render } from 'react-dom'
import { browserHistory } from 'react-router'
import { syncHistoryWithStore } from 'react-router-redux'

prePolyfill.then(()=>{
	const Root = require('./containers/Root').default;
	const configureStore = require('./store/configureStore').default;
	const store = configureStore()
	const history = syncHistoryWithStore(browserHistory, store)

	render(
	  <Root store={store} history={history}/>,
	  document.getElementById('root')
	)
})
