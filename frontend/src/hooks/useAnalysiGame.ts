import useGameStore from "@/stores/useGameStore";
import { useState } from "react";


export default function useAnalysisGame() {
    const game = useGameStore((state) => state.game);
    const setGame = useGameStore((state) => state.setGame);
    const currentAnalysisMoveIndex = useGameStore((state) => state.currentAnalysisMoveIndex);
    const analysisMoves = useGameStore((state) => state.analysisMoves);
    const isInAnalysisTree = useGameStore((state) => state.isInAnalysisTree);
    const resetAnalysisState = useGameStore((state) => state.resetAnalysisState);
    const setCurrentAnalysisMoveIndexInStore = useGameStore((state) => state.setCurrentAnalysisMoveIndex);
    const setIsInAnalysisTree = useGameStore((state) => state.setIsInAnalysisTree);
    const pushAnalysisMoveToStore = useGameStore((state) => state.pushAnalysisMove);

    const setCurrentAnalysisMoveIndex= (change:number) =>  {
        const newValue = currentAnalysisMoveIndex + change;
        if(!isInAnalysisTree) {
            setCurrentAnalysisMoveIndexInStore(()=>0);
            setIsInAnalysisTree(false);
            return;
        };
        if (newValue < 0) {
            resetAnalysisState();
            return;
        }
        if(newValue >= analysisMoves.length){
           setCurrentAnalysisMoveIndexInStore(()=> analysisMoves.length -1);
           return;
        }
        
       setCurrentAnalysisMoveIndexInStore(()=> newValue);
       setIsInAnalysisTree(true);
    }

    


    return {
        setCurrentAnalysisMoveIndex,
        
        currentAnalysisMoveIndex,
        analysisMoves,
        isInAnalysisTree,
    }
}