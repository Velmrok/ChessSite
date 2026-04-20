
import { useEffect } from "react";
import { Link } from "react-router-dom";
import useUserStore from "@/stores/useUserStore";
import Loading from "../global/Loading";
import PaginationButtons from "../global/Pagination_buttons";
import useHomeStore from "@/stores/useHomeStore";
import { useTranslation } from "react-i18next";



export default function FriendsOnline() {

    const { t } = useTranslation("home");
    const user = useUserStore((state) => state.user);
    if (!user) throw new Error("User not found");

    const fetchOnlineFriends = useHomeStore(state => state.fetchOnlineFriends);
    const nickname = user.nickname;
    const friends = useHomeStore(state => state.friends);
    const loadingFriends = useHomeStore(state => state.loadingFriends);
    const friendsPage = useHomeStore(state => state.friendsPage);
    const totalFriendsPages = useHomeStore(state => state.totalFriendsPages);
    const setFriendsPage = useHomeStore(state => state.setFriendsPage);

    useEffect(() => {

        fetchOnlineFriends(nickname!);
    }, [nickname, friendsPage]);
    if (friends.length === 0) {
        return (
            <>

                <div className="grid grid-cols-1 px-4 py-2 place-items-center
         w-full mt-4 bg-gray-800/[30%] rounded-lg gap-4 text-white min-h-[300px] relative">
                    <div className="font-MyFancyFont text-lg text-white flex justify-center absolute top-3">{t("friendsOnline")}</div>
                    {t("noFriendsOnline")}</div>
            </>
        );
    }
    const handleChangePage = (newPage: number) => {
        if (newPage < 1 || newPage > totalFriendsPages) return;
        setFriendsPage(newPage);
    }

    return (
        <>

            <div className="grid grid-cols-3 px-4 py-2 lg:p-5 items-center 
        w-full max-w-[540px] mt-4 bg-gray-800/[30%] rounded-lg gap-4">
                <div className="font-MyFancyFont col-span-3 text-2xl text-white flex justify-center ">{t("friendsOnline")}</div>
                {loadingFriends ? <Loading /> :
                    friends && friends.map((friend) => (
                        <Link to={`/users/${friend.nickname}/profile`} className="text-xs md:text-sm text-white hover:outline-2 rounded-md
                     transition-transform hover:scale-105">
                            <div key={friend.nickname} className="aspect-square flex w-full  flex-col items-center gap-4
                 bg-gray-900/50  rounded-md w-28 justify-between  p-2 relative">

                                <img src={`${friend.avatar}`} alt="Profile" className="h-13 w-13 min-h-13 min-w-13 md:w-18 md:h-18
                 md:min-h-18 md:min-w-18 rounded-full outline-2 border-2 border-black mt-2" />

                                <span className="text-white font-MyFancyFont text-[10px] sm:text-base">{friend.nickname}</span>

                                {friend.onlineStatus === 'online' ? (
                                    <div className="text-green-500  text-sm sm:text-lg absolute right-[1px] top-[1px] sm:right-2 sm:top-2">●</div>
                                ) : (
                                    <div className="text-red-500 text-sm sm:text-lg absolute right-[1px] top-[1px] sm:right-2 sm:top-2">●</div>
                                )}




                            </div>
                        </Link>

                    ))}

            </div>
            <div className="w-full text-gray-200 grid grid-cols-3 place-items-center mb-2 px-2">
                <PaginationButtons handleChangePage={handleChangePage} currentPage={friendsPage} totalPages={totalFriendsPages}
                />
            </div>

        </>
    )
}