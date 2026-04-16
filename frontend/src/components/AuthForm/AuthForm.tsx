import { useState,useEffect, useContext } from "react";
import { useNavigate } from "react-router-dom";
import AuthInputs from "./AuthInputs";
import { getMe, loginUser, registerUser } from "../../services/authService";
import useToastStore from "@/stores/useToastStore";
import useLanguageStore from "@/stores/useLanguageStore";
import { useApi } from "@/hooks/useApi";

type Props = {
    initialisLogin: boolean;
    setUser: (user: User) => void;
    
};
export type formType = {
    login: string;
    email: string;
    nickname: string;
    password: string;
}
export default function AuthForm({initialisLogin, setUser}: Props) {
    const t = useLanguageStore((state) => state.t);
    const navigate = useNavigate();
    const [isLogin, setIsLogin] = useState(initialisLogin);
    const [error, setError] = useState("");
    const setToast = useToastStore((state) => state.setToast);
    const { request, loading } = useApi();
    useEffect(() => {
        setIsLogin(initialisLogin);
    }, [initialisLogin]);
    
     const handleSubmit = async (e: React.FormEvent, form: formType) => {
        e.preventDefault();
        setError("");

        const fn = isLogin
            ? async () => await loginUser({ login: form.login, password: form.password })
            : async () => await registerUser({ login: form.login, email: form.email, password: form.password, nickname: form.nickname });
        await request(fn,
            {
                onError: (message: string) => setError(message),
                showToast: (message: string) => setToast({ msg: message, type: "error" })
            }
        );

        if (isLogin) {
            const me = await getMe();
            setUser(me);
            setToast({ msg: t.toast.success.login, type: "success" });
    }else setToast({ msg: t.toast.success.register, type: "success" });
    navigate("/");
            

        
    };
    
    return (
        <div className="bg-white p-6 md:p-8 rounded-xl shadow-2xl w-full mx-4 max-w-md text-gray-800">
            <h2 className="text-2xl md:text3xl font-bold mb-6 text-center">
                {isLogin ? t.auth.titleLogin : t.auth.titleRegister}
            </h2>
            {error && (
                    <div className="bg-red-100 border border-red-400 text-red-700 py-1 rounded text-sm text-center ">
                        {error}
                    </div>
                )}
            <AuthInputs isLogin={isLogin} handleSubmit={handleSubmit} />
            <div className="mt-4 text-center text-sm">
                <p>
                    {isLogin ? t.auth.questionRegister : t.auth.questionLogin}
                    <button
                        onClick={() => navigate(isLogin ? "/register" : "/login")}
                        className="text-blue-600 ml-4 font-bold hover:underline"
                    >
                        {isLogin ? t.auth.actionRegister : t.auth.actionLogin}
                    </button>
                </p>
            </div>
        </div>
    );
}
