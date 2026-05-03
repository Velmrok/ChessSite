import { startHeartBeat, stopHeartBeat } from "@/services/signalR/connection";
import { useQueueStore } from "@/stores/useQueueStore";
import useUserStore from "@/stores/useUserStore";
import type { GetMeResponse } from "@/types/auth";


export function useAuthActions() {

    const setUser = useUserStore((state) => state.setUser);
    const setQueueData = useQueueStore((state) => state.setQueueData);
    


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