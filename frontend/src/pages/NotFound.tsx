import { useTranslation } from "react-i18next";
import { Link } from "react-router-dom";

type Props = {
    whatIsMissing?: string;   
}
export default function NotFound({ whatIsMissing }: Props){
    const { t } = useTranslation('global');
    const message = t(`notFound.${whatIsMissing}` as any) || t('notFound.title');
    return(
        <div className="bg-cyan-800 w-full h-screen flex justify-center items-center
         flex-col gap-10 font-bold">
        <div className="
         text-white text-8xl">{message}</div> 
        <Link to="/" className="bg-white text-gray-500 px-4 py-2 rounded-lg
         hover:scale-110 transition-transform duration-300">{t('backToHome')}</Link>
        </div>
    )
}