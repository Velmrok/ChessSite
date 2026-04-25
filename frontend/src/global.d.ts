import type { Message } from "postcss";

export { };
declare global {



    type Message = {
        id: string;
        nickname: string;
        text: string;
        timestamp: string;

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


    
}