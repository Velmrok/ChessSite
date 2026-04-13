import { socket } from "./socketService";

export const leaveQueue = () => {
    socket.emit("queue:leave");
}
export const joinGame = ( time: number, increment: number) => {
    const gameType = time<3 ? 'bullet' : time<10 ? 'blitz' : 'rapid';
    socket.emit("queue:join", { gameType, time, increment });
}
export const rejoinQueue = () => {
    socket.emit("queue:rejoin");
    
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