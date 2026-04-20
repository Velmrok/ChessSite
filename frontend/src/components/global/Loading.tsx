import { useTranslation } from "react-i18next";



export default function Loading() {
    const { t } = useTranslation("global");
    return (
        <div className="w-full h-70 flex flex-col justify-center items-center gap-4 ">

            <div className="w-12 h-12 border-4 border-cyan-600 border-t-transparent
             rounded-full animate-spin"></div>

            <div className="text-white text-2xl font-MyFancyFont animate-pulse">
                {t('loadingMessage')}
            </div>
        </div>
    );
}