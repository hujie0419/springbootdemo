import * as ActionTypes from '../actions'
import { routerReducer as routing } from 'react-router-redux'
import { combineReducers } from 'redux'

function entities(state = {data: [], totalIndex: 1, isSuccess: false}, action) {
	if (action.data) {
		// state = Object.assign({}, state, {data: action.data})
		/*if (action.data.hasArticleTitle) {
			console.log(333, action)
			state = {
				data: action.data.data,
				totalIndex: action.data.TotalPage,
				isSuccess: action.data.success 
			}
		}*/
		// delete repeat and undefined
		if (state.data.length > 0) {
			state.data.forEach((sta, n) => {
				if ( action.data.data ) {
					action.data.data.forEach((act, m) => {
						if ( !act || sta.Id === act.Id ) {
							action.data.data.splice(m, 1);
						};
					})
				};	
			})
		}

		
		state = { 	
					data: (state.data).concat(action.data.data),
					totalIndex: action.data.TotalPage,
					isSuccess: action.data.success 
				};
	}

	return state
}

const rootReducer = combineReducers({
  entities,
  routing
})

export default rootReducer