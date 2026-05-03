import { startHeartBeat, stopHeartBeat } from "@/services/signalR/connection";
import useUserStore from "@/stores/useUserStore";
import type { GetMeResponse } from "@/types/auth";


export function useAuthActions() {

    const setUser = useUserStore((state) => state.setUser);
    const setQueueData = useUserStore((state) => state.setQueueData);
    


     const applyAuth = (me: GetMeResponse) => {
        const {queueData, ...userData} = me;
        setUser(userData);
        setQueueData(queueData);
        startHeartBeat();
    }
    const clearAuth = () => {
        setUser(null);
        setQueueData({isInQueue: false});
        stopHeartBeat();
    }

    return { applyAuth, clearAuth };
}