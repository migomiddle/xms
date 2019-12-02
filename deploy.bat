@echo off

cd /d %~dp0

cd Presentation\Xms.Web

echo ".net build"

dotnet restore

dotnet build

md publish

dotnet publish -c Release -o publish

echo 'deploy completed...'

pause