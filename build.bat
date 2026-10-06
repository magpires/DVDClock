@echo off
echo Construindo o DVDClock...
dotnet publish -c Release -o .\Release

echo.
echo Copiando como .scr...
copy /Y .\Release\DVDClock.exe .\Release\DVDClock.scr

echo.
echo Build concluido! O arquivo DVDClock.scr esta na pasta Release.
pause
