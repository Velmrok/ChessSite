
import { Link } from "react-router-dom";
import AddFriendButton from "../global/AddFriendButton";
import useSearchStore from "@/stores/useSearchStore";
import SortArrows from "./SortArrows";
import Rating from "../global/Rating";
import useLanguageStore from "@/stores/useLanguageStore";
import DeleteAccountButton from "../global/DeleteAccountButton";
import { useEffect, useState } from "react";
import EditAccountButton from "../global/EditAccountButton";

const API_URL = import.meta.env.VITE_API_URL;
type Props = {
    users: User[];
};

export default function UserList({ users}: Props) {
    const t = useLanguageStore((state) => state.t);
    const setOrder = useSearchStore((state) => state.setOrder);
    const params = useSearchStore((state) => state.params);
    const [deletedAccounts, setDeletedAccounts] = useState<string[]>([]);
    useEffect(() => {
        setDeletedAccounts([]);
    }, [users]);
    const deltaLastActive = (lastActive:string)=>{
        const now = new Date();
        const delta = now.getTime() - new Date(lastActive).getTime();
       
        const minutes = Math.floor(delta / 60000);
        const hours = Math.floor(minutes / 60);
        const days = Math.floor(hours / 24);
        
        if (days > 0) return `${days}d `+t.search.ago;
        if (hours > 0) return `${hours}`+t.search.hour+' '+t.search.ago;
        if (minutes > 0) return `${minutes}m `+t.search.ago;
        return t.search.now;
    }
    const handleDeleteAccount = (nickname:string) => {
        setDeletedAccounts([...deletedAccounts, nickname]);
    }
  
   const getSortOrderForColumn = (columnName: string) => {
    return params.sortBy === columnName ? params.sortOrder?'asc': 'desc' : null;
};


    return (
        <>
            <div className="flex flex-row w-full bg-black/[50%] md:gap-5 
                  text-white py-1 sm:py-4 rounded-lg shadow-lg justify-between font-MyFancyFont px-2 md:px-5 lg:px-10 ">
                <button className="text-[10px] md:text-lg flex items-center" onClick={() => setOrder('nickname')}>
                    {t.search.nickname}
                    <SortArrows sortOrder={getSortOrderForColumn('nickname')} />
                    </button>
                    
                    
                <button className="text-[10px] md:text-lg flex items-center" onClick={() => setOrder('rating')}>
                    {t.search.rating}
                    <SortArrows sortOrder={getSortOrderForColumn('rating')} />
                    </button>
                    
                <button className="text-[10px] md:text-lg flex items-center" onClick={() => setOrder('onlineStatus')}>
                    {t.search.status}
                    <SortArrows sortOrder={getSortOrderForColumn('onlineStatus')} />
                    </button>
                <button className="flex flex-row items-center"  onClick={() => setOrder('lastActive')}>
                    <div className="text-[10px] md:text-lg flex flex-col items-center"> 
                    <h3>{t.search.last}</h3>
                    <h3>{t.search.active}</h3>
                    </div>
                    <SortArrows sortOrder={getSortOrderForColumn('lastActive')} />
                    </button>
            </div>
            {users.map((user) => {
                const isDeleted = deletedAccounts.includes(user.nickname);
                return(
                <div key={user.nickname}
                    className={`flex w-full bg-gray-900/[60%] 
                  text-white py-4 rounded-lg shadow-lg  justify-between px-1 md:px-5  items-center relative`}>
                    {isDeleted && <div className="absolute w-full h-1 bg-red-500 left-0"></div>}
                    <Link to={`/users/${user?.nickname}/profile`}
                        className="max-w-22 md:max-w-35 w-full  text-xs md:text-sm hover:text-amber-200 transition-transform duration-300
                                    flex items-center justify-center ">
                        <div className=" w-full bg-black/40 py-2  rounded-lg flex flex-col 
                                        items-center justify-center gap-2">

                            <img src={`${API_URL}${user?.avatar}`} alt="Profile"
                             className="h-12 w-12 md:w-16 md:h-16 rounded-full outline-2 border-2 border-black" />


                        <div className="text-[10px] md:text-base font-bold mt-auto">{user.nickname}</div>
                        </div>
                    </Link>
                    <div className="pr-10 max-w-7 md:max-w-30 w-full h-full flex items-center justify-center text-sm md:text-xl font-bold  ">
                        <Rating rating={user.rating} />
                    </div>
                    <div className="pr-10 max-w-7 md:max-w-30 w-full h-full flex items-center justify-center text-sm md:text-xl font-bold ">{user.onlineStatus}</div>
                    
                    <div>{/* wrapper */}
                    <div className="pr-7 max-w-7 md:max-w-30 w-full h-full flex items-center justify-center text-sm md:text-xl font-bold ">{deltaLastActive(user.lastActive!)}</div>
                    
                    <div className={`${isDeleted ? 'hidden' : ''} `}>
                    <AddFriendButton userNickname={user.nickname}
                     className="text-sm md:text-2xl absolute  right-2 top-2" />
                     <EditAccountButton nickname={user.nickname}
                      className="text-sm md:text-2xl absolute right-2 top-[50%] translate-y-[-50%]" />
                        
                    <DeleteAccountButton nickname={user.nickname} cb={() => handleDeleteAccount(user.nickname)}
                     className="text-sm md:text-2xl absolute right-2 bottom-2" />

                     </div>
                     </div>
                </div>
            )})}
            
        </>
    );
}