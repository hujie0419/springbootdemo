using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tuhu.Provisioning.DataAccess.DAO.Feedback;
using Tuhu.Provisioning.DataAccess.Entity.Feedback;

namespace Tuhu.Provisioning.Business.Feedback
{
    public class QuestionTypeBLL
    {
        private readonly QuestionTypeDAL _questionTypeDal;
        public QuestionTypeBLL()
        {
            _questionTypeDal = new QuestionTypeDAL();
        }
        /// <summary>
        /// 添加问题类型
        /// </summary>
        /// <param name="typeName">问题类型</param>
        /// <param name="describtion">默认描述</param>
        /// <param name="currentName">当前用户</param>
        /// <returns></returns>
        public async Task<int> AddQuestionType(string typeName, string describtion, string currentName)
        {
            if (string.IsNullOrEmpty(typeName))
                return 0;
            return await _questionTypeDal.AddQuestionType(typeName, describtion, currentName);
        }
        /// <summary>
        /// 更新问题类型
        /// </summary>
        /// <param name="typeName">问题类型</param>
        /// <param name="describtion">默认描述</param>
        /// <param name="currentName">当前用户</param>
        /// <param name="id">问题类型ID</param>
        /// <returns></returns>
        public async Task<int> UpdateQuestionType(string typeName, string describtion, string currentName, int id)
        {
            return await _questionTypeDal.UpdateQuestionType(typeName, describtion, currentName, id);
        }
        /// <summary>
        /// 通过Id删除问题类型
        /// </summary>
        /// <param name="id">问题类型Id</param>
        /// <returns></returns>
        public async Task<int> DeleteQuestionType(int id)
        {
            return await _questionTypeDal.DeleteQuestionType(id);
        }

        /// <summary>
        /// 查询问题类型
        /// </summary>
        /// <returns></returns>
        public IEnumerable<QuestionTypeEntity> GetQuestionTypeList()
        {
            return _questionTypeDal.GetQuestionTypeList();
        }
    }
}
