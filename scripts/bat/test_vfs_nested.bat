@echo off
setlocal
pushd "%~dp0..\.."

echo === VFS with three directory levels ===
(
    echo ls /
    echo cd level1
    echo ls /level1
    echo cd level2
    echo ls level3
    echo cd level3
    echo ls /level1/level2/level3
    echo cd /level1/level2
    echo ls level3
    echo ls /level1/level2/level3
    echo cd /level1/level2/level3
    echo ls /level1/level2/level3
    echo cd /
    echo ls /
    echo exit
) | dotnet run --project Practic1 -- --vfsPath scripts/vfs_data/nested

popd
endlocal
