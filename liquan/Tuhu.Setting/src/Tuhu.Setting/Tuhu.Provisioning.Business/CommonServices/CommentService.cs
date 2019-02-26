using Common.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tuhu.Service.Comment;
using Tuhu.Service.Comment.Request;
using Tuhu.Service.Comment.Response;

namespace Tuhu.Provisioning.Business.CommonServices
{
    public static class CommentService
    {
        private static readonly Common.Logging.ILog logger = LogManager.GetLogger(typeof(CommentService));

        public static CommentOrderResponse CreateProductComment(CreateProductCommentRequest request)
        {
            CommentOrderResponse result = null;
            try
            {
                using (var client = new ProductCommentClient())
                {
                    var getResult = client.CreateProductComment(request);
                    getResult.ThrowIfException(true);
                    result = getResult.Result;
                }
            }
            catch (Exception ex)
            {
                logger.Error(ex);
            }
            return result;
        }

    }
}
