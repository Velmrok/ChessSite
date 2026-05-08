import { UserManager, WebStorageStateStore } from "oidc-client-ts";

export const userManager = new UserManager({
    authority: import.meta.env.VITE_KEYCLOAK_AUTHORITY,
    client_id: "chess-frontend",
    redirect_uri: import.meta.env.VITE_KEYCLOAK_REDIRECT_URI,
    post_logout_redirect_uri: import.meta.env.VITE_KEYCLOAK_POST_LOGOUT_URI,
    response_type: "code",
    scope: "openid profile email",
    userStore: new WebStorageStateStore({ store: window.sessionStorage }),
});