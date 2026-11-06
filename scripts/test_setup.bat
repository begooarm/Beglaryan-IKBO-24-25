@echo off
setlocal
pushd "%~dp0.."

echo === Testing setup with paths from testData ===
(
    echo setup --vfsPath Practic1/testData --startScriptPath Practic1/testData/starting.txt
    echo conf-dump
    echo ls
    echo cd Practic1/testData
    echo exit
) | dotnet run --project Practic1

echo === Testing setup with config.ini ===
(
    echo setup --vfsPath Practic1/testData --startScriptPath Practic1/testData/starting.txt --configPath Practic1/testData/config.ini
    echo conf-dump
    echo exit
) | dotnet run --project Practic1

popd
pause
endlocal
