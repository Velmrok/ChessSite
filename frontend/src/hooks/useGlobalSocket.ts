import { useEffect } from 'react';
import {getConnection} from "@/services/signalR/connection";
import useToastStore from '@/stores/useToastStore';
import useUserStore from '@/stores/useUserStore';
import { useNavigate } from 'react-router-dom';
import useGameStore from '@/stores/useGameStore';
import { useTranslation } from 'react-i18next';



export function useGlobalSocket() {
  const setToast = useToastStore((state) => state.setToast);
  const { t } = useTranslation('toast');
  const setIsInQueue = useUserStore((state) => state.setIsInQueue);
  const setGameHasJustStarted = useGameStore((state) => state.setGameHasJustStarted);
  const setIsConnected = useUserStore((state) => state.setIsConnected);
  const navigate = useNavigate();
  useEffect(() => {
    const conn = getConnection();

    conn.on("queue:leave:status", ({ status }: { status: string }) => {
      console.log("Left queue with status:", status);
      if (status === 'left') {
        setIsInQueue(false);
        setToast({ msg: t('success.leftQueue'), type: 'success' });
      } else {
        setToast({ msg: t('error.didntLeaveQueue'), type: 'error' });
      }
    });
    conn.on('error', (message: string) => {
      console.log("socketError: ", message);
    });
    conn.on('queue:join:status', ({ status, gameId, joinedAt }: { status: string; gameId?: string; joinedAt?: string }) => {
      console.log("Join queue status:", status, "Game ID:", gameId);
      if (gameId) {
        useUserStore.getState().setIsInQueue(false);
        setGameHasJustStarted(true);
        navigate(`/game/${gameId}`);
      } else if (status === 'alreadyInGame') {
        setToast({ msg: t('error.alreadyInGame'), type: "error" });
      } else if (status === 'alreadyInQueue') {
        setToast({ msg: t('error.alreadyInQueue'), type: "error" });
      } else if (status === 'waiting') {
        useUserStore.getState().setIsInQueue(true);
        const now = Date.now();
        useUserStore.getState().setQueueTime(now - new Date(joinedAt!).getTime());
        setToast({ msg: t('info.addedToQueue'), type: "info" });
      }
    });

    conn.on('disconnect', () => {
      setIsConnected(false);
    });

    conn.on('connect', () => {
      setIsConnected(true);
    });



    return () => {
      conn.off("queue:leave:status");
      conn.off('error');
      conn.off('queue:join:status');
      
    }
  }, [setToast, t, setIsInQueue]);

}