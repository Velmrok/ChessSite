
import { useEffect, useState } from "react";
import { Link } from "react-router-dom";
import AddFriendButton from "../global/AddFriendButton";
import useUserStore from "@/stores/useUserStore";
import Loading from "../global/Loading";
import { useUserProfileStore } from "@/stores/useUserProfileStore";
import PaginationButtons from "../global/Pagination_buttons";
import Rating from "../global/Rating";
import { useTranslation } from "react-i18next";
import { fetchUserFriends } from "@/services/userService";
import { useApi } from "@/hooks/useApi";


export default function FriendList() {
    const nickname = useUserProfileStore(state => state.profile?.nickname);
    const { t } = useTranslation("profile");
    const user = useUserStore((state) => state.user);
    const friends = useUserProfileStore((state) => state.friends);
    const [loading, setLoading] = useState(false);
    const setFriends = useUserProfileStore(state => state.setFriends);
    const friendsPage = useUserProfileStore(state => state.friendsPage);
    const totalFriendsPages = useUserProfileStore(state => state.totalFriendsPages);
    const setFriendsPage = useUserProfileStore(state => state.setFriendsPage);

    const { request } = useApi();
    useEffect(() => {
        setLoading(true);
        const fetch = async () => { 
            const response = await request(() => fetchUserFriends(nickname!, friendsPage));
            if (response) {
                setFriends(response.friends);
                
            }
            setLoading(false);
        };
        fetch();
    }, [nickname, friendsPage]);
    if (!friends || friends.length === 0) {
        return (
            <>
                <div className="font-MyFancyFont text-lg text-white flex justify-center">{t('friends')}</div>
                <div className="grid grid-cols-1 px-4 py-2 place-items-center
         w-full mt-4 bg-gray-800/[30%] font-MyFancyFont rounded-lg gap-4 text-white min-h-[300px] ">
                    {t('noFriends')}</div>
            </>
        );
    }
    const handleChangePage = (newPage: number) => {
        if (newPage < 1 || newPage > totalFriendsPages) return;
        setFriendsPage(newPage);
    }

    return (
        <>

            <div className="font-MyFancyFont text-2xl text-white flex justify-center ">{t('friends')}</div>
            <div className="w-full text-gray-200 grid grid-cols-3 place-items-center mb-2 px-2">
                <PaginationButtons handleChangePage={handleChangePage} currentPage={friendsPage} totalPages={totalFriendsPages}
                />
            </div>
            <div className="grid grid-cols-1 px-4 py-2 lg:p-5 items-center 
        w-full xl:w-100 mt-4 bg-gray-800/[30%] rounded-lg gap-4">
                {loading ? <Loading /> :
                    friends && friends.map((friend) => (
                        <div key={friend.nickname} className="flex items-center gap-4
                 bg-gray-900/50 p-2 rounded-md w-full justify-between px-3 sm:px-6 relative">
                            <Link to={`/users/${friend.nickname}/profile`} className="text-xs md:text-sm text-white hover:text-amber-200
                             transition-transform duration-300 rounded-full outline-2 border-2 border-black">
                                <img src={`${friend.profilePictureUrl}`} alt="Profile" className="h-8 w-8 min-h-8 min-w-8 md:w-12 md:h-12 md:min-h-12 md:min-w-12 rounded-full" />
                            </Link>
                            <span className="text-white font-MyFancyFont text-[10px] sm:text-base">{friend.nickname}</span>
                            <span className="text-white font-MyFancyFont text-[10px] sm:text-base">
                                <Rating rating={friend.rating} className="text-white mr-2" /></span>
                            {friend.isOnline? (
                                <div className="text-green-500  text-sm sm:text-lg absolute right-2">●</div>
                            ) : (
                                <div className="text-red-500 text-sm sm:text-lg absolute right-2">●</div>
                            )}
                            {user?.nickname === nickname &&

                                <AddFriendButton className="absolute right-1 top-1 text-sm " userNickname={friend.nickname}
                                    key={friend.nickname} />}

                        </div>

                    ))}

            </div>

        </>
    )
}