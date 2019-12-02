@echo off
cls
color 0A

cd /d %~dp0

cd Presentation\Xms.Web

echo ".net build"

dotnet restore

dotnet build

dotnet publish -c Release -o ../../publish

echo 'deploy completed...'

set/p sel=starting now(y/n)?:
if "%sel%"=="y" goto ok
if "%sel%"=="n" goto end

:ok
cd ../../publish
dotnet Xms.Web.dll

:end
pause