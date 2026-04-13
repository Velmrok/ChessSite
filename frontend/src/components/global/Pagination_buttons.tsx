

import useLanguageStore from "@/stores/useLanguageStore";
import { FaArrowLeft, FaArrowRight } from "react-icons/fa";

type Props = {
    handleChangePage: (newPage: number) => void;
    currentPage: number;
    totalPages: number;

}
export default function PaginationButtons({ handleChangePage, currentPage, totalPages}: Props) {
    const t = useLanguageStore((state) => state.t);

    
    const hideLeftButton = currentPage <= 1;
    const hideRightButton = currentPage >= totalPages || totalPages === 0 || currentPage === 0;
    const hidePageInfo = totalPages <= 1;
    return (
        <>
        

            <div>
            <button className={"text-2xl hover:scale-110 hover:text-amber-200 transition-transform duration-300 "
                + (hideLeftButton ? " hidden " : "")
            }
                onClick={() => handleChangePage(currentPage - 1)}><FaArrowLeft />
            </button>
            </div>
            <span className={"font-MyFancyFont text-sm md:text-base"+ (hidePageInfo ? " hidden " : "")}>{t.search.page + ": "}{currentPage + "/" + totalPages}</span>
            <div>
            <button className={"text-2xl hover:scale-110 hover:text-amber-200 transition-transform duration-300 "
                + (hideRightButton ? " hidden " : "")
            }
                onClick={() => handleChangePage(currentPage + 1)}><FaArrowRight />
            </button>
            </div>
        </>
    );
}