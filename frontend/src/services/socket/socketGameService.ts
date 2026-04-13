import { socket } from "./socketService";

export const joinGameRoom = (gameId: string) => {
    socket.emit('game:join', { gameId });
}
export const sendMessage = (text: string, gameId: string) => {
        socket.emit('chat:send_message', { gameId, message: text });
    };
export const surrenderGame = (gameId: string) => {
    socket.emit('game:surrender', {gameId});
  }
  export const offerDraw = (gameId: string) => {
    socket.emit('game:offer_draw', {gameId});
  }

export const makeMove = (gameId: string, move: string) => {
    socket.emit('game:make_move', { gameId, move });
}