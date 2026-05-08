import { useState, useEffect } from "react";
import { authService } from "../services/authService";
import { useAuthActions } from "./useAuthActions";

export function useAuth() {
    const [loading, setLoading] = useState(true);
    const { applyAuth, clearAuth } = useAuthActions();

    useEffect(() => {
        (async () => {
            if (window.location.pathname === "/auth/callback") {
                setLoading(false);
                return;
            }
            const isAuth = await authService.isAuthenticated();
            if (isAuth) {
                const me = await authService.getMe();
                if (me) applyAuth(me);
                else clearAuth();
            } else {
                clearAuth();
            }
            setLoading(false);
        })();
    }, []);

    return { loading };
}