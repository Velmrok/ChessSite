import { makeMove } from "@/services/socket/socketGameService";
import useGameStore from "@/stores/useGameStore";
import useUserStore from "@/stores/useUserStore";
import { FindPiece } from "@/utils/findPiece";
import { Chess } from "chess.js";
import { useEffect, useRef, useState } from "react";
import type { PieceDropHandlerArgs } from "react-chessboard";
import { useSounds } from "./useSounds";

export function useChessboardLive() {
    const chessRef = useRef<Chess>(new Chess());
    const user = useUserStore((state) => state.user);
    const game = useGameStore((state) => state.game);
    const currentMoveIndex = useGameStore((state) => state.currentMoveIndex);
    const {playMove, playCapture, playCastle, playCheck} = useSounds();
    const [fen, setFen] = useState<string>("");
    const [isCheck, setIsCheck] = useState<boolean>(false);
    const [kingPosition, setKingPosition] = useState<string>("");
    const [allPossibleMoves, setAllPossibleMoves] = useState<Move[]>([]);
const [lastMove, setLastMove] = useState<Move | undefined>(undefined);
const isLive = game?.status === "active";
    useEffect(() => {
        if(!game) return;
        if(!isLive) return
    const c = new Chess();

    for (let i = 0; i <= currentMoveIndex; i++) {
      c.move(game.moves[i].move);
     
    }
    
    chessRef.current = c;
    const history = c.history({ verbose: true });
     const newLastMove = history.length > 0 ? history[history.length - 1] : undefined;
    setLastMove(newLastMove);
       
    
    if (newLastMove) {
      if(c.inCheck()){
        playCheck();
      }else if (newLastMove.isCapture()) {
            playCapture();
        } else if(newLastMove.isKingsideCastle() || newLastMove.isQueensideCastle()) {
            playCastle();
        }
        else {
            playMove();
        }
    }
    
    setLastMove(history.length > 0 ? history[history.length - 1] : undefined);
        setIsCheck(c.inCheck());
    setFen(c.fen());
    setKingPosition(FindPiece.getKingSquare(c, c.turn()));
    setAllPossibleMoves(c.moves({ verbose: true }));
   
  }, [currentMoveIndex, game]);








  function onPieceDrop({
      sourceSquare,
      targetSquare
    }: PieceDropHandlerArgs) {
      if(!game) return false;
      const userColor = user?.nickname === game.white.nickname ? 'white' : 'black';
      if(userColor !== (chessRef.current.turn() === 'w' ? 'white' : 'black')) {
        return false;

      }
      if (!targetSquare) {
        return false;
      }
      const chessGame = chessRef.current;

     
      try {
        const move = { from: sourceSquare, to: targetSquare, promotion: 'q' };

        chessGame.move(move);
        
        makeMove(game!.id,chessGame.history()[chessGame.history().length -1]);
        return true;
      } catch {
      
        return false;
      }
    }
    
    return { chess: chessRef.current, onPieceDrop, fen, kingPosition, allPossibleMoves,
       lastMove, isCheck, chessRef  };
}