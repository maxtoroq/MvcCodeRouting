MSBuild.exe ..\..\src\MvcCodeRouting\MvcCodeRouting.csproj /p:Configuration=Release
MSBuild.exe ..\..\src\MvcCodeRouting.Web.Http\MvcCodeRouting.Web.Http.csproj /p:Configuration=Release
MSBuild.exe ..\..\src\MvcCodeRouting.Web.Http.WebHost\MvcCodeRouting.Web.Http.WebHost.csproj /p:Configuration=Release
MSBuild.exe MvcCodeRouting.shfbproj
MSBuild.exe ..\..\submodules\sandcastle-md\sandcastle-md.sln
rd /s /q ..\..\docs\api
..\..\submodules\sandcastle-md\src\sandcastle-md\bin\Debug\sandcastle-md.exe output ..\..\docs\api
