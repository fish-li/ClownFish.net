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
    private static readonly JsonSerializerSettings s_jsonSettingsElasticsearch = Get0(SimpleEsClient.EsJsonStyle);

    internal static JsonSerializerSettings Get(JsonStyle style = JsonStyle.None)
    {
        return style switch {
            JsonStyle.None => s_jsonSettingsNone,
            JsonStyle.Indented => s_jsonSettingsIndented,
            JsonStyle.CamelCase => s_jsonSettingsCamelCase,
            SimpleEsClient.EsJsonStyle => s_jsonSettingsElasticsearch,

            _ => s_dict.GetOrAdd(style, Get0)
        };
    }

    private static JsonSerializerSettings Get0(JsonStyle style)
    {
        JsonSerializerSettings settings = new JsonSerializerSettings();

        if( style.HasFlag(JsonStyle.KeepNull) )
            settings.NullValueHandling = NullValueHandling.Include;
        else
            settings.NullValueHandling = NullValueHandling.Ignore;


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


    internal class LowerCaseContractResolver : DefaultContractResolver
    {
        protected override string ResolvePropertyName(string propertyName)
        {
            return propertyName.NameToLower();
        }
    }

}
