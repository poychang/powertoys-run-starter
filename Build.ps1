$version = Get-Date -Format "yyyy-MM-dd"

foreach ($platform in "ARM64", "x64")
{
    if (Test-Path -Path "$PSScriptRoot\Community.PowerToys.Run.Plugin.Starter\bin")
    {
        Remove-Item -Path "$PSScriptRoot\Community.PowerToys.Run.Plugin.Starter\bin\*" -Recurse
    }

    dotnet build $PSScriptRoot\Community.PowerToys.Run.Plugin.Starter.sln -c Release /p:Platform=$platform

    Remove-Item -Path "$PSScriptRoot\Community.PowerToys.Run.Plugin.Starter\bin\*" -Recurse -Include *.xml, *.pdb, PowerToys.*, Wox.*
    Rename-Item -Path "$PSScriptRoot\Community.PowerToys.Run.Plugin.Starter\bin\$platform\Release" -NewName "Starter"

    Compress-Archive -Path "$PSScriptRoot\Community.PowerToys.Run.Plugin.Starter\bin\$platform\Starter" -DestinationPath "$PSScriptRoot\Starter-$version-$platform.zip"
}
