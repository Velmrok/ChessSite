import useToastStore from "@/stores/useToastStore";
import { deleteAccount } from "../../services/userService";
import useLanguageStore from "@/stores/useLanguageStore";
import { FaWindowClose } from "react-icons/fa";
import useUserStore from "@/stores/useUserStore";
import { useState } from "react";
import ConfirmModal from "./ConfirmModal";

type Props = {
    nickname: string;
    className?: string;
    cb?: ()=>void;
};

export default function DeleteAccountButton({ nickname, className, cb }: Props) {
    const user = useUserStore((state) => state.user);
    const setToast = useToastStore((state) => state.setToast);
    const t = useLanguageStore((state) => state.t);
    const [isConfirming, setIsConfirming] = useState(false);
    const handleDeleteAccount = async () => {
        try {
            await deleteAccount(nickname);
            if(cb) cb();
            setToast({ msg: t.toast.success.deleteAccount, type: "success" });
           
        } catch (error) {
            console.error("Error deleting account:", error);
            setToast({ msg: t.toast.error.deleteAccount, type: "error" });
        }
    };

    const askForConfirmation = () => {setIsConfirming(true);}


    return (<>
        {user&&user.role=="admin" && <button
            className={className + " hover:scale-110 hover:text-red-700 text-red-500 "}
            onClick={askForConfirmation}
            ><FaWindowClose/>
        </button>}

        <ConfirmModal
            isOpen={isConfirming}
            onClose={() => setIsConfirming(false)}
            onConfirm={handleDeleteAccount}
            title={t.modal.deleteAccount.title}
            message={t.modal.deleteAccount.message}
        />
    </>
    )
}