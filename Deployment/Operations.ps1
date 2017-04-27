<#
.SYNOPSIS
Enables Log Analytics for remaining resources.

.DESCRIPTION
Enables Log Analytics for website.

.PARAMETER OMSWorkspaceName
Name of the the OMS Workspace.

.PARAMETER OperationsResourceGroupName
Name of the Resource Group where the OMS Workspace is.

.PARAMETER APIResourceGroupName
Name of the Resource Group where the app is.

.PARAMETER DiagnosticsStorageAccountName
Name of the Storage Account which will be used to store diaganostics from selected resources.

.PARAMETER AzureDiagnosticsAndLogAnalyticsModulePath
Location of the AzureDiagnosticsAndLogAnalytics powershell module.

.NOTES
This script is for use as a part of deployment in VSTS only.
#>

Param(    
    [Parameter(Mandatory=$true)] [string] $OMSWorkspaceName,
    [Parameter(Mandatory=$true)] [string] $OperationsResourceGroupName,
    [Parameter(Mandatory=$true)] [string] $APIResourceGroupName,
    [Parameter(Mandatory=$true)] [string] $DiagnosticsStorageAccountName,
    [Parameter(Mandatory=$true)] [string] $AzureDiagnosticsAndLogAnalyticsModulePath
)
$ErrorActionPreference = "Stop"

function Log([Parameter(Mandatory=$true)][string]$LogText){
    Write-Host ("{0} - {1}" -f (Get-Date -Format "HH:mm:ss.fff"), $LogText)
}

function Set-Diagnostics(
    [Parameter(Mandatory=$true)][array]$Resources,
    [Parameter(Mandatory=$true)][PSCustomObject]$Workspace,
    [Parameter(Mandatory=$true)][string]$StorageAccountId){
    foreach($resource in $Resources){
        Log "Enabling diagnostics for $($resource.ResourceName)"
        Set-AzureRmDiagnosticSetting -ResourceId $resource.ResourceId -StorageAccountId $StorageAccountId -Enabled $true -RetentionEnabled $true -RetentionInDays 30
        Log "Adding diagnostics for $($resource.ResourceName)"
        Add-AzureDiagnosticsToLogAnalytics -ResourceForLogs $resource -WorkspaceResource $Workspace -ErrorAction SilentlyContinue -ErrorVariable noDiagnostics
        if ($noDiagnostics){
            Log "Failed updating Log Analytics connection for $($resource.ResourceName)"
            Log $noDiagnostics
            $noDiagnostics=$null
        }
    }
}

Log "Importing module from $AzureDiagnosticsAndLogAnalyticsModulePath"
Import-Module $AzureDiagnosticsAndLogAnalyticsModulePath

Log "Getting Storage Account"
$storageAccount=Get-AzureRmStorageAccount -ResourceGroupName $OperationsResourceGroupName -Name $DiagnosticsStorageAccountName

Log "Getting OMS workspace resource"
$workspaceResource=Find-AzureRmResource -ResourceType "Microsoft.OperationalInsights/Workspaces" -ResourceNameContains $OMSWorkspaceName

$resourceTypes=@("Microsoft.Web/Sites")

foreach($resourceType in $resourceTypes){
    Log "Getting all $($resourceType)"
    $resources=Find-AzureRmResource -ResourceType $resourceType | Where-Object ResourceGroupName -EQ $APIResourceGroupName
    if ($resources -eq $null) {
        Log "No resources found";
    }
    else {
        Log "Creating diagnostics"
        Set-Diagnostics -Resources $resources -Workspace $workspaceResource -StorageAccountId $storageAccount.Id
    }
}

Log "Job wel done!"