import { joinQueue, leaveQueue } from "@/services/socket/signalRGlobalService";
import useToastStore from "@/stores/useToastStore";
import { useTranslation } from "react-i18next";
import {v4 as uuidv4} from "uuid";
import { useApi } from "./useApi";
import { useQueueStore } from "@/stores/useQueueStore";

export default function useQueue() {
    const setToast = useToastStore((state) => state.setToast);
    const {t} = useTranslation('toast')


    const {request} = useApi();
    const setQueueTime = useQueueStore(state => state.setQueueTime);
    const setQueueData = useQueueStore(state => state.setQueueData);
    const queueData = useQueueStore(state => state.queueData);
   

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
      
        
        
        setQueueTime(0);
        setQueueData({ ...queueData, joinedQueueAt: new Date().toString(), time, increment, isInQueue: true });
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
            setQueueData({isInQueue: false});
            setToast({ msg: t('info.leftQueue'), type: "info" });
        }

    }
    return { handleJoinQueue, handleCancelQueue};
}

