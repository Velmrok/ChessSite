import { Link, useNavigate } from "react-router-dom";
import ReactCountryFlag from "react-country-flag";
import useUserStore from "@/stores/useUserStore";
import useToastStore from "@/stores/useToastStore";
import { logoutUser } from "@/services/authService";
import { formatTimeFromMs, leaveQueue } from "@/services/socket/socketGlobalService";
import { CiMenuBurger } from "react-icons/ci";
import { useState } from "react";
import SmallScreenMenu from "./SmallScreenMenu";
import { useTranslation } from "react-i18next";
import i18n from "@/i18n/config";
import Avatar from "./Avatar";
import { disconnectSignalR } from "@/services/signalR/connection";


const GuestMenu = () => {
    const { t } = useTranslation("navbar");
    return (

        <>
            <Link to="/login" className="text-[8px] md:text-sm hover:text-amber-200 transition-transform duration-300">{t("login")}</Link>

            <Link to="/register" className="text-[8px] md:text-sm hover:text-amber-200 transition-transform duration-300">{t("register")}</Link>
        </>
    );
}

const UserMenu = ({ onLogout }: { onLogout: () => void }) => {
    const user = useUserStore((state) => state.user);
    const { t } = useTranslation("navbar");

    const isInQueue = useUserStore(state => state.user?.isInQueue);
    const queueTime = useUserStore(state => state.queueTime);

    return (
        <>
            {isInQueue &&

                <div className="text-[13px]  flex flex-col items-center">
                    <div className="flex gap-2 items-center">
                        <div>{t("inQueue")}</div>
                        <div >({formatTimeFromMs(queueTime!)})</div></div>
                    <button className="hover:text-amber-200 transition-transform duration-300 text-[13px] text-gray-400"
                        onClick={() => leaveQueue()}>
                        {t("leaveQueue")} </button>
                </div>}




            <button onClick={onLogout} className="text-xs md:text-sm hover:text-amber-200
         transition-color duration-300 cursor-pointer bg-transparent border-none font-MyFancyFont">
                {t("logout")}
            </button>
            <Link to={`/users/${user?.nickname}/profile`} className="text-xs md:text-sm hover:text-amber-200
         transition-color duration-300 rounded-full outline-2 border-2 border-black">
                <Avatar avatarUrl={user?.profilePictureUrl} className="h-8 w-8 md:w-11 md:h-11 rounded-full" />
            </Link>
        </>
    );
}


export default function Navbar() {
    const user = useUserStore((state) => state.user);
    const { t: toastT } = useTranslation("toast");
    const setUser = useUserStore((state) => state.setUser);
    const [isMenuOpen, setIsMenuOpen] = useState(false);
    const setToast = useToastStore((state) => state.setToast);
    const navigate = useNavigate();

    const handleLogout = async () => {
        try {
            await logoutUser();
            disconnectSignalR();
            useUserStore.getState().setUser(null);
            navigate('/login');
        } catch (error: any) {
            if (error.status === 401) setUser(null);
            else setToast({ msg: toastT("error.logout"), type: "error" });
            return;
        }
        setToast({ msg: toastT("success.logout"), type: "success" });

    };



    const handleChangeLanguage = (lang: string) => {

        i18n.changeLanguage(lang);
    }

    return (
        <nav className="bg-gray-800 w-full h-15 shadow-md shadow-black/40 text-3xl text-white flex
         justify-between items-center px-2 md:px-10 font-MyFancyFont z-100 relative">
            <Link to="/" className="hidden xl:block text-sm  md:text-3xl hover:text-amber-200 transition-color duration-300">ChessSite</Link>
            <button onClick={() => setIsMenuOpen(!isMenuOpen)}
                className="flex xl:hidden scale-125 "><CiMenuBurger /></button>

            <SmallScreenMenu isOpen={isMenuOpen} setIsOpen={setIsMenuOpen} />

            <div className="flex text-lg md:text-2xl text-white gap-4 md:gap-8 items-center">
                {user ? <UserMenu onLogout={handleLogout} /> : <GuestMenu />}


                <div className="flex gap-4">
                    <button onClick={() => handleChangeLanguage("pl")}>
                    <ReactCountryFlag countryCode="PL" className="hover:scale-110 transition-transform duration-300 !w-[1.5em]" svg  />
                </button>
                    <button onClick={() => handleChangeLanguage("en")}>
                        <ReactCountryFlag countryCode="GB" className="hover:scale-110 transition-transform duration-300 !w-[1.5em]" svg />
                    </button>
                </div>
            </div>
        </nav>
    )
}

