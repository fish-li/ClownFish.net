ClownFish.net 的版权问题
====================================
ClownFish.net 是一款免费的软件，可用于任何项目，没有任何版权限制。
由于 ClownFish.net 的BUG造成的任何产品问题，也请自行承担。



如何在IIS中运行示例
====================================
请参考网址：
http://www.cnblogs.com/fish-li/archive/2012/02/26/2368989.html
或者参考：《配置 MyMVC 演示程序.docx》



DEMO对SQLSERVER的支持
====================================

MvcDemoWebSite1 网站现在可使用三种数据访问方式：
1. 早期的XML文件（读写全是一个文件）。
2. 用存储过程的方式访问SQLSERVER。
3. 用XmlCommand的方式访问SQLSERVER。


切换方式：
打开MvcDemoWebSite1\web.config,找到appSettings配置节，参考里面的注释。



SQLSERVER数据库文件：
示例所需的MyNorthwind数据库文件的下载地址：http://files.cnblogs.com/fish-li/MyNorthwind.7z
关于数据库的配置可参考：http://www.cnblogs.com/fish-li/archive/2012/02/26/2368989.html


说明：
后面二种数据访问方式采用了ClownFish，它是一个通用的数据访问层，可支持多种数据库，
更多的ClownFish介绍可参考：http://www.cnblogs.com/fish-li/archive/2012/07/17/ClownFish.html


