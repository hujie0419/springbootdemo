import { CALL_API } from '../middleware/api'

export const LIST_REQUEST = 'LIST_REQUEST'
export const LIST_SUCCESS = 'LIST_SUCCESS'
export const LIST_FAILURE = 'LIST_FAILURE'

function fetchList(page, tag, author, query) {
  return {
    [CALL_API]: {
      types: [ LIST_REQUEST, LIST_SUCCESS, LIST_FAILURE ],
      page: page,
      tag: tag,
      author: author,
      query: query
    }
  }
}

export function loadList(page, tag, author, query) {
  return (dispatch, getState) => {
    return dispatch(fetchList(page, tag, author, query))
  }
}
