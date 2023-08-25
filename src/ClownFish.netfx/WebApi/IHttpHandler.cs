using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ClownFish.Http.Pipleline;

namespace ClownFish.WebApi
{
    /// <summary>
    /// 
    /// </summary>
    public interface IHttpHandler
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="context"></param>
        void ProcessRequest(NHttpContext context);
    }
}
