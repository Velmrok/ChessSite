$envFile = ".env"
if (-not (Test-Path $envFile)) { Write-Error ".env not found"; exit 1 }

$envMap = @{}
Get-Content $envFile | ForEach-Object {
    if ($_ -match '^([^#=]+)=(.*)$') { $envMap[$matches[1].Trim()] = $matches[2].Trim() }
}

$required = @("KC_ADMIN_CLIENT_SECRET")
foreach ($k in $required) {
    if (-not $envMap.ContainsKey($k) -or -not $envMap[$k]) {
        Write-Error "Missing $k in .env"; exit 1
    }
}

$template = Get-Content ".keycloak-export/chess-realm.template.json" -Raw
$result = $template -replace '__KC_ADMIN_CLIENT_SECRET__', $envMap["KC_ADMIN_CLIENT_SECRET"]
[IO.File]::WriteAllText("$PWD/.keycloak-export/chess-realm.json", $result)

Write-Host "chess-realm.json generated"