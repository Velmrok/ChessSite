import { useEffect, useRef } from 'react';
import useToastStore from '@/stores/useToastStore';
import useUserStore from '@/stores/useUserStore';
import { useNavigate } from 'react-router-dom';
import useGameStore from '@/stores/useGameStore';
import { useTranslation } from 'react-i18next';



export function useGlobalSignalR() {
 
  const queueIntervalRef = useRef<number| null>(null);
  const isInQueue = useUserStore(state => state.queueData?.isInQueue);
  const setQueueTime = useUserStore(state => state.setQueueTime);
  const joinedQueueAt = useUserStore(state => state.queueData?.joinedQueueAt);


  useEffect(() => {
    if (isInQueue) {
      queueIntervalRef.current = setInterval(() => {
  
        setQueueTime(Date.now() - new Date(joinedQueueAt ?? '').getTime());
      }, 1000);
    }

    return () => {
      if (queueIntervalRef.current) {
        clearInterval(queueIntervalRef.current);
      }
    };
  }, [isInQueue]);
}