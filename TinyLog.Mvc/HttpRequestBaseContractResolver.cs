using Newtonsoft.Json.Serialization;
using System;
using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json;
using System.Web;
using System.Reflection;
using System.Collections.Specialized;

namespace TinyLog.Mvc
{
    /// <summary>
    /// Custom converter for (de)serializing NameValueCollection
    /// Add an instance to the settings Converters collection
    /// </summary>
    public class NameValueCollectionConverter : JsonConverter
    {
        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            var collection = value as NameValueCollection;
            if (collection == null)
                return;

            writer.WriteStartObject();
            foreach (var key in collection.AllKeys)
            {
                writer.WritePropertyName(key);
                try
                {
                    writer.WriteValue(collection.Get(key));
                }
                catch (Exception exWrite)
                {
                    writer.WriteValue(string.Format("Error writing the value: {0}", exWrite.Message));
                }
            }
            writer.WriteEndObject();
        }

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            var nameValueCollection = new NameValueCollection();
            var key = "";
            while (reader.Read())
            {
                if (reader.TokenType == JsonToken.StartObject)
                {
                    nameValueCollection = new NameValueCollection();
                }
                if (reader.TokenType == JsonToken.EndObject)
                {
                    return nameValueCollection;
                }
                if (reader.TokenType == JsonToken.PropertyName)
                {
                    key = reader.Value.ToString();
                }
                if (reader.TokenType == JsonToken.String)
                {
                    nameValueCollection.Add(key, reader.Value.ToString());
                }
            }
            return nameValueCollection;
        }

        public override bool CanConvert(Type objectType)
        {
            if (objectType == typeof(NameValueCollection) || objectType.BaseType == typeof(NameValueCollection))
            {
                return true;
            }
            return false;
        }
    }

    /// <summary>
    /// A Json contract resolver which ensures, that streams and assemblies aren't serialized
    /// </summary>
    public class HttpRequestBaseContractResolver : DefaultContractResolver
    {
        public HttpRequestBaseContractResolver()
            : base()
        {
            base.IgnoreSerializableAttribute = true;
            base.IgnoreSerializableInterface = true;
        }

        private bool MatchType(Type type, Type matchType, bool matchBaseTypes = true)
        {
            bool b = type == matchType;

            if (b || !matchBaseTypes)
            {
                return b;
            }
            Type baseType = type.BaseType;
            while (baseType != null)
            {
                if (baseType == matchType)
                {
                    return true;
                }
                baseType = baseType.BaseType;
            }
            return false;
        }

        protected override JsonProperty CreateProperty(MemberInfo member, MemberSerialization memberSerialization)
        {
            JsonProperty property = base.CreateProperty(member, memberSerialization);
            string[] ignore = new string[] { "HttpChannelBinding", "InputStream", "Files","Filter", "RequestContext" };
            if (MatchType(member.ReflectedType,typeof(System.IO.Stream)) || MatchType(member.ReflectedType,typeof(Assembly)) || (property.DeclaringType == typeof(HttpRequestWrapper) && ignore.Contains(property.PropertyName ,StringComparer.OrdinalIgnoreCase)))
            {
                property.ShouldSerialize = x => { return false; };
            }

            return property;
        }

        protected override IList<JsonProperty> CreateProperties(Type type, MemberSerialization memberSerialization)
        {
            return base.CreateProperties(type, memberSerialization);
        }
    }
}
