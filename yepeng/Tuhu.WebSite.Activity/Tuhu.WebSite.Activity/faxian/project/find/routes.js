import React from 'react'
import { Route } from 'react-router'
import App from './containers/App'
import FirstLine from './containers/FirstLine'
import Ask from './containers/Ask'

export default (
	<div>
	  <Route path="/" component={App} />
	  <Route path="/firstline" component={FirstLine} />
	  <Route path="/ask" component={Ask} />
  </div>
)