

import GameForm from "@/components/FindGame/GameForm";
import ChessBoard from "../components/FindGame/ChessBoard";
import PlayerBar from "@/components/FindGame/PlayerBar";
import useUserStore from "@/stores/useUserStore";
import { useState } from "react";

export default function FindGame() {
    const user = useUserStore((state) => state.user);
    const [currentTime, setCurrentTime] = useState("10+0");

    if (!user || typeof (user.rating) == 'number') throw new Error("Error on FindGame that doesnt make any sense");
    const gameType = (): "bullet" | "blitz" | "rapid" => {
        const base = currentTime.split("+").map(Number)[0];
        if (base < 3) return 'bullet';
        if (base < 10) return 'blitz';
        return 'rapid';
    }

    return (
        <div className="flex  justify-center  h-auto bg-cyan-800 text-white p-4 overflow-hidden ">
            <div className="bg-cyan-900 rounded-md h-auto shadow-md  justify-center mb-20 pb-15
            w-full w-[95%]  md:w-[90%] lg:w-[80%] max-w-[1300px] flex flex-col xl:flex-row items-center
             items-center xl:items-start overflow-hidden xl:gap-5">
                <div className="h-full max-h-[800px] w-full xl:w-[70%]  max-w-[650px] xl:min-w-[500px]
                 flex flex-col items-center justify-center mt-5">

                    <PlayerBar nickname="Opponent" avatarUrl="/default-avatar.webp"
                        time={currentTime} withLink={false} />
                    <ChessBoard />
                    <PlayerBar nickname={user.nickname} avatarUrl={user.profilePictureUrl} withLink={true}
                        rating={user.rating[gameType()]} time={currentTime} />
                </div>
                <div className="h-full w-[70%] xl:w-[30%] flex items-start justify-center xl:mt-9">
                    <GameForm currentTime={currentTime} setCurrentTime={setCurrentTime} />
                </div>
            </div>
        </div>
    );
}