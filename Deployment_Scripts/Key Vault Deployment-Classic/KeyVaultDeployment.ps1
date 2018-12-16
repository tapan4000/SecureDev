Login-AzureRmAccount

$certificateFilePath = "D:\Misc\Business Idea\SecurityDev\Deployment_Scripts\Certificates\KeyVaultAccess\RestServerKeyVault.cer"
$certificate = New-Object System.Security.Cryptography.X509Certificates.X509Certificate2
$certificate.Import($certificateFilePath)
$rawCertificateData = $certificate.GetRawCertData()
$credential = [System.Convert]::ToBase64String($rawCertificateData)

$startDate = $certificate.GetEffectiveDateString()
$endDate = $certificate.GetExpirationDateString()

$adApplication = Get-AzureRmADApplication -DisplayName "RestServerKeyVaultApplication"
if($adApplication -eq $null){
    $adApplication = New-AzureRmADApplication -DisplayName "RestServerKeyVaultApplication" -HomePage "http://www.tapan_test_page.com" -IdentifierUris "http://www.tapan_test_page.com/RestServerKeyVaultApplication" -CertValue $credential -StartDate $certificate.NotBefore -EndDate $certificate.NotAfter
}

$servicePrincipal = Get-AzureRmADServicePrincipal -ApplicationId $adApplication.ApplicationId
if($servicePrincipal -eq $null){
    $servicePrincipal = New-AzureRmADServicePrincipal -ApplicationId $adApplication.ApplicationId
}

Set-AzureRmKeyVaultAccessPolicy -ResourceGroupName "cmprgstg" -VaultName "cmpkvstg" -ObjectId $servicePrincipal.Id -PermissionsToSecrets get,list,set,delete

$servicePrincipal.ApplicationId                    