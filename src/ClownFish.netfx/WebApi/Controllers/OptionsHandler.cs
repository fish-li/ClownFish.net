using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using ClownFish.Http.Pipleline;

namespace ClownFish.WebApi.Controllers
{
	internal sealed class OptionsHandler : IHttpHandler
	{
		public static readonly OptionsHandler Instance = new OptionsHandler();

		public void ProcessRequest(NHttpContext httpContext)
		{
		}

	}
}
