param (
    [string]$branch,
    [string]$token
 )
 
$parsedSourceBranch = $branch | ConvertFrom-String -TemplateContent "refs/pull/{PR:pr}/merge"
$newBranch = "GitHubPullRequest/$($parsedSourceBranch.PR)"

$base64AuthInfo = [Convert]::ToBase64String([Text.Encoding]::ASCII.GetBytes(":$env:SYSTEM_ACCESSTOKEN"))

$base64AuthInfo

$uri = "$($env:SYSTEM_TEAMFOUNDATIONCOLLECTIONURI)$env:SYSTEM_TEAMPROJECTID/_apis/git/repositories/Query/pullrequests?api-version=4.1"

$body = @{targetRefName = "refs/heads/master"; sourceRefName = "refs/heads/$($newBranch)"; title = $newBranch} | ConvertTo-Json

Invoke-RestMethod -Uri $uri -Method Post -ContentType "application/json" -Headers @{Authorization = "Basic $base64AuthInfo"} -Body $body

