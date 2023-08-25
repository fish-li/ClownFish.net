## ClownFish.CodeGen是什么？

ClownFish.CodeGen是一个命令行工具，用于代码生成相关工作。

目前的功能只有一个：
- 为数据实体类型生成代理类和加载器类，并编译生成程序集。

<br /><br />



## 命令行用法

```
dotnet.exe ClownFish.CodeGen.dll  D:\xxxxx\Demo.App1\bin\net6.0  _xxx.EntityProxy.dll
# or
dotnet.exe ClownFish.CodeGen.dll  D:\xxxxx\Demo.App1\bin\net6.0
```
用途：搜索指定目录下的所有DLL中的所有实体类型，生成对应的代理程序集。  
第1个参数：包含DLL文件的路径，其中包含有实体程序集。  
第2个参数：输出的程序集名称，此参数可选。  
程序运行后，将生成一个 xxx.EntityProxy.dll 文件，如果文件已存在会先删除。


<br /><br />


## 创建编译环境镜像
为了方便使用，可以将此工具添加到编译环境镜像中。  
工具目录下的Dockerfile已准备好，使用方法：

```sh
#!/bin/bash

# 删除已有的镜像
docker rmi -f  clownfish/codegen:6.0
docker rmi -f  yyw-registry.cn-hangzhou.cr.aliyuncs.com/nebula/sdk:6.0

# 生成镜像，根据当前目录下 Dockfile 生成镜像
docker build -t clownfish/codegen:6.0  .
```

将镜像推送到阿里云镜像仓库
```sh
docker login --username=xxxx@mingyuanyun -p xxxxxxxxxxxxxx yyw-registry.cn-hangzhou.cr.aliyuncs.com
docker login --username=xxxx@mingyuanyun -p xxxxxxxxxxxxxx yyw-registry-vpc.cn-hangzhou.cr.aliyuncs.com

docker tag clownfish/codegen:6.0  yyw-registry.cn-hangzhou.cr.aliyuncs.com/nebula/sdk:6.0

docker push yyw-registry.cn-hangzhou.cr.aliyuncs.com/nebula/sdk:6.0
```

<br /><br />


## 在 Gitlab CI 中使用
建议在 docker build 前运行此工具，例如：
```
pushd ext/Nebula.Mercury/bin


# 运行命令行工具，生成代理类和加载器类
docker run --rm  -v $PWD:/xbin  -w /xbin yyw-registry.cn-hangzhou.cr.aliyuncs.com/nebula/sdk:6.0  dotnet /clownfish/ClownFish.CodeGen.dll  /xbin/publish


cp ../Dockerfile .
docker build -t nebula_mercury  .
docker tag nebula_mercury  registry-vpc.cn-hangzhou.aliyuncs.com/newops/nebula_mercury:$version
docker push registry-vpc.cn-hangzhou.aliyuncs.com/newops/nebula_mercury:$version
popd
```

