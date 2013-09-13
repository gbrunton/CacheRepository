REM Update the following files before running this:
REM ./CacheRepository/CacheRepository.nuspec Add new release note
REM ./CacheRepository/Properties/AssemblyInfo.cs Update AssemblyVersion attribute

"./.nuget/nuget" pack ./CacheRepository/CacheRepository.csproj

"./.nuget/nuget" push *.nupkg

pause