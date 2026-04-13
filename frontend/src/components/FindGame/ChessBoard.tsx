import useLanguageStore from "@/stores/useLanguageStore";
import useUserStore from "@/stores/useUserStore";
import { Chessboard } from "react-chessboard";



export default function ChessBoard() {
    if(!useUserStore.getState().user){
        throw new Error("Should not happen: ChessBoard rendered without user logged in");
    }
    const t = useLanguageStore((state) => state.t);
    const isInQueue = useUserStore((state) => state.user?.isInQueue);
  return (
    <div className="w-full min-w-[330px] py-3 relative">
      <Chessboard />
      {isInQueue && (
        <div className="absolute my-3 inset-0 bg-black/50 z-50 flex flex-col items-center justify-center pointer-events-none">
          <span className="font-MyFancyFont text-2xl animate-pulse">{t.findGame.lookingForOpponent}</span>
          <div className="animate-spin rounded-full h-12 w-12 border-t-4 border-cyan-600 border-gray-200" />
          
        </div>
      )}
    </div>
  );
}