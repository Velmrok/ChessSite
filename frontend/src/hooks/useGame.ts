
import useLanguageStore from "@/stores/useLanguageStore";
import useToastStore from "@/stores/useToastStore";
import {socket} from '../services/socket/socketService';
import { useNavigate, useParams } from "react-router-dom";

import { useEffect, useState } from "react";

import useGameStore from "@/stores/useGameStore";
import useUserStore from "@/stores/useUserStore";
import { joinGameRoom } from "@/services/socket/socketGameService";


export default function useGame() {
    const gameId = useParams().gameId!;
    
    const setToast = useToastStore((state) => state.setToast);
    const t = useLanguageStore((state) => state.t);
  const fetchGame = useGameStore((state) => state.fetchGame);
  const game = useGameStore((state) => state.game);
  const currentMoveIndex = useGameStore((state) => state.currentMoveIndex);
  const setCurrentWhiteTime = useGameStore((state) => state.setCurrentWhiteTime);
  const setCurrentBlackTime = useGameStore((state) => state.setCurrentBlackTime);
  const [gameJustEnded, setGameJustEnded] = useState(false);
  const [endData, setEndData] = useState<{winnerNickname: string; reason: string} | null>(null);
  const setGame = useGameStore((state) => state.setGame);
 
 const pushMove = useGameStore((state) => state.pushMove);
  const user = useUserStore((state) => state.user);
  

  const navigate = useNavigate();

  useEffect(() => {

    const fetch = async () => {
      
      try {
        joinGameRoom(gameId);
        setGameJustEnded(false);
        await fetchGame(gameId);
        
        
      } catch (error) {
        setToast({ msg: t.toast.error.generic, type: "error" });
      }
     
    };
    fetch();
   
     socket.on('game:end', (data: { gameId: string;winner: string; reason: string }) => {
        if (data.gameId === gameId) {
            setTimeout(() => {
                useGameStore.getState().endGame(data.winner);
                setGameJustEnded(true);
                setEndData({winnerNickname: data.winner, reason: data.reason });
            }, 500);
        }
      }
      );
      socket.on('game:move_made', (data:{gameId: string, move: MoveInfo}) => {
          if (data.gameId === gameId) {
              pushMove(data.move);
          }

        });

      socket.on('game:draw_offered', (data:{gameId:string, isDrawOffered: string | null}) => {
        const currentGame = useGameStore.getState().game;
        console.log(currentGame);
          if(data.gameId === gameId && currentGame){
            console.log("Draw offered event received for game:", data.gameId);
          
              setGame({...currentGame, isDrawOffered: data.isDrawOffered});
              setToast({ msg: t.toast.info.drawOffered, type: "info" });
          }
    });
    return () => {
      socket.off('game:end');
      socket.off('game:move_made');
      socket.off('game:draw_offered');
      socket.emit('game:leave_room', { gameId });
    };
  }, [gameId]);

  useEffect(() => {
    
     if (!game) return;
     const newCurrentTurn = currentMoveIndex % 2 === 0 ? "white" : "black";
     
     if(game.status == "live") return;
         if (currentMoveIndex < 0) {
            setCurrentWhiteTime(game.time * 60 * 1000);
            setCurrentBlackTime(game.time * 60 * 1000);
            return;
        }
        const move = game.moves[currentMoveIndex];
        const previousMove = game.moves[currentMoveIndex - 1];
        if (!move) return;
        
       
        if (currentMoveIndex === 0) {
            setCurrentWhiteTime(move.absoluteTime);
            setCurrentBlackTime(game.time * 60 * 1000);
            return;
        }
        if (currentMoveIndex % 2 === 0) {
            setCurrentWhiteTime(move.absoluteTime);
            setCurrentBlackTime(previousMove.absoluteTime);
            
        } else {
            setCurrentBlackTime(move.absoluteTime);
            setCurrentWhiteTime(previousMove.absoluteTime);
            
        }

  }, [currentMoveIndex, game]);

  const formatEndData = () => {
    if(!endData) return {title: "", reasonText: ""};
    const winnerNickname = endData.winnerNickname;
    const reason = endData.reason;
    
    const title = !user? winnerNickname +" " + t.game.title.won : //user=observer 
    winnerNickname? winnerNickname == user.nickname ? t.game.title.youWon //user won
    : t.game.title.youLost : //user lost
     t.game.title.draw; // winnerNikname == null => draw
      
    const reasonText = reason === 'checkmate' ? t.game.reason.checkmate :
     reason === 'surrender' ? t.game.reason.surrender :
      reason === 'timeout' ? t.game.reason.timeout : 
      reason === 'threefold_repetition' ? t.game.reason.threefoldRepetition :
       reason === 'insufficient_material' ? t.game.reason.insufficientMaterial :
        reason === 'agreement' ? t.game.reason.agreement :
          reason === 'stalemate' ? t.game.reason.stalemate :
            reason === 'disconnect' ? t.game.reason.disconnect : '';
    return {title, reasonText};
  }


  return {gameJustEnded, formatEndData};
}
