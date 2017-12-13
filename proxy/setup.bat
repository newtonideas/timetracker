@echo off
cls
C:\Users\Administrator\Downloads\nuget restore proxy.sln
"C:\Program Files (x86)\Microsoft Visual Studio\2017\BuildTools\MSBuild\15.0\bin\MSBuild.exe" proxy.sln
