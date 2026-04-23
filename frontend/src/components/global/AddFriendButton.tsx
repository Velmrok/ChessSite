
import { useApi } from "@/hooks/useApi";
import { addFriend, deleteFriend } from "@/services/userService";
import useToastStore from "@/stores/useToastStore";
import useUserStore from "@/stores/useUserStore";
import { useTranslation } from "react-i18next";
import { FaUserFriends } from "react-icons/fa";
import { useNavigate } from "react-router-dom";

type Props = {
    userNickname: string;
    className?: string;

};


export default function AddFriendButton({ userNickname, className }: Props) {
    const user = useUserStore((state) => state.user);
    const friendList = useUserStore((state) => state.user?.friendNicknames);
    const deleteFriendFromStore = useUserStore((state) => state.deleteFriend);
    const addFriendToStore = useUserStore((state) => state.addFriend);
    const setToast = useToastStore((state) => state.setToast)
    const { t: toastT } = useTranslation("toast");
    const {request} = useApi();
    const handleChangeFriendStatus = async (userNickname: string) => {
        if (!user) return;
      
        if (friendList!.includes(userNickname)) {
            await request(() => deleteFriend(userNickname),{
                onError: (message) =>  setToast({ msg: toastT(message), type: "error" })
            });
            setToast({ msg: toastT("success.friendDeleted"), type: "success" });

            deleteFriendFromStore(userNickname);
        } else {
            await request(() => addFriend(userNickname),{
                onError: (message) =>  setToast({ msg: toastT(message), type: "error" })
            });
            setToast({ msg: toastT("success.friendAdded"), type: "success" });
            addFriendToStore(userNickname);
        }
    
    }
    return (
        <>
        { user && user.nickname !== userNickname &&
        <button className={className}
            onClick={() => handleChangeFriendStatus(userNickname)}>
            <FaUserFriends className={` hover:scale-110
                    ${friendList && friendList.includes(userNickname) ? "text-yellow-500 hover:text-red-500/80" : "text-white"}`} />
        </button>}
        </>
    );
}