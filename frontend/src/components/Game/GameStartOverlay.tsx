import { useEffect, useState } from "react";
const API_URL = import.meta.env.VITE_API_URL;

type Props = {
    whitePlayer: { nickname: string; avatar: string; rating?: number };
    blackPlayer: { nickname: string; avatar: string; rating?: number };
    onAnimationEnd: () => void;
};

export default function GameStartOverlay({ whitePlayer, blackPlayer, onAnimationEnd }: Props) {
    const [isVisible, setIsVisible] = useState(true);

    useEffect(() => {
        const timer = setTimeout(() => {
            setIsVisible(false);      
            setTimeout(onAnimationEnd, 1000);
        }, 1500);

        return () => clearTimeout(timer);
    }, [onAnimationEnd]);

    return (
        <div className="fixed inset-0 z-50 flex pointer-events-none">

            <div
                className={`flex flex-col items-center justify-center bg-gray-900/80 h-full w-1/2 transition-transform duration-1000 ease-in-out pointer-events-auto 
                    ${isVisible ? "translate-x-0" : "-translate-x-full"
                    }`}
            >
                <img
                    src={API_URL + whitePlayer.avatar}
                    alt={whitePlayer.nickname}
                    className="w-32 h-32 rounded-full outline-12 border-white shadow-lg mb-4"
                />
                <h2 className="text-3xl font-bold text-white">{whitePlayer.nickname}</h2>
                <p className="text-gray-400 text-xl">{whitePlayer.rating}</p>
            </div>


            <div
                className={`absolute left-1/2 top-1/2 transform -translate-x-1/2 -translate-y-1/2 text-6xl font-black text-yellow-500 z-50 transition-opacity duration-500 
                    ${isVisible ? "opacity-100" : "opacity-0"
                    }`}
            >
                VS
            </div>

                <div
                    className={`flex flex-col items-center justify-center bg-gray-900/80 h-full w-1/2 transition-transform duration-1000 ease-in-out pointer-events-auto 
                        ${isVisible ? "translate-x-0" : "translate-x-full"
                        }`}
                >
                    <img
                        src={API_URL + blackPlayer.avatar}
                        alt={blackPlayer.nickname}
                        className="w-32 h-32 rounded-full outline-12 text-black shadow-lg mb-4"
                    />
                    <h2 className="text-3xl font-bold text-white">{blackPlayer.nickname}</h2>
                    <p className="text-gray-400 text-xl">{blackPlayer.rating}</p>
                </div>
        </div>
    );
}