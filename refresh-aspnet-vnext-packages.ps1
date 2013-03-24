$nightly = "http://www.myget.org/F/aspnetwebstacknightly/"
$output = "packages\aspnet-vnext"
Remove-Item $output\*.* -Recurse -ErrorAction Ignore

.\.nuget\NuGet.exe install Microsoft.AspNet.Mvc -Prerelease -OutputDirectory $output -ExcludeVersion -Source $nightly
.\.nuget\NuGet.exe install Microsoft.Net.Http -Prerelease -OutputDirectory $output -ExcludeVersion
.\.nuget\NuGet.exe install Microsoft.AspNet.WebApi.WebHost -Prerelease -OutputDirectory $output -ExcludeVersion -Source $nightly
.\.nuget\NuGet.exe install Microsoft.AspNet.WebApi.SelfHost -Prerelease -OutputDirectory $output -ExcludeVersion -Source $nightly