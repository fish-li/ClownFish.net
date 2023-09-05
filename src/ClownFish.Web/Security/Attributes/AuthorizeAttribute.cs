namespace ClownFish.Web.Security.Attributes;

/// <summary>
/// 执行授权检查验证的标记
/// </summary>
[AttributeUsage(AttributeTargets.Class | AttributeTargets.Method, AllowMultiple = false, Inherited = false)]
public class AuthorizeAttribute : Attribute
{
    private string _user;
    private string[] _users;

    private string _role;
    private string[] _roles;

    private string _rights;
    private string[] _rightsArray;

    private Type _userInfoType;

    /// <summary>
    /// 允许访问的用户列表，用逗号分隔。
    /// </summary>
    public string Users {
        get { return _user; }
        set {
            _user = value;
            _users = value.ToArray2();
        }
    }

    /// <summary>
    /// 允许访问的角色列表，用逗号分隔。
    /// </summary>
    public string Roles {
        get { return _role; }
        set {
            _role = value;
            _roles = value.ToArray2();
        }
    }

    /// <summary>
    /// 允许访问的权限编号列表，用逗号分隔。
    /// </summary>
    public string Rights {
        get { return _rights; }
        set {
            _rights = value;
            _rightsArray = value.ToArray2();
        }
    }


    /// <summary>
    /// 允许的 context.GetUserInfo() 返回结果类型
    /// </summary>
    public Type UserInfoType {
        get => _userInfoType;
        set => _userInfoType = value;
    }

    /// <summary>
    /// 执行授权检查
    /// </summary>
    /// <param name="context"></param>
    /// <returns></returns>
    public virtual bool AuthenticateRequest(NHttpContext context)
    {
        IUserInfo userInfo = context.GetUserInfo();

        // 当前用户没有登录
        if( userInfo == null )
            return false;

        // 检查 IUserInfo 的实例类型是不是预期类型
        if( _userInfoType != null && userInfo.GetType() != _userInfoType )
            return false;

        // 没有任何明确的授权要求，此时只要是“已登录”用户就算通过检查
        if( _users == null && _roles == null && _rightsArray == null )
            return true;


        // 三种授权条件，只要一个符合就认为是检查通过
        if( CheckUser(context, userInfo) || CheckRole(context, userInfo) || CheckRights(context, userInfo) )
            return true;


        // 所有条件都不匹配，授权检查失败！
        return false;
    }

    /// <summary>
    /// 根据用户名执行授权检查
    /// </summary>
    /// <param name="context"></param>
    /// <param name="userInfo"></param>
    /// <returns></returns>
    public virtual bool CheckUser(NHttpContext context, IUserInfo userInfo)
    {
        return (_users != null && _users.Contains(userInfo.UserId, StringComparer.OrdinalIgnoreCase));
    }

    /// <summary>
    /// 根据用户角色执行授权检查
    /// </summary>
    /// <param name="context"></param>
    /// <param name="userInfo"></param>
    /// <returns></returns>
    public virtual bool CheckRole(NHttpContext context, IUserInfo userInfo)
    {
        return (_roles != null && _roles.Any(context.User.IsInRole));
    }


    /// <summary>
    /// 检查当前用户是否拥有指定编号的权限，
    /// 注意：此方法【没有真正的实现逻辑】，实际使用时需要重写！
    /// </summary>
    /// <param name="context"></param>
    /// <param name="userInfo"></param>
    /// <returns></returns>
    public virtual bool CheckRights(NHttpContext context, IUserInfo userInfo)
    {
        if( _rightsArray == null )
            return false;

        if( userInfo == null )
            return false;

        // 需要重写检查过程
        return s_checkRightsImpl.CheckRights(context, userInfo, _rightsArray);
    }


    private static ICheckRights s_checkRightsImpl = new NullCheckRights();

    internal static void SetCheckRightsImpl(ICheckRights impl)
    {
        if( impl == null)
            throw new ArgumentNullException(nameof(impl));

        s_checkRightsImpl = impl;
    }
}


/// <summary>
/// 检查权限编号的接口
/// </summary>
public interface ICheckRights
{
    bool CheckRights(NHttpContext context, IUserInfo userInfo, string[] rightArray);
}


internal sealed class NullCheckRights : ICheckRights
{
    public bool CheckRights(NHttpContext context, IUserInfo userInfo, string[] rightArray)
    {
        return true;
    }
}