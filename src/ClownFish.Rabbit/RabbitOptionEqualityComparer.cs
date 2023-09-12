namespace ClownFish.Rabbit;

internal class RabbitOptionEqualityComparer : EqualityComparer<RabbitOption>
{
    public override bool Equals([AllowNull] RabbitOption x, [AllowNull] RabbitOption y)
    {
        if( x == null && y == null )
            return true;
        else if( x == null || y == null )
            return false;

        if( object.ReferenceEquals(x, y) )
            return true;

        return x.Server == y.Server
            && x.Port == y.Port
            && x.HttpPort == y.HttpPort
            && x.Username == y.Username
            && x.Password == y.Password;
    }

    public override int GetHashCode([DisallowNull] RabbitOption obj)
    {
        unchecked {
            int hash = obj.Server.GetHashCode();
            hash = 31 * hash + obj.Port;
            hash = 31 * hash + obj.HttpPort;
            hash = 31 * hash + obj.Username.GetHashCode();
            return 31 * hash + obj.Password.GetHashCode();
        }
    }
}
