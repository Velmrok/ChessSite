import { MdAccessTime } from "react-icons/md";
import { SiPushbullet, SiStackblitz } from "react-icons/si";
import type { Rating } from "@/types/User";

type Props = {
    rating: Rating;
    className?: string;
}

export default function Rating({ rating, className = "flex flex-col" }: Props) {

    if (typeof rating === 'number') {
        throw new Error("Rating component received a number instead of an object.");
    }
    return (
        <div className={`${className}`}>
            <div className="flex items-center gap-2"> <MdAccessTime className="text-green-500 text-base md:text-2xl" />{rating.rapid}</div>
            <div className="flex items-center gap-2"> <SiStackblitz className="text-yellow-300 text-base md:text-2xl" />{rating.blitz}</div>
            <div className="flex items-center gap-2"> <SiPushbullet className="text-red-500 text-base md:text-2xl" />{rating.bullet}</div>
        </div>
    )
}