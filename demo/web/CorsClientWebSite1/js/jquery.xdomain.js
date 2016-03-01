//PROJECT MOVED TO https://github.com/Ovea/cors

/**
 * https://gist.github.com/1114981
 *
 * By default, support transferring session cookie with XDomainRequest for IE. The cookie value is by default 'jsessionid'
 *
 * You can change the session cookie value like this, before including this script:
 *
 * window.SESSION_COOKIE_NAME = 'PHP_SESSION';
 * window.SESSION_COOKIE_NAME = 'ID';
 *
 * Or if you want to disable cookie session support:
 *
 * window.SESSION_COOKIE_NAME = null;
 * 
 * ================
 * MINIFIED VERSION
 * ================
 * (function(c){if(c.browser.msie&&"XDomainRequest"in window&&!("__jquery_xdomain__"in c)){c.__jquery_xdomain__=c.support.cors=!0;var e=function(a){return"object"===c.type(a)?a:(a=/^(((([^:\/#\?]+:)?(?:\/\/((?:(([^:@\/#\?]+)(?:\:([^:@\/#\?]+))?)@)?(([^:\/#\?]+)(?:\:([0-9]+))?))?)?)?((\/?(?:[^\/\?#]+\/+)*)([^\?#]*)))?(\?[^#]+)?)(#.*)?/.exec(a))?{href:a[0]||"",hrefNoHash:a[1]||"",hrefNoSearch:a[2]||"",domain:a[3]||"",protocol:a[4]||"",authority:a[5]||"",username:a[7]||"",password:a[8]||"",host:a[9]||"", hostname:a[10]||"",port:a[11]||"",pathname:a[12]||"",directory:a[13]||"",filename:a[14]||"",search:a[15]||"",hash:a[16]||""}:{}},f=c.ajaxSettings.xhr,d="SESSION_COOKIE_NAME"in window?window.SESSION_COOKIE_NAME:"jsessionid",g=e(document.location.href).domain;c.ajaxSettings.xhr=function(){var a=e(this.url).domain;if(""===a||a===g)return f.call(c.ajaxSettings);try{var b=new XDomainRequest;if(!b.setRequestHeader)b.setRequestHeader=c.noop;if(!b.getAllResponseHeaders)b.getAllResponseHeaders=c.noop;if(d){var h= b.open;b.open=function(){var a=RegExp("(?:^|; )"+d+"=([^;]*)","i").exec(document.cookie);if(a=a&&a[1]){var b=arguments[1].indexOf("?");arguments[1]=-1==b?arguments[1]+(";"+d+"="+a):arguments[1].substring(0,b)+";"+d+"="+a+arguments[1].substring(b)}return h.apply(this,arguments)}}b.onload=function(){if("function"===typeof b.onreadystatechange)b.readyState=4,b.status=200,b.onreadystatechange.call(b,null,!1)};b.onerror=b.ontimeout=function(){if("function"===typeof b.onreadystatechange)b.readyState=4, b.status=500,b.onreadystatechange.call(b,null,!1)};return b}catch(i){}}}})(jQuery);
 * ================
 */
(function ($) {

    var ns = '__jquery_xdomain__',
        sc = 'SESSION_COOKIE_NAME';

    if ($.browser.msie && 'XDomainRequest' in window && !(ns in $)) {

        $[ns] = $.support.cors = true;

        function parseUrl(url) {
            if ($.type(url) === "object") {
                return url;
            }
            var matches = /^(((([^:\/#\?]+:)?(?:\/\/((?:(([^:@\/#\?]+)(?:\:([^:@\/#\?]+))?)@)?(([^:\/#\?]+)(?:\:([0-9]+))?))?)?)?((\/?(?:[^\/\?#]+\/+)*)([^\?#]*)))?(\?[^#]+)?)(#.*)?/.exec(url);
            return matches ? {
                href:matches[0] || "",
                hrefNoHash:matches[1] || "",
                hrefNoSearch:matches[2] || "",
                domain:matches[3] || "",
                protocol:matches[4] || "",
                authority:matches[5] || "",
                username:matches[7] || "",
                password:matches[8] || "",
                host:matches[9] || "",
                hostname:matches[10] || "",
                port:matches[11] || "",
                pathname:matches[12] || "",
                directory:matches[13] || "",
                filename:matches[14] || "",
                search:matches[15] || "",
                hash:matches[16] || ""
            } : {};
        }

    	var oldxhr = $.ajaxSettings.xhr,
			// ASP.NET 的 SessionId 是 HttpOnly 的，所以这样的写法不起作用，暂且不说名称不对的问题。
            //sessionCookie = sc in window ? window[sc] : "jsessionid",
            domain = parseUrl(document.location.href).domain;

        $.ajaxSettings.xhr = function () {
            var target = parseUrl(this.url).domain;
            if (target === "" || target === domain) {
                return oldxhr.call($.ajaxSettings)
            } else {
                try {
                    var xdr = new XDomainRequest();
                    if (!xdr.setRequestHeader) {
                        xdr.setRequestHeader = $.noop;
                    }
                    if (!xdr.getAllResponseHeaders) {
                        xdr.getAllResponseHeaders = $.noop;
                    }
                    //if (sessionCookie) {
                    //    var open = xdr.open;
                    //    xdr.open = function () {
                    //        var cookie = new RegExp('(?:^|; )' + sessionCookie + '=([^;]*)', 'i').exec(document.cookie);
                    //        cookie = cookie && cookie[1];
                    //        if (cookie) {
                    //            var q = arguments[1].indexOf('?');
                    //            if (q == -1) {
                    //                arguments[1] += ';' + sessionCookie + '=' + cookie;
                    //            } else {
                    //                arguments[1] = arguments[1].substring(0, q) + ';' + sessionCookie + '=' + cookie + arguments[1].substring(q);
                    //            }
                    //        }
                    //        return open.apply(this, arguments);
                    //    };
                    //}
                    xdr.onload = function () {
                        if (typeof xdr.onreadystatechange === 'function') {
                            xdr.readyState = 4;
                            xdr.status = 200;
                            xdr.onreadystatechange.call(xdr, null, false);
                        }
                    };
                    xdr.onerror = xdr.ontimeout = function () {
                        if (typeof xdr.onreadystatechange === 'function') {
                            xdr.readyState = 4;
                            xdr.status = 500;
                            xdr.onreadystatechange.call(xdr, null, false);
                        }
                    };
                    return xdr;
                } catch (e) {
                }
            }
        };

    }
})(jQuery);