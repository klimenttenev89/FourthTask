@echo off
setlocal

set "ROOT=%~dp0..\.."
pushd "%ROOT%"
set "ROOT=%CD%"
popd

echo === CustomerService -> http://localhost:5001 ===
echo.

dotnet run --project "%ROOT%\src\CustomerService" --configuration Release
endlocal
