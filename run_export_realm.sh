#!/bin/bash

CONTAINER_NAME="chess_keycloak_production"
EXPORT_DIR="./.keycloak-export"
EXPORT_FILE="chess-realm.json"

docker exec "$CONTAINER_NAME" /opt/keycloak/bin/kc.sh export --file /tmp/"$EXPORT_FILE"

docker cp "$CONTAINER_NAME":/tmp/"$EXPORT_FILE" "$EXPORT_DIR"/"$EXPORT_FILE"
