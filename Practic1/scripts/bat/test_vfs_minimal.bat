@echo off
setlocal
pushd "%~dp0..\.."

echo === Minimal VFS ===
(
    echo ls /
    echo cd /
    echo ls /
    echo exit
) | dotnet run --project Practic1 -- --vfsPath scripts/vfs_data/minimal

popd
endlocal
