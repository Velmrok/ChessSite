import type { Message } from "postcss";

export { };
declare global {


    
    type UserInfo = {
        rating: {
            rapid: number;
            blitz: number;
            bullet: number;
        };
        joinDate: string;
        gamesPlayed: number;
        wins: number;
        losses: number;
        draws: number;
    }
    type Friend = {
        nickname: string;
        avatar: string;
        onlineStatus: 'online' | 'offline';
        rating: {
            rapid: number;
            blitz: number;
            bullet: number;
        }
    }
    type UserProfile = {
        nickname: string;
        avatar: string;
        bio: string;
        onlineStatus: 'online' | 'offline';

        userInfo: UserInfo;
    };
    type Message = {
        id: string;
        nickname: string;
        text: string;
        timestamp: string;

    }
    type GameState = {
        id: string;
        winnerNickname: string | null;
        date: string;
        status: GameStatus;
        white: Omit<User>;
        black: Omit<User>;
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
    type Move = {
        from: string;
        to: string;
        promotion?: string;
    };
    type MoveInfo = {
        move: string;
        deltaTime: number;
        absoluteTime: number;
    }

    type Toast = "success" | "error" | "info";
    type GameStatus = "waiting" | "live" | "finished";
    type scoreStatus = 'win' | 'loss' | 'draw';
    type ProfileGame = {
        id: string;
        winnerNickname: string | null;
        date: string;
        profileNickname: string;
        status: GameStatus;
        gameType: gameType;
        scoreStatus: scoreStatus;

        whiteNickname: string;
        whiteAvatar: string;
        whiteRating: number;

        blackNickname: string;
        blackAvatar: string;
        blackRating: number;

    }
    type HomeInfo = {
        leaderboard: {
            rapid: Array<User>;
            blitz: Array<User>;
            bullet: Array<User>;
        };
        friendsOnline: Array<Friend>;
    }
    type gameType = 'bullet' | 'blitz' | 'rapid';
    type QueueList = {
        queues: Array<
            {
                id: string;
                avatar: string;
                nickname: string;
                time: number;
                increment: number;
                rating: number;
            }>;
    }


    type GameSummary = {
        id: string;
        whitePlayer: {nickname: string;avatar: string;};
        blackPlayer: {nickname: string;avatar: string;};
        gameType: string;
        time: number;
        increment: number;
        status: string;
        date: string;
        moves: Array<MoveInfo>;
    };
}