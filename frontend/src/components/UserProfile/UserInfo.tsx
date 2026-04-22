
import { useTranslation } from "react-i18next";
import Rating from "../global/Rating";
import type { UserInfo } from "@/types/user";

type Props = {
    userInfo: UserInfo;
};

export default function UserInfo({ userInfo }: Props) {
    const { t } = useTranslation("profile");


    return (
        <div className="bg-gray-900/[30%] min-h-full rounded-md shadow-md  sm:w-[70%] lg:w-full  lg:col-span-4 py-2 pr-3 ">
            <div className="grid grid-cols-2 gap-3 ml-4 text-white text-lg  font-MyFancyFont">
                <div className="">{t('rating')}</div>
                <div className="text-center">
                    <Rating className="flex flex-col" rating={userInfo.rating} />
                </div>
                <div className="">{t('joinDate')}</div>
                <div className="text-center">{userInfo?.joinDate}</div>
                <div className="">{t('gamesPlayed')}</div>
                <div className="text-center">{userInfo?.gamesPlayed}</div>
                <div className="">{t('wins')}</div>
                <div className="text-center">{userInfo?.wins}</div>
                <div className="">{t('losses')}</div>
                <div className="text-center">{userInfo?.losses}</div>
                <div className="">{t('draws')}</div>
                <div className="text-center">{userInfo?.draws}</div>
            </div>
        </div>
    );
}