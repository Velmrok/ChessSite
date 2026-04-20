import { joinGame, leaveQueue } from "@/services/socket/socketGlobalService";
import useToastStore from "@/stores/useToastStore";
import useUserStore from "@/stores/useUserStore";
import { useTransition } from "react";
import { useTranslation } from "react-i18next";


export default function useQueue() {
    const setToast = useToastStore((state) => state.setToast);
    const {t} = useTranslation('toast')
    const setIsInQueue = useUserStore((state) => state.setIsInQueue);
    const handleJoinGame = joinGame;

    const handleCancelGame = () => {

        setIsInQueue(false);
        leaveQueue();
        setToast({ msg: t('info.leftQueue'), type: "info" });

    }
    return { handleJoinGame, handleCancelGame };
}