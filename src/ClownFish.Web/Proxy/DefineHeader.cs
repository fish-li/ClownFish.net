//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Text;
//using System.Threading.Tasks;

//namespace ClownFish.Web.Proxy
//{
//    class DefineHeader
//    {
//    }
//}



//HttpWorkerRequest.DefineHeader(true, true, 0, "Cache-Control", "HTTP_CACHE_CONTROL");
//HttpWorkerRequest.DefineHeader(true, true, 1, "Connection", "HTTP_CONNECTION");
//HttpWorkerRequest.DefineHeader(true, true, 2, "Date", "HTTP_DATE");
//HttpWorkerRequest.DefineHeader(true, true, 3, "Keep-Alive", "HTTP_KEEP_ALIVE");
//HttpWorkerRequest.DefineHeader(true, true, 4, "Pragma", "HTTP_PRAGMA");
//HttpWorkerRequest.DefineHeader(true, true, 5, "Trailer", "HTTP_TRAILER");
//HttpWorkerRequest.DefineHeader(true, true, 6, "Transfer-Encoding", "HTTP_TRANSFER_ENCODING");
//HttpWorkerRequest.DefineHeader(true, true, 7, "Upgrade", "HTTP_UPGRADE");
//HttpWorkerRequest.DefineHeader(true, true, 8, "Via", "HTTP_VIA");
//HttpWorkerRequest.DefineHeader(true, true, 9, "Warning", "HTTP_WARNING");
//HttpWorkerRequest.DefineHeader(true, true, 10, "Allow", "HTTP_ALLOW");
//HttpWorkerRequest.DefineHeader(true, true, 11, "Content-Length", "HTTP_CONTENT_LENGTH");
//HttpWorkerRequest.DefineHeader(true, true, 12, "Content-Type", "HTTP_CONTENT_TYPE");
//HttpWorkerRequest.DefineHeader(true, true, 13, "Content-Encoding", "HTTP_CONTENT_ENCODING");
//HttpWorkerRequest.DefineHeader(true, true, 14, "Content-Language", "HTTP_CONTENT_LANGUAGE");
//HttpWorkerRequest.DefineHeader(true, true, 15, "Content-Location", "HTTP_CONTENT_LOCATION");
//HttpWorkerRequest.DefineHeader(true, true, 16, "Content-MD5", "HTTP_CONTENT_MD5");
//HttpWorkerRequest.DefineHeader(true, true, 17, "Content-Range", "HTTP_CONTENT_RANGE");
//HttpWorkerRequest.DefineHeader(true, true, 18, "Expires", "HTTP_EXPIRES");
//HttpWorkerRequest.DefineHeader(true, true, 19, "Last-Modified", "HTTP_LAST_MODIFIED");
//HttpWorkerRequest.DefineHeader(true, false, 20, "Accept", "HTTP_ACCEPT");
//HttpWorkerRequest.DefineHeader(true, false, 21, "Accept-Charset", "HTTP_ACCEPT_CHARSET");
//HttpWorkerRequest.DefineHeader(true, false, 22, "Accept-Encoding", "HTTP_ACCEPT_ENCODING");
//HttpWorkerRequest.DefineHeader(true, false, 23, "Accept-Language", "HTTP_ACCEPT_LANGUAGE");
//HttpWorkerRequest.DefineHeader(true, false, 24, "Authorization", "HTTP_AUTHORIZATION");
//HttpWorkerRequest.DefineHeader(true, false, 25, "Cookie", "HTTP_COOKIE");
//HttpWorkerRequest.DefineHeader(true, false, 26, "Expect", "HTTP_EXPECT");
//HttpWorkerRequest.DefineHeader(true, false, 27, "From", "HTTP_FROM");
//HttpWorkerRequest.DefineHeader(true, false, 28, "Host", "HTTP_HOST");
//HttpWorkerRequest.DefineHeader(true, false, 29, "If-Match", "HTTP_IF_MATCH");
//HttpWorkerRequest.DefineHeader(true, false, 30, "If-Modified-Since", "HTTP_IF_MODIFIED_SINCE");
//HttpWorkerRequest.DefineHeader(true, false, 31, "If-None-Match", "HTTP_IF_NONE_MATCH");
//HttpWorkerRequest.DefineHeader(true, false, 32, "If-Range", "HTTP_IF_RANGE");
//HttpWorkerRequest.DefineHeader(true, false, 33, "If-Unmodified-Since", "HTTP_IF_UNMODIFIED_SINCE");
//HttpWorkerRequest.DefineHeader(true, false, 34, "Max-Forwards", "HTTP_MAX_FORWARDS");
//HttpWorkerRequest.DefineHeader(true, false, 35, "Proxy-Authorization", "HTTP_PROXY_AUTHORIZATION");
//HttpWorkerRequest.DefineHeader(true, false, 36, "Referer", "HTTP_REFERER");
//HttpWorkerRequest.DefineHeader(true, false, 37, "Range", "HTTP_RANGE");
//HttpWorkerRequest.DefineHeader(true, false, 38, "TE", "HTTP_TE");
//HttpWorkerRequest.DefineHeader(true, false, 39, "User-Agent", "HTTP_USER_AGENT");
//HttpWorkerRequest.DefineHeader(false, true, 20, "Accept-Ranges", null);
//HttpWorkerRequest.DefineHeader(false, true, 21, "Age", null);
//HttpWorkerRequest.DefineHeader(false, true, 22, "ETag", null);
//HttpWorkerRequest.DefineHeader(false, true, 23, "Location", null);
//HttpWorkerRequest.DefineHeader(false, true, 24, "Proxy-Authenticate", null);
//HttpWorkerRequest.DefineHeader(false, true, 25, "Retry-After", null);
//HttpWorkerRequest.DefineHeader(false, true, 26, "Server", null);
//HttpWorkerRequest.DefineHeader(false, true, 27, "Set-Cookie", null);
//HttpWorkerRequest.DefineHeader(false, true, 28, "Vary", null);
//HttpWorkerRequest.DefineHeader(false, true, 29, "WWW-Authenticate", null);


//internal HeaderInfo(string name, bool requestRestricted, bool responseRestricted, bool multi, HeaderParser p)

//static HeaderInfoTable() {

//	HeaderInfo[] InfoArray = new HeaderInfo[] {
//		new HeaderInfo(HttpKnownHeaderNames.Age,                false,  false,  false,  SingleParser),
//		new HeaderInfo(HttpKnownHeaderNames.Allow,              false,  false,  true,   MultiParser),
//		new HeaderInfo(HttpKnownHeaderNames.Accept,             true,   false,  true,   MultiParser),
//		new HeaderInfo(HttpKnownHeaderNames.Authorization,      false,  false,  true,   MultiParser),
//		new HeaderInfo(HttpKnownHeaderNames.AcceptRanges,       false,  false,  true,   MultiParser),
//		new HeaderInfo(HttpKnownHeaderNames.AcceptCharset,      false,  false,  true,   MultiParser),
//		new HeaderInfo(HttpKnownHeaderNames.AcceptEncoding,     false,  false,  true,   MultiParser),
//		new HeaderInfo(HttpKnownHeaderNames.AcceptLanguage,     false,  false,  true,   MultiParser),
//		new HeaderInfo(HttpKnownHeaderNames.Cookie,             false,  false,  true,   MultiParser),
//		new HeaderInfo(HttpKnownHeaderNames.Connection,         true,   false,  true,   MultiParser),
//		new HeaderInfo(HttpKnownHeaderNames.ContentMD5,         false,  false,  false,  SingleParser),
//		new HeaderInfo(HttpKnownHeaderNames.ContentType,        true,   false,  false,  SingleParser),
//		new HeaderInfo(HttpKnownHeaderNames.CacheControl,       false,  false,  true,   MultiParser),
//		new HeaderInfo(HttpKnownHeaderNames.ContentRange,       false,  false,  false,  SingleParser),
//		new HeaderInfo(HttpKnownHeaderNames.ContentLength,      true,   true,   false,  SingleParser),
//		new HeaderInfo(HttpKnownHeaderNames.ContentEncoding,    false,  false,  true,   MultiParser),
//		new HeaderInfo(HttpKnownHeaderNames.ContentLanguage,    false,  false,  true,   MultiParser),
//		new HeaderInfo(HttpKnownHeaderNames.ContentLocation,    false,  false,  false,  SingleParser),
//		new HeaderInfo(HttpKnownHeaderNames.Date,               true,   false,  false,  SingleParser),
//		new HeaderInfo(HttpKnownHeaderNames.ETag,               false,  false,  false,  SingleParser),
//		new HeaderInfo(HttpKnownHeaderNames.Expect,             true,   false,  true,   MultiParser),
//		new HeaderInfo(HttpKnownHeaderNames.Expires,            false,  false,  false,  SingleParser),
//		new HeaderInfo(HttpKnownHeaderNames.From,               false,  false,  false,  SingleParser),
//		new HeaderInfo(HttpKnownHeaderNames.Host,               true,   false,  false,  SingleParser),
//		new HeaderInfo(HttpKnownHeaderNames.IfMatch,            false,  false,  true,   MultiParser),
//		new HeaderInfo(HttpKnownHeaderNames.IfRange,            false,  false,  false,  SingleParser),
//		new HeaderInfo(HttpKnownHeaderNames.IfNoneMatch,        false,  false,  true,   MultiParser),
//		new HeaderInfo(HttpKnownHeaderNames.IfModifiedSince,    true,   false,  false,  SingleParser),
//		new HeaderInfo(HttpKnownHeaderNames.IfUnmodifiedSince,  false,  false,  false,  SingleParser),
//		new HeaderInfo(HttpKnownHeaderNames.KeepAlive,          false,  true,   false,  SingleParser),
//		new HeaderInfo(HttpKnownHeaderNames.Location,           false,  false,  false,  SingleParser),
//		new HeaderInfo(HttpKnownHeaderNames.LastModified,       false,  false,  false,  SingleParser),
//		new HeaderInfo(HttpKnownHeaderNames.MaxForwards,        false,  false,  false,  SingleParser),
//		new HeaderInfo(HttpKnownHeaderNames.Pragma,             false,  false,  true,   MultiParser),
//		new HeaderInfo(HttpKnownHeaderNames.ProxyAuthenticate,  false,  false,  true,   MultiParser),
//		new HeaderInfo(HttpKnownHeaderNames.ProxyAuthorization, false,  false,  true,   MultiParser),
//		new HeaderInfo(HttpKnownHeaderNames.ProxyConnection,    true,   false,  true,   MultiParser),
//		new HeaderInfo(HttpKnownHeaderNames.Range,              true,   false,  true,   MultiParser),
//		new HeaderInfo(HttpKnownHeaderNames.Referer,            true,   false,  false,  SingleParser),
//		new HeaderInfo(HttpKnownHeaderNames.RetryAfter,         false,  false,  false,  SingleParser),
//		new HeaderInfo(HttpKnownHeaderNames.Server,             false,  false,  false,  SingleParser),
//		new HeaderInfo(HttpKnownHeaderNames.SetCookie,          false,  false,  true,   MultiParser),
//		new HeaderInfo(HttpKnownHeaderNames.SetCookie2,         false,  false,  true,   MultiParser),
//		new HeaderInfo(HttpKnownHeaderNames.TE,                 false,  false,  true,   MultiParser),
//		new HeaderInfo(HttpKnownHeaderNames.Trailer,            false,  false,  true,   MultiParser),
//		new HeaderInfo(HttpKnownHeaderNames.TransferEncoding,   true,   true,   true,   MultiParser),
//		new HeaderInfo(HttpKnownHeaderNames.Upgrade,            false,  false,  true,   MultiParser),
//		new HeaderInfo(HttpKnownHeaderNames.UserAgent,          true,   false,  false,  SingleParser),
//		new HeaderInfo(HttpKnownHeaderNames.Via,                false,  false,  true,   MultiParser),
//		new HeaderInfo(HttpKnownHeaderNames.Vary,               false,  false,  true,   MultiParser),
//		new HeaderInfo(HttpKnownHeaderNames.Warning,            false,  false,  true,   MultiParser),
//		new HeaderInfo(HttpKnownHeaderNames.WWWAuthenticate,    false,  true,   true,   SingleParser)
//	};

