import type { GameState } from "@/types/game";

import {create} from "zustand";
import type { MoveInfo } from "@/types/game";



type GameStoreType = {
    game: GameState | null;
    currentMoveIndex: number;
    currentWhiteTime: number;
    currentBlackTime: number;
    gameHasJustStarted: boolean;
    ratingType : 'bullet' | 'blitz' | 'rapid';
    setGame: (game: GameState) => void;
    setCurrentMoveIndex: (cb:(index: number)=> number) => void;
    setCurrentWhiteTime: (time: number) => void;
    setCurrentBlackTime: (time: number) => void;
    resetAnalysisState: () => void;
    pushAnalysisMove: (move: string) => void;
    analysisMoves: Array<string>;
    currentAnalysisMoveIndex: number;
    setCurrentAnalysisMoveIndex: (cb:(index: number) => number) => void;
    setIsInAnalysisTree: (value: boolean) => void;
    pushMove: (move: MoveInfo) => void;
    isInAnalysisTree: boolean;
    endGame: (winnerNickname: string | null) => void;
    setAbsoluteCurrentMoveIndex: (index: number) => void;
    resetGame: () => void;
    setGameHasJustStarted: (value: boolean) => void;
    setAnalysisMoves: (moves: Array<string>) => void;
}


const useGameStore = create<GameStoreType>((set) => ({
    game: null,
    currentMoveIndex: -1,
    currentWhiteTime: 0,
    currentBlackTime: 0,
    analysisMoves: [],
    currentAnalysisMoveIndex: -1,
    isInAnalysisTree: false,
    gameHasJustStarted: false,
    ratingType: "blitz",
    resetGame: () => set({
        game: null,
        currentMoveIndex: -1,
        currentWhiteTime: 0,
        currentBlackTime: 0,
        analysisMoves: [],
        currentAnalysisMoveIndex: -1,
        isInAnalysisTree: false,
    }),
    setGameHasJustStarted: (value: boolean) => set({gameHasJustStarted: value}),
    setGame: (game: GameState) => set({ game: { ...game } }),
   
    setCurrentMoveIndex: (cb: (index: number) => number) => set((state) => ({currentMoveIndex: cb(state.currentMoveIndex)})),
        
    setCurrentWhiteTime: (time: number) => set({currentWhiteTime: time}),
    setCurrentBlackTime: (time: number) => set({ currentBlackTime: time }),
    pushAnalysisMove: (move: string) =>
        set((state) => {
            if (!state.game) return {};

            return {
                game: {
                    ...state.game,
                    currentTurn:
                        state.game.isWhiteTurn ? false : true,
                },
                analysisMoves: [...state.analysisMoves, move],
                currentAnalysisMoveIndex: state.analysisMoves.length,
                isInAnalysisTree: true,
            };
        }),

    
setCurrentAnalysisMoveIndex: (cb: (index: number) => number) => set((state) => ({ currentAnalysisMoveIndex: cb(state.currentAnalysisMoveIndex) })),
    resetAnalysisState: () => set({
        analysisMoves: [],
        currentAnalysisMoveIndex: -1,
        isInAnalysisTree: false,
    }),
    setIsInAnalysisTree: (value: boolean) => set({isInAnalysisTree: value}),
    pushMove: (move: MoveInfo) => set((state) => {
        if (!state.game) return {}; 
        const isWhiteTurn = state.game.isWhiteTurn;
        const newWhiteTime = isWhiteTurn ? move.absoluteTime : state.currentWhiteTime;
        const newBlackTime = !isWhiteTurn ? move.absoluteTime : state.currentBlackTime;
      
        const newMoves = [...state.game.moves, move];
        return {
            game: {
                ...state.game,
                moves: newMoves,
                isWhiteTurn : !isWhiteTurn,
               
            },
            currentWhiteTime: newWhiteTime,
            currentBlackTime: newBlackTime,
            currentMoveIndex: newMoves.length - 1
        };
    }),
    endGame: (winnerNickname) => set((state) => {
        if (!state.game) return {};
        return {
            game: { 
                ...state.game, 
                status: "finished", 
                winnerNickname: winnerNickname,
                
            },
            isInAnalysisTree: false, 
            analysisMoves: [],
            currentAnalysisMoveIndex: -1
        };
    }),
    setAbsoluteCurrentMoveIndex: (index: number) => set({
        currentMoveIndex: index,
        isInAnalysisTree: false,
         analysisMoves: [],
          currentAnalysisMoveIndex: -1}),
        setAnalysisMoves: (moves: Array<string>) => set({analysisMoves: moves}),

    
   
}));
export default useGameStore;