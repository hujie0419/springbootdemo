import * as Config from '../constants/Config'
import Urls from '../commons/Urls'

export const CALL_API = Symbol('Call API')
export default store => next => action => {
  const callAPI = action[CALL_API]
  if (typeof callAPI === 'undefined') {
    return next(action)
  }

  const { types, page , tag , author , query } = callAPI
  const [ requestType, successType, failureType ] = types
  next({type: requestType})

  return Urls.callApi(page, tag, author, query).then(
    response => next({type: successType, data: response}),
    error => next({type: failureType})
  )
}
