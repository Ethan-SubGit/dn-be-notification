using CHIS.Framework;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using Newtonsoft.Json;
using System;

namespace CHIS.NotificationCenter.Infrastructure.Utils
{
    public class ObjectToJsonConverter<TModel> : ValueConverter<TModel, string>
    {
        /// <summary>
        /// ObjectToJsonConverter
        /// </summary>
        /// <param name="mappingHints"></param>
        public ObjectToJsonConverter([CanBeNull] ConverterMappingHints mappingHints = null)
            : base(v => JsonConvert.SerializeObject(v), v => v == null ? (TModel)Activator.CreateInstance(typeof(TModel), null) : JsonConvert.DeserializeObject<TModel>(v), null)
        {
        }
        /// <summary>
        /// DefaultInfo
        /// </summary>
        public static ValueConverterInfo DefaultInfo { get; } = new ValueConverterInfo(typeof(TModel), typeof(string), i => new ObjectToJsonConverter<TModel>(i.MappingHints), null);
    }
}
