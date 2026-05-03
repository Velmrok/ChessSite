import type { PublicUser, PlayerSummary } from "./user"; 

export type GameSearchParams = {
    query: string; 
    limit: number;
    gameType: GameType | null;
    status: GameStatus | null;
    page: number;
    sortBy : 'date' | 'time' | 'nickname' ;
    SortDescending: boolean; 
};

export type GameType = 'bullet' | 'blitz' | 'rapid';

export type GameStatus = 'active' | 'finished';

export type GameState = {
        id: string;
        winnerNickname: string | null;
        date: string;
        status: GameStatus;
        white: PublicUser;
        black: PublicUser;
        fen: string;
        moves: Array<MoveInfo>;
        gameType: gameType;
        time: number;
        increment: number;
        currentTurn: 'white' | 'black';
        currentWhiteTime: number;
        currentBlackTime: number;
        messages: Array<Message>;
        isDrawOffered: string | null;
        reason?: string;
}

export type GameSummary = {
        id: string;
        whitePlayer: PlayerSummary;
        blackPlayer: PlayerSummary;
        winnerNickname: string | null;
        gameType: string;
        time: number;
        increment: number;
        status: string;
    finishedAt: string;
    moves: Array<string>;
};
export type TimeControl =
    "1+0" | "1+1" | "1+2" |
    "2+0" | "2+1" | "2+2" |
    "3+0" | "3+1" | "3+2" |
    "5+0" | "5+3" | "5+5" |
    "10+0" | "10+2" | "15+0" |
    "15+5" | "30+0" | "30+15";
export type MatchFoundResponse = {
    gameUrl: string;
}
