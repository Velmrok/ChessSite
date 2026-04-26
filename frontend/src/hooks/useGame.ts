import useToastStore from "@/stores/useToastStore";
import { useNavigate, useParams } from "react-router-dom";

import { useEffect, useState } from "react";

import useGameStore from "@/stores/useGameStore";
import useUserStore from "@/stores/useUserStore";
import { joinGameRoom } from "@/services/socket/socketGameService";
import { useTranslation } from "react-i18next";
import { getConnection , invokeSignalR} from "@/services/signalR/connection";


export default function useGame() {
  const gameId = useParams().gameId!;

  const setToast = useToastStore((state) => state.setToast);
  const { t: toastT } = useTranslation('toast');
  const { t } = useTranslation('game');
  const fetchGame = useGameStore((state) => state.fetchGame);
  const game = useGameStore((state) => state.game);
  const currentMoveIndex = useGameStore((state) => state.currentMoveIndex);
  const setCurrentWhiteTime = useGameStore((state) => state.setCurrentWhiteTime);
  const setCurrentBlackTime = useGameStore((state) => state.setCurrentBlackTime);
  const [gameJustEnded, setGameJustEnded] = useState(false);
  const [endData, setEndData] = useState<{ winnerNickname: string; reason: string } | null>(null);
  const setGame = useGameStore((state) => state.setGame);

  const pushMove = useGameStore((state) => state.pushMove);
  const user = useUserStore((state) => state.user);

  useEffect(() => {

    const fetch = async () => {

      try {
        joinGameRoom(gameId);
        setGameJustEnded(false);
        await fetchGame(gameId);


      } catch (error) {
        setToast({ msg: toastT('error.generic'), type: "error" });
      }

    };
    fetch();

    const conn = getConnection();

    conn.on('GameEnded', (data: { gameId: string; winner: string; reason: string }) => {
      if (data.gameId === gameId) {
        setTimeout(() => {
          useGameStore.getState().endGame(data.winner);
          setGameJustEnded(true);
          setEndData({ winnerNickname: data.winner, reason: data.reason });
        }, 500);
      }
    }
    );
    conn.on('MoveMade', (data: { gameId: string, move: MoveInfo }) => {
      if (data.gameId === gameId) {
        pushMove(data.move);
      }

    });

    conn.on('DrawOffered', (data: { gameId: string, isDrawOffered: string | null }) => {
      const currentGame = useGameStore.getState().game;
      console.log(currentGame);
      if (data.gameId === gameId && currentGame) {
        console.log("Draw offered event received for game:", data.gameId);

        setGame({ ...currentGame, isDrawOffered: data.isDrawOffered });
        setToast({ msg: toastT('info.drawOffered'), type: "info" });
      }
    });
    return () => {
      conn.off('GameEnded');
      conn.off('MoveMade');
      conn.off('DrawOffered');
      invokeSignalR('LeaveGameRoom', { gameId });
    };
  }, [gameId]);

  useEffect(() => {

    if (!game) return;


    if (game.status == "active") return;
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
    const reason = endData.reason;

    const title = !user ? winnerNickname + " " + t('won') : //user=observer 
      winnerNickname ? winnerNickname == user.nickname ? t('youWon') //user won
        : t('youLost') : //user lost
        t('draw'); // winnerNikname == null => draw

    const reasonText = reason === 'checkmate' ? t('checkmate') :
      reason === 'surrender' ? t('surrender') :
        reason === 'timeout' ? t('timeout') :
          reason === 'threefold_repetition' ? t('threefoldRepetition') :
            reason === 'insufficient_material' ? t('insufficientMaterial') :
              reason === 'agreement' ? t('agreement') :
                reason === 'stalemate' ? t('stalemate') :
                  reason === 'disconnect' ? t('disconnectEnd') : '';
    return { title, reasonText };
  }


  return { gameJustEnded, formatEndData };
}
