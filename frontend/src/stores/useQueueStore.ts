import type { QueueData } from "@/types/auth";
import type { TimeControl } from "@/types/game";
import { create } from "zustand";

type QueueStoreType = {
    selectedTime: TimeControl
    queueData?: QueueData;
    queueTime?: number;
    setQueueTime: (queueTime: number) => void;
    setQueueData: (queueData: QueueData) => void;
    setSelectedTime: (time: TimeControl) => void;
}

export const useQueueStore = create<QueueStoreType>((set) => ({
    selectedTime: "10+0",
    queueData: undefined,
    queueTime: undefined,
    setQueueTime: (queueTime: number) => {set({ queueTime })},
    setQueueData: (queueData: QueueData) => set({ queueData }),
    setSelectedTime: (time) => set({ selectedTime: time })
}))