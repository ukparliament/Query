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
    [Parameter(Mandatory=$true)] [string] $FixedQueryAPIName
)

$ErrorActionPreference = "Stop"

$productTitle="Parliament -Fixed Query API"

function Log([Parameter(Mandatory=$true)][string]$LogText){
    Write-Host ("{0} - {1}" -f (Get-Date -Format "HH:mm:ss.fff"), $LogText)
}

Log "Retrives API Management"
$apiManagement=Get-AzureRmApiManagement -ResourceGroupName $ClusterResourceGroupName

Log "Get API Management context"
$management=New-AzureRmApiManagementContext -ResourceGroupName $ClusterResourceGroupName -ServiceName $apiManagement.Name

Log "Check if product already installed"
$productFixedQuery=(Get-AzureRmApiManagementProduct -Context $management | Where-Object Title -Match $productTitle)
if ($productFixedQuery -eq $null) {
    Log "Access for Fixed Query API"
    $productFixedQuery=New-AzureRmApiManagementProduct -Context $management -Title $productTitle -Description "For parliament use only." -ApprovalRequired $true -SubscriptionsLimit 1
    $productWeb=New-AzureRmApiManagementProduct -Context $management -Title "Parliament - website" -Description "For parliament use only." -ApprovalRequired $true -SubscriptionsLimit 1

    $api=New-AzureRmApiManagementApi -Context $management -Name "Fixed Query" -Description "All routes on Fixed Query API" -ServiceUrl "https://$FixedQueryAPIName.azurewebsites.net/" -Protocols @("https") -Path "/fixedquery"
    New-AzureRmApiManagementOperation -Context $management -ApiId $api.ApiId -Name "Fixed Query (catch all)" -Method "GET" -UrlTemplate "/*"
    Add-AzureRmApiManagementApiToProduct -Context $management -ProductId $productFixedQuery.ProductId -ApiId $api.ApiId
    Add-AzureRmApiManagementApiToProduct -Context $management -ProductId $productWeb.ProductId -ApiId $api.ApiId
    Log "Add sparql endpoint api to Product ($productTitle)"
    $api=Get-AzureRmApiManagementApi -Context $management | Where-Object Path -EQ "data"
    Add-AzureRmApiManagementApiToProduct -Context $management -ProductId $productFixedQuery.ProductId -ApiId $api.ApiId
    Add-AzureRmApiManagementApiToProduct -Context $management -ProductId $productWeb.ProductId -ApiId $api.ApiId
}

Log "Retrives subscription"
$subscription=Get-AzureRmApiManagementSubscription -Context $management -ProductId $productFixedQuery.ProductId

Log "Setting variables to use during deployment"
Write-Host "##vso[task.setvariable variable=SubscriptionKey]$($subscription.PrimaryKey)"
Write-Host "##vso[task.setvariable variable=APIManagementIP]$($apiManagement.StaticIPs[0])"

Log "Job well done!"