
import {create} from 'zustand';
type UserStore = {
    user: User | null;
    friendList: string[];
    queueTime?: number;
    queueTimeInterval?: ReturnType<typeof setInterval>;
    isConnected: boolean;
    setQueueTime: (queueTime: number) => void;
    setUser: (user: User | null) => void;
    clearUser: () => void;
    deleteFriend: (nickname: string) => void;
    addFriend: (nickname: string) => void;
    setIsConnected: (isConnected: boolean) => void;
    setIsInQueue: (isInQueue: boolean) => void;


};
const useUserStore = create<UserStore>((set, get) => ({
    user: null,
    friendList: [],
    queueTime: undefined,
    queueTimeInterval: undefined,
    isConnected: false,
    setQueueTime: (queueTime: number) =>{
         set({queueTime})
         get().queueTimeInterval && clearInterval(get().queueTimeInterval);
         const intervalId = setInterval(() => {
            set((state) => ({ queueTime: state.queueTime! + 1000 }));

        },1000);
        set({queueTimeInterval: intervalId});
    },
    setUser: (user: User | null) => set({user}),
    clearUser: () => set({user: null}),
    deleteFriend: (nickname: string) => set((state) => {
        if (!state.user) return state;
        return {
            user: {
                ...state.user,
                friendList: state.user.friendList.filter(friend => friend !== nickname)
            }
        };
    }),
    addFriend: (nickname: string) => set((state) => {
        if (!state.user) return state;
        return {
            user: {
                ...state.user,
                friendList: [...state.user.friendList, nickname]
            }
        };
    }),
    setIsInQueue: (isInQueue: boolean) => set((state) => {
        if (!state.user) return state;
        if(!isInQueue){
            get().queueTimeInterval && clearInterval(get().queueTimeInterval);
            set({queueTime: undefined, queueTimeInterval: undefined});
        }
        return { ...state, user: { ...state.user, isInQueue } };
    }),
    setIsConnected: (isConnected: boolean) => set({isConnected}),
    
   
}));
export default useUserStore;