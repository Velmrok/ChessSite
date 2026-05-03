import { useEffect, useRef } from 'react';
import { useQueueStore } from '@/stores/useQueueStore';
import { leaveEvent } from '@/services/signalR/connection';



export function useGlobalSignalR() {
 
  const queueIntervalRef = useRef<number| null>(null);
  const isInQueue = useQueueStore(state => state.queueData?.isInQueue);
  const setQueueTime = useQueueStore(state => state.setQueueTime);
  const joinedQueueAt = useQueueStore(state => state.queueData?.joinedQueueAt);


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
      if (isInQueue) {
        
        leaveEvent("GameFound");
      }
    };
  }, [isInQueue]);
}