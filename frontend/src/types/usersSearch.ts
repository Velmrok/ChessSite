import type { PublicUser, Rating } from "./user";

export type SearchParams = {
    search: string;
    limit: number;
    minRating: number ;
    maxRating: number;
    online: boolean;
    sortBy: SortByOption;
    sortDescending: boolean ;
    ratingType: 'Rapid' | 'Blitz' | 'Bullet';
    page: number;
};

export type SortByOption = 'Rating' | 'Nickname' | 'OnlineStatus' | 'LastActive' | 'CreatedAt';

export type UsersSearch = Array<{
        nickname: string;
        profilePictureUrl: string;
        rating: Rating;
        isOnline: boolean;
        createdAt: string;
        lastActive: string;
        role: 'user' | 'admin';
    }>

export type UsersSearchResponse = {
    users: UsersSearch ;
    totalPages: number;
}
