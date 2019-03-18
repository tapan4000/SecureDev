$Details = 

Test-AzureRmResourceGroupDeployment -ResourceGroupName "cmpclsrgstg" -TemplateFile .\CmpClsArmTemplate.json -TemplateParameterFile .\CmpClsArmParameters.json

Write-Output $Details.Details


Login-AzureRmAccount
New-AzureRmResourceGroupDeployment -Name "SampleDeployment" -ResourceGroupName "cmpclsrgstg" -TemplateFile .\CmpClsArmTemplate.json -TemplateParameterFile .\CmpClsArmParameters.json


Microsoft.Azure.ServiceFabric



Get-AzureRmVMExtensionImage -Location "East US" -PublisherName "Microsoft.Azure.ServiceFabric" -Type "ServiceFabricNode"

Get-AzureRmSubscription

Connect-ServiceFabricCluster -ConnectionEndpoint cmpclsstgntfrontend.eastus.cloudapp.azure.com:19000 -KeepAliveIntervalInSec 10 -AzureActiveDirectory -ServerCertThumbprint "7D6496BFE961B2AB444F5B267FB86DA413CD4803"


$clusterFQDN = "cmpclsstgntfrontend.eastus.cloudapp.azure.com"
#$clusterFQDN = "testcls.eastus.cloudapp.azure.com"
$clusterEndpoint = $clusterFQDN+':19000'
$certThumbprint = (Get-ChildItem -Path Cert:\LocalMachine\My | where {$_.Thumbprint -like "*7D6496BFE961B2AB444F5B267FB86DA413CD4803*" }).Thumbprint
Write-Output $certThumbprint
Connect-ServiceFabricCluster -ConnectionEndpoint $clusterEndpoint -KeepAliveIntervalInSec 10 -X509Credential -ServerCertThumbprint $certThumbprint -FindType FindByThumbprint -FindValue $certThumbprint -StoreLocation LocalMachine -StoreName My

Get-ServiceFabricClusterHealth

Set-ExecutionPolicy -ExecutionPolicy Unrestricted -Force -Scope CurrentUser

Connect-ServiceFabricCluster

Get-ServiceFabricClusterHealth