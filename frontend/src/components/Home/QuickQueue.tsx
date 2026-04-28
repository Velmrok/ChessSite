import useQueue from "@/hooks/useQueue";
import { formatTimeFromMs } from "@/services/socket/socketGlobalService";
import useUserStore from "@/stores/useUserStore";
import { useTranslation } from "react-i18next";


export default function QuickQueue() {
  const { t } = useTranslation("home");
  const isInQueue = useUserStore((state) => state.user?.isInQueue);
  const queueTime = useUserStore((state) => state.queueTime);
  const { handleJoinGame, handleCancelGame } = useQueue();
  const handleLeaveQueue = () => {
    if (isInQueue) {
      handleCancelGame();

    }
  }
  if (isInQueue) {
    return (
      <div className="max-w-[540px] bg-black/[20%] rounded-md shadow-md min-h-[540px]
     flex flex-col items-center justify-center w-full gap-4 relative">
        <button onClick={handleLeaveQueue}
          className="bg-cyan-500/60 w-[90%] py-2 rounded hover:bg-cyan-500 absolute top-6
                               transition-all font-MyFancyFont">{isInQueue ? `${t('cancelQueue')} ( ${formatTimeFromMs(queueTime!)} )` : `${t('startGame')}`}</button>
        <div className="text-white text-2xl font-MyFancyFont animate-pulse">
          {t('inQueue')}
        </div>
        <div className="animate-spin rounded-full h-12 w-12 border-t-4 border-cyan-600 border-gray-200" />


      </div>
    )
  }


  return (
    <div className="max-w-[540px] grid grid-cols-3 gap-2 md:gap-4 w-full">
      {["1+0", "2+0", "3+0", "3+2", "5+3", "10+0", "10+5", "15+10", "30+0"].map((t) => (
        <button
          //onClick={() => handleJoinGame(parseInt(t.split('+')[0]), parseInt(t.split('+')[1]))}
          key={t}
          className="aspect-square w-full flex items-center justify-center transition-colors duration-300
        bg-black/50 hover:bg-black/70 shadow-md text-white font-MyFancyFont text-lg md:text-2xl rounded-xl ">
          {t}

        </button>
      ))}
    </div>

  )
}