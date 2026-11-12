@echo off
setlocal
pushd "%~dp0..\.."

echo === Testing wc and uptime with a startup script ===
dotnet run --project Practic1 -- --vfsPath scripts/vfs_data/wc_uptime --startScriptPath scripts/startup/test_wc_uptime.txt

popd
endlocal
