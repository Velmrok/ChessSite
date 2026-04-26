import type { PublicUser, UserSummary } from "./user";

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
        whitePlayer: UserSummary;
        blackPlayer: UserSummary;
        winnerNickname: string | null;
        gameType: string;
        time: number;
        increment: number;
        status: string;
        finishedAt: string;
        moves: Array<string>;
    };
