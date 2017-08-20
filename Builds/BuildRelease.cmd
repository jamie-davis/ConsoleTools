call "c:\Program Files (x86)\Microsoft Visual Studio 15.0\Common7\Tools\VSVARS32.bat"

msbuild ..\ConsoleToolkit.sln /t:clean
msbuild ..\ConsoleToolkit.sln /t:Rebuild /p:Configuration=Release

if NOT EXIST "ConsoleToolkit %consoletoolsversion%" md "ConsoleToolkit %consoletoolsversion%"

copy ..\ConsoleToolKit\bin\release\*.dll "ConsoleToolkit %consoletoolsversion%"
copy ..\ConsoleToolKit\bin\release\*.xml "ConsoleToolkit %consoletoolsversion%"
copy ..\ConsoleToolKit\bin\release\*.dll "ConsoleToolkit_current"
copy ..\ConsoleToolKit\bin\release\*.xml "ConsoleToolkit_current"

.nuget\nuget pack ..\ConsoleToolkit\ConsoleToolkit.csproj -outputdirectory "ConsoleToolkit_current" -IncludeReferencedProjects -Prop Configuration=Release

copy "ConsoleToolkit_current\*.nupkg" "ConsoleToolkit %consoletoolsversion%"

git\git add "ConsoleToolkit %consoletoolsversion%\*.dll" -f
git\git add "ConsoleToolkit %consoletoolsversion%\*.xml" -f
git\git add "ConsoleToolkit %consoletoolsversion%\*.nupkg" -f
git\git add "ConsoleToolkit_current\*.dll" -f
git\git add "ConsoleToolkit_current\*.xml" -f
git\git add "ConsoleToolkit_current\*.nupkg" -f
pause