import type { User } from "@/types/User";
import { FaWindowClose } from "react-icons/fa";

type Props = {
    endData: () => { title: string; reasonText: string };
    game: GameState;
    user: User;
    setGameJustEndedState: (state: boolean) => void;
}

export default function EndGameWindow({ endData, game, user, setGameJustEndedState }: Props) {
    return (
        <>
            <div className="absolute w-70 h-35 outline-4 inset-0 top-[50%] left-[50%] -translate-x-1/2 -translate-y-1/2 
                 rounded-md bg-gray-700 z-50 flex flex-col items-center justify-start pt-5 ">
                <span className="font-MyFancyFont text-2xl ">
                    {endData().title}
                </span>
                <span>{endData().reasonText}</span>
                <span className={`text-2xl mt-3 ${game.winnerNickname == user.nickname ? "text-green-500" : "text-red-500"} font-bold`}>
                    {game.winnerNickname == user.nickname ? "+10 elo" : game.winnerNickname ? "-10 elo" : "+0 elo"}</span>
                <button className="absolute top-2 right-2 text-red-500 hover:scale-110 text-2xl "
                    onClick={() => setGameJustEndedState(false)}><FaWindowClose /></button>
            </div>
            <div className="absolute inset-0 my-3 bg-black/30 z-40 flex flex-col items-center justify-center pointer-events-none" />
        </>
    )
}