@echo off
setlocal
pushd "%~dp0.."

echo === Testing ls and cd ===
(
    echo ls
    echo cd Practic1/testData
    echo ls
    echo exit
) | dotnet run --project Practic1

popd
pause
endlocal
