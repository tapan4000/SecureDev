#param (
#[string]$ApplicationPackagePath = $(throw "-ApplicationPackagePath is required.")
#)

Login-AzureRmAccount

$ApplicationPackagePath="D:\Misc\Business Idea\SecurityDev\RestServer\RestServer\pkg\Debug"
$ClusterEndpoint = "cmpclsstg.eastus.cloudapp.azure.com:19000"
$ClusterAccessCertificateThumbprint = "73FABD7F2ACAD7AD16A2F7CAA92FA2C44DADF852"
$ApplicationPackagePathInImageStore = "RestServer_V100"
$ApplicationTypeName = "RestServerType"
$ApplicationTypeVersion = "1.0.0"
$ApplicationName = "fabric:/RestServer"

# 1. Connect to cluster
Connect-ServiceFabricCluster -ConnectionEndpoint $ClusterEndpoint -FindType FindByThumbprint -Findvalue $ClusterAccessCertificateThumbprint -X509Credential -ServerCertThumbprint $ClusterAccessCertificateThumbprint -StoreLocation LocalMachine -StoreName My

# 2. Remove existing deployment
$Application = Get-ServiceFabricApplication -ApplicationName $ApplicationName
if ($Application)
{
    Write-Host "`nRemoving application '$ApplicationName'" -ForegroundColor Green
    Remove-ServiceFabricApplication -ApplicationName $ApplicationName -TimeoutSec 600 -Force
}

$ApplicationType = Get-ServiceFabricApplicationType -ApplicationTypeName $ApplicationTypeName
if ($ApplicationType)
{
    Write-Host "`nUnregistering application type '$ApplicationTypeName' and version '$ApplicationTypeVersion'" -ForegroundColor Green
    Unregister-ServiceFabricApplicationType -ApplicationTypeName $ApplicationType.ApplicationTypeName $ApplicationType.ApplicationTypeVersion -TimeoutSec 600 -Force

    Write-Host "`nRemoving application package '$ApplicationPackagePathInImageStore'" -ForegroundColor Green
    Remove-ServiceFabricApplicationPackage -ImageStoreConnectionString fabric:ImageStore -ApplicationPackagePathInImageStore $ApplicationPackagePathInImageStore -TimeoutSec 600
}

# 3. Deploy new application and services
Write-Host "`nDelpoying application and services for application type: '$ApplicationTypeName' | application version: '$ApplicationTypeVersion'" -ForegroundColor Green

Copy-ServiceFabricApplicationPackage  -ApplicationPackagePath $ApplicationPackagePath -ImageStoreConnectionString fabric:ImageStore -ApplicationPackagePathInImageStore $ApplicationPackagePathInImageStore
Register-ServiceFabricApplicationType -ApplicationPathInImageStore $ApplicationPackagePathInImageStore -TimeoutSec 600
New-ServiceFabricApplication -ApplicationName $ApplicationName -ApplicationTypeName $ApplicationTypeName -ApplicationTypeVersion $ApplicationTypeVersion -TimeoutSec 600 
New-ServiceFabricService -ApplicationName $ApplicationName -ServiceName "$ApplicationName/FrontEndService" -ServiceTypeName "FrontEndServiceType" -Stateless -PartitionSchemeSingleton -InstanceCount -1 -PlacementConstraint "(nodeType==cmppri)"

Write-Host "`nDeploy application and services succeeded"