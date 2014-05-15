set /p Increment=Really increment the revision number for ConsoleTools? Enter Y to proceed: 
if %Increment%==Y tools\setcsprojver .. /i:fv,r /env:consoletoolsversion

pause