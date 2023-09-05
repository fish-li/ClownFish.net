﻿using System.Security.Claims;

namespace ClownFish.Web.Security.Auth;

/// <summary>
/// 已登录用户的身份凭证
/// </summary>
public sealed class NbPrincipal : ClaimsPrincipal
{
    /// <summary>
    /// 登录凭证
    /// </summary>
    public LoginTicket Ticket { get; private set; }

    /// <summary>
    /// 登录凭证来源标记。
    /// </summary>
    public LoginTicketSource Source { get; private set; }


    /// <summary>
    /// 构造方法
    /// </summary>
    /// <param name="ticket"></param>
    /// <param name="source"></param>
    /// <exception cref="ArgumentNullException"></exception>
    public NbPrincipal(LoginTicket ticket, LoginTicketSource source)
    {
        if( ticket == null )
            throw new ArgumentNullException(nameof(ticket));


        this.Ticket = ticket;
        this.Source = source;

        this.AddIdentity(new NbIdentity(ticket.User));
    }


    /// <summary>
    /// IsInRole
    /// </summary>
    /// <param name="role"></param>
    /// <returns></returns>
    public override bool IsInRole(string role)
    {
        if( role.IsNullOrEmpty() )
            return false;

        return role.EqualsIgnoreCase(this.Ticket.User.UserRole);
    }
}
