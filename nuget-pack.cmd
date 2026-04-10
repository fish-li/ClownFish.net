cd /d %~dp0

dotnet pack --include-symbols -c Release -o D:\TempNuget src\ClownFish.net\ClownFish.net.csproj
dotnet pack --include-symbols -c Release -o D:\TempNuget src\ClownFish.Web\ClownFish.Web.csproj
::dotnet pack --include-symbols -c Release -o D:\TempNuget src\ClownFish.EntityProxyGen\ClownFish.EntityProxyGen.csproj

dotnet pack --include-symbols -c Release -o D:\TempNuget src\ClownFish.Office\ClownFish.Office.csproj
dotnet pack --include-symbols -c Release -o D:\TempNuget src\ClownFish.Redis\ClownFish.Redis.csproj
dotnet pack --include-symbols -c Release -o D:\TempNuget src\ClownFish.Rabbit\ClownFish.Rabbit.csproj
dotnet pack --include-symbols -c Release -o D:\TempNuget src\ClownFish.Email\ClownFish.Email.csproj
dotnet pack --include-symbols -c Release -o D:\TempNuget src\ClownFish.ImClients\ClownFish.ImClients.csproj


:: dotnet pack --include-symbols -c ReleaseAOT -o D:\TempNuget src\ClownFish.net\ClownFish.net.csproj
:: dotnet pack --include-symbols -c Release -o D:\TempNuget src\ClownFish.Tracing\ClownFish.Tracing.csproj
pause
