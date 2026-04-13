import { changePassword } from "@/services/userService";
import useLanguageStore from "@/stores/useLanguageStore";
import useToastStore from "@/stores/useToastStore";
import useUserStore from "@/stores/useUserStore";
import { useFormik } from "formik";
import { FaWindowClose } from "react-icons/fa";

type Props = {
    onClose: () => void;
};
export default function ChangePasswordForm({ onClose }: Props) {
    const user = useUserStore((state) => state.user);
    const setToast = useToastStore((state) => state.setToast);
    const t = useLanguageStore((state) => state.t);
    if(!user) return null;
    const formik = useFormik({
        initialValues: {
            currentPassword: '',
            newPassword: '',
        },
        onSubmit: async (values) => {
            try{
            await changePassword(user.nickname, values.currentPassword, values.newPassword);
            setToast({ msg: t.toast.success.changePassword, type: "success" });
            formik.resetForm();
            onClose();

            } catch (error) {
                console.error("Failed to change password:", error);
                if((error as Error).message==="InvalidPassword"){
                    setToast({ msg: t.toast.error.wrongPassword, type: "error" });
                    return;
                }
                setToast({ msg: t.toast.error.changePassword, type: "error" });
            }
        }

    });
    return(
        <form onSubmit={formik.handleSubmit} className="flex flex-col gap-4">
            <div className="bg-black/20 fixed w-full h-screen top-0 left-0 z-20 flex items-center justify-center">
            <div className="flex flex-col justify-between items-center bg-white p-6 rounded-lg h-[220px]
             w-[90%] max-w-[400px] relative z-30 pointer-events-auto">
                <button onClick={onClose} className="absolute top-2 right-2 text-red-500 text-lg hover:text-red-700 transition-colors duration-300"><FaWindowClose/></button>
            <input type="password"
                name="currentPassword"
                placeholder={t.profile.currentPassword}
                onChange={formik.handleChange}
                value={formik.values.currentPassword}
                className="border border-gray-300 p-2 rounded focus:outline-none focus:border-blue-500 w-[90%]">
                
            </input>
            <input type="password"
                name="newPassword"
                placeholder={t.profile.newPassword}
                onChange={formik.handleChange}
                value={formik.values.newPassword}
                className="border border-gray-300 p-2 rounded focus:outline-none focus:border-blue-500 w-[90%]">
            </input>
            <button type="submit" className="bg-blue-500 hover:bg-blue-600 text-white font-bold py-2 px-4 rounded transition-color duration-300">
                {t.profile.submit}
            </button>
            </div>
            </div>
            </form>

    )
}