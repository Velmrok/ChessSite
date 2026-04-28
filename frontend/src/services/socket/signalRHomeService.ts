import type { SignalRRequest } from '@/types/signalR';
import {invokeSignalR} from '../signalR/connection';

export const joinHomeGroup = (request : SignalRRequest) => {
   return  invokeSignalR('JoinHomeGroup', request);
};