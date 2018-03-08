@echo Before first run, set the Nuget API key in your NuGet.config to whatever is found on your profile on nuget.org
@echo For config file locations, see https://docs.microsoft.com/en-us/nuget/consume-packages/configuring-nuget-behavior#config-file-locations-and-uses
@echo If you have nuget on your path, you can use nuget config -Set ApiKey ^<your-api-key^>

call CreateNuget.cmd

dotnet nuget push nuget_packages/*.nupkg --source https://nuget.org
