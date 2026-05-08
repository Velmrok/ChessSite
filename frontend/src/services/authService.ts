import { userManager } from "./auth/oidc";

export const authService = {
    login: () => userManager.signinRedirect(),
    register: () => userManager.signinRedirect({ extraQueryParams: { action: "register" } }),


    handleCallback: () => userManager.signinRedirectCallback(),

    logout: async () => {
        const user = await userManager.getUser();
        await userManager.signoutRedirect({
            id_token_hint: user?.id_token,
        });
        await userManager.removeUser();
    },

    getToken: async (): Promise<string | null> => {
    const user = await userManager.getUser();
    if (!user) return null;
    if (user.expired) {
        try {
            const refreshed = await userManager.signinSilent();
            return refreshed?.access_token ?? null;
        } catch {
            return null;
        }
    }
    return user.access_token;
},

    isAuthenticated: async (): Promise<boolean> => {
        const user = await userManager.getUser();
        return user != null && !user.expired;
    },

    getMe: async () => {
        const token = await authService.getToken();
        if (!token) return null;
        const res = await fetch("/api/auth/me", {
            headers: { Authorization: `Bearer ${token}` },
        });
        if (!res.ok) return null;
        return res.json();
    },
};