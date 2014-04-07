rem call "c:\Program Files (x86)\Microsoft Visual Studio 12.0\Common7\Tools\VSVARS32.bat"

rem msbuild ..\ConsoleToolkit.sln /t:clean
rem msbuild ..\ConsoleToolkit.sln /t:Rebuild /p:Configuration=Release
rem copy ..\ConsoleToolKit\bin\release\*.dll "ConsoleToolkit 1.0.0.0"
copy ..\ConsoleToolKit\bin\release\*.dll "ConsoleToolkit_current"
git\git add .
pause