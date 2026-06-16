import { useState, useEffect } from "react";
import { authService } from "../services/authService";
import { useAuthActions } from "./useAuthActions";

export function useAuth() {
    const [loading, setLoading] = useState(true);
    const { applyAuth, clearAuth } = useAuthActions();

    useEffect(() => {
        (async () => {
            const me = await authService.getMe();
            if (me) applyAuth(me);
            else clearAuth();
            setLoading(false);
        })();
    }, []);

    return { loading };
}