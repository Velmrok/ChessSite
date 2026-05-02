import { useEffect, useRef } from 'react';
import useToastStore from '@/stores/useToastStore';
import useUserStore from '@/stores/useUserStore';
import { useNavigate } from 'react-router-dom';
import useGameStore from '@/stores/useGameStore';
import { useTranslation } from 'react-i18next';



export function useGlobalSignalR() {
  const setToast = useToastStore((state) => state.setToast);
  const { t } = useTranslation('toast');
  const setIsInQueue = useUserStore((state) => state.setIsInQueue);
  const setGameHasJustStarted = useGameStore((state) => state.setGameHasJustStarted);
  const setIsConnected = useUserStore((state) => state.setIsConnected);
  const navigate = useNavigate();

  const queueIntervalRef = useRef<number| null>(null);
  const isInQueue = useUserStore(state => state.user?.isInQueue);
  const setQueueTime = useUserStore(state => state.setQueueTime);
  const setQueueJoinedAt = useUserStore(state => state.setQueueJoinedAt);
  const joinedQueueAt = useUserStore(state => state.joinedQueueAt);


  useEffect(() => {
    console.log('isInQueue changed:', isInQueue);
    if (isInQueue) {
      queueIntervalRef.current = setInterval(() => {
        console.log('Updating queue time');
        setQueueTime(Date.now() - joinedQueueAt!.getTime());
      }, 1000);
    }

    return () => {
      if (queueIntervalRef.current) {
        clearInterval(queueIntervalRef.current);
      }
    };
  }, [isInQueue]);
}