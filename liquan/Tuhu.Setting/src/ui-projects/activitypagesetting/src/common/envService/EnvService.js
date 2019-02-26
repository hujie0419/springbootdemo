import env from 'env';

// export class EnvService {
//   e
//   constructor(){

//   }
// }
/**
 * 获取环境对象
 *
 * @export
 * @returns {object}
 */
export function envFactory() {
    return env.default;
}
