import { FaLongArrowAltDown, FaLongArrowAltUp } from "react-icons/fa";

type Props = {
    sortOrder: 'asc' | 'desc' | null;
};
export default function SortArrows({ sortOrder }: Props) {
    const arrowUpSyle = sortOrder == 'asc' ? "text-green-500 text-base" : " text-sm text-gray-400";
    const arrowDownStyle = sortOrder == 'desc' ? "text-green-500 text-base" : " text-sm text-gray-400";
    return (
        <>
            <div className={arrowUpSyle + " transition-all duration-400"}>
                <FaLongArrowAltUp />

            </div>
            <div className={arrowDownStyle + " transition-all duration-400"}>
                <FaLongArrowAltDown />
            </div>
        </>

    )
}