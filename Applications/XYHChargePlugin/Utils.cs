using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;

namespace XYHChargePlugin
{
    public static class Utils
    {

        public static List<TEntity> AddedItems<TEntity>(this IList<TEntity> source, IList<TEntity> target, Func<TEntity,TEntity,bool> check)
        {
            return source.Where(x => !target.Any(y => check(x, y))).ToList();
        }

        public static List<TEntity> RemovedItems<TEntity>(this IList<TEntity> source, IList<TEntity> target, Func<TEntity, TEntity, bool> check)
        {
            return target.Where(x => !source.Any(y => check(x, y))).ToList();
        }

        public static List<TEntity> UpdatedItems<TEntity>(this IList<TEntity> source, IList<TEntity> target, Func<TEntity, TEntity, bool> check)
        {
            return source.Where(x => target.Any(y => check(x, y))).ToList();
        }
    }
}
