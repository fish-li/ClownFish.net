namespace ClownFish.Jwt;

internal sealed class JwtParts
{
    public string Header => Parts[0];

    public string Payload => Parts[1];

    public string Signature => Parts[2];

    public string[] Parts { get; }

    public JwtParts(string token)
    {
        if( string.IsNullOrWhiteSpace(token) ) {
            throw new ArgumentNullException(nameof(token));
        }

        string[] array = token.Split('.');

        if( array.Length != 3 )
            throw new InvalidTokenPartsException("token格式不正确");


        if( array[0].Length == 0 || array[1].Length == 0 || array[2].Length == 0 )
            throw new InvalidTokenPartsException("token格式不正确2");

        Parts = array;
    }
}
