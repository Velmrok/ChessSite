import type { PublicUser } from "./user";

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

export type UsersResponse = {
    users: PublicUser[];
    totalPages: number;
}

