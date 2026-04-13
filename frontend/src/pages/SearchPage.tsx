import useSearchStore from "@/stores/useSearchStore";
import UserSearchBar from "../components/Search/UserSearchBar";
import UserList from "../components/Search/UserList";
import {  useEffect, useRef, useState } from "react";
import { fetchAllUsers } from "@/services/searchPlayersService";
import FilterMenu from "@/components/Search/FilterMenu";
import Loading from "../components/global/Loading";
import PaginationButtons from "../components/global/Pagination_buttons";
import useLanguageStore from "@/stores/useLanguageStore";
import useUserStore from "@/stores/useUserStore";
export default function SearchPage() {
    const params = useSearchStore((state) => state.params)
    const getParamsLink = useSearchStore((state) => state.getParamsLink);
    const setFilters = useSearchStore((state) => state.setFilters); 
    const resetFilters = useSearchStore((state) => state.resetFilters);
    const [list, setList] = useState<User[]>([]);
    const totalPagesCount = useRef<number>(1);
    const user = useUserStore((state) => state.user);
   const t = useLanguageStore((state) => state.t);
    const [isLoading, setIsLoading] = useState(false);
    
    useEffect(() => {
        resetFilters();
    }, []);
    useEffect(() => {
        const timeoutId = setTimeout(() => {
        setIsLoading(true);
            }, 300);
        const fetchUsers = async () => {
            try {
                const response = await fetchAllUsers(getParamsLink());
                setList(response.users);
                totalPagesCount.current = response.totalPages;
            } catch (error) {
                console.error("Error fetching users:", error);
            }finally {
                clearTimeout(timeoutId);
                setIsLoading(false);
            }
        };
        fetchUsers();
    }, [params]);
    const handleChangePage = (newPage: number) => {
        if (newPage < 1 || newPage > totalPagesCount.current) return;
        setFilters({ page: newPage });
    }
   

    return (
        <>
        <div className="flex flex-col items-center justify-center min-h-screen
         bg-cyan-800 text-white p-4 gap-6">
           <div className="w-full max-w-lg">
            <UserSearchBar />
            
           </div>
        <div className="font-MyFancyFont">
        
        </div>
          

            <div className=" w-[95%]  md:w-[90%] lg:w-[70%] max-w-[1000px]  flex flex-col gap-5">
                <FilterMenu />
                <div className="flex flex-col items-center text-white font-MyFancyFont  ">
                <div className="grid grid-cols-3 place-items-center mb-4 px-5 min-h-[30px] w-full">
                 
                    
                 <PaginationButtons handleChangePage={handleChangePage} currentPage={params.page}
                  totalPages={totalPagesCount.current} />
                  
                </div>
                    <div className="w-full bg-cyan-900 min-h-screen p-2 md:p-6
                      rounded-lg shadow-lg flex flex-col items-center gap-6 mb-10">
                        {isLoading ? <Loading /> : <UserList users={list} />}
                    </div>
                     </div>
            </div>
          
            
        </div>
       
        </>
    );
}