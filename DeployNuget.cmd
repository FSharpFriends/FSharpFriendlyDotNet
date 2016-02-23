REM ON FIRST RUN, RUN THIS (change the key to whatever is found on your profile on www.nuget.org ->   
REM .nuget\NuGet.exe setapikey e39ea-get-the-full-key-on-nuget.org

call CreateNuget.cmd
NuGet.exe push nuget_packages\*.symbols.nupkg
NuGet.exe push nuget_packages\*.nupkg

pause 

rmdir nuget_packages  /s /q
