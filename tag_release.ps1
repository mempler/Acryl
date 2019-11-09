$TYPE = "DEV"
$LAST_TAG = git describe --tags --abbrev=0
$COMMIT_COUNT = git rev-list "$LAST_TAG..HEAD" --count
$DATE = Get-Date -Format "yyyy.ddMM"

Write-Output "Create Tag $TYPE-$DATE.$COMMIT_COUNT"

git tag "$TYPE-$DATE.$COMMIT_COUNT"
