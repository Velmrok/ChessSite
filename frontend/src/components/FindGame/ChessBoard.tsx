import { useQueueStore } from "@/stores/useQueueStore";
import useUserStore from "@/stores/useUserStore";
import { Chessboard } from "react-chessboard";
import { useTranslation } from "react-i18next";



export default function ChessBoard() {
    if(!useUserStore.getState().user){
        throw new Error("Should not happen: ChessBoard rendered without user logged in");
    }
    const { t } = useTranslation('game');
    const isInQueue = useQueueStore((state) => state.queueData?.isInQueue);
  return (
    <div className="w-full min-w-[330px] py-3 relative">
      <Chessboard />
      {isInQueue && (
        <div className="absolute my-3 inset-0 bg-black/50 z-50 flex flex-col items-center justify-center pointer-events-none">
          <span className="font-MyFancyFont text-2xl animate-pulse">{t('lookingForOpponent')}</span>
          <div className="animate-spin rounded-full h-12 w-12 border-t-4 border-cyan-600 border-gray-200" />
          
        </div>
      )}
    </div>
  );
}