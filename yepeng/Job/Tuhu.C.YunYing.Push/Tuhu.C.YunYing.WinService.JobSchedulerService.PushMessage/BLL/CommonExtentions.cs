using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace Tuhu.C.YunYing.WinService.PushJobSchedulerService.PushMessage.BLL
{
    public static class CommonExtentions
    {
        public static T DeepCopy<T>(this T value) where T : class
        {
            ParameterExpression parameterExpression = Expression.Parameter(typeof(T), "p");
            List<MemberBinding> memberBindingList = new List<MemberBinding>();

            foreach (var item in typeof(T).GetProperties())
            {
                if (!item.CanWrite)
                    continue;
                if (typeof(T).GetProperty(item.Name) == null)
                    continue;
                MemberExpression property = Expression.Property(parameterExpression, typeof(T).GetProperty(item.Name));
                MemberBinding memberBinding = Expression.Bind(item, property);
                memberBindingList.Add(memberBinding);
            }

            MemberInitExpression memberInitExpression = Expression.MemberInit(Expression.New(typeof(T)), memberBindingList.ToArray());
            Expression<Func<T, T>> lambda = Expression.Lambda<Func<T, T>>(memberInitExpression, new ParameterExpression[] { parameterExpression });

            var func = lambda.Compile();
            return func(value);
        }
    }
}
