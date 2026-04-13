

import MovesHistory from "@/components/Game/MovesHistory";
import PlayerBar from "@/components/Game/PlayerBar";
import Chat from "@/components/Game/Chat";
import { useParams } from "react-router-dom";
import useGameStore from "@/stores/useGameStore";
import Loading from "../components/global/Loading";
import { Buttons } from "@/components/Game/Buttons";
import { surrenderGame, offerDraw } from '@/services/socket/socketGameService';
import useGame from "@/hooks/useGame";
import useKeyboardKeys from "../hooks/useKeyboardKeys";
import { useEffect, useState } from "react";
import useUserStore from "@/stores/useUserStore";
import ChessBoard from "@/components/Game/Chessboard";
import useAnalysisGame from "@/hooks/useAnalysiGame";
import useLiveGame from "@/hooks/useLiveGame";
import GameStartOverlay from "@/components/Game/GameStartOverlay";

export default function GamePage() {
    const gameId = useParams().gameId!;
    
    const currentBlackTime = useGameStore((state) => state.currentBlackTime);
    const currentWhiteTime = useGameStore((state) => state.currentWhiteTime);
    const user = useUserStore((state) => state.user);
    const isLive = useGameStore((state) => state.game?.status === "live");
    const isInAnalysisTree = useGameStore((state) => state.isInAnalysisTree);
    const gameHasJustStarted = useGameStore((state) => state.gameHasJustStarted);
    const resetGame = useGameStore((state) => state.resetGame);
    const game = useGameStore((state) => state.game);
    const [orientation, setOrientation] = useState<"white" | "black">("white");
    const { gameJustEnded, formatEndData } = useGame();
    const pushAnalysisMove = useGameStore((state) => state.pushAnalysisMove);
    const { setCurrentAnalysisMoveIndex } = useAnalysisGame();
    const { setCurrentMoveIndex } = useLiveGame();
    useEffect(() => {
        
        return () => {
            resetGame();
        }
    }, []);

    const left = ()=>{
        if(!isInAnalysisTree) setCurrentMoveIndex(-1);
        else setCurrentAnalysisMoveIndex(-1);
    }
    const right = ()=>{
        if(!isInAnalysisTree) setCurrentMoveIndex(1);
        else setCurrentAnalysisMoveIndex(1);
    }
    useKeyboardKeys({ 
        onLeft:  left,
        onRight: right, 
         onfKey: () => setOrientation((prev) => prev === "white" ? "black" : "white") 
        });   
    
    useEffect(() => {
        
        if (game) {
            if (user) {
                if (user.nickname === game.black.nickname) {
                    setOrientation("black");
                } else {
                    setOrientation("white");
                }
            } else {
                setOrientation("white");
            }
        }
    }, [game, user]);

    if(!game){
        return (
            <div className="flex justify-center items-center h-screen bg-cyan-800">
                <Loading />;
            </div>
        
        )
    }
        
    

            
        
    
    return (
        <div className="flex  justify-center  h-auto bg-cyan-800 text-white p-4 overflow-hidden ">
            {gameHasJustStarted && <GameStartOverlay whitePlayer={game.white} blackPlayer={game.black} onAnimationEnd={() => useGameStore.getState().setGameHasJustStarted(false)} />}


            <div className="bg-cyan-900 rounded-md h-auto shadow-md  justify-center mb-10 pb-15
            w-full w-[95%]  md:w-[90%] lg:w-[90%] max-w-[1300px] flex flex-col xl:flex-row items-center
             items-center xl:items-start overflow-hidden xl:gap-5">
                <div className="h-full max-h-[800px] w-full xl:w-[70%]  max-w-[650px] xl:min-w-[500px]
                 flex flex-col items-center justify-center mt-5">

                {orientation === "black" ?
                <>
                <PlayerBar nickname={game.white.nickname} avatarUrl={game.white.avatar} withLink={true}
                 rating={game.white.rating} time={currentWhiteTime}/>
                
                <ChessBoard game={game} boardOrientation="black" gameJustEnded={gameJustEnded} endData={formatEndData}
                pushAnalysisMove={pushAnalysisMove}  />
                
                <PlayerBar nickname={game.black.nickname} avatarUrl={game.black.avatar} withLink={true}
                 rating={game.black.rating} time={currentBlackTime}/>
                
                 </>
                    :
                    <>
                <PlayerBar nickname={game.black.nickname} avatarUrl={game.black.avatar} withLink={true}
                 rating={game.black.rating} time={currentBlackTime}/>
                
                <ChessBoard game={game} boardOrientation="white" gameJustEnded={gameJustEnded} endData={formatEndData}
                 pushAnalysisMove={pushAnalysisMove}/>
                
                <PlayerBar nickname={game.white.nickname} avatarUrl={game.white.avatar} withLink={true}
                 rating={game.white.rating} time={currentWhiteTime}/>
                 </>
                }
                
                </div>
                <div className="h-full w-[70%] w-full xl:w-[33%] flex items-start justify-center xl:mt-9">
                <div className="bg-black/20 rounded-md shadow-md   p-6 mt-3 xl:mt-0
            flex flex-col items-center w-full gap-6 h-full max-h-[770px] ">
                <MovesHistory moves={game.moves} time={game.time} />
                <Buttons />
                {isLive && <Chat previousMessages={game.messages}/>}

            </div>
                </div>
            </div>
        </div>
    );
}