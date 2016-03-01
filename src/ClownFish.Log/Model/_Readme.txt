
需要做日志持久化的数据类型定义

BaseInfo：所有需要做日志持久化的数据类型基类

2个主要的数据结构：
ExceptionInfo：异常信息描述结构
PerformanceInfo：性能日志信息描述结构


用于嵌入到其它数据结构的子结构：
BusinessInfo：业务信息描述结构
HttpInfo：HTTP请求信息描述结构
SqlInfo：SQL语句信息描述结构
