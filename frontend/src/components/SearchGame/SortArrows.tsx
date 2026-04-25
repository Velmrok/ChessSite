import { FaLongArrowAltDown, FaLongArrowAltUp } from "react-icons/fa";

type Props = {
    sortDescending: boolean | null;
};
export default function SortArrows({ sortDescending }: Props) {
    const arrowUpSyle = sortDescending === true ? "text-green-500 text-base" : " text-sm text-gray-400";
    const arrowDownStyle = sortDescending === false ? "text-green-500 text-base" : " text-sm text-gray-400";
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