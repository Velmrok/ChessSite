import useToastStore from "@/stores/useToastStore";
import { useParams } from "react-router-dom";

import { useEffect, useState } from "react";

import useGameStore from "@/stores/useGameStore";
import useUserStore from "@/stores/useUserStore";
import { joinGameRoom } from "@/services/socket/socketGameService";
import { useTranslation } from "react-i18next";
import {invokeSignalR, leaveEvent, signUpForEvent} from "@/services/signalR/connection";
import {v4 as uuid} from "uuid";
import { useApi } from "./useApi";
import { fetchGame } from "@/services/gameService";
import type { DrawOfferedResponse, GameEndedResponse, GameState, MoveInfo } from "@/types/game";
import type { SignalRResponse } from "@/types/signalR";

export default function useGame() {
  const gameId = useParams().gameId!;

  const setToast = useToastStore((state) => state.setToast);
  const { t: toastT } = useTranslation('toast');
  const { t } = useTranslation('game');
  const game = useGameStore((state) => state.game);
  const isWhiteTurn = useGameStore((state) => state.game);
  const currentMoveIndex = useGameStore((state) => state.currentMoveIndex);
  const setCurrentWhiteTime = useGameStore((state) => state.setCurrentWhiteTime);
  const setCurrentBlackTime = useGameStore((state) => state.setCurrentBlackTime);
  const [gameJustEnded, setGameJustEnded] = useState(false);
  const [endData, setEndData] = useState<{ winnerNickname: string; reason: string } | null>(null);

  const setGame = useGameStore((state) => state.setGame);
  const setCurrentMoveIndex = useGameStore((state) => state.setCurrentMoveIndex);
  const setAnalysisMoves = useGameStore((state) => state.setAnalysisMoves);
  const setCurrentAnalysisMoveIndex = useGameStore((state) => state.setCurrentAnalysisMoveIndex);
  const setIsInAnalysisTree = useGameStore((state) => state.setIsInAnalysisTree);
  const endGame = useGameStore((state) => state.endGame);

  const pushMove = useGameStore((state) => state.pushMove);
  const user = useUserStore((state) => state.user);
  const {request} = useApi();

  const [isLoading, setIsLoading] = useState(true);

  useEffect(() => {

    const fetch = async () => {
        setIsLoading(true);
        const joinGameGroupResponse = request(() => joinGameRoom({type: "Game:"+gameId, correlationId: uuid()}));
        setGameJustEnded(false);
        const response = await request<GameState>(()=>fetchGame(gameId));
        if (response) {
            setGame(response);
            setCurrentMoveIndex(() => response.moves.length - 1);
            setCurrentWhiteTime(response.currentWhiteTime);
            setCurrentBlackTime(response.currentBlackTime);
            setAnalysisMoves([]);
            setCurrentAnalysisMoveIndex((x) => x-1);
            setIsInAnalysisTree(false);
           
        }
        setIsLoading(false);

      

    };
    fetch();
    signUpForEvent('GameEnded', (response: SignalRResponse<GameEndedResponse>) => {
      const data = response.data;
    
      if (!data) {
        console.error("Received GameEnded event with null data");
        return;
      }
      
      if (data.gameId === gameId) {
        setTimeout(() => {
          endGame(data.winner);
          setGameJustEnded(true);
          setEndData({ winnerNickname: data.winner, reason: data.reason });

        }, 500);
      }
    }
    );
    signUpForEvent('MoveMade', (response: SignalRResponse<MoveInfo>) => {
      const data = response.data;
      if (!data) {
        console.error("Received MoveMade event with null data");
        return;
      }
      
      if (response.correlationId === gameId) {
        pushMove(data);
      }

    });

    signUpForEvent('DrawOffered', (response: SignalRResponse<DrawOfferedResponse>) => {
      const data = response.data;
      if (!data) {
        console.error("Received DrawOffered event with null data");
        return;
      }

      if (data.gameId === gameId && game) {
        console.log("Draw offered event received for game:", data.gameId);

        setGame({ ...game, drawOfferedBy: data.drawOfferedBy });
        setToast({ msg: toastT('info.drawOffered'), type: "info" });
      }
    });
    return () => {
      leaveEvent('GameEnded');
      leaveEvent('MoveMade');
      leaveEvent('DrawOffered');
      invokeSignalR('LeaveGroup', { type: "Game:"+gameId, correlationId: crypto.randomUUID() });
    };
  }, [gameId]);

  useEffect(() => {

    if (!game) return;

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
    if (!endData) return { title: "", reasonText: "" };
    const winnerNickname = endData.winnerNickname;
    const reasonText = endData.reason;

    const title = !user ? winnerNickname + " " + t('won') : //user=observer 
      winnerNickname ? winnerNickname == user.nickname ? t('youWon') //user won
        : t('youLost') : //user lost
        t('draw'); // winnerNikname == null => draw

   
    return { title, reasonText};
  }


  return { gameJustEnded, formatEndData, isLoading };
}
