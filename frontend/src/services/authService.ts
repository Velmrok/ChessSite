export const authService = {
    login: () => { window.location.href = "/api/auth/login"; },
    register: () => { window.location.href = "/api/auth/login"; }, 
    logout: () => { window.location.href = "/api/auth/logout"; },

    getMe: async () => {
        const res = await fetch("/api/auth/me", { credentials: "include" });
        if (!res.ok) return null;
        return res.json();
    },
};