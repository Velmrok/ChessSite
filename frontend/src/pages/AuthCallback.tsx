// src/pages/AuthCallback.tsx
import { useEffect } from "react";
import { useNavigate } from "react-router-dom";
import { authService } from "../services/authService";
import { useAuthActions } from "../hooks/useAuthActions";
import { reconnectSignalR } from "@/services/signalR/connection";

export default function AuthCallback() {
    const navigate = useNavigate();
    const { applyAuth } = useAuthActions();

    useEffect(() => {
        authService.handleCallback().then(async () => {
            const me = await authService.getMe();
            if (me) applyAuth(me);
            await reconnectSignalR();
            navigate("/");
            
        });
    }, []);

    return <p>Logowanie...</p>;
}