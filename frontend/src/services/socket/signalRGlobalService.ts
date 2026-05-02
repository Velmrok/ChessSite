import type { SignalRRequest } from '@/types/signalR';
import {invokeSignalR} from '../signalR/connection';

export const leaveQueue = (request: SignalRRequest) => {
    return invokeSignalR('LeaveQueue', request);
}
export const joinQueue = ( request : SignalRRequest) => {

    return invokeSignalR('JoinQueue', request);
}
export const rejoinQueue = (request : SignalRRequest) => {
    return invokeSignalR('RejoinQueue', request);
}
export const formatTimeFromMs = (ms: number | undefined): string => {
    if (ms === undefined || ms < 0) {
        return "00:00";
    }
    const diffSec = Math.floor(ms / 1000);
    const minutes = Math.floor(diffSec / 60);
    const seconds = diffSec % 60;
    return `${minutes.toString().padStart(2, '0')}:${seconds.toString().padStart(2, '0')}`;
}