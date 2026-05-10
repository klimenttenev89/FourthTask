@echo off
setlocal

set "ROOT=%~dp0..\.."
pushd "%ROOT%"
set "ROOT=%CD%"
popd

echo === Gateway -> http://localhost:5000 (Swagger: http://localhost:5000/swagger) ===
echo.

dotnet run --project "%ROOT%\src\Gateway" --configuration Release
endlocal
