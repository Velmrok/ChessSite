import { joinGame, leaveQueue } from "@/services/socket/socketGlobalService";
import useLanguageStore from "@/stores/useLanguageStore";
import useToastStore from "@/stores/useToastStore";
import useUserStore from "@/stores/useUserStore";


export default function useQueue() {
    const setToast = useToastStore((state) => state.setToast);
    const t = useLanguageStore((state) => state.t);
    const setIsInQueue = useUserStore((state) => state.setIsInQueue);
    const handleJoinGame = joinGame;

    const handleCancelGame = () => {

        setIsInQueue(false);
        leaveQueue();
        setToast({ msg: t.toast.info.leftQueue, type: "info" });

    }
    return { handleJoinGame, handleCancelGame };
}