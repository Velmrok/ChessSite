
import { addFriend, deleteFriend } from "@/services/userService";
import useLanguageStore from "@/stores/useLanguageStore";
import useToastStore from "@/stores/useToastStore";
import useUserStore from "@/stores/useUserStore";
import { FaUserFriends } from "react-icons/fa";
import { useNavigate } from "react-router-dom";

type Props = {
  userNickname: string;
    className?: string;  
   
};


export default function AddFriendButton({ userNickname,className}: Props) {
    const user = useUserStore((state) => state.user);
    const friendList = useUserStore((state) => state.user?.friendList);
    const deleteFriendFromStore = useUserStore((state) => state.deleteFriend);
    const addFriendToStore = useUserStore((state) => state.addFriend);
    const setToast = useToastStore((state) => state.setToast)
    const t = useLanguageStore((state) => state.t);
    const navigate = useNavigate();
     const handleChangeFriendStatus = async (userNickname: string) => {
        if(!user){
            navigate("/login");
            return;
        }
        try {
            if(friendList!.includes(userNickname)){
                await deleteFriend(userNickname);
                setToast({msg: t.friendDeleted, type: "success"});
                
                deleteFriendFromStore(userNickname);
            }else{
                await addFriend(userNickname);
                setToast({msg: t.friendAdded, type: "success"});
                addFriendToStore(userNickname);
            } 
        } catch (error) {
            console.log(error);
            setToast({msg: t.friendAddError, type: "error"});
        }
    }
    return (
        <button className={className}
                    onClick={() => handleChangeFriendStatus(userNickname)}>
                    <FaUserFriends className={` hover:scale-110
                    ${friendList && friendList.includes(userNickname) ? "text-yellow-500 hover:text-red-500/80" : "text-white"}`}  />
                    </button>
    );
}