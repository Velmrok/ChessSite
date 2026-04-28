import type { PublicUser, UserProfile, FriendsProfile} from "@/types/user";
import { create } from "zustand";

type UserProfileStore = {

  profile: UserProfile | null;
  games: Array<ProfileGame>;
  friends: FriendsProfile;
  gamesPage: number;
  friendsPage: number;
  totalFriendsPages: number;
  totalGamesPages: number;

  setGames: (games : Array<ProfileGame>) => void;
  setGamesPage: (page: number) => void;
  setFriends: (friends: FriendsProfile) => void;
  setFriendsPage: (page: number) => void;
  setProfile: (profile: UserProfile | null) => void;
};

export const useUserProfileStore = create<UserProfileStore>((set, get) => ({
  profile: null,
  games: [],
  friends: [],

  gamesPage: 1,
  friendsPage: 1,
  totalFriendsPages: 1,
  totalGamesPages: 1,

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
