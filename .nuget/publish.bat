@echo off

set version=%1
if [%version%] neq [] goto askPublish
set /P version="Version: "

:askPublish

set /P c=Are you sure you want to publish version %version%? [Y/N] 
if /I "%c%" EQU "Y" goto :publish
if /I "%c%" EQU "N" goto :exitCommand

:publish
dotnet publish ..\src\MindustrySchematics.Deserializer\MindustrySchematics.Deserializer.csproj -c Release /p:Version=%version%
nuget push ..\src\MindustrySchematics.Deserializer\bin\Release\MindustrySchematics.Deserializer.%version%.nupkg -Source "GitHub Rene-Sackers"

pause
exit

:exitCommand
echo publish canceled

pause