import useHomeStore from "@/stores/useHomeStore";
import { socket } from "../services/socket/socketService";
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
    socket.emit('lobbyRoom:join');


    socket.on("usersOnline", (count: number) => {

      setUsersOnline(count);
    });

    socket.on("matchesInProgress", (count: number) => {
      setMatchesInProgress(count);
    });

    socket.on("createdAccounts", (count: number) => {
      setCreatedAccounts(count);
    });
    socket.on("queueList:delete", (q: { queueId: string }) => {
      console.log("Received queueList:delete for queueId:", q.queueId);
      const queueId = q.queueId;
      setDeletedQueuesById(queueId);
    })
    socket.on("queue:enter", () => {
      setIsInQueue(true);
    });
    socket.on("queueSize", (size: number) => {
      setQueueSize(size);
    });

    setIsInitialized(true);

    return () => {
      socket.emit('lobbyRoom:leave');
      socket.off("usersOnline");
      socket.off("matchesInProgress");
      socket.off("createdAccounts");
      socket.off("queueList:delete");
      socket.off("queue:enter");
      socket.off("queueSize");
      setIsInitialized(false);
    }
  }, [setUsersOnline, setMatchesInProgress, setCreatedAccounts, setDeletedQueuesById, setIsInQueue]);
}
