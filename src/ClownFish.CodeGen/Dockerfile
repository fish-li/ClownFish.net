FROM mcr.microsoft.com/dotnet/sdk:6.0

ENV TZ=Asia/Shanghai
RUN ln -snf /usr/share/zoneinfo/$TZ /etc/localtime && echo $TZ > /etc/timezone

WORKDIR /clownfish
ADD ./publish /clownfish
