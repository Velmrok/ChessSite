import { getConnection, invokeSignalR } from "@/services/signalR/connection";
import useHomeStore from "@/stores/useHomeStore";

import useUserStore from "@/stores/useUserStore";
import { useEffect, useState } from "react";


export function useHomeSocket() {
  const [isInitialized, setIsInitialized] = useState(false);
  const setIsInQueue = useUserStore((state) => state.setIsInQueue);
  const setUsersOnline = useHomeStore((state) => state.setUsersOnline);
  const setMatchesInProgress = useHomeStore((state) => state.setMatchesInProgress);
  const setCreatedAccounts = useHomeStore((state) => state.setCreatedAccounts);
  const setDeletedQueuesById = useHomeStore((state) => state.setDeletedQueuesById);
  const setQueueSize = useHomeStore((state) => state.setQueueSize);
  useEffect(() => {
    if (isInitialized) return;
    invokeSignalR('JoinHomeGroup').then(x=>console.log(x));
    const conn = getConnection();


    conn.on("usersOnline", (count: number) => {

      setUsersOnline(count);
    });

    conn.on("matchesInProgress", (count: number) => {
      setMatchesInProgress(count);
    });

    conn.on("createdAccounts", (count: number) => {
      setCreatedAccounts(count);
    });
    conn.on("queueList:delete", (q: { queueId: string }) => {
      console.log("Received queueList:delete for queueId:", q.queueId);
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
      invokeSignalR('lobbyRoom:leave');
      conn.off("usersOnline");
      conn.off("matchesInProgress");
      conn.off("createdAccounts");
      conn.off("queueList:delete");
      conn.off("queue:enter");
      conn.off("queueSize");
      setIsInitialized(false);
    }
  }, [setUsersOnline, setMatchesInProgress, setCreatedAccounts, setDeletedQueuesById, setIsInQueue]);
}
