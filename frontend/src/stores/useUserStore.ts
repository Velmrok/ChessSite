
import type { GetMeResponse, QueueData } from '@/types/auth';
import {create} from 'zustand';
type UserStore = {
    user: Omit<GetMeResponse, 'queueData'> | null;
    queueData?: QueueData   ;
    queueTime?: number;
    isConnected: boolean;
    setQueueTime: (queueTime: number) => void;
    setQueueData: (queueData: QueueData) => void;
    setUser: (user: Omit<GetMeResponse, 'queueData'> | null) => void;
    clearUser: () => void;
    deleteFriend: (nickname: string) => void;
    addFriend: (nickname: string) => void;
    setIsConnected: (isConnected: boolean) => void;



};
const useUserStore = create<UserStore>((set, get) => ({
    user: null,
    queueTime: undefined,
    isConnected: false,
    queueData: undefined,
    setQueueTime: (queueTime: number) =>{
         set({queueTime})
         
         
    },
    setQueueData: (queueData: QueueData) => set({ queueData }),
    setUser: (user: Omit<GetMeResponse, 'queueData'> | null) => set({user}),
    clearUser: () => set({user: null}),
    deleteFriend: (nickname: string) => set((state) => {
        if (!state.user) return state;
        return {
            user: {
                ...state.user,
                friendNicknames: state.user.friendNicknames.filter(friend => friend !== nickname)
            }
        };
    }),
    addFriend: (nickname: string) => set((state) => {
        if (!state.user) return state;
        return {
            user: {
                ...state.user,
                friendNicknames: [...state.user.friendNicknames, nickname]
            }
        };
    }),
    
    setIsConnected: (isConnected: boolean) => set({ isConnected }),


}));
export default useUserStore;