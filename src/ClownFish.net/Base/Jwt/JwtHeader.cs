namespace ClownFish.Base.Jwt;

internal sealed class JwtHeader
{
    [JsonProperty("typ")]
    public string Type { get; set; }

    [JsonProperty("alg")]
    public string Algorithm { get; set; }


    //[JsonProperty("cty")]
    //public string ContentType { get; set; }        

    //[JsonProperty("kid")]
    //public string KeyId { get; set; }

    //[JsonProperty("x5u")]
    //public string X5u { get; set; }

    //[JsonProperty("x5c")]
    //public string[] X5c { get; set; }

    //[JsonProperty("x5t")]
    //public string X5t { get; set; }
}
