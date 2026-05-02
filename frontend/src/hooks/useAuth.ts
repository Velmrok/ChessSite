import useUserStore from "@/stores/useUserStore";
import { useApi } from "./useApi";
import { use, useEffect, useState } from "react";

import { getMe, refresh } from "@/services/authService";
import { reconnectSignalR, startHeartBeat } from "@/services/signalR/connection";
import type { GetMeResponse } from "@/types/auth";


export function useAuth() {
    const user = useUserStore((state) => state.user);
    const setUser = useUserStore((state) => state.setUser);
    const setQueueData = useUserStore((state) => state.setQueueData);
    const { request } = useApi();
    const [loading, setLoading] = useState(true);

    useEffect(() => {
        const checkAuth = async () => {
            let shouldRefresh = false;

            let me = await request<GetMeResponse>(getMe, {
                onError: (_, status) => {
                    if (status === 401) shouldRefresh = true;
                }
            });
            if (shouldRefresh) me = await request<GetMeResponse>(refresh);
            if (me) {
                setUser(me);
                setQueueData(me.queueData);
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