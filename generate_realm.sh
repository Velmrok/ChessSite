#!/bin/bash
set -e

ENV_FILE=".env"
if [ ! -f "$ENV_FILE" ]; then
    echo ".env not found" >&2
    exit 1
fi

declare -A envMap
while IFS= read -r line || [ -n "$line" ]; do
    [[ "$line" =~ ^([^#=]+)=(.*)$ ]] || continue
    key="${BASH_REMATCH[1]}"
    value="${BASH_REMATCH[2]}"
    key="${key#"${key%%[![:space:]]*}"}"
    key="${key%"${key##*[![:space:]]}"}"
    value="${value#"${value%%[![:space:]]*}"}"
    value="${value%"${value##*[![:space:]]}"}"
    envMap["$key"]="$value"
done < "$ENV_FILE"

required=("KC_ADMIN_CLIENT_SECRET")
for k in "${required[@]}"; do
    if [ -z "${envMap[$k]:-}" ]; then
        echo "Missing $k in .env" >&2
        exit 1
    fi
done

template=$(<".keycloak-export/chess-realm.template.json")
printf '%s' "${template//__KC_ADMIN_CLIENT_SECRET__/${envMap[KC_ADMIN_CLIENT_SECRET]}}" > ".keycloak-export/chess-realm.json"

echo "chess-realm.json generated"
