import type { QmViewMode } from "@/types/home";
import { useTranslation } from "react-i18next";

type Props = {
    buttons: Array<() => void>;
    qmViewMode: QmViewMode;
}
export default function QuickMenu({ buttons, qmViewMode }: Props) {
    const {t} = useTranslation("home");

    function btn(text: string, index: number, isActive: boolean) {
        return (
            <button onClick={buttons[index]} className={`text-[11px] sm:text-[13px] text-white font-MyFancyFont
                hover:scale-105 transition-all duration-100 ${isActive ? "scale-105 border-b-3 border-white" : ""}`}>
                {text}
            </button>
        )
    }

    return (
        <div className="max-w-[540px] bg-gray-900/50 w-full h-15 rounded-md flex justify-between items-center px-2 sm:px-10 py-2
        shadow-md gap-2 sm:gap-5">


            {btn(t('queue'), 0, qmViewMode === 'queue')}
            {btn(t('leaderBoard'), 1, qmViewMode === 'leaderboard')}
            {btn(t('friendsOnline'), 2, qmViewMode === 'friends')}
           

        </div>
    )
}