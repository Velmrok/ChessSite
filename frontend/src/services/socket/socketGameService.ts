import type { SignalRRequest } from '@/types/signalR';
import {invokeSignalR} from '../signalR/connection';

export const joinGameRoom = (request : SignalRRequest) => {
    return invokeSignalR('JoinGameRoom', request);
}
export const sendMessage = (request : SignalRRequest) => {
       return invokeSignalR('SendGameMessage', request);
    };
export const surrenderGame = (request : SignalRRequest) => {
    return invokeSignalR('SurrenderGame', request);
  }
  export const offerDraw = (request : SignalRRequest) => {
    return invokeSignalR('OfferDraw', request);
  }

export const makeMove = (request : SignalRRequest) => {
    return invokeSignalR('MakeMove', request);
}