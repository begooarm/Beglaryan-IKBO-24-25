@echo off
setlocal
pushd "%~dp0Practic2" || exit /b 1
dotnet run --project practic2.csproj -- %*
set "status=%errorlevel%"
popd
exit /b %status%
