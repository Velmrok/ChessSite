import {invokeSignalR} from '../signalR/connection';

export const joinGameRoom = (gameId: string) => {
    return invokeSignalR('JoinGameRoom', gameId);
}
export const sendMessage = (text: string, gameId: string) => {
       return invokeSignalR('SendGameMessage', gameId, text);
    };
export const surrenderGame = (gameId: string) => {
    return invokeSignalR('SurrenderGame', gameId);
  }
  export const offerDraw = (gameId: string) => {
    return invokeSignalR('OfferDraw', gameId);
  }

export const makeMove = (gameId: string, move: string) => {
    return invokeSignalR('MakeMove', gameId, move);
}