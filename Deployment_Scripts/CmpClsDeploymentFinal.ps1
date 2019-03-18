Login-AzureRmAccount
Set-AzureRmKeyVaultAccessPolicy -VaultName 'cmpkvstg' -EnabledForDeployment

$resourceGroupName="cmpclsrgstg"
$parameterFilePath=".\CmpClsArmParameters.json"
$templateFilePath=".\CmpClsArmTemplate.json"
#$secretID="https://cmpkvstg.vault.azure.net:443/secrets/clusteraccesscert/3ebe4bcfcde3489c9fa2ec03114f3f76"

# New-AzureRmServiceFabricCluster -ResourceGroupName $resourceGroupName -SecretIdentifier $secretId -TemplateFile $templateFilePath -ParameterFile $parameterFilePath
Get-AzureRmResourceGroup -Name $resourceGroupName -ErrorVariable notPresent -ErrorAction SilentlyContinue
if($notPresent){
    Write-Output "Resource group does not exist"
    New-AzureRmResourceGroup -Name $resourceGroupName -Location 'East US'
}
else
{
    Write-Output "Resource group already exist"
}
#Test-AzureRmResourceGroupDeployment -ResourceGroupName $resourceGroupName -TemplateFile $templateFilePath -TemplateParameterFile $parameterFilePath
New-AzureRmResourceGroupDeployment -ResourceGroupName $resourceGroupName -TemplateFile $templateFilePath -TemplateParameterFile $parameterFilePath;


# API URL: http://cmpclsstgntfrontend.eastus.cloudapp.azure.com:8085/FrontEndService/api/anon/loginUser