import {create} from "zustand";
import { fetchUserOnlineFriends } from "@/services/userService";
import { getQueueList } from "@/services/homeService";
import type { PublicUser } from "@/types/user";
type HomeStore = {
    usersOnline: number;
    matchesInProgress: number;
    createdAccounts:number;
    queueSize:number;
    friendsPage: number;
    loadingFriends: boolean;
    totalFriendsPages: number;
    friends: Array<Omit<PublicUser, "rating">>;
    queueList: QueueList | null;
    deletedQueues: Array<string>;
    lobbyGameType: gameType| 'any';
    setQueueSize: (size: number) => void;
    setDeletedQueuesById: (queueId: string) => void;
    setUsersOnline: (count: number) => void;
    setMatchesInProgress: (count: number) => void;
    setCreatedAccounts: (count: number) => void;
    fetchOnlineFriends: (nickname: string) => Promise<void>;
    setFriendsPage: (page: number) => void;
    setQueueList: (queueList: QueueList) => void;
    fetchQueueList: (gameType: gameType| 'any') => Promise<void>;
    setLobbyGameType: (gameType: gameType | 'any') => void;
};

const useHomeStore = create<HomeStore>((set, get) => ({
    usersOnline: 0,
    matchesInProgress: 0,
    createdAccounts:0,
    queueSize:0,
    friendsPage: 1,
    loadingFriends: false,
    totalFriendsPages: 1,
    friends: [],
    lobbyGameType: 'any',
    queueList: null,
    deletedQueues: [],
    setQueueSize: (size: number) => set({queueSize: size}),
    setQueueList: (queueList: QueueList) => set({queueList}),
    setUsersOnline: (count: number) => set({usersOnline: count}),
    setMatchesInProgress: (count: number) => set({matchesInProgress: count}),
    setCreatedAccounts: (count: number) => set({createdAccounts: count}),

     async fetchOnlineFriends(nickname) {
        const page = get().friendsPage;
        const timeoutid = setTimeout(() => set({ loadingFriends: true }), 300);
        try{
        const res = await fetchUserOnlineFriends(nickname, page,9);
        if(res.totalPages < get().friendsPage){
            set({ friendsPage: res.totalPages });
        }
        set({ friends: res.friendList, loadingFriends: false, totalFriendsPages: res.totalPages });
        } catch (error) {
          console.error("Error fetching friends:", error);
          set({ friends: [], loadingFriends: false, totalFriendsPages: 1 });
        }finally {
          clearTimeout(timeoutid);
        }
    },
    async fetchQueueList(gameType) {
        try {
          const data = await getQueueList(gameType);
          console.log("Fetched queue list:", data);
          set({ queueList: data });
          set({ lobbyGameType: gameType });
        } catch (error) {
          console.error("Error fetching queue list:", error);
        }
    },
    setFriendsPage(page) {
    
    if(page < 1 || page > get().totalFriendsPages) return
    set({ friendsPage: page });
  },
   setDeletedQueuesById(queueId: string) {
    if(get().deletedQueues.includes(queueId)) return;
    if(!get().queueList?.queues.some(q => q.id === queueId)) return;
    if(!get().queueList || !get().queueList?.queues) return;
  
    
    if(get().queueList?.queues.length!=0 && (get().deletedQueues.length+1)/get().queueList?.queues.length!>0.4){
       get().fetchQueueList(get().lobbyGameType);
    }
    if(get().queueList && get().queueList?.queues.some(q => q.id === queueId)) {
    set((state) => ({ deletedQueues: [...state.deletedQueues, queueId] }))
    }
    
    },
    setLobbyGameType(gameType) {
        set({ lobbyGameType: gameType });
    }

}));

export default useHomeStore;