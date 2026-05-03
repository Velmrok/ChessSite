import useUserStore from "@/stores/useUserStore";
import { Link } from "react-router-dom";
import Avatar from "../global/Avatar";
import { useQueueStore } from "@/stores/useQueueStore";

type Props = {
    nickname: string,

    avatarUrl: string,
    rating?: number,
    time: string,
    withLink: boolean
}
const API_URL = import.meta.env.VITE_API_URL;
export default function PlayerBar({ nickname, avatarUrl, rating, time, withLink }: Props) {

    const isInQueue = useQueueStore((state) => state.queueData?.isInQueue);

    const formatedTime = `${time.split("+")[0]}:00`;

    const userInfo = (
        <div className={`flex gap-3 ${isInQueue && !rating ? "animate-pulse" : ""}`}>
            <Avatar avatarUrl={avatarUrl} className="w-10 h-10 rounded-full outline-2" />
            <div className="h-full">

                <span>{nickname}</span>

                {rating && <span className="ml-2 text-gray-400">({rating})</span>}
            </div>
        </div>
    )
    return (
        <div className="flex w-full h-10 md:h-15 bg-black/30 rounded-md p-2
          items-center justify-between font-MyFancyFont min-w-[300px] " >

            {withLink ? (
                <Link to={`/users/${nickname}/profile`} className="hover:text-amber-200">
                    {userInfo}
                </Link>) :
                userInfo
            }

            <div className="bg-black/50 text-gray-400 h-full px-5 rounded-md flex items-center justify-center">
                <span>{formatedTime}</span>
            </div>
        </div>
    )
}