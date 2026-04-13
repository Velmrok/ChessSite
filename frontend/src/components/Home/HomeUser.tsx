
import {  useEffect, useState } from "react";
import QuickQueue from "./QuickQueue";
import useHomeStore from "@/stores/useHomeStore";
import { socket } from "../../services/socket/socketService";
import QuickMenu from "./QuickMenu";
import FriendsOnline from "./FriendsOnline";
import LeaderBoard from "./Leaderboard";
import { getHomeInfo } from "@/services/homeService";
import Lobby from "./Lobby";
import useLanguageStore from "@/stores/useLanguageStore";
import { useHomeSocket } from "@/hooks/useHomeSocket";
import { useNavigate } from "react-router-dom";

import AdminPanel from "./AdminPanel";
import useUserStore from "@/stores/useUserStore";

export default function HomeUser() {
    const t = useLanguageStore((state) => state.t);
    const user = useUserStore((state) => state.user);
    const usersOnline = useHomeStore((state) => state.usersOnline);
    const matchesInProgress = useHomeStore((state) => state.matchesInProgress);
    const createdAccounts = useHomeStore((state) => state.createdAccounts);
    const queueSize = useHomeStore((state) => state.queueSize);
    const [leaderboard, setLeaderboard] = useState<{ rapid: Array<User>; blitz: Array<User>; bullet: Array<User>; }>({ rapid: [], blitz: [], bullet: [] });
    const [qmViewMode, setQmViewMode] = useState<string>('queue');
    const navigate = useNavigate();
    useHomeSocket();
     useEffect(() => {
        try {
            if (!socket.connected) {
                socket.connect();
            }
            const fetchHomeInfo=async()=>{
                const data = await getHomeInfo();
                setLeaderboard(data.leaderboard);
            }
            fetchHomeInfo();
        } catch (error) {
            console.error(error);
        }
        }, []);

  
    const changeQMViewMode=(mode:string)=>{
        return () => setQmViewMode(mode);
    }


    return (
        <div className="w-full min-h-screen flex flex-col justify-center gap-5   items-center relative">
           {user?.role === 'admin' && <AdminPanel />}
            <div className="mt-5 flex justify-between w-full  md:w-[80%] lg:w-[83%] max-w-[1300px] px-4">
            <div className="w-[33%] text-[10px] md:text-base text-white text-center font-MyFancyFont">{usersOnline} {t.home.playersOnline}</div>
            <div className="w-[33%] text-[10px] md:text-base text-white text-center font-MyFancyFont">{matchesInProgress} {t.home.matchesInProgress}</div>
            <div className="w-[33%] text-[10px] md:text-base text-white text-center font-MyFancyFont">{createdAccounts} {t.home.createdAccounts}</div>
            <div className="w-[33%] text-[10px] md:text-base text-white text-center font-MyFancyFont">{queueSize} {t.home.queueSize}</div>
            </div>
            <div className="grid xl:grid-cols-2 gap-6 mb-10 min-h-screen w-[90%]  md:w-[80%] lg:w-[83%]
                 max-w-[1300px]  bg-cyan-900 rounded-lg shadow-lg p-6 relative
                transition-all duration-500 ">
                <div className="hidden xl:grid w-full my-5  grid-cols-1 grid-rows-10 place-items-center text-white font-MyFancyFont gap-10">          
                    <span className="text-4xl">{t.home.welcome}</span>
                    <div className="flex flex-col items-center gap-4">
                        <span>{t.home.wantToPlay}</span>
                        <button onClick={()=>navigate('/find-game')}
                         className="bg-cyan-500/60 px-8 py-2 rounded hover:bg-cyan-500 hover:scale-105 transition-all">{t.home.findMatch}</button>
                    </div>
                    <div className="flex flex-col items-center gap-4">
                        <span>{t.home.lookingForSomeone}</span>
                        <button onClick={()=>navigate('/search')}
                         className="bg-cyan-500/60 px-8 py-2 rounded hover:bg-cyan-500
                         hover:scale-105 transition-all">{t.home.findMatch}</button>
                         
                    </div>
                    <div className="flex flex-col items-center gap-4">
                        <span>{t.home.wantToFindGame}</span>
                        <button onClick={()=>navigate('/games')}
                         className="bg-cyan-500/60 px-8 py-2 rounded hover:bg-cyan-500
                         hover:scale-105 transition-all">{t.home.find}</button>
                         
                    </div>
                    
                    

                </div>
                
                <div className="max-w-[100%] overflow-x-auto flex flex-col text-white font-MyFancyFont  items-center gap-3">
                <QuickMenu buttons={[
                    changeQMViewMode('queue'),
                    changeQMViewMode('leaderboard'),
                    changeQMViewMode('friends'),
                    changeQMViewMode('lobby')]}
                    qmViewMode={qmViewMode}
                    />
                {qmViewMode==='queue' && <QuickQueue />}
                {qmViewMode==='friends' && <FriendsOnline  />}
                {qmViewMode==='leaderboard' && <LeaderBoard leaderboard={leaderboard} />}
                {qmViewMode==='lobby' && <Lobby />}
                </div>
            
            
            </div>
        
        </div>
    )
}
