using ClownFish.Base.Internals;
using ClownFish.Http.Clients.Elastic;
using Newtonsoft.Json.Serialization;

namespace ClownFish.Base.Json;

internal static class JsonSerializerSettingsUtils
{
    private static readonly TSafeDictionary<JsonStyle, JsonSerializerSettings> s_dict = new TSafeDictionary<JsonStyle, JsonSerializerSettings>();

    private static readonly JsonSerializerSettings s_jsonSettingsNone = Get0(JsonStyle.None);
    private static readonly JsonSerializerSettings s_jsonSettingsIndented = Get0(JsonStyle.Indented);
    private static readonly JsonSerializerSettings s_jsonSettingsCamelCase = Get0(JsonStyle.CamelCase);

    internal static bool EnableCache = true;  // 单元测试可修改

    internal static JsonSerializerSettings Get(JsonStyle style = JsonStyle.None)
    {
        if( EnableCache ) {
            return style switch {
                JsonStyle.None => s_jsonSettingsNone,
                JsonStyle.Indented => s_jsonSettingsIndented,
                JsonStyle.CamelCase => s_jsonSettingsCamelCase,

                _ => s_dict.GetOrAdd(style, Get0)
            };
        }
        else {
            return Get0(style);
        }
    }
#if NET10_0_OR_GREATER
    [UnconditionalSuppressMessage("TrimAnalyzer", "IL2026: JsonSerializer")]
    [UnconditionalSuppressMessage("TrimAnalyzer", "IL3050: JsonSerializer")]
#endif
    private static JsonSerializerSettings Get0(JsonStyle style)
    {
        JsonSerializerSettings settings = new JsonSerializerSettings();

        if( style.HasFlag(JsonStyle.KeepNull) )
            settings.NullValueHandling = NullValueHandling.Include;
        else
            settings.NullValueHandling = NullValueHandling.Ignore;

        if( style.HasFlag(JsonStyle.IgnoreDefaultValue ) )
            settings.DefaultValueHandling = DefaultValueHandling.Ignore;


        if( style.HasFlag(JsonStyle.KeepType) )
            settings.TypeNameHandling = TypeNameHandling.Auto;

        if( style.HasFlag(JsonStyle.Indented) )
            settings.Formatting = Formatting.Indented;


        if( style.HasFlag(JsonStyle.CamelCase) ) {
            settings.ContractResolver = new CamelCasePropertyNamesContractResolver();
        }
        else if( style.HasFlag(JsonStyle.NameToLower) ) {
            settings.ContractResolver = new LowerCaseContractResolver();
        }
        else if( ClownFishOptions.JsonSerializer_CamelCase ) {
            settings.ContractResolver = new CamelCasePropertyNamesContractResolver();
        }
        //else {
        //    默认值：new Newtonsoft.Json.Serialization.DefaultContractResolver() ，采用C#的大驼峰风格
        //}


        if( style.HasFlag(JsonStyle.UtcTime) ) {
            settings.DateTimeZoneHandling = DateTimeZoneHandling.Utc;
            settings.DateFormatHandling = DateFormatHandling.IsoDateFormat;
            settings.DateFormatString = null;
        }
        else {
            settings.DateTimeZoneHandling = DateTimeZoneHandling.Local;

            if( style.HasFlag(JsonStyle.TimeFormat19) ) {
                settings.DateFormatString = DateTimeStyle.Time19;
            }
        }

        return settings;
    }

#if NET10_0_OR_GREATER
    [UnconditionalSuppressMessage("TrimAnalyzer", "IL2026: JsonSerializer")]
    [UnconditionalSuppressMessage("TrimAnalyzer", "IL3050: JsonSerializer")]
#endif
    internal class LowerCaseContractResolver : DefaultContractResolver
    {
        protected override string ResolvePropertyName(string propertyName)
        {
            return propertyName.NameToLower();
        }
    }

}
