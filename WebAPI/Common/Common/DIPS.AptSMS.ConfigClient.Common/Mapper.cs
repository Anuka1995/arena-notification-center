using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace DIPS.AptSMS.ConfigClient.Common.Mapper
{
    public class Mapper<TSource, TDest> where TDest : new()
    {
        protected readonly IList<Action<TSource, TDest>> Mappings = new List<Action<TSource, TDest>>();

        protected virtual void CopyMatchingProperties(TSource source, TDest dest)
        {
            foreach (var destProp in typeof(TDest).GetProperties(BindingFlags.Public | BindingFlags.Instance).Where(p => p.CanWrite))
            {
                var sourceProp = typeof(TSource).GetProperties(BindingFlags.Public | BindingFlags.Instance)
                    .FirstOrDefault(p => p.Name == destProp.Name && p.PropertyType == destProp.PropertyType);
                if (sourceProp != null)
                {
                    destProp.SetValue(dest, sourceProp.GetValue(source, null), null);
                }
            }
        }

        public virtual void AddMapping(Action<TSource, TDest> mapping)
        {
            Mappings.Add(mapping);
        }

        public virtual TDest MapObject(TSource source, TDest dest)
        {
            CopyMatchingProperties(source, dest);
            foreach (var action in Mappings)
            {
                action(source, dest);
            }

            return dest;
        }

        public virtual TDest CreateMappedObject(TSource source)
        {
            TDest dest = new TDest();
            return MapObject(source, dest);
        }
    }
}
