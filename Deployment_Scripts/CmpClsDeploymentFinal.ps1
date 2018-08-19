Login-AzureRmAccount
Set-AzureRmKeyVaultAccessPolicy -VaultName 'cmpkvstg' -EnabledForDeployment

$resourceGroupName="cmprgstage"
$parameterFilePath="D:\Misc\Business Idea\SecurityDev\Deployment_Scripts\CmpClsArmParameters.json"
$templateFilePath="D:\Misc\Business Idea\SecurityDev\Deployment_Scripts\CmpClsArmTemplate.json"
$secretID="https://cmpkvstg.vault.azure.net:443/secrets/clusteraccesscert/3ebe4bcfcde3489c9fa2ec03114f3f76"

New-AzureRmServiceFabricCluster -ResourceGroupName $resourceGroupName -SecretIdentifier $secretId -TemplateFile $templateFilePath -ParameterFile $parameterFilePath