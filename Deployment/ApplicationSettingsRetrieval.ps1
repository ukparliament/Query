<#
.SYNOPSIS
Get settings for API app.

.DESCRIPTION
Retrieves value of Subscription Key and IP of API Management and sets task variables.

.PARAMETER APIResourceGroupName
Name of the Resource Group where the API Management is.

.NOTES
This script is for use as a part of deployment in VSTS only.
#>

Param(
	[Parameter(Mandatory=$true)] [string] $APIResourceGroupName,
	[Parameter(Mandatory=$true)] [string] $APIManagementName,
	[Parameter(Mandatory=$true)] [string] $APIPrefix
)
$ErrorActionPreference = "Stop"

function Log([Parameter(Mandatory=$true)][string]$LogText){
    Write-Host ("{0} - {1}" -f (Get-Date -Format "HH:mm:ss.fff"), $LogText)
}

Log "Get API Management context"
$management=New-AzureRmApiManagementContext -ResourceGroupName $APIResourceGroupName -ServiceName $APIManagementName
Log "Retrives subscription"
$apiProduct=Get-AzureRmApiManagementProduct -Context $management -Title "$APIPrefix - Parliament [Fixed Query]"
$subscription=Get-AzureRmApiManagementSubscription -Context $management -ProductId $apiProduct.ProductId

Log "Setting variables to use during deployment"
Write-Host "##vso[task.setvariable variable=SubscriptionKeyFixedQuery]$($subscription.PrimaryKey)"
Write-Host "##vso[task.setvariable variable=APIManagementIP]$($apiManagement.StaticIPs[0])"

Log "Job well done!"