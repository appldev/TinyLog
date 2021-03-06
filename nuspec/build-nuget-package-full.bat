ECHO Removing and re-creating directories
rmdir C:\GitHub\TinyLog\nuspec\content /S /Q
rmdir C:\GitHub\TinyLog\nuspec\lib /S /Q
mkdir C:\GitHub\TinyLog\nuspec\content
mkdir C:\GitHub\TinyLog\nuspec\lib\net45
ECHO Copying nuspec library assemblies
copy C:\GitHub\TinyLog\TinyLog.Json\bin\release\TinyLog.Json.dll C:\GitHub\TinyLog\nuspec\lib\net45 /Y
copy C:\GitHub\TinyLog\TinyLog.MSMQ\bin\release\TinyLog.MSMQ.dll C:\GitHub\TinyLog\nuspec\lib\net45 /Y
copy C:\GitHub\TinyLog\TinyLog.Sql\bin\release\TinyLog.Sql.dll C:\GitHub\TinyLog\nuspec\lib\net45 /Y
ECHO Creating nuget package
nuget.exe pack TinyLog-full.nuspec -NonInteractive
