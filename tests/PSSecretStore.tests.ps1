Import-Module "$PSScriptRoot/../output/PSSecretStore.psd1"

Describe "PSSecretStore" {
    Context "Export-SSKey" {
        It "exports a key file" {
            $Tempfile = [IO.Path]::GetTempFileName()

            Export-SSKey -KeyPath $tempFile

            Get-ChildItem $tempFile | should not be $null
        }
    }

    Context "Set-SSSecret" {
        It "sets a secret by password" {
            $Store = [IO.Path]::GetTempFileName()

            Set-SSSecret -Name "Secret" -Value "This is a secret" -Password "Password" -StorePath $Store

            Get-SSSecret -Name 'Secret' -StorePath $Store -Password "Password" | Should be "This is a secret"
        }
        It "appends existing files with new secrets" {
            $Store = [IO.Path]::GetTempFileName()

            Set-SSSecret -Name "Secret" -Value "This is a secret" -Password "Password" -StorePath $Store

            Set-SSSecret -Name "UltraSecret" -Value "This is secret 2" -Password "Password" -StorePath $Store

            Get-SSSecret -Name 'Secret' -StorePath $Store -Password "Password" | Should be "This is a secret"
            Get-SSSecret -Name "UltraSecret" -StorePath $Store -Password "Password" | Should be "This is secret 2"
        }

        

        It "sets a secret by key file" {
            $Store = [IO.Path]::GetTempFileName()
            $KeyFile = [IO.Path]::GetTempFileName()

            Export-SSKey -KeyPath $KeyFile

            Set-SSSecret -Name "Secret" -Value "This is a secret" -KeyPath $KeyFile -StorePath $Store
            Set-SSSecret -Name "Secret2" -Value "This is a secret" -KeyPath $KeyFile -StorePath $Store

            Get-SSSecret -Name 'Secret' -StorePath $Store -KeyPath $KeyFile | Should be "This is a secret"
            Get-SSSecret -Name 'Secret2' -StorePath $Store -KeyPath $KeyFile | Should be "This is a secret"
        }
    }
    context "Get-SSSecret" {
        It "Gets all the secrets" {
            $Store = [IO.Path]::GetTempFileName()

            Set-SSSecret -Name "Secret" -Value "This is a secret" -Password "Password" -StorePath $Store

            Set-SSSecret -Name "UltraSecret" -Value "This is secret 2" -Password "Password" -StorePath $Store

            (Get-SSSecret -All -StorePath $Store -Password "Password").count | Should be 2
        }
    }
}
