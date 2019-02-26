using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using AutoMapper;
using Tuhu.Service.Activity.Models;
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

    public static class ModelConvertExtensions
    {
        private static readonly ConcurrentDictionary<string, IMapper> Mappers = new ConcurrentDictionary<string, IMapper>();

        private static IMapper GetMapper<TIn, TOut>(TIn model)
        {
            var key = typeof(TIn).FullName + typeof(TOut).FullName;
            if (Mappers.ContainsKey(key))
            {
                return Mappers[key];
            }
            else
            {
                var config = new MapperConfiguration(cfg => cfg.CreateMap<TIn, TOut>());

                if (key == typeof(ActivePageListModel).FullName + typeof(Zip.Models.ActivePageListModel).FullName)
                {
                    config = new MapperConfiguration(cfg =>
                    {
                        cfg.CreateMap<ActivePageListModel, Zip.Models.ActivePageListModel>()
                            .MaxDepth(6)
                            .ForMember(p => p.FlashSaleRows, p => p.Ignore())
                            .ForMember(p => p.ActivePageGroupContents, p => p.Ignore());
                        cfg.CreateMap<string, string>().ConvertUsing<StringTypeConverter>();
                    });
                }
                if (key == typeof(ActivePageContentModel).FullName + typeof(Zip.Models.ActivePageContentModel).FullName)
                {
                    config = new MapperConfiguration(cfg =>
                    {
                        cfg.CreateMap<ActivePageContentModel, Zip.Models.ActivePageContentModel>()
                            .MaxDepth(6).ForMember(p => p.Contents, p => p.Ignore());
                        cfg.CreateMap<string, string>().ConvertUsing<StringTypeConverter>();
                    });
                }

                Mappers[key] = config.CreateMapper();
                return Mappers[key];
            }
        }

        public static TOut ModelConvert<TIn, TOut>(TIn model)
            => GetMapper<TIn, TOut>(model).Map<TOut>(model);

        public class StringTypeConverter : ITypeConverter<string, string>
        {
            public string Convert(string source, string destination, ResolutionContext context)
            {
                if (string.IsNullOrEmpty(source))
                    return null;
                return source.ToString();
            }
        }

        public static Zip.Models.ActivePageGroupContentModel ConvertToZipActivePageGroupContentModel(
            this ActivePageGroupContentModel source)
        {
            return new Zip.Models.ActivePageGroupContentModel()
            {
                Contents = source.Contents
                    .Select(p => {
                        var zipContents = ModelConvert<ActivePageContentModel, Zip.Models.ActivePageContentModel>(p);
                        zipContents.Contents = p.Contents?.Select(ModelConvert<ActivePageContentModel, Zip.Models.ActivePageContentModel>)?.ToList() ?? new List<Zip.Models.ActivePageContentModel>();
                        return zipContents;
                    }).ToList(),
                Order = source.Order,
                OrigionType = source.OrigionType,
                RowType = source.RowType,
                Type = source.Type
            };
        }

        public static Zip.Models.FlashSaleActivityMenu ConvertToZipFlashSaleActivityMenu(
            this FlashSaleActivityMenu source)
        {
            return new Zip.Models.FlashSaleActivityMenu()
            {
                EndDateTime = source.EndDateTime,
                Group = source.Group,
                HourTime = source.HourTime,
                Rows = source.Rows.Select(ConvertToZipActivePageGroupContentModel)?.ToList(),
                StartDateTime = source.StartDateTime,
                Status = source.Status
            };
        }
        
    }

}


