@echo off
setlocal

set "ROOT=%~dp0..\.."
pushd "%ROOT%"
set "ROOT=%CD%"
popd

echo === Frontend -> http://localhost:5002 ===
echo.

dotnet run --project "%ROOT%\src\Frontend" --configuration Release
endlocal
