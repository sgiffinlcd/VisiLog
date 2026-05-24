using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace VisiLog.Model.Service.WebAPI
{
    /// <summary>
    /// System.Text.Json source-generation context for WebAPI response DTOs.
    /// Register with the serializer (e.g. <c>options.TypeInfoResolverChain.Insert(0, VisiLogJsonSerializerContext.Default)</c>)
    /// to opt into source-generated, reflection-free serialization.
    /// </summary>
    [JsonSerializable(typeof(LogMessageResponse))]
    [JsonSerializable(typeof(List<LogMessageResponse>))]
    [JsonSerializable(typeof(List<string>))]
    public partial class VisiLogJsonSerializerContext : JsonSerializerContext
    {
    }
}
