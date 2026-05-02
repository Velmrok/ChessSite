import { useEffect, useRef } from 'react';
import useToastStore from '@/stores/useToastStore';
import useUserStore from '@/stores/useUserStore';
import { useNavigate } from 'react-router-dom';
import useGameStore from '@/stores/useGameStore';
import { useTranslation } from 'react-i18next';



export function useGlobalSignalR() {
 
  const queueIntervalRef = useRef<number| null>(null);
  const isInQueue = useUserStore(state => state.user?.isInQueue);
  const setQueueTime = useUserStore(state => state.setQueueTime);
  const joinedQueueAt = useUserStore(state => state.joinedQueueAt);


  useEffect(() => {
    if (isInQueue) {
      queueIntervalRef.current = setInterval(() => {
  
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