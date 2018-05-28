using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;

namespace DomainDrivenDesign.Core.Utils
{
    public static class LinqExtensions
    {
        private class DelegateComparer<T> : IEqualityComparer<T>
        {
            private readonly Func<T, T, bool> _equals;
            private readonly Func<T, int> _getHashCode;

            public DelegateComparer(Func<T, T, bool> equals)
            {
                _equals = equals;
            }

            public DelegateComparer(Func<T, T, bool> equals, Func<T, int> getHashCode)
            {
                _equals = equals;
                _getHashCode = getHashCode;
            }

            public bool Equals(T a, T b)
            {
                return _equals(a, b);
            }

            public int GetHashCode(T a)
            {
                if (_getHashCode != null)
                    return _getHashCode(a);

                return a.GetHashCode();
            }
        }

        public static void ForEach<T>(this IEnumerable<T> items, Action<T> action)
        {
            foreach (T item in items)
                action(item);
        }

        public static bool ContainsAny<T>(this IEnumerable<T> items, IEnumerable<T> values)
        {
            return items.Any(item => values.Contains(item));
        }

        public static bool None<T>(this IEnumerable<T> items)
        {
            return !items.Any();
        }

        public static Func<T, bool> And<T>(this Func<T, bool> f1, Func<T, bool> f2)
        {
            return a => f1(a) && f2(a);
        }

        public static Func<T, bool> Or<T>(this Func<T, bool> f1, Func<T, bool> f2)
        {
            return a => f1(a) || f2(a);
        }

        public static IEnumerable<T> Distinct<T>(this IEnumerable<T> items, Func<T, T, bool> comparer)
        {
            return items.Distinct(new DelegateComparer<T>(comparer));
        }

        public static IEnumerable<T> Distinct<T>(this IEnumerable<T> items, Func<T, T, bool> comparer, Func<T, int> hashCoder)
        {
            return items.Distinct(new DelegateComparer<T>(comparer, hashCoder));
        }

        public static Expression<Func<TSource, object>> GetExpression<TSource>(string propertyName)
        {
            var param = Expression.Parameter(typeof(TSource), "x");
            Expression conversion = Expression.Convert(Expression.Property
            (param, propertyName), typeof(object));   //important to use the Expression.Convert
            return Expression.Lambda<Func<TSource, object>>(conversion, param);
        }

        //makes deleget for specific prop
        public static Func<TSource, object> GetFunc<TSource>(string propertyName)
        {
            return GetExpression<TSource>(propertyName).Compile();  //only need compiled expression
        }

        //OrderBy overload
        public static IOrderedEnumerable<TSource> OrderByFieldName<TSource>(this IEnumerable<TSource> source, string propertyName)
        {
            return source.OrderBy(GetFunc<TSource>(propertyName));
        }


        public static IOrderedEnumerable<TSource> OrderByDescendingFieldName<TSource>(this IEnumerable<TSource> source, string propertyName)
        {
            return source.OrderByDescending(GetFunc<TSource>(propertyName));
        }

       
    }
}
