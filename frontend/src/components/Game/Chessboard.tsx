import { Chessboard } from "react-chessboard";
import { useChessboardInteraction } from "@/hooks/useChessboardInteraction";
import { useEffect, useState } from "react";
import useUserStore from "@/stores/useUserStore";
import EndGameWindow from "./EndGameWindow";
import { useTranslation } from "react-i18next";
import type { GameState } from "@/types/game";

type Props = {
  game: GameState;
  boardOrientation?: "white" | "black";
  gameJustEnded?: boolean;
  endData: () => { title: string; reasonText: string };
  pushAnalysisMove: (move: string) => void;

};

export default function ChessBoard({ game, boardOrientation = "white", gameJustEnded, endData, pushAnalysisMove }: Props) {
  const {
    live,
    analysis,
    isLive,
    squareStyles,
    onSquareClick,
    onSquareRightClick,
    animationSpeed,
    isInAnalysisTree
  } = useChessboardInteraction({ game, pushAnalysisMove });
  const user = useUserStore((state) => state.user);
  const { t } = useTranslation("global");
  const isConnected = useUserStore((state) => state.isConnected);
  const [gameJustEndedState, setGameJustEndedState] = useState(gameJustEnded);

  useEffect(() => {
    setGameJustEndedState(gameJustEnded);
  }, [gameJustEnded]);

  const options = {
    position: isLive ? live.fen : analysis.fen,
    boardOrientation: boardOrientation,
    onPieceDrop: isLive ? live.onPieceDrop : analysis.onPieceDrop,
    animationDurationInMs: animationSpeed,
    squareStyles,
    onSquareRightClick,
    //onSquareMouseDown: onSquareClick,
    onSquareClick
  };


  return (
    <>

      <div className="w-full min-w-[330px] py-3 relative">
        {!isConnected && isLive &&
          <div className="absolute inset-0 bg-black/40 my-3 z-50 flex flex-col items-center justify-center pointer-events-none">
            <span className="text-5xl font-MyFancyFont text-white-700 animate-pulse">
              {t('loadingMessage')}
            </span>
            <div className="border-b-4 mt-5 rounded-full animate-spin w-15 h-15"></div>
          </div>

        }
        <Chessboard options={options} />
        {isInAnalysisTree && (
          <div className="absolute inset-0 my-3 bg-gray-400/30 z-50 flex flex-col items-center justify-center pointer-events-none" />

        )}
        {gameJustEndedState && user &&
          <EndGameWindow endData={endData} game={game} user={user} setGameJustEndedState={setGameJustEndedState} />
        }

      </div>
    </>
  );
}