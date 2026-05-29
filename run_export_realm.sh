#!/bin/bash
set -e

CONTAINER_NAME="chess_keycloak_production"
EXPORT_DIR_LOCAL="./.keycloak-export"
EXPORT_DIR_CONTAINER="/tmp/kc-export"

mkdir -p "$EXPORT_DIR_LOCAL"

docker exec "$CONTAINER_NAME" sh -c "rm -rf $EXPORT_DIR_CONTAINER && mkdir -p $EXPORT_DIR_CONTAINER"

docker exec "$CONTAINER_NAME" /opt/keycloak/bin/kc.sh export --dir "$EXPORT_DIR_CONTAINER" --realm chess --users realm_file

if ! docker exec "$CONTAINER_NAME" test -f "$EXPORT_DIR_CONTAINER/chess-realm.json"; then
    echo "Plik nie powstal - lokalny folder zachowany" >&2
    exit 1
fi

rm -rf "$EXPORT_DIR_LOCAL"/* 2>/dev/null || true
docker cp "${CONTAINER_NAME}:${EXPORT_DIR_CONTAINER}/." "$EXPORT_DIR_LOCAL"

jq '
    del(.components."org.keycloak.keys.KeyProvider")
    | (if .users then .users |= map(select(.serviceAccountClientId)) else . end)
    | (if .clients then .clients |= map(if .clientId == "chess-backend-admin" then .secret = "__KC_ADMIN_CLIENT_SECRET__" else . end) else . end)
' "$EXPORT_DIR_LOCAL/chess-realm.json" > "$EXPORT_DIR_LOCAL/chess-realm.template.json"

echo "Template Ok"
