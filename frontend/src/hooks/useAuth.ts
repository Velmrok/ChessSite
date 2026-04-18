import useUserStore from "@/stores/useUserStore";
import { useApi } from "./useApi";
import { useEffect, useState } from "react";
import { connectSocket, disconnectSocket } from "@/services/socket/socketService";
import { getMe, refresh } from "@/services/authService";


export function useAuth() {
    const user = useUserStore((state) => state.user);
    const setUser = useUserStore((state) => state.setUser);
    const { request } = useApi();
    const [loading, setLoading] = useState(true);

    useEffect(() => {
        const checkAuth = async () => {
            try {
                const userData = await request(getMe);
                if (!userData) {
                    const refreshed = await request(refresh);
                    if (refreshed) {
                        setUser(refreshed);
                    }
                } else {
                    setUser(userData);
                }
            } finally {
                setLoading(false);
            }
        };
        if (!user) checkAuth();


    }, [user]);

    useEffect(() => {
        if (!user) return;
        connectSocket();
        return () => disconnectSocket();
    }, [user?.nickname]);
    return {loading };
}