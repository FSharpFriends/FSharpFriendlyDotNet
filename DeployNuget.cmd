@echo Before first run, set the Nuget API key in your NuGet.config to whatever is found on your profile on nuget.org
@echo For config file locations, see https://docs.microsoft.com/en-us/nuget/consume-packages/configuring-nuget-behavior#config-file-locations-and-uses
@echo If you have nuget on your path, you can use nuget config -Set ApiKey ^<your-api-key^>

call CreateNuget.cmd

cd nuget_packages
dotnet nuget push *.symbols.nupkg --source https://nuget.org
dotnet nuget push *.nupkg --source https://nuget.org
cd ..