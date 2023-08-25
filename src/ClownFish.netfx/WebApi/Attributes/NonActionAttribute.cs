using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ClownFish.WebApi
{
    /// <summary>
    /// 
    /// </summary>
    [AttributeUsage(AttributeTargets.Method, AllowMultiple = false, Inherited = true)]
    public sealed class NonActionAttribute : Attribute
    {
    }


}
