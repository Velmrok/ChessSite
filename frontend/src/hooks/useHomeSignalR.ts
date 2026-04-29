import { signUpForEvent, leaveEvent, invokeSignalR } from "@/services/signalR/connection";
import useHomeStore from "@/stores/useHomeStore";
import { v4 as uuidv4 } from "uuid";
import useUserStore from "@/stores/useUserStore";
import type { HomeStats } from "@/types/home";
import { useEffect, useState } from "react";
import { useApi } from "./useApi";


export function useHomeSignalR() {
  const [isInitialized, setIsInitialized] = useState(false);
  const setIsInQueue = useUserStore((state) => state.setIsInQueue);
  const setUsersOnline = useHomeStore((state) => state.setUsersOnline);
  const setMatchesInProgress = useHomeStore((state) => state.setMatchesInProgress);
  const setCreatedAccounts = useHomeStore((state) => state.setCreatedAccounts);
  const setDeletedQueuesById = useHomeStore((state) => state.setDeletedQueuesById);
  const setQueueSize = useHomeStore((state) => state.setQueueSize);

  const {request} = useApi();
  useEffect(() => {
    if (isInitialized) return;

    request(() => invokeSignalR('JoinGroup', {
      type: "Home",
      correlationId: uuidv4(),
    }));

    signUpForEvent("StatsUpdated", (stats: HomeStats) => {
      setUsersOnline(stats.usersOnline);
      setMatchesInProgress(stats.matchesInProgress);
      setCreatedAccounts(stats.createdAccounts);
    });
    
    signUpForEvent("queueList:delete", (q: { queueId: string }) => {
      const queueId = q.queueId;
      setDeletedQueuesById(queueId);
    });
    signUpForEvent("queue:enter", () => {
      setIsInQueue(true);
    });
    signUpForEvent("queueSize", (size: number) => {
      setQueueSize(size);
    });

    setIsInitialized(true);

    return () => {
      request(() => invokeSignalR("LeaveGroup", {
        type: "Home",
        correlationId: uuidv4(),
      }));
      leaveEvent("StatsUpdated");
      leaveEvent("queueList:delete");
      leaveEvent("queue:enter");
      leaveEvent("queueSize");
      setIsInitialized(false);
    }
  }, [setUsersOnline, setMatchesInProgress, setCreatedAccounts, setDeletedQueuesById, setIsInQueue]);
}
