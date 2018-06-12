import React, { Component, PropTypes } from 'react'
import { connect } from 'react-redux'
import { browserHistory } from 'react-router'

import geren from '../img/list/drawable-hdpi/geren.png'
import tiwen from '../img/list/drawable-hdpi/tiwen.png'
import sa from '../img/list/drawable-hdpi/sa.png'

class App extends Component {
  constructor(props, context) {
    super(props, context);

    this.state = {
      index: 0
    }
  }

  handleClick(index) {
    if (index === 0) {
      browserHistory.push('/firstline');
    } else if (index === 1) {
      browserHistory.push('/ask');
    }

    this.setState({
      index: index
    });
  }

  render() {
    const { children } = this.props
    
    return (
      <div>
        {this.props.children}
      </div>
    )
  }
}

App.propTypes = {
  children: PropTypes.node
}

function mapStateToProps(state, ownProps) {
  return {}
}

export default connect(mapStateToProps, {})(App)
