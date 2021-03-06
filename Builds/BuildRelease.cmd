msbuild ..\ConsoleToolkit.sln /t:clean
msbuild ..\ConsoleToolkit\ConsoleToolkit.csproj /t:Rebuild /p:Configuration=Release

if NOT EXIST "ConsoleToolkit %consoletoolsversion%" md "ConsoleToolkit %consoletoolsversion%"

copy ..\ConsoleToolKit\bin\release\*.dll "ConsoleToolkit %consoletoolsversion%"
copy ..\ConsoleToolKit\bin\release\*.xml "ConsoleToolkit %consoletoolsversion%"
copy ..\ConsoleToolKit\bin\release\*.dll "ConsoleToolkit_current"
copy ..\ConsoleToolKit\bin\release\*.xml "ConsoleToolkit_current"

copy "..\ConsoleToolkit\bin\Release\*.nupkg" "ConsoleToolkit %consoletoolsversion%"

git\git add "ConsoleToolkit %consoletoolsversion%\*.dll" -f
git\git add "ConsoleToolkit %consoletoolsversion%\*.xml" -f
git\git add "ConsoleToolkit %consoletoolsversion%\*.nupkg" -f
git\git add "ConsoleToolkit_current\*.dll" -f
git\git add "ConsoleToolkit_current\*.xml" -f
git\git add "ConsoleToolkit_current\*.nupkg" -f
pause