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
            let shouldRefresh = false;

            const me = await request(getMe, {
                onError: (_, status) => {
                    if (status === 401) shouldRefresh = true;
                }
            });

            if (me) {
                setUser(me);
            } else if (shouldRefresh) {
                const refreshResult = await request(refresh);
                if (refreshResult) setUser(refreshResult);
            }

            setLoading(false); 
        };

        if (!user) {
            checkAuth();
        } else {
            setLoading(false);
        }
    }, []);

    useEffect(() => {
        if (!user) return;
        connectSocket();
        return () => disconnectSocket();
    }, [user?.nickname]);
    return {loading };
}