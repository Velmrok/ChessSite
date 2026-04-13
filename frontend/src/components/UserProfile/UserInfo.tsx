
import Rating from "../global/Rating";
import useLanguageStore from "@/stores/useLanguageStore";

type Props = {
    userInfo: UserInfo;
};
type UserInfo = {
    rating: {
        rapid: number;
        blitz: number;
        bullet: number;
    };
    joinDate: string;
    gamesPlayed: number;
    wins: number;
    losses: number;
    draws: number;
    }
export default function UserInfo({ userInfo }: Props) {
   const t = useLanguageStore((state) => state.t);


    return (
        <div className="bg-gray-900/[30%] min-h-full rounded-md shadow-md  sm:w-[70%] lg:w-full  lg:col-span-4 py-2 pr-3 ">
            <div className="grid grid-cols-2 gap-3 ml-4 text-white text-lg  font-MyFancyFont">
            <div className="">{t.profile.userInfo.rating}</div>
            <div className="text-center">
                <Rating className="flex flex-col" rating={userInfo.rating}  />
                </div>
            <div className="">{t.profile.userInfo.joinDate}</div>
            <div className="text-center">{userInfo?.joinDate}</div>
            <div className="">{t.profile.userInfo.gamesPlayed}</div>
            <div className="text-center">{userInfo?.gamesPlayed}</div>
            <div className="">{t.profile.userInfo.wins}</div>
            <div className="text-center">{userInfo?.wins}</div>
            <div className="">{t.profile.userInfo.losses}</div>
            <div className="text-center">{userInfo?.losses}</div>
            <div className="">{t.profile.userInfo.draws}</div>
            <div className="text-center">{userInfo?.draws}</div>
            </div>
        </div>
    );
}