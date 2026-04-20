import { useEffect } from 'react';
import { socket } from '../services/socket/socketService';
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

    socket.on("queue:leave:status", ({ status }: { status: string }) => {
      console.log("Left queue with status:", status);
      if (status === 'left') {
        setIsInQueue(false);
        setToast({ msg: t('success.leftQueue'), type: 'success' });
      } else {
        setToast({ msg: t('error.didntLeaveQueue'), type: 'error' });
      }
    });
    socket.on('error', (message: string) => {
      console.log("socketError: ", message);
    });
    socket.on('queue:join:status', ({ status, gameId, joinedAt }: { status: string; gameId?: string; joinedAt?: string }) => {
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

    socket.on('disconnect', () => {
      setIsConnected(false);
    });

    socket.on('connect', () => {
      setIsConnected(true);
    });



    return () => {
      socket.off("queue:leave:status");
      socket.off('error');
      socket.off('queue:join:status');
      socket.emit('lobbyRoom:leave');
    }
  }, [setToast, t, setIsInQueue]);

}