using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using ClownFish.Base;
using ClownFish.Web;

namespace ClownFish.Web.UnitTest.Controllers
{
	public class ActionAutoFindService : BaseController
	{
		public string Base64(string input)
		{
			return Convert.ToBase64String(Encoding.Default.GetBytes(input));
		}

		public string Md5(string input)
		{
            return HashHelper.Md5(input);
		}
		
		public string Sha1(string input)
		{
            return HashHelper.Sha1(input);
        }




	}
}
