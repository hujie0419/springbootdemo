import React, { Component, PropTypes } from 'react'
import { connect } from 'react-redux'
import { loadList } from '../actions'

class Ask extends Component {
  componentDidMount() {
    
  }

  render() {
    return (
      <div>
        hello, welcome ask
      </div>
    )
  }
}

Ask.propTypes = {
  
}

function mapStateToProps(state, ownProps) {
  return {data: state.entities.data};
}

export default connect(mapStateToProps, {
  
})(Ask)
