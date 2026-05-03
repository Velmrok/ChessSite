import useUserStore from "@/stores/useUserStore";
import { useApi } from "./useApi";
import { use, useEffect, useState } from "react";

import { getMe, refresh } from "@/services/authService";
import { reconnectSignalR, startHeartBeat } from "@/services/signalR/connection";
import type { GetMeResponse } from "@/types/auth";
import { useAuthActions } from "./useAuthActions";


export function useAuth() {
    const user = useUserStore((state) => state.user);
    const { applyAuth } = useAuthActions();
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
            if (me) applyAuth(me);


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

   



    return { loading};
}