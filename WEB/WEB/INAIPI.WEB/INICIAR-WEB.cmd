@echo off
cd /d "%~dp0"
node --use-system-ca --env-file-if-exists=.env server.js
pause
