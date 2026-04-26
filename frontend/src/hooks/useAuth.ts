import useUserStore from "@/stores/useUserStore";
import { useApi } from "./useApi";
import { useEffect, useState } from "react";

import { getMe, refresh } from "@/services/authService";
import { connectSignalR, disconnectSignalR } from "@/services/signalR/connection";


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
                await connectSignalR();
            } else if (shouldRefresh) {
                const refreshResult = await request(refresh);
                if (refreshResult) {
                    await connectSignalR();
                    setUser(refreshResult);
                }
            }

            setLoading(false);
        };

        if (!user) {
            checkAuth();
        } else {

            setLoading(false);
        }
        return () => void disconnectSignalR();
    }, []);




    return { loading };
}