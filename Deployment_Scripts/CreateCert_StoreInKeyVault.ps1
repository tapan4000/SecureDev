#param(
#    [string] [Parameter(Mandatory=$true)] $Password,
#    [string] [Parameter(Mandatory=$true)] $CertDNSName,
#    [string] [Parameter(Mandatory=$true)] $KeyVaultName,
#    [string] [Parameter(Mandatory=$true)] $KeyVaultSecretName
#)

$Password = "P@ssword"# | ConvertTo-SecureString -AsPlainText -Force 
$CertDNSName = "cmpclsstgclient.eastus.cloudapp.azure.com"
$KeyVaultName = "cmpkvstg"
$KeyVaultSecretName = "clusteraccesscertclient"
$CertOutputFolder = "D:\Misc\Business Idea\SecurityDev\Deployment_Scripts\Certificates"

Login-AzureRmAccount
Set-AzureRmKeyVaultAccessPolicy -VaultName $KeyVaultName -EnabledForDeployment

$SecurePassword = ConvertTo-SecureString -String $Password -AsPlainText -Force
$CertFileFullPath = $(Join-Path $CertOutputFolder "\$CertDNSName.pfx")

$NewCert = New-SelfSignedCertificate -CertStoreLocation Cert:\LocalMachine\My -DnsName $CertDNSName 
Export-PfxCertificate -FilePath $CertFileFullPath -Password $SecurePassword -Cert $NewCert

$Bytes = [System.IO.File]::ReadAllBytes($CertFileFullPath)
$Base64 = [System.Convert]::ToBase64String($Bytes)

$JSONBlob = @{
    data = $Base64
    dataType = 'pfx'
    password = $Password
} | ConvertTo-Json

$ContentBytes = [System.Text.Encoding]::UTF8.GetBytes($JSONBlob)
$Content = [System.Convert]::ToBase64String($ContentBytes)

$SecretValue = ConvertTo-SecureString -String $Content -AsPlainText -Force
$NewSecret = Set-AzureKeyVaultSecret -VaultName $KeyVaultName -Name $KeyVaultSecretName -SecretValue $SecretValue -Verbose

Write-Host
Write-Host "Source Vault Resource Id: "$(Get-AzureRmKeyVault -VaultName $KeyVaultName).ResourceId
Write-Host "Certificate URL : "$NewSecret.Id
Write-Host "Certificate Thumbprint : "$NewCert.Thumbprint