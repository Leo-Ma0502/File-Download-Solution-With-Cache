@echo off
cd .\FileDownloadWithCache\FileServer\bin\Debug\net7.0-windows
start "" ".\FileServer.exe"

cd ..\..\..\..\..\
start "" ".\FileDownloadWithCache\Cache\bin\Debug\net7.0-windows\Cache.exe"

cd .\FileDownloadWithCache\Client\bin\Debug\net7.0-windows
start "" ".\Client.exe"