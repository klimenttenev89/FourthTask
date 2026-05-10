@echo off
setlocal enabledelayedexpansion

set "ROOT=%~dp0..\.."
pushd "%ROOT%"
set "ROOT=%CD%"
popd

echo === NorthwindTraders - Build ===
echo Root: %ROOT%
echo.

dotnet build "%ROOT%\NorthwindTraders.sln" --configuration Release
if errorlevel 1 (
    echo.
    echo Build FAILED.
    exit /b 1
)

echo.
echo Build succeeded.
endlocal
