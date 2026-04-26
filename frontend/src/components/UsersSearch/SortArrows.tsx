import useUsersSearchStore from "@/stores/useUsersSearchStore";
import { FaLongArrowAltDown, FaLongArrowAltUp } from "react-icons/fa";

type Props = {
    sortBy: string;
}
export default function SortArrows({ sortBy }: Props) {
    const SortDescending = useUsersSearchStore((state) => state.params.sortDescending);
    const currentSortBy = useUsersSearchStore((state) => state.params.sortBy);

    const arrowUpSyle = !SortDescending && currentSortBy === sortBy ? "text-green-500 text-base" : " text-sm text-gray-400";
    const arrowDownStyle = SortDescending && currentSortBy === sortBy ? "text-green-500 text-base" : " text-sm text-gray-400";
    return (
        <>
        <div className={arrowUpSyle+" transition-all duration-400 text-[10px] md:text-base" }>
        <FaLongArrowAltUp />
        
        </div>
        <div className={arrowDownStyle+" transition-all duration-400 text-[10px] md:text-base"}>
        <FaLongArrowAltDown />
        </div>
        </>
 
    )
}