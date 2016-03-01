
当前目录主要存放也日志序列化（持久化）有关的实现类型，这些实现类型称为 Writer

本项目主要支持以下Writer：
FileWriter：将日志信息写入到文本文件中（XML序列化）
MailWriter：将日志信息以邮件形式发送（XML序列化）
MsmqWriter：将日志信息写入到MSMQ中（XML序列化）
WinLogWriter：将日志信息写入到Windows日志中（XML序列化）
NullWriter：不执行写入的NULL操作，可用于开发


ILogWriter：所有Writer的接口定义
WriterFactory：Writer工厂，用于根据配置获取不同的Writer

