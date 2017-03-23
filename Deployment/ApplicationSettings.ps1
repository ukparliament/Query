<#
.SYNOPSIS
Sets instrumentation key and connection string for use by API app.

.DESCRIPTION
Retrieves value of Application Insights' Instrumentation Key to and adds it to application settings of API app.
It also adds connection string (SparqlEndpoint).

.PARAMETER APIResourceGroupName
Name of the Resource Group where the API app is.

.PARAMETER ApplicationInsightsName
name of the Application Insights.

.NOTES
This script is for use as a part of deployment in VSTS only.
#>

Param(
    [Parameter(Mandatory=$true)] [string] $APIResourceGroupName,
    [Parameter(Mandatory=$true)] [string] $FixedQueryAPIName,
    [Parameter(Mandatory=$true)] [string] $ApplicationInsightsName,
    [Parameter(Mandatory=$true)] [string] $SparqlEndpoint
)
$ErrorActionPreference = "Stop"

function Log([Parameter(Mandatory=$true)][string]$LogText){
    Write-Host ("{0} - {1}" -f (Get-Date -Format "HH:mm:ss.fff"), $LogText)
}

Log "Getting Instrumentation Key"
$properties=Get-AzureRmResource -ResourceGroupName $APIResourceGroupName -ResourceName $ApplicationInsightsName -ExpandProperties | Select-Object Properties -ExpandProperty Properties

Log "Gets current settings"
$webApp = Get-AzureRmwebApp -ResourceGroupName $APIResourceGroupName -Name $FixedQueryAPIName
$webAppSettings = $webApp.SiteConfig.AppSettings
$settings=@{}
foreach($set in $webAppSettings){ 
    $settings[$set.Name]=$set.Value
}

Log "Sets new settings values"
$settings["ApplicationInsightsInstrumentationKey"]=$properties.InstrumentationKey
$settings["SparqlEndpoint"]=$SparqlEndpoint
Set-AzureRmWebApp -ResourceGroupName $APIResourceGroupName -Name $FixedQueryAPIName -AppSettings $settings

Log "Job wel done!"