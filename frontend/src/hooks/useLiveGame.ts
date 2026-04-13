import useGameStore from "@/stores/useGameStore";
import { useEffect, useRef, useState } from "react";

export default function useLivesGame() {

    const game = useGameStore((state) => state.game);
    const timerIdRef = useRef<number | null>(null);
    
   
    const setCurrentBlackTime = useGameStore((state) => state.setCurrentBlackTime);
    const setCurrentWhiteTime = useGameStore((state) => state.setCurrentWhiteTime);
    const currentBlackTime = useGameStore((state) => state.currentBlackTime);
    const currentWhiteTime = useGameStore((state) => state.currentWhiteTime);
    const whiteTimeRef = useRef(currentWhiteTime);
    const blackTimeRef = useRef(currentBlackTime);
    const currentMoveIndex = useGameStore((state) => state.currentMoveIndex);
    
    const setCurrentMoveIndexToStore = useGameStore((state) => state.setCurrentMoveIndex);
    useEffect(() => {
        whiteTimeRef.current = currentWhiteTime;
        blackTimeRef.current = currentBlackTime;
    }, [currentWhiteTime, currentBlackTime]);
    
   useEffect(() => {
        if (game?.status === 'finished') {
            if (timerIdRef.current) clearInterval(timerIdRef.current);
            timerIdRef.current = null;
        }
    }, [game?.status]);

    useEffect(() => {
        if(game?.status !== "live")return;
        if(game.moves.length ===0)return;
        
        setTimer(game?.currentTurn || "white");
    }, [game?.currentTurn]);

    useEffect(() => {
        return () => {
            if (timerIdRef.current) clearInterval(timerIdRef.current);
        }
    }, []);


    
    const setTimer = (color: "white" | "black"|null) => {
        if(!color){
            if(timerIdRef.current )clearInterval(timerIdRef.current);
            timerIdRef.current = null;
            return;
        }
        if(timerIdRef.current ){
            clearInterval(timerIdRef.current);
        }
        
        const startTime = Date.now();
        const startWhiteTime = whiteTimeRef.current;
        const startBlackTime = blackTimeRef.current;

        timerIdRef.current = setInterval(()=>{
            const elapsed = Date.now() - startTime;
            if(color==="white"){
                setCurrentWhiteTime(Math.max(0, startWhiteTime - elapsed));
            }else{
                setCurrentBlackTime(Math.max(0, startBlackTime - elapsed));
            }
        },100);

    }

    
    
    const setCurrentMoveIndex= (change:number) =>  {
        const newValue = currentMoveIndex + change;

        if (!game) return 
        if (newValue < -1) {
            setCurrentMoveIndexToStore(()=> -1);
            return;
        }
        if(newValue >= game.moves.length){
            setCurrentMoveIndexToStore(()=> game.moves.length -1);
            return;
        }
        setCurrentMoveIndexToStore(()=> newValue);
    }







    return {
        setCurrentMoveIndex,
    }


}