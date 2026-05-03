import type { TimeControl } from "@/types/game";
import {create} from "zustand";

type QueueStoreType = {
    selectedTime: TimeControl
    setSelectedTime: (time: TimeControl) => void;
}

export  const useQueueStore = create<QueueStoreType>((set) => ({
    selectedTime: "10+0",
    setSelectedTime: (time) => set({selectedTime: time})
}))