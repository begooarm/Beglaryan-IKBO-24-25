@echo off
setlocal
pushd "%~dp0..\.."

echo === VFS with several files ===
(
    echo ls /
    echo ls /
    echo cd /
    echo ls /
    echo exit
) | dotnet run --project Practic1 -- --vfsPath scripts/vfs_data/files

popd
endlocal
