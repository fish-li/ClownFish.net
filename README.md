
##ClownFish.net 是什么

ClownFish.net是一个基于 .net framework 的开发框架，主要包含 **ClownFish.Base** ，**ClownFish.Web** ，**ClownFish.Data** 和 **ClownFish.Log** 四大部分。  


##ClownFish.Web
ClownFish.Web 的前身是 MyMVC项目，您可以查看我的博客链接简单的了解MyMVC的初始功能：  
<http://www.cnblogs.com/fish-li/archive/2012/02/12/2348395.html>  
<http://www.cnblogs.com/fish-li/archive/2012/02/21/2361982.html>


ClownFish.Web 是一个运行在ASP.NET平台下的MVC框架，可以让您开发出完全符合MVC规范的应用程序，  
它与ASP.NET MVC & WEB API框架有着类似的功能，但更简单，更容易学习，可以满足绝大多数的WEB开发需求，  
下面是它支持的部分特性列表： 

 - 支持Contrller, View, Model分离
 - 支持ASP.NET Routing
 - 支持BigPipe
 - 支持Action自匹配（配合jQuery.form开发AJAX非常简单）
 - 支持Service开发（XML, JSON请求/响应）
 - 支持HttpClient（简单且强大，支持同步/异步）
 - 支持Controller同名，以命名空间方式区分（适合于大项目开发）
 - 支持自定义权限验证
 - 支持OutputCache
 - 支持自定义Form数据的转换器
 - 支持跨域请求
 - 支持Razor
 - 支持await/async异步Controller
 - 支持灰度发布（内建反向代理）
 - 支持强大的扩展特性（类型拦截，实例事件扩展订阅）
 - 支持404错误诊断
 - 封装了ASP.NET的文件监视，使用更简单


ClownFish.Web也有一些独有设计，可以简化开发任务，甚至实现一些微软框架没有的功能。  
在持续优化的过程中，ClownFish.Web更关注应用程序的扩展性，例如，  

 - 支持类型拦截
 - 支持实例事件扩展订阅


除此之外，ClownFish.Web还包含了我多年ASP.NET的开发经验，  
已将我认为最有价值的功能，特性，开发模式融入其中，  
留给大家的是一个简单，易学习，功能强大的WEB开发框架。


##ClownFish.Data
ClownFish.Data 是一个通用的数据访问层，前身是ClownFish项目，  
<http://www.cnblogs.com/fish-li/archive/2012/07/17/ClownFish.html>  
现在，这个项目经过重写，拥有更清晰，更易于使用的API接口，要以点击下面的链接浏览简要使用说明：  
<http://note.youdao.com/yws/public/redirect/share?id=84f8d3d0062170ba552a7406c99050de&type=false>


##ClownFish.Log
ClownFish.Log 是一个通用的日志组件，专为大型项目而设计：
 - 提供同步和异常写入API
 - 内置5种日志写入器（MongoDb,File,Msmq,WinLog,Mail），并允许自行扩展
 - 允许为每个数据结构指定一个或者多个写入器
 - 允许自行扩展写入器的过滤器
 - 允许在组件发生异常时指定重试机制或者订阅事件
 - 内置完整的HTTP请求记录功能
 - 内置完整的DbCommand记录功能
 - 内置性能日志模块
