using System.Collections.Generic;
using System.Reflection;

using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

namespace CCL
{
    public class JsonHelper
    {

        /// <summary>
        /// Helper class for Newtonsoft.Json Library to ignore properties from serialization.
        /// <br/>
        /// <b><br>Usage:</br></b>
        /// <see cref="JsonConvert"/>.SerializeObject(YourObject, new <see cref="JsonSerializerSettings"/>()
        ///     { ContractResolver = new <see cref="IgnorePropertiesResolver"/>(new[] { "Prop1", "Prop2" }) });
        /// </summary>
        public class IgnorePropertiesResolver : DefaultContractResolver {

            #region Variables
            private readonly HashSet<string> ignoreProps;
            #endregion

            #region Constructor
            public IgnorePropertiesResolver(IEnumerable<string> propNamesToIgnore)
            {
                ignoreProps = new HashSet<string>(propNamesToIgnore);
            }
            #endregion

            protected override JsonProperty CreateProperty(MemberInfo member, MemberSerialization memberSerialization)
            {
                JsonProperty property = base.CreateProperty(member, memberSerialization);
                if (ignoreProps.Contains(property.PropertyName)) {
                    property.ShouldSerialize = _ => false;
                }
                return property;
            }

        }

    }
}
