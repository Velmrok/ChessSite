import { useState, useEffect, useRef } from 'react';
import { useChessboardLive } from "@/hooks/useChessboardLive";
import { useChessboardAnalysis } from "@/hooks/useChessboardAnalysis";
import useGameStore from "@/stores/useGameStore";
import useUserStore from "@/stores/useUserStore";
import type { PieceDataType } from 'react-chessboard';



type Props = {
    game: GameState;
    pushAnalysisMove: (move: string) => void;
};

export function useChessboardInteraction({ game, pushAnalysisMove }: Props) {
    const live = useChessboardLive();
    const analysis = useChessboardAnalysis({ pushAnalysisMove });
    const user = useUserStore((state) => state.user);
    const isLive = game.status === "live";

    const isInAnalysisTree = useGameStore((state) => state.isInAnalysisTree);
    const currentMoveIndex = useGameStore((state) => state.currentMoveIndex);
    const currentAnalysisMoveIndex = useGameStore((state) => state.currentAnalysisMoveIndex);
    
    const prevMoveIndexRef = useRef(currentMoveIndex);
    const prevIsInAnalysisTreeRef = useRef(currentAnalysisMoveIndex);
    const [squareStyles, setSquareStyles] = useState<Record<string, React.CSSProperties>>({});
    const [clickedSquareStyles, setClickedSquareStyles] = useState<Record<string, React.CSSProperties>>({});
    const lastInteractionTimeRef = useRef(0);
    const [clickedSquare, setClickedSquare] = useState<string | null>(null);
    const [clickedPiece, setClickedPiece] = useState<PieceDataType | null>(null);
    const [animationSpeed, setAnimationSpeed] = useState(250);
    
    useEffect(() => {
        setClickedSquareStyles({});
        const now = Date.now();
        const timeSinceLastInteraction = now - lastInteractionTimeRef.current;
        lastInteractionTimeRef.current = now;
        if (prevMoveIndexRef.current > currentMoveIndex || prevIsInAnalysisTreeRef.current > currentAnalysisMoveIndex) {
            setAnimationSpeed(60);
        } else if (timeSinceLastInteraction < 300) {
            setAnimationSpeed(100);
            
        } else {
           
            setAnimationSpeed(220);
        }

        prevMoveIndexRef.current = currentMoveIndex;
        prevIsInAnalysisTreeRef.current = currentAnalysisMoveIndex;
    }, [currentMoveIndex, currentAnalysisMoveIndex]);

    const activeLastMove = isLive ? live.lastMove : analysis.lastMove;

    useEffect(() => {
        const lastMove = isLive ? live.lastMove : analysis.lastMove;
        const currentShownTurn = isLive ? live.chessRef.current.turn() : analysis.chessRef.current.turn();
        const isUserAPlayer = user && (user.nickname === game.white.nickname || user.nickname === game.black.nickname);
        const isUserTurn = isLive ? (user && ((currentShownTurn === "w" && user.nickname === game.white.nickname) ||
            (currentShownTurn === "b" && user.nickname === game.black.nickname))) : true;
        const hasUndoBeenMade = isLive ? game.moves.length > currentMoveIndex + 1 : false;

        const newStyles: Record<string, React.CSSProperties> = {};

        if (lastMove) {
            newStyles[lastMove.from] = { backgroundColor: "rgba(255, 255, 0, 0.4)" };
            newStyles[lastMove.to] = { backgroundColor: "rgba(229, 229, 0, 0.4)" };
        }
         if (lastMove) {
            const isCheck = isLive ? live.isCheck : analysis.isCheck;
            if (isCheck) {
                const kingPosition = isLive ? live.kingPosition : analysis.kingPosition;
                
                newStyles[kingPosition] = { backgroundColor: "rgba(255, 0, 0, 0.6)" };
            }
        }
        console.log(`currentShownTurn: ${currentShownTurn} for user: ${user?.nickname} in game: ${game.white.nickname} vs ${game.black.nickname}`);
        console.log(`isUserAPlayer: ${isUserAPlayer}, isUserTurn: ${isUserTurn}, hasUndoBeenMade: ${hasUndoBeenMade}`);
        if (!isUserAPlayer || !isUserTurn || hasUndoBeenMade) {
            setSquareStyles(newStyles);
            return;
        }

       
        
        setSquareStyles({
            ...newStyles,
            ...clickedSquareStyles
        });
    }, [
        activeLastMove?.from, 
        activeLastMove?.to, 
        clickedSquareStyles, 
        isLive, 
        user?.nickname, 
        game.white.nickname, 
        game.black.nickname, 
        game.moves.length, 
        currentMoveIndex,
        live.isCheck,
        analysis.isCheck,
        live.kingPosition,
        analysis.kingPosition,
        live.fen,
        analysis.fen
    ]);


    const onSquareClick = (square: { square: string, piece: PieceDataType | null }) => {
        
        if (clickedPiece && clickedSquareStyles[square.square]) {

            const onPieceDrop = isLive ? live.onPieceDrop : analysis.onPieceDrop;

            const formatedPiece = { isSparePiece: false, position: square.square, pieceType: clickedPiece.pieceType };

            onPieceDrop({
                piece: formatedPiece,
                sourceSquare: clickedSquare!,
                targetSquare: square.square
            });
            setClickedSquareStyles({});
            setClickedSquare(null);
            setClickedPiece(null);

            return;
        }


        if (clickedSquare == square.square) {
            setClickedSquareStyles({});
            setClickedSquare(null);
            setClickedPiece(null);
            return;
        }
        if (clickedSquare === square.square) {
            setClickedSquareStyles({});
            setClickedSquare(null);
            setClickedPiece(null);
            return;
        }

        if (!square.piece) {
            setClickedSquareStyles({});
            setClickedSquare(null);
            setClickedPiece(null);
            return;
        }


        const allPossibleMoves = isLive ? live.allPossibleMoves : analysis.allPossibleMoves;
        const movesFromSquare = allPossibleMoves.filter((move) => move.from === square.square);

        setClickedSquareStyles({});
        movesFromSquare.forEach((move) => {
            setClickedSquareStyles((p) => ({
                ...p,
                [move.to]: { backgroundColor: "rgba(39, 211, 9, 0.5)" },
                [square.square]: { backgroundColor: "rgba(39, 211, 9, 0.5)" }
            }));
        });
        setClickedPiece(square.piece);
        setClickedSquare(square.square);
    }

    const onSquareRightClick = (square: any) => {
        setClickedSquareStyles({});
        setClickedSquare(null);
    }

    return {
        live,
        analysis,
        isLive,
        squareStyles,
        onSquareClick,
        onSquareRightClick,
        animationSpeed,
        isInAnalysisTree
    };
}
