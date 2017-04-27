<#
.SYNOPSIS
Get instrumentation key for use by API app.

.DESCRIPTION
Retrieves value of Application Insights' Instrumentation Key and sets task variable.

.PARAMETER OperationsResourceGroupName
Name of the Resource Group where the Application Insights is.

.PARAMETER ApplicationInsightsName
Name of the Application Insights.

.NOTES
This script is for use as a part of deployment in VSTS only.
#>

Param(
    [Parameter(Mandatory=$true)] [string] $OperationsResourceGroupName,
    [Parameter(Mandatory=$true)] [string] $ApplicationInsightsName
)
$ErrorActionPreference = "Stop"

function Log([Parameter(Mandatory=$true)][string]$LogText){
    Write-Host ("{0} - {1}" -f (Get-Date -Format "HH:mm:ss.fff"), $LogText)
}

Log "Getting Instrumentation Key"
$properties=Get-AzureRmResource -ResourceGroupName $OperationsResourceGroupName -ResourceName $ApplicationInsightsName -ExpandProperties | Select-Object Properties -ExpandProperty Properties

Log "Setting variables to use during deployment"
Log "Instrumentation Key: $($properties.InstrumentationKey)"
Write-Host "##vso[task.setvariable variable=ApplicationInsightsInstrumentationKey]$($properties.InstrumentationKey)"

Log "Job wel done!"