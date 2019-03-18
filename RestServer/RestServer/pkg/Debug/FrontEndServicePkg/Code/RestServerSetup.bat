REM Set a system environment variable. This requires administrator privilege
echo Begin Execution 
setx -m TestVariable Hello
echo System TestVariable set to > out.txt
echo %TestVariable% >> out.txt
START "" "C:\Windows\System32\notepad.exe"
echo Execution Complete

echo Installing winlogbeat Service
PowerShell.exe -ExecutionPolicy UnRestricted -File .\install-service-winlogbeat.ps1
echo Installation of winlogbeat service complete
REM To delete this system variable us
REM REG delete "HKEY_LOCAL_MACHINE\SYSTEM\CurrentControlSet\Control\Session Manager\Environment" /v TestVariable /f

REM powershell.exe -ExecutionPolicy Bypass -Command ".\RestServerSetup.ps1"