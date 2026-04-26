
import UserSearchBar from "../components/UsersSearch/UserSearchBar";
import UserList from "../components/UsersSearch/UserList";
import { useEffect, useRef, useState } from "react";
import { fetchAllUsers } from "../services/searchUsersService";
import FilterMenu from "../components/UsersSearch/FilterMenu";
import Loading from "../components/global/Loading";
import PaginationButtons from "../components/global/Pagination_buttons";
import useUsersSearchStore from "../stores/useUsersSearchStore";
import type { PublicUser, UserSearchItem } from "@/types/user";
import { useApi } from "@/hooks/useApi";


export default function UsersSearchPage() {
    const params = useUsersSearchStore((state) => state.params)
    const getParamsLink = useUsersSearchStore((state) => state.getParamsLink);
    const { request } = useApi();
    const setFilters = useUsersSearchStore((state) => state.setFilters);
    const resetFilters = useUsersSearchStore((state) => state.resetFilters);
    const [list, setList] = useState<UserSearchItem[]>([]);
    const totalPagesCount = useRef<number>(1);
    const [isLoading, setIsLoading] = useState(false);

    useEffect(() => {
        resetFilters();
    }, []);
    useEffect(() => {
        const timeoutId = setTimeout(() => {
            setIsLoading(true);
        }, 300);
        const fetchUsers = async () => {
            const response = await request(() => fetchAllUsers(getParamsLink()), { onError: (e) => console.error(e) });

            if (response) {
                setList(response.users);
                totalPagesCount.current = response.totalPages;
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