import useUserStore from "@/stores/useUserStore";
import { useApi } from "./useApi";
import { use, useEffect, useState } from "react";

import { getMe, refresh } from "@/services/authService";
import { reconnectSignalR, startHeartBeat } from "@/services/signalR/connection";


export function useAuth() {
    const user = useUserStore((state) => state.user);
    const setUser = useUserStore((state) => state.setUser);
    const { request } = useApi();
    const [loading, setLoading] = useState(true);

    useEffect(() => {
        const checkAuth = async () => {
            let shouldRefresh = false;

            let me = await request(getMe, {
                onError: (_, status) => {
                    if (status === 401) shouldRefresh = true;
                }
            });
            if (shouldRefresh) me = await request(refresh);
            if (me) {
                setUser(me);
                startHeartBeat();
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
        reconnectSignalR();
    }, [user?.nickname])




    return { loading };
}