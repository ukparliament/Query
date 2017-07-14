<
.SYNOPSI
Get settings for API app

.DESCRIPTIO
Retrieves value of Subscription Key and IP of API Management and sets task variables

.PARAMETER APIResourceGroupNam
Name of the Resource Group where the API Management is

.NOTE
This script is for use as a part of deployment in VSTS only
#

Param
	[Parameter(Mandatory=$true)] [string] $APIResourceGroupNam

$ErrorActionPreference = "Stop

function Log([Parameter(Mandatory=$true)][string]$LogText)
    Write-Host ("{0} - {1}" -f (Get-Date -Format "HH:mm:ss.fff"), $LogText


Log "Get API Management
$apiManagement=Get-AzureRmApiManagement -ResourceGroupName $APIResourceGroupNam
Log "Get API Management context
$management=New-AzureRmApiManagementContext -ResourceGroupName $APIResourceGroupName -ServiceName $apiManagement.Nam
Log "Retrives subscription
$apiProduct=Get-AzureRmApiManagementProduct -Context $management -Title "Parliament - Fixed Query
$subscription=Get-AzureRmApiManagementSubscription -Context $management -ProductId $apiProduct.ProductI

Log "Setting variables to use during deployment
Write-Host "##vso[task.setvariable variable=SubscriptionKeyFixedQuery]$($subscription.PrimaryKey)
Write-Host "##vso[task.setvariable variable=APIManagementIP]$($apiManagement.StaticIPs[0])

Log "Job well done!"