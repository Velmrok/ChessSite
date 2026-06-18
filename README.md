# ChessSite 
## Autor: Maciej Spławiński grupa 3

## Diagram usług

```mermaid
flowchart LR
    U["Przeglądarka<br/>React SPA"] --> N["nginx<br/>reverse proxy"]
    N -->|"/api/*"| B["Backend<br/>ASP.NET Core 8<br/>"]
    N -->|"/realms/* (ekran logowania)"| K["Keycloak 26<br/>Authorization Server"]
    B -->|"wymiana kodu + PKCE<br/>(backchannel)"| K
    B --> R[("Redis<br/>cache + sesje")]
    B --> P[("PostgreSQL<br/>dane aplikacji")]
    B -->|"ruchy AI"| E["Silnik szachowy<br/>(Python)"]
    K --> KP[("PostgreSQL<br/>dane Keycloak")]
```
## Diagram przepływu logowania
```mermaid
sequenceDiagram
    participant U as Przeglądarka
    participant B as Backend 
    participant K as Keycloak
    U->>B: GET /api/auth/login
    B-->>U: 302 → Keycloak /auth (z code_challenge = PKCE)
    U->>K: logowanie (login + hasło)
    K-->>U: 302 → /api/signin-oidc?code=...
    U->>B: GET /api/signin-oidc?code=...
    B->>K: code + code_verifier → tokeny (kanał serwerowy)
    K-->>B: id / access / refresh token
    B-->>U: ustawia cookie sesji (HttpOnly), 302 → /
    U->>B: GET /api/... (z cookie)
    B-->>U: dane (użytkownik zalogowany)
```
## Opcja HTTPS
```
Aplikacja działa w pełni po http wewnętrznie.
Nginx posiada dwie konfiguracje w frontend/nginx dla http oraz https. W zmiennej środowiskowej
można zmienić wybraną konfiguracje a następnie użyć serwera reverse proxy obsługującego HTTPS.
```


## Jak uruchomić 

```bash
cp .env.example .env                 # hasła i sekrety
mkdir -p .secrets                    # 3 pliki (cf_* mogą być atrapą przy STORAGE_PROVIDER=Local)
echo "mojeHaslo"   > .secrets/db_password.txt
echo "x"           > .secrets/cf_access_key.txt
echo "x"           > .secrets/cf_secret_key.txt

docker compose -f  docker-compose.yml up --build -d           # start aplikacji
```

## Konto testowe (admin)

```
login: chessadmin    hasło: admin123    rola: admin
```

## Szybkie testy (curl)

```bash
curl -i http://localhost/api/health        # 200  – endpoint publiczny
curl -i http://localhost/api/auth/me        # 401  – chroniony, bez sesji
curl -i http://localhost/api/admin/users    # 401 403 200 bez sesji, bez roli, z sesja admina
```

## Przykładowe Endpointy

- **Publiczne:**
 `GET /api/health`, `GET /api/home/leaderboard`
- **Chronione (`[Authorize]`):** 
`GET /api/auth/me`, `GET /api/games`, całe `/api/users/*`
- **Na rolę (`[Authorize(Roles="admin")]`):** 
`GET /api/admin/users`, `GET /api/admin/stats`

## Deklaracja realmu keycloaka

```
keycloak/chess-realm.yaml
```



