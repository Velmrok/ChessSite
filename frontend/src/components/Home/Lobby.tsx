import useHomeStore from "@/stores/useHomeStore";
import {  useEffect } from "react";
import Loading from "../global/Loading";
import useLanguageStore from "@/stores/useLanguageStore";
import {socket} from '../../services/socket/socketService';
import { MdAccessTime } from "react-icons/md";
import { SiPushbullet, SiStackblitz } from "react-icons/si";
import { IoIosInfinite } from "react-icons/io";

export default function Lobby() {
    const queueList = useHomeStore((state) => state.queueList);
    const fetchQueueList = useHomeStore((state) => state.fetchQueueList);
    const deletedQueues = useHomeStore((state) => state.deletedQueues);
    const t = useLanguageStore((state) => state.t);
    const gameType = useHomeStore((state) => state.lobbyGameType);
    const setLobbyGameType = useHomeStore((state) => state.setLobbyGameType);

    const handleChangeGameType = async (e: any, type: gameType | 'any') => {
        e.preventDefault();
        setLobbyGameType(type);
        await fetchQueueList(type);
    }
    useEffect(() => {
        socket.emit('join:lobbyRoom');
        const loadQueueList = async () => {
            await fetchQueueList('any');


        }
        loadQueueList();
        return () => {
            socket.emit('leave:lobbyRoom');
        }
    }, [fetchQueueList]);

    if (!queueList) {
        return (
            <div className="bg-black/[50%] w-full h-[400px] text-white p-4 rounded-lg shadow-lg">
                <Loading></Loading>
            </div>
        )
    }
    const gameTypeIcon = {
        rapid: <MdAccessTime className="text-green-500 text-base md:text-xl inline " />,
        blitz: <SiStackblitz className="text-yellow-300 text-base md:text-xl inline" />,
        bullet: <SiPushbullet className="text-red-500 text-base md:text-xl inline" />,
        any: <IoIosInfinite className="text-white text-base md:text-xl inline" />
    }

    return (
        <div className="bg-black/[20%] h-full max-h-[800px] flex flex-col gap-4 w-full text-white p-4 rounded-lg shadow-lg ">
            <div className="w-full text-center text-2xl">{t.home.quickMenu.lobby}</div>
            <div className="flex gap-2 md:gap-3 justify-center">
                <button className={`hover:scale-110 ${gameType === 'rapid' ? 'scale-80 border-b-3 border-green-500' : ''}`}
                    onClick={(e) => handleChangeGameType(e, 'rapid')}>
                    {gameTypeIcon.rapid}</button>
                <button className={`hover:scale-110 ${gameType === 'blitz' ? 'scale-80 border-b-3 border-yellow-300' : ''}`}
                    onClick={(e) => handleChangeGameType(e, 'blitz')}>
                    {gameTypeIcon.blitz}</button>
                <button className={`hover:scale-110 ${gameType === 'bullet' ? 'scale-80 border-b-3 border-red-500' : ''}`}
                    onClick={(e) => handleChangeGameType(e, 'bullet')}>
                    {gameTypeIcon.bullet}</button>
                <button className={`hover:scale-110 ${gameType === 'any' ? 'scale-80 border-b-3 border-white' : ''}`}
                    onClick={(e) => handleChangeGameType(e, 'any')}>
                    {gameTypeIcon.any}</button>


            </div>

            <table className="w-full border-separate" style={{ borderCollapse: 'separate', borderSpacing: '0 12px' }}>
                <thead className="w-full">
                    <tr>
                        <th className="text-center w-[33%]">{t.home.lobby.player}</th>
                        <th className="text-center w-[33%]">{t.home.lobby.rating}</th>
                        <th className="text-center w-[33%]">{t.home.lobby.time}</th>
                    </tr>
                </thead>
                {queueList.queues.length === 0 ?
                    <tbody>
                        <tr>

                            <td colSpan={3} className="text-center p-4 pt-10">{t.home.lobby.noPlayersInQueue}</td>


                        </tr>
                    </tbody>
                    :

                    <tbody className="w-full">
                        {queueList.queues.map((queue) => {
                            let disabledClass = ''
                            if (deletedQueues.some(id => id === queue.id)) {
                                disabledClass = 'opacity-50 line-through';
                            }
                            return (

                                <tr key={queue.id} className={`bg-black/[25%] hover:bg-black/[50%]  rounded-md ${disabledClass}`}>
                                    <td className="text-center w-[33%] p-3">{queue.nickname}</td>
                                    <td className="text-center w-[33%] p-3">{queue.rating}</td>
                                    <td className="text-center w-[33%] p-3">{queue.time}+{queue.increment}</td>
                                </tr>
                            )
                        }
                        )}

                    </tbody>}
            </table>
        </div>
    );
}