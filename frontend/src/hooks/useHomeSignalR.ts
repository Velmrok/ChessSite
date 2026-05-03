import { signUpForEvent, leaveEvent, invokeSignalR } from "@/services/signalR/connection";
import useHomeStore from "@/stores/useHomeStore";
import { v4 as uuidv4 } from "uuid";
import useUserStore from "@/stores/useUserStore";
import type { HomeStats } from "@/types/home";
import { useEffect, useState } from "react";
import { useApi } from "./useApi";
import type { SignalRResponse } from "@/types/signalR";


export function useHomeSignalR() {
 
  
  const setUsersOnline = useHomeStore((state) => state.setUsersOnline);
  const setMatchesInProgress = useHomeStore((state) => state.setMatchesInProgress);
  const setCreatedAccounts = useHomeStore((state) => state.setCreatedAccounts);
  const setQueueSize = useHomeStore((state) => state.setQueueSize);
  const isConnected = useUserStore(state => state.isConnected);
  const {request} = useApi();
  useEffect(() => {
   

    request(() => invokeSignalR('JoinGroup', {
      type: "Home",
      correlationId: uuidv4(),
    }));

    signUpForEvent("StatsUpdated", (response : SignalRResponse<HomeStats>) => {
      const stats = response.data;
      if (!stats) {
        console.error("Received StatsUpdated event with null data");
        return;
      }
      console.log("Received StatsUpdated event:", stats);
      setUsersOnline(stats.usersOnline);
      setMatchesInProgress(stats.activeGames);
      setCreatedAccounts(stats.totalUsers);
      setQueueSize(stats.usersInQueue);
    });
    
    // signUpForEvent("queueList:delete", (q: { queueId: string }) => {
    //   const queueId = q.queueId;
    //   setDeletedQueuesById(queueId);
    // });
    // signUpForEvent("queue:enter", () => {
    //   setIsInQueue(true);
    // });
    // signUpForEvent("queueSize", (size: number) => {
    //   setQueueSize(size);
    // });

    

    return () => {
      request(() => invokeSignalR("LeaveGroup", {
        type: "Home",
        correlationId: uuidv4(),
      }));
      leaveEvent("StatsUpdated");
      leaveEvent("queueList:delete");
      leaveEvent("queue:enter");
      leaveEvent("queueSize");
     
    }
  }, [isConnected]);
}
