ECHO Removing and re-creating directories
rmdir C:\GitHub\TinyLog\nuspec\content /S /Q
rmdir C:\GitHub\TinyLog\nuspec\lib /S /Q
mkdir C:\GitHub\TinyLog\nuspec\content
mkdir C:\GitHub\TinyLog\nuspec\lib\net45
ECHO Copying nuspec library assemblies
copy C:\GitHub\TinyLog\TinyLog\bin\release\TinyLog.dll C:\GitHub\TinyLog\nuspec\lib\net45 /Y
ECHO Creating nuget package
nuget.exe pack TinyLog-core.nuspec -NonInteractive
