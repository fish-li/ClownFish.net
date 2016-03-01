using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using ClownFish.Base.Reflection;

namespace ClownFish.Web.Reflection
{
	internal class BaseDescription
	{
		public OutputCacheAttribute OutputCache { get; protected set; }
		public SessionModeAttribute SessionMode { get; protected set; }
		public AuthorizeAttribute Authorize { get; protected set; }

		protected BaseDescription(MemberInfo m)
		{
			this.OutputCache = m.GetMyAttribute<OutputCacheAttribute>(false);
			this.SessionMode = m.GetMyAttribute<SessionModeAttribute>(false);
			this.Authorize = m.GetMyAttribute<AuthorizeAttribute>(true /* inherit */);
		}
	}



}
