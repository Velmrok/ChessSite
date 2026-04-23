
import { useTranslation } from "react-i18next";
import Rating from "../global/Rating";
import type { UserInfo } from "@/types/user";

type Props = {
    className?: string;
    userInfo: UserInfo;
};

const separator = (<div className="col-span-2 h-[3px] bg-gradient-to-r from-transparent via-cyan-600 to-transparent "/>);
export default function UserInfo({ className, userInfo }: Props) {
    const { t } = useTranslation("profile");

    const defaultClass = "bg-gray-900/[30%]  rounded-md shadow-md  sm:w-[70%] lg:w-full  lg:col-span-4 py-2 pr-3 ";
    return (
        <div className={className || defaultClass}>
            <div className="grid grid-cols-2 gap-3 ml-4 text-white text-lg  font-MyFancyFont">


                <div className="">{t('rating')}</div>
                <div className="text-center">
                    <Rating className="flex flex-col items-center -translate-x-4" rating={userInfo.rating} />
                </div>
                {separator}

                <div className="">{t('joinDate')}</div>
                <div className="text-center">{userInfo?.createdAt}</div>
                {separator}

                <div className="">{t('gamesPlayed')}</div>
                <div className="text-center">{userInfo?.gamesPlayed}</div>
                {separator}

                <div className="">{t('totalWins')}</div>
                <div className="text-center">{userInfo?.totalWins}</div>
                {separator}
                
                <div className="">{t('totalLosses')}</div>
                <div className="text-center">{userInfo?.totalLosses}</div>
                {separator}
                <div className="">{t('totalDraws')}</div>
                <div className="text-center">{userInfo?.totalDraws}</div>
                {separator}
                <div className="">{t('wins')}</div>
                <div className="text-center ">
                    <Rating className="flex flex-col items-center -translate-x-4" rating={userInfo.wins} />
                </div>
                {separator}

                <div className="">{t('losses')}</div>
                <div className="text-center">
                    <Rating className="flex flex-col items-center -translate-x-4" rating={userInfo.losses} />
                </div>
                {separator}
                <div className="">{t('draws')}</div>
                <div className="text-center">
                    <Rating className="flex flex-col items-center -translate-x-4" rating={userInfo.draws} />
                </div>
            </div>
        </div>
    );
}