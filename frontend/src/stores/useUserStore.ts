
import type { User } from '@/types/user';
import {create} from 'zustand';
type UserStore = {
    user: User | null;
    queueTime?: number;
    isConnected: boolean;
    joinedQueueAt?: Date;
    setQueueTime: (queueTime: number) => void;
    setUser: (user: User | null) => void;
    clearUser: () => void;
    deleteFriend: (nickname: string) => void;
    addFriend: (nickname: string) => void;
    setIsConnected: (isConnected: boolean) => void;
    setIsInQueue: (isInQueue: boolean) => void;
    setQueueJoinedAt: (joinedAt: Date) => void;


};
const useUserStore = create<UserStore>((set, get) => ({
    user: null,
    queueTime: undefined,
    isConnected: false,
    joinedQueueAt: undefined,
    setQueueTime: (queueTime: number) =>{
         set({queueTime})
         
         
    },
    setQueueJoinedAt: (joinedAt: Date) => set({joinedQueueAt: joinedAt}),
    setUser: (user: User | null) => set({user}),
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
    setIsInQueue: (isInQueue: boolean) => set((state) => {
        if (!state.user) return state;
        return { ...state, user: { ...state.user, isInQueue } };
    }),
    setIsConnected: (isConnected: boolean) => set({isConnected}),
    
   
}));
export default useUserStore;