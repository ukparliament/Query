param (
    [string]$branch,
    [string]$token
 )

$parsedSourceBranch = $branch | ConvertFrom-String -TemplateContent "refs/pull/{PR:pr}/merge"
$newBranch = "GitHubPullRequest/$($parsedSourceBranch.PR)"

$base64AuthInfo = [Convert]::ToBase64String([Text.Encoding]::ASCII.GetBytes(":$($token)"))
$uri = "https://data-parliament.visualstudio.com/Platform/_apis/git/repositories/Query/pullrequests?api-version=4.1
$body = @{targetRefName = "refs/heads/master"; sourceRefName = "refs/heads/$($newBranch)"; title = $newBranch} | ConvertTo-Json
$result = Invoke-RestMethod -Uri $uri -Method Post -ContentType "application/json" -Headers @{Authorization = "Basic $($base64AuthInfo)"} -Body $body
