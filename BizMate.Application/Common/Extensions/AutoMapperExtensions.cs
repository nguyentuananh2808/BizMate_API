using AutoMapper;
using System.Reflection;

namespace BizMate.Public.Extensions
{
    public static class AutoMapperExtensions
    {
        public static IMappingExpression<TSource, TDestination> IgnoreAllMembers<TSource, TDestination>(this IMappingExpression<TSource, TDestination> expr)
        {
            PropertyInfo[] properties = typeof(TDestination).GetProperties();
            foreach (PropertyInfo propertyInfo in properties)
            {
                expr.ForMember(propertyInfo.Name, delegate (IMemberConfigurationExpression<TSource, TDestination, object> opt)
                {
                    opt.Ignore();
                });
            }

            return expr;
        }
    }
}
