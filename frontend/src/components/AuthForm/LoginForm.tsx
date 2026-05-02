import { useState } from "react";
import { useNavigate } from "react-router-dom";
import { loginUser } from "../../services/authService";
import useToastStore from "@/stores/useToastStore";
import { useApi } from "@/hooks/useApi";

import type { LoginFormType } from "../../types/auth";
import LoginInputs from "./LoginInputs";
import useUserStore from "@/stores/useUserStore";
import { useTranslation } from "react-i18next";
import { useErrorTranslation } from "@/hooks/useErrorTranslation";

export default function LoginForm() {
    const {t: authT} = useTranslation("auth");
    const errorT = useErrorTranslation();
    const {t: toastT} = useTranslation("toast");
    const setUser = useUserStore((state) => state.setUser);
    const navigate = useNavigate();
    const setToast = useToastStore((state) => state.setToast);

    const [error,setError] = useState("");
    const { request } = useApi();
    const setQueueData = useUserStore((state) => state.setQueueData);

    const handleSubmit = async (e: React.FormEvent, form: LoginFormType) => {
        e.preventDefault();

        const user = await request(() => loginUser(form), {
            onError: (message) => {
                const translation = errorT(message);
                setError(translation);
            }
        });
        if (user) {

            setUser(user);
            setQueueData(user.queueData);
            setToast({ msg: toastT('success.login'), type: "success" });
            navigate("/");
        }


    };

    return (
        <div className="bg-white p-6 md:p-8 rounded-xl shadow-2xl w-full mx-4 max-w-md text-gray-800">
            <h2 className="text-2xl md:text3xl font-bold mb-6 text-center">
                {authT('login')}
            </h2>
            {error && (
                    <div className="bg-red-100 border border-red-400 text-red-700 py-1 rounded text-sm text-center ">
                        {error}
                    </div>
                )}
            <LoginInputs handleSubmit={handleSubmit} />
            <div className="mt-4 text-center text-sm">
                <p>
                    {authT('questionRegister')}
                    <button
                        onClick={() => navigate("/register")}
                        className="text-blue-600 ml-4 font-bold hover:underline"
                    >
                        {authT('actionRegister')}
                    </button>
                </p>
            </div>
        </div>
    );
}
