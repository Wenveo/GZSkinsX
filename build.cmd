@echo off

powershell -ExecutionPolicy ByPass -NoProfile -Command "& '%~dp0build.ps1'"
exit /b %ERRORLEVEL%