import useGameStore from "@/stores/useGameStore";
import { Chess, type Square } from "chess.js";
import { useEffect, useRef, useState } from "react";
import type { PieceDropHandlerArgs } from "react-chessboard";
import { FindPiece } from "@/utils/findPiece";
import { useSounds } from "./useSounds";
type Props = {
    pushAnalysisMove: (move: string) => void;
};

export function useChessboardAnalysis({ pushAnalysisMove }: Props) {
    const chessRef = useRef<Chess>(new Chess());
    
    const game = useGameStore((state) => state.game);
    const currentMoveIndex = useGameStore((state) => state.currentMoveIndex);
    const setCurrentMoveIndex = useGameStore((state) => state.setCurrentMoveIndex);
    const currentAnalysisMoveIndex = useGameStore((state) => state.currentAnalysisMoveIndex);
    const isInAnalysisTree = useGameStore((state) => state.isInAnalysisTree);
    const analysisMoves = useGameStore((state) => state.analysisMoves);
    const [fen, setFen] = useState<string>("");
    const {playMove, playCapture, playCastle, playCheck} = useSounds();
    const [isCheck, setIsCheck] = useState<boolean>(false);
    const [kingPosition, setKingPosition] = useState<string>("");
    const [allPossibleMoves, setAllPossibleMoves] = useState<Move[]>([]);
    
const [lastMove, setLastMove] = useState<Move | undefined>(undefined);
const isLive = game?.status === "active";
const prevStatusRef = useRef(game?.status);

useEffect(() => {
        if(!game) return; 
        
    const c = new Chess();
      setIsCheck(false);
      
    for (let i = 0; i <= currentMoveIndex; i++) {
        try{
            c.move(game!.moves[i].move);
            
        }catch{
            console.error("Invalid game move at index", i, ":", game!.moves[i].move);
        }
      
     
    }
    if(isInAnalysisTree){
        
        for (let i = 0; i <= currentAnalysisMoveIndex; i++) {
            try{
                c.move(analysisMoves[i]);
            }catch{
                console.error("Invalid analysis move at index", i, ":", analysisMoves[i]);
            }
           
        }
    }
    
    chessRef.current = c;
    const history = c.history({ verbose: true });
     const newLastMove = history.length > 0 ? history[history.length - 1] : undefined;
    setLastMove(newLastMove);
       
    
    

        setIsCheck(c.inCheck());
        setKingPosition(FindPiece.getKingSquare(c, c.turn()));
        setAllPossibleMoves(c.moves({ verbose: true }));
        setFen(c.fen());

        const prevStatus = prevStatusRef.current;
        prevStatusRef.current = game?.status;

       if (newLastMove && !isLive) {
            if (prevStatus === 'active' && game?.status === 'finished') {
            } else {
                if (c.inCheck()) {
                    playCheck();
                } else if (newLastMove.isCapture() || newLastMove.isEnPassant()) { 
                    playCapture();
                } else if (newLastMove.isKingsideCastle() || newLastMove.isQueensideCastle()) { 
                    playCastle();
                } else {
                    playMove();
                }
            }
        }
        

   return () => {
    chessRef.current = new Chess();
    }
  }, [currentMoveIndex, currentAnalysisMoveIndex, isInAnalysisTree, game, analysisMoves]); // Usunięto drugi useEffect

  function onPieceDrop({
      sourceSquare,
      targetSquare
    }: PieceDropHandlerArgs) {
     
      if (!targetSquare) {
        return false;
      }
      const chessGame = chessRef.current;
      
     
      try {
        const move = { from: sourceSquare, to: targetSquare, promotion: 'q' };
        chessGame.move(move);
        
      
        const san = chessGame.history()[chessGame.history().length -1];
        
        if( ! (game&&game.moves[currentMoveIndex +1] && game.moves[currentMoveIndex +1].move === san)){
        pushAnalysisMove(chessGame.history()[chessGame.history().length -1]);
        
        }else{
            setCurrentMoveIndex((c) => c +1);
        }
        return true;
      } catch {
      
        return false;
      }
    }
    function isPieceOnSquare(square: Square) {
        const piece = chessRef.current.get(square);
        return piece !== null;
    }
   
    
   
    return {  onPieceDrop, fen, allPossibleMoves,
       lastMove,isCheck, kingPosition, chessRef, isPieceOnSquare  };
}