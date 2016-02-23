if not exist .\nuget_packages mkdir nuget_packages
if not exist .\distro mkdir distro
xcopy FriendlyDotNet\*.fs             distro\src\ /Y /Q /E
xcopy FriendlyDotNet\bin\Debug\*.dll  distro\lib\net45\ /Q
xcopy FsharpFriendlyDotNet.nuspec           distro\ /Q
cd distro

..\nuget.exe pack FSharpFriendlyDotNet.nuspec -symbols -Prop Platform=AnyCPU

xcopy *.nupkg ..\nuget_packages\ /Y /Q

pause
cd ..
rmdir distro  /s /q
