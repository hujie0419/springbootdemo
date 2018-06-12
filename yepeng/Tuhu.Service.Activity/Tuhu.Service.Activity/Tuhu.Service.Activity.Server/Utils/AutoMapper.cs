using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using AutoMapper;
using Tuhu.Service.Activity.Models.Requests;
using OrderRequest = Tuhu.Service.Order.Request.CreateOrderRequest;

namespace Tuhu.Service.Activity.Server.Utils
{
    public static class TransExp<TIn, TOut>
    {
        private static readonly Func<TIn, TOut> Cache = GetFunc();
        private static Func<TIn, TOut> GetFunc()
        {
            ParameterExpression parameterExpression = Expression.Parameter(typeof(TIn), "p");
            List<MemberBinding> memberBindingList = new List<MemberBinding>();

            foreach (var item in typeof(TOut).GetProperties())
            {
                if (!item.CanWrite)
                    continue;

                MemberExpression property = Expression.Property(parameterExpression, typeof(TIn).GetProperty(item.Name));
                MemberBinding memberBinding = Expression.Bind(item, property);
                memberBindingList.Add(memberBinding);
            }

            MemberInitExpression memberInitExpression = Expression.MemberInit(Expression.New(typeof(TOut)), memberBindingList.ToArray());
            Expression<Func<TIn, TOut>> lambda = Expression.Lambda<Func<TIn, TOut>>(memberInitExpression, new ParameterExpression[] { parameterExpression });

            return lambda.Compile();
        }

        public static TOut Trans(TIn tIn)
        {
            return Cache(tIn);
        }

    }

    public class ObjectReflection
    {
        public static PropertyInfo[] GetPropertyInfos(Type type)
        {
            return type.GetProperties(BindingFlags.Public | BindingFlags.Instance);
        }

        public static void AutoMapping<S, T>(S s, T t)
        {

            PropertyInfo[] pps = GetPropertyInfos(s.GetType());

            Type target = t.GetType();

            foreach (var pp in pps)
            {
                PropertyInfo targetObj = target.GetProperty(pp.Name);
                object value = pp.GetValue(s, null);
                if (targetObj == null)
                    continue;
                if (targetObj.Name == "Items")
                
                    continue;

                    //var orderItems = value as List<OrderItem>;
                    //if (orderItems != null)
                    //    foreach (var v in orderItems)
                    //    {

                    //    }                
                //if (targetObj.Name == "PackageItems")
                //{

                //}
                var type = targetObj.PropertyType;
                if (type.IsClass && type != typeof(string) && type.Name != "Nullable`1")
                {
                    if (value != null)
                    {
                        var subT = Activator.CreateInstance(type);
                        AutoMapping(pp.GetValue(s, null), subT);
                        targetObj.SetValue(t, subT, null);
                        continue;
                    }

                }

                if (value != null)
                {
                    targetObj.SetValue(t, value, null);
                }
            }
        }

    }

    public static class EntityHelper<TIn, TOut>
    {
        private static readonly Func<TIn, TOut> Cache = TransExp<TIn, TOut>;
        public static TOut TransExp<TIn, TOut>(TIn source)
        {
            var config = new MapperConfiguration(c =>
            {
                c.CreateMap<CreateOrderRequest, OrderRequest>();
                c.CreateMap<OrderItem, Order.Request.OrderItem>();
                c.CreateMap<OrderProductTypes, Order.Request.OrderProductTypes>();
                c.CreateMap<OrderListExtModel, Order.Models.OrderListExtModel>();
                c.CreateMap<OrderDelivery, Order.Request.OrderDelivery>();
                c.CreateMap<OrderPayment, Order.Request.OrderPayment>();
                c.CreateMap<OrderMoney, Order.Request.OrderMoney>();
                c.CreateMap<OrderCarModel, Order.Models.OrderCarModel>();
                c.CreateMap<OrderCustomer, Order.Request.OrderCustomer>();
                c.CreateMap<OrderAddressModel, Order.Models.OrderAddressModel>();

            });

            var marper = config.CreateMapper();
            //var source = new TIn();
            return marper.Map<TOut>(source);
        }
        //public static TOut GetCache()
        //{
        //    return Cache;
        //}
        public static TOut Trans(TIn tIn)
        {
            return Cache(tIn);
        }
    }

}


