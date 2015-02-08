call "c:\Program Files (x86)\Microsoft Visual Studio 12.0\Common7\Tools\VSVARS32.bat"

msbuild ..\ConsoleToolkit.sln /t:clean
msbuild ..\ConsoleToolkit.sln /t:Rebuild /p:Configuration=Release

if NOT EXIST "ConsoleToolkit %consoletoolsversion%" md "ConsoleToolkit %consoletoolsversion%"

copy ..\ConsoleToolKit\bin\release\*.dll "ConsoleToolkit %consoletoolsversion%"
copy ..\ConsoleToolKit\bin\release\*.dll "ConsoleToolkit_current"

git\git add "ConsoleToolkit %consoletoolsversion%\*.dll" -f
pause