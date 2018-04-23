PUSHD "%~dp0"
WHERE /Q NuGet.exe || ECHO ERROR: Please download the NuGet.exe command line tool. && POPD && EXIT /B 1
NuGet.exe pack -OutputDirectory .. || POPD && EXIT /B 1
POPD
