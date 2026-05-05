import type { Message } from "postcss";

export { };
declare global {



   
    type Move = {
        from: string;
        to: string;
        promotion?: string;
    };
   

    type Toast = "success" | "error" | "info";
    type GameStatus = "waiting" | "live" | "finished";

    
    
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