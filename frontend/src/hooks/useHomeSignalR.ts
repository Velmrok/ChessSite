import { getConnection, invokeSignalR } from "@/services/signalR/connection";
import useHomeStore from "@/stores/useHomeStore";

import useUserStore from "@/stores/useUserStore";
import type { HomeStats } from "@/types/home";
import { useEffect, useState } from "react";


export function useHomeSignalR() {
  const [isInitialized, setIsInitialized] = useState(false);
  const setIsInQueue = useUserStore((state) => state.setIsInQueue);
  const setUsersOnline = useHomeStore((state) => state.setUsersOnline);
  const setMatchesInProgress = useHomeStore((state) => state.setMatchesInProgress);
  const setCreatedAccounts = useHomeStore((state) => state.setCreatedAccounts);
  const setDeletedQueuesById = useHomeStore((state) => state.setDeletedQueuesById);
  const setQueueSize = useHomeStore((state) => state.setQueueSize);
  useEffect(() => {
    if (isInitialized) return;
    invokeSignalR('JoinHomeGroup');
    const conn = getConnection();

    conn.on("StatsUpdated", (stats: HomeStats) => {
      setUsersOnline(stats.usersOnline);
      setMatchesInProgress(stats.matchesInProgress);
      setCreatedAccounts(stats.createdAccounts);
    });
    
    conn.on("queueList:delete", (q: { queueId: string }) => {
      const queueId = q.queueId;
      setDeletedQueuesById(queueId);
    })
    conn.on("queue:enter", () => {
      setIsInQueue(true);
    });
    conn.on("queueSize", (size: number) => {
      setQueueSize(size);
    });

    setIsInitialized(true);

    return () => {
      invokeSignalR("LeaveHomeGroup");
      conn.off("StatsUpdated");
      conn.off("queueList:delete");
      conn.off("queue:enter");
      conn.off("queueSize");
      setIsInitialized(false);
    }
  }, [setUsersOnline, setMatchesInProgress, setCreatedAccounts, setDeletedQueuesById, setIsInQueue]);
}
