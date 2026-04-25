export type GameSearchParams = {
    query: string; 
    limit: number;
    gameType: GaameSearchType;
    status: 'active' | 'finished' | null;
    page: number;
    sortBy : 'date' | 'time' | 'nicknames' ;
    SortDescending: boolean; 
};

export type GameType = 'bullet' | 'blitz' | 'rapid';
export type GaameSearchType = GameType | null;