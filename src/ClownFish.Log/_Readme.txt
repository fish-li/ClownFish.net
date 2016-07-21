
当前目录主要存放与日志有关的核心框架类型
--------------------------------------
LogHelper：对外提供调用接口的工具类，提供 Write<T> 方法用于写入各种日志信息
CacheQueue：实现了缓存写入的日志队列
PerformanceMoudle：性能日志Moudle







MongoDB支持说明
--------------------------------------
由于MongoDB的驱动文件有3个DLL，实际使用中并不所有项目都需要将日志写到MongoDB中，
因此，ClownFish.Log中MongoDB部分采用了预编译方式控制。
如果需要启用（或者禁用），请修改ClownFish.Log的项目属性，
在【条件编译符号】中如果存在【_MongoDB_】表示启用MongoDB功能，否则为禁止。
ClownFish.Log.UnitTest项目属性中也有同样的设置，请注意一起调整。

MongoDB驱动下载地址：https://github.com/fish-li/mongo-csharp-driver