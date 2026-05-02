import { joinQueue, leaveQueue } from "@/services/socket/signalRGlobalService";
import useToastStore from "@/stores/useToastStore";
import useUserStore from "@/stores/useUserStore";
import { useTransition } from "react";
import { useTranslation } from "react-i18next";
import {v4 as uuidv4} from "uuid";
import { useApi } from "./useApi";
import type { SignalRError } from "@/types/signalR";

export default function useQueue() {
    const setToast = useToastStore((state) => state.setToast);
    const {t} = useTranslation('toast')
    const setIsInQueue = useUserStore((state) => state.setIsInQueue);

    const {request} = useApi();
    const setQueueTime = useUserStore(state => state.setQueueTime);
    const setQueueJoinedAt = useUserStore(state => state.setQueueJoinedAt);
    const joinedQueueAt = useUserStore(state => state.joinedQueueAt);

    const handleJoinQueue = async (time: number, increment: number) => {
        const response = await request( () =>
        joinQueue({
            type: "Queue",
            correlationId: uuidv4(),
            payload: {
                time,
                increment
            }
        }),{
            onError: (message:string) => {setToast({msg: t(`error.${message}`), type: 'error'})}
        })
       
        if (response) {
      
        setIsInQueue(true);
        
        setQueueTime(0);
        setQueueJoinedAt(new Date());
        setToast({ msg: t('info.addedToQueue'), type: "info" });
      
    }
    }

    const handleCancelQueue = async () => {

        const response = await request(() => leaveQueue({
            type: "Queue",
            correlationId: uuidv4(),
            payload: {}
        }),{
            onError: () => {
                setToast({msg: t('error.leaveQueueFailed'), type: 'error'})
            }
        })
        if (response) {
            setIsInQueue(false);
            setToast({ msg: t('info.leftQueue'), type: "info" });
        }

    }
    return { handleJoinQueue, handleCancelQueue };
}

