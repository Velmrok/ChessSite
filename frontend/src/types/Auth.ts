import type { Rating } from "./user";

export type LoginFormType = {
    login: string;
    password: string;
}
export type RegisterFormType = {
    login: string;
    email: string;
    nickname: string;
    password: string;
}

export type GetMeResponse = {
    nickname: string;
    profilePictureUrl: string;
    createdAt: string;
    lastActive: string;
    rating: Rating;
    friendNicknames: Array<string>;
    queueData: QueueData;
    role: 'user' | 'admin';
}

export type QueueData = {
    isInQueue: boolean;
    time?: number;
    increment?: number;
    joinedQueueAt?: string;
}



    
