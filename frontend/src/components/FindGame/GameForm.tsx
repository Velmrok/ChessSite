import useQueue from "@/hooks/useQueue";
import { formatTimeFromMs } from "@/services/socket/signalRGlobalService";
import useUserStore from "@/stores/useUserStore";
import { useTranslation } from "react-i18next";
import { MdAccessTime } from "react-icons/md";
import { SiPushbullet, SiStackblitz } from "react-icons/si";


type Props = {
    currentTime: string;
    setCurrentTime: (time: string) => void;
}
export default function GameForm({ currentTime, setCurrentTime }: Props) {
    const { t } = useTranslation('game');
    const isInQueue = useUserStore((state) => state.user?.isInQueue);
    const queueTime = useUserStore((state) => state.queueTime);
    const gameTypeIcon = {
        rapid: <MdAccessTime className="text-green-500 text-base md:text-xl inline" />,
        blitz: <SiStackblitz className="text-yellow-300 text-base md:text-xl inline" />,
        bullet: <SiPushbullet className="text-red-500 text-base md:text-xl inline" />,
    }
    const isGameType = (s: string) =>
        s === "rapid" || s === "blitz" || s === "bullet";
    const handleSetCurrentTime = (e: any, time: string) => {
        e.preventDefault();
        if (isInQueue) {
            return;
        }
        setCurrentTime(time);
    }
    const { handleJoinGame, handleCancelGame } = useQueue();
    const handleButtonClick = () => {
        if (isInQueue) {
            handleCancelGame();

        } else {
            handleJoinGame({ type: "Queue", correlationId: crypto.randomUUID(), payload: { timeControl: parseInt(currentTime.split('+')[0]), increment: parseInt(currentTime.split('+')[1]) } });
        }
    }

    return (
        <div className="bg-black/20 rounded-md shadow-md   p-6 mt-3 xl:mt-0
            flex flex-col items-center w-full gap-6 h-full max-h-[770px] ">
            <button onClick={handleButtonClick}
                className="bg-cyan-500/60 w-[90%] py-2 rounded hover:bg-cyan-500
                         transition-all font-MyFancyFont">{isInQueue ? `${t('cancel')} ( ${formatTimeFromMs(queueTime!)} )` : `${t('startGame')}`}</button>

            <div className="grid grid-cols-3 place-items-center gap-4 w-full ">
                {["bullet", "1+0", "1+1", "1+2", "2+0", "2+1", "2+2",
                    "blitz", "3+0", "3+1", "3+2", "5+0", "5+3", "5+5",
                    "rapid", "10+0", "10+2", "15+0", "15+5", "30+0", "30+15"].map((text) => (

                        !isGameType(text) ? (
                            <button
                                key={text}
                                onClick={(e) => handleSetCurrentTime(e, text)}
                                className={`${currentTime === text ? "outline-2 outline-amber-400" : "bg-cyan-700"} w-full flex items-center
                         justify-center transition-colors duration-300 bg-cyan-700 hover:bg-cyan-500 
                         text-white font-MyFancyFont text-base md:text-lg rounded-sm `}>
                                {text}
                            </button>)
                            : (
                                <div className="col-span-3 mt-3 mb-3 w-full font-MyFancyFont flex gap-3 items-center
                        ">{gameTypeIcon[text]} {t(text)}</div> 
                            )
                    ))}
            </div>
        </div>
    )
}