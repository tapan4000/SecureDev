#param (
#[string]$ApplicationPackagePath = $(throw "-ApplicationPackagePath is required.")
#)

$ApplicationPackagePath="D:\temp\deploy"
$ClusterEndpoint = "localhost:19000"
$ApplicationPackagePathInImageStore = "RestServer_V101"
$ApplicationTypeName = "RestServerType"
$ApplicationTypeVersion = "1.0.0"
$ApplicationName = "fabric:/RestServer"

# 1. Connect to cluster
Connect-ServiceFabricCluster -ConnectionEndpoint $ClusterEndpoint

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
    Remove-ServiceFabricApplicationPackage -ApplicationPackagePathInImageStore $ApplicationPackagePathInImageStore -TimeoutSec 600
}

# 3. Deploy new application and services
Write-Host "`nDelpoying application and services for application type: '$ApplicationTypeName' | application version: '$ApplicationTypeVersion'" -ForegroundColor Green

Copy-ServiceFabricApplicationPackage  -ApplicationPackagePath $ApplicationPackagePath -ApplicationPackagePathInImageStore $ApplicationPackagePathInImageStore
Register-ServiceFabricApplicationType -ApplicationPathInImageStore $ApplicationPackagePathInImageStore -TimeoutSec 600
New-ServiceFabricApplication -ApplicationName $ApplicationName -ApplicationTypeName $ApplicationTypeName -ApplicationTypeVersion $ApplicationTypeVersion -TimeoutSec 600 
New-ServiceFabricService -ApplicationName $ApplicationName -ServiceName "$ApplicationName/FrontEndService" -ServiceTypeName "FrontEndServiceType" -Stateless -PartitionSchemeSingleton -InstanceCount -1 -PlacementConstraint "(nodeType==cmppri)"

Write-Host "`nDeploy application and services succeeded"