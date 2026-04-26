import { offerDraw, surrenderGame } from "@/services/socket/socketGameService";
import useGameStore from "@/stores/useGameStore";
import useUserStore from "@/stores/useUserStore";
import { useTranslation } from "react-i18next";
import { useParams } from "react-router-dom";

export function Buttons() {
    const gameId = useParams().gameId!;
    const user = useUserStore((state) => state.user);
    const { t } = useTranslation("game");
    const isLive = useGameStore((state) => state.game?.status === "active");
    const game = useGameStore((state) => state.game);

    if (!game || !user) return null;
    const isDrawOffered = game.isDrawOffered;
    const getPGN = () => {
        let grouped = []
        for (let i = 0; i < game.moves.length; i += 2) {
            const move = game.moves[i].move;
            if (game.moves[i + 1]) {
                const nextMove = game.moves[i + 1].move;
                grouped.push(`${move} ${nextMove} `);
            } else {
                grouped.push(`${move}`);
            }
        }
        return grouped.map((mv, index) => `${index + 1}. ${mv}`).join(" ");
    }

    const onSurrender = () => {
        surrenderGame(gameId);
    }
    const onOfferDraw = () => {
        offerDraw(gameId);
    }
    const onAcceptDraw = () => {
        offerDraw(gameId);
    }
    return (
        <>
            {game && isLive &&
                <div className="flex w-full">
                    <button
                        onClick={onSurrender}
                        className="bg-red-600 hover:bg-red-700 text-white font-bold py-1 px-3 rounded mr-4"
                    >
                        {t('surrender')}
                    </button>
                    {!isDrawOffered ?

                        <button
                            onClick={onOfferDraw}
                            className="bg-yellow-600 hover:bg-yellow-700 text-white font-bold py-1 px-3 rounded"
                        >
                            {t('offerDraw')}
                        </button> :
                        isDrawOffered !== user.nickname ?
                            <button
                                onClick={onAcceptDraw}
                                className="bg-green-600 hover:bg-green-700 text-white font-bold py-1 px-3 rounded animation-pulse"
                            >
                                {t('acceptDraw')}
                            </button> :
                            <div className="bg-yellow-800  text-white font-bold py-1 px-3 rounded animation-pulse">
                                {t('drawOffered')}</div>
                    }
                </div>

            }

            {game && !isLive &&
                <div className="w-full bg-black/25 h-[20%] flex flex-col items-center justify-center mt-2 ">
                    <div className="w-full h-full py-2 px-3 text-white overflow-y-auto ">
                        {getPGN()}
                    </div>
                </div>
            }
        </>
    );
}