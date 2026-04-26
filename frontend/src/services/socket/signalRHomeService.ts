import {invokeSignalR} from '../signalR/connection';

export const joinHomeGroup = () => {
   return  invokeSignalR('JoinHomeGroup');
};