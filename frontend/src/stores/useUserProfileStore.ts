import { fetchUserFriends, fetchUserGameHistory, fetchUserProfile } from "@/services/userService";
import type { PublicUser, UserProfile } from "@/types/user";
import { create } from "zustand";

type UserProfileStore = {
  profile: UserProfile | null;
  games: Array<ProfileGame>;
  friends: PublicUser[];

  gamesPage: number;
  friendsPage: number;

  //loadingProfile: boolean;
  //oadingGames: boolean;
  // loadingFriends: boolean;

  totalFriendsPages: number;
  totalGamesPages: number;

  // fetchProfile: (nickname: string) => Promise<void>;
  //fetchGames: (nickname: string) => Promise<void>;
  // fetchFriends: (nickname: string) => Promise<void>;
 

  setGames: (games : Array<ProfileGame>) => void;
  setGamesPage: (page: number) => void;
  setFriends: (friends: Array<PublicUser>) => void;
  setFriendsPage: (page: number) => void;
  setProfile: (profile: UserProfile | null) => void;
};

export const useUserProfileStore = create<UserProfileStore>((set, get) => ({
  profile: null,
  games: [],
  friends: [],

  gamesPage: 1,
  friendsPage: 1,

  //loadingProfile: false,
  //loadingGames: false,
  // loadingFriends: false,

  totalFriendsPages: 1,
  totalGamesPages: 1,

  // async fetchProfile(nickname) {
  //   const timeoutid = setTimeout(() => set({ loadingProfile: true }), 300);
  //   try {
  //     const profile = await fetchUserProfile(nickname);
  //     set({ profile, loadingProfile: false });
  //   } catch (error) {
  //     console.error("Error fetching profile:", error);
  //     set({ profile: null, loadingProfile: false });
  //   } finally {
  //       clearTimeout(timeoutid);
  //   }
  // },

  // async fetchGames(nickname) {
  //   const page = get().gamesPage;
  //   const timeoutid = setTimeout(() => set({ loadingGames: true }), 300);
  //   try {
  //     const res = await fetchUserGameHistory(nickname, page);
  //     set({ games: res.gameHistory, loadingGames: false, totalGamesPages: res.totalPages });
  //   } catch (error) {
  //     console.error("Error fetching games:", error);
  //     set({ games: [], loadingGames: false, totalGamesPages: 1 });
  //   } finally {
  //     clearTimeout(timeoutid);
  //   }
  // },

//   async fetchFriends(nickname) {
//     const page = get().friendsPage;
//     const timeoutid = setTimeout(() => set({ loadingFriends: true }), 300);
//     try{
//     const res = await fetchUserFriends(nickname, page);
//     if(res.totalPages < get().friendsPage){
//         set({ friendsPage: res.totalPages });
//     }
//     set({ friends: res.friendList, loadingFriends: false, totalFriendsPages: res.totalPages });
//     } catch (error) {
//       console.error("Error fetching friends:", error);
//       set({ friends: [], loadingFriends: false, totalFriendsPages: 1 });
//     }finally {
//       clearTimeout(timeoutid);
//     }
// },
setFriends(friends) {
    set({ friends });
  },
  setGames(games) {
    set({ games });
  },
  setGamesPage(page) {
    if(page < 1 || page > get().totalGamesPages) return
    set({ gamesPage: page });
  },

  setFriendsPage(page) {
    
    if(page < 1 || page > get().totalFriendsPages) return
    set({ friendsPage: page });
  },
  setProfile(profile) {
    set((state) => (profile ? { profile: { ...state.profile, ...profile } } : { profile: null }));
  }
}));
