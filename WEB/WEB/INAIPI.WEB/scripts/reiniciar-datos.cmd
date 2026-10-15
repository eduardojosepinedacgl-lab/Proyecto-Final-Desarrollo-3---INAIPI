@echo off
chcp 65001 >nul
echo =========================================================
echo INAIPI WEB - Reinicio de Base de Datos Local SQLite
echo =========================================================
cd /d "%~dp0\.."

if exist "data\inaipi-web.sqlite" del /f /q "data\inaipi-web.sqlite"
if exist "data\inaipi-web.sqlite-wal" del /f /q "data\inaipi-web.sqlite-wal"
if exist "data\inaipi-web.sqlite-shm" del /f /q "data\inaipi-web.sqlite-shm"

echo Archivos SQLite eliminados correctamente.
echo Al iniciar nuevamente el servidor WEB se regenerara una base limpia.
pause