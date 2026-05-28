# ChessSite — Docker

## Usługi

| Usługa | Obraz | Port (host) | Sieci |
|---|---|---|---|
| `db` | `postgres:15` | 5432 (dev) / — (prod) | backend-net (+db-net prod) |
| `backend` | dev: `dotnet/sdk:8.0` + `dotnet watch`; prod: `velmrok/chess-backend:v1` | 5062 (dev) / — (prod) | frontend-net, backend-net (+db-net prod) |
| `frontend` | dev: `node:20-alpine` + vite; prod: `velmrok/chess-frontend:v1` → nginx | 5173 (dev) / 80 (prod) | frontend-net |
| `redis` | `redis:7-alpine` | 6379 (dev) / — (prod) | backend-net |
| `keycloak` | `quay.io/keycloak/keycloak:26.0` | 8080 (dev)/  — (prod) | frontend-net, backend-net, keycloak-net |
| `keycloak-db` | `postgres:16` | — | keycloak-net |
| `local-chess-engine` | `velmrok/chess-engine:v1`  | 5000 (dev) / — (prod) | backend-net |

## Wolumeny

| Wolumen | Usługa | Ścieżka w kontenerze | Opis |
|---|---|---|---|
| `postgres_data*` | db | `/var/lib/postgresql/data` | Dane aplikacji (users, partie, ratingi) |
| `redis_data*` | redis | `/data` | Dane Redisa (cache, rate limit) |
| `keycloak_db_data*` | keycloak-db | `/var/lib/postgresql/data` | Dane Keycloaka (użytkownicy IdP, sesje, klienty) |
| `dotnet_packages` (dev) | backend | `/root/.nuget/packages` | Cache NuGet — szybszy restart kontenera |
| `data_protection_keys` (prod) | backend | `/root/.aspnet/DataProtection-Keys` | Klucze ASP.NET (cookies, antiforgery) |
| bind `./backend` (dev) | backend | `/app` | Kod backendu — `dotnet watch` |
| bind `./frontend` (dev) | frontend | `/app` | Kod frontendu — HMR vite |
| bind `./backend/wwwroot/uploads` (dev) | backend | `/app/wwwroot/uploads` | Lokalne avatary gdy `Storage=Local` |
| bind `./.keycloak-export` | keycloak | `/opt/keycloak/data/import` | Import realmu `chess` przy starcie |

Sekrety: `db_password` (dev+prod), `cf_access_key`, `cf_secret_key` (prod).

## Architektura

Dev:
```
         ┌─▶ frontend(react)
         ├─▶ backend ───┬─▶ db
         ├─▶ db         ├─▶ redis
Host ────┼─▶ redis      ├─▶ engine
         ├─▶ engine     └─▶ keycloak ─▶ keycloak-db
         └─▶ keycloak
```

Prod:
```
                                                                 ┌─▶ db
                                                                 ├─▶ redis
Browser ──▶ frontend(nginx) ─[reverse proxy]─┬─▶ backend ───────┼─▶ chess-engine
                                              └──────────────────└─▶ keycloak ─▶ keycloak-db
```

## Wymagania

- Docker Engine ≥ 24 + Compose V2.
- Wolne porty dev: 5173, 5062, 5432, 6379, 8080, 5000.
- `.env` w katalogu głównym wg [.env.example](.env.example).
- `.secrets/ w katalogu głównym wg .secrets.example/ (w dev tylko db_password)
- Produkcja wymaga domeny z HTTPS, która przekazuje ruch do aplikacji działającej lokalnie na HTTP.

## Uruchomienie

```bash
./run_dev_app.sh #/.ps1         # devV
./run_production_app.sh #/.ps1   # prod
```
## Weryfikacja
 - curl.exe https://chessite.tailf13eb6.ts.net/api/health HEALTH
 - curl.exe https://chessite.tailf13eb6.ts.net/api/items LIST ITEMS
 - curl.exe -X POST "https://chessite.tailf13eb6.ts.net/api/items" -H "Content-Type: application/json" -d '{\"name\":\"test\"}' ADD ITEM
 - curl.exe -i https://chessite.tailf13eb6.ts.net/api/users?search="" CACHE STATUS

 - curl https://chessite.tailf13eb6.ts.net/api/health HEALTH
 - curl https://chessite.tailf13eb6.ts.net/api/items  LIST ITEMS
 - curl -X POST "https://chessite.tailf13eb6.ts.net/api/items" -H "Content-Type: application/json" -d '{"name":"test"}' ADD ITEM
 - curl -i https://chessite.tailf13eb6.ts.net/api/users?search=""    CACHE STATUS

## Pliki

- [docker-compose.dev.yml](docker-compose.dev.yml), [docker-compose.yml](docker-compose.yml)
- [backend/Dockerfile](backend/Dockerfile), [frontend/Dockerfile](frontend/Dockerfile), [local.Chess.Engine/Dockerfile](local.Chess.Engine/Dockerfile)
- [frontend/vite.config.ts](frontend/vite.config.ts), [frontend/nginx/default.conf](frontend/nginx/default.conf)
- [.env.example](.env.example)
- [run_dev_app.sh](run_dev_app.sh), [run_production_app.sh](run_production_app.sh), [run_export_realm.sh](run_export_realm.sh)
