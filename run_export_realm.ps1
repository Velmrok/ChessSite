$CONTAINER_NAME = "chess_keycloak_production"
$EXPORT_DIR_LOCAL = "./.keycloak-export"
$EXPORT_DIR_CONTAINER = "/tmp/kc-export"

if (-not (Test-Path $EXPORT_DIR_LOCAL)) {
    New-Item -ItemType Directory -Force -Path $EXPORT_DIR_LOCAL | Out-Null
}

docker exec $CONTAINER_NAME sh -c "rm -rf $EXPORT_DIR_CONTAINER && mkdir -p $EXPORT_DIR_CONTAINER"

docker exec $CONTAINER_NAME /opt/keycloak/bin/kc.sh export --dir $EXPORT_DIR_CONTAINER --realm chess --users realm_file

docker exec $CONTAINER_NAME test -f "$EXPORT_DIR_CONTAINER/chess-realm.json"
if ($LASTEXITCODE -ne 0) {
    Write-Error "Plik nie powstal - lokalny folder zachowany"
    exit 1
}

Remove-Item -Recurse -Force "$EXPORT_DIR_LOCAL/*" -ErrorAction SilentlyContinue
docker cp "${CONTAINER_NAME}:${EXPORT_DIR_CONTAINER}/." $EXPORT_DIR_LOCAL


$realm = Get-Content "$EXPORT_DIR_LOCAL/chess-realm.json" -Raw | ConvertFrom-Json

if ($realm.components -and $realm.components.'org.keycloak.keys.KeyProvider') {
    $realm.components.PSObject.Properties.Remove('org.keycloak.keys.KeyProvider')
}
if ($realm.users) {
    $realm.users = @($realm.users | Where-Object { $_.serviceAccountClientId })
}

foreach ($client in $realm.clients) {
    if ($client.clientId -eq 'chess-backend-admin') {
        $client.secret = '__KC_ADMIN_CLIENT_SECRET__'
    }
}

$out = $realm | ConvertTo-Json -Depth 100
[IO.File]::WriteAllText("$PWD/$EXPORT_DIR_LOCAL/chess-realm.template.json", $out)

Write-Host "Template Ok"