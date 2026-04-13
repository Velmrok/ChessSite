import useUserStore from "@/stores/useUserStore";
import AuthForm from "../components/AuthForm/AuthForm";
import {  useNavigate } from "react-router-dom";
import { useEffect } from "react";
type Props = {
    initialisLogin?: boolean;
};
export default function AuthPage({ initialisLogin }: Props) {
    const user = useUserStore((state) => state.user);
    const navigate = useNavigate();
    const setUser = useUserStore((state) => state.setUser);
    useEffect (() => {
        if(user){
            navigate("/");
        }
    }, [user, navigate]);
    return (
        <div className="bg-cyan-800 w-full min-h-screen flex justify-center items-center">
            <AuthForm initialisLogin={initialisLogin!} setUser={setUser} />
        </div>
    );
}