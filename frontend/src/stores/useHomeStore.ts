import {create} from "zustand";
import { getQueueList } from "@/services/homeService";
import type {FriendsOnline } from "@/types/user";
type HomeStore = {
    usersOnline: number;
    matchesInProgress: number;
    createdAccounts:number;
    queueSize:number;
    friendsPage: number;

    totalFriendsPages: number;
    friends: FriendsOnline;

    setQueueSize: (size: number) => void;
    setUsersOnline: (count: number) => void;
    setMatchesInProgress: (count: number) => void;
    setCreatedAccounts: (count: number) => void;
    setFriends: (friends: FriendsOnline) => void;
    setFriendsPage: (page: number) => void;

};

const useHomeStore = create<HomeStore>((set, get) => ({

    usersOnline: 0,
    matchesInProgress: 0,
    createdAccounts:0,
    queueSize:0,

    friendsPage: 1,
    totalFriendsPages: 1,
    friends: [],

    setQueueSize: (size: number) => set({queueSize: size}),
    setUsersOnline: (count: number) => set({usersOnline: count}),
    setMatchesInProgress: (count: number) => set({matchesInProgress: count}),
    setCreatedAccounts: (count: number) => set({createdAccounts: count}),
    setFriends: (friends: FriendsOnline) => set({friends}),
 
    setFriendsPage(page) {
    
    if(page < 1 || page > get().totalFriendsPages) return
    set({ friendsPage: page });
  },


}));

export default useHomeStore;