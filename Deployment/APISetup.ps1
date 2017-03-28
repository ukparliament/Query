<#
.SYNOPSIS
Generates API.

.DESCRIPTION
Creates endpoints to access GraphDB.

.PARAMETER ClusterResourceGroupName
Name of the Resource Group where the cluster is.

.PARAMETER APIManagementName
Name of the API Management.

.PARAMETER FixedQueryAPIName
Name of the Fixed Query API.

.NOTES
This script is for use as a part of deployment in VSTS only.
#>

Param(
    [Parameter(Mandatory=$true)] [string] $ClusterResourceGroupName,
    [Parameter(Mandatory=$true)] [string] $APIManagementName,
    [Parameter(Mandatory=$true)] [string] $FixedQueryAPIName
)

$ErrorActionPreference = "Stop"

function Log([Parameter(Mandatory=$true)][string]$LogText){
    Write-Host ("{0} - {1}" -f (Get-Date -Format "HH:mm:ss.fff"), $LogText)
}

Log "Get API Management context"
$management=New-AzureRmApiManagementContext -ResourceGroupName $ClusterResourceGroupName -ServiceName $APIManagementName

Log "Access for Fixed Query API"
$productFixedQuery=New-AzureRmApiManagementProduct -Context $management -Title "Parliament - Fixed Query API" -Description "For parliament use only." -ApprovalRequired $true -SubscriptionsLimit 1
$api=New-AzureRmApiManagementApi -Context $management -Name "Fixed Query" -Description "All routes on Fixed Query API" -ServiceUrl "https://$FixedQueryAPIName.azurewebsites.net/" -Protocols @("https") -Path "/fixedquery"
New-AzureRmApiManagementOperation -Context $management -ApiId $api.ApiId -Name "Fixed Query (catch all)" -Method "GET" -UrlTemplate "/"
Add-AzureRmApiManagementApiToProduct -Context $management -ProductId $productFixedQuery.ProductId -ApiId $api.ApiId

Log "Retrives subscription"
$subscription=Get-AzureRmApiManagementSubscription -Context $management -ProductId $productFixedQuery.ProductId

Log "Retrives IP address"
$apiManagement=Get-AzureRmApiManagement -ResourceGroupName $ClusterResourceGroupName -Name $APIManagementName

Log "Setting variables to use during deployment"
Write-Host "##vso[task.setvariable variable=SubscriptionKey]$($subscription.PrimaryKey)"
Write-Host "##vso[task.setvariable variable=APIManagementIP]$($apiManagement.StaticIPs[0])"

Log "Job well done!"