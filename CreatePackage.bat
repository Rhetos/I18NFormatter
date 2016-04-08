PUSHD "%~dp0"
IF NOT EXIST packages\NugetExe MD packages\NugetExe || POPD && EXIT /B 1
IF NOT EXIST packages\NugetExe\NuGet.exe PowerShell.exe -Command "Invoke-WebRequest http://nuget.org/nuget.exe -OutFile packages\NugetExe\NuGet.exe" || POPD && EXIT /B 1
packages\NugetExe\NuGet.exe pack -o .. || POPD && EXIT /B 1
POPD
