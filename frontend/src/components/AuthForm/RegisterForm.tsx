import { useState,useEffect, useContext } from "react";
import { useNavigate } from "react-router-dom";
import { getMe, loginUser, registerUser } from "../../services/authService";
import useToastStore from "@/stores/useToastStore";
import useLanguageStore from "@/stores/useLanguageStore";
import { useApi } from "@/hooks/useApi";
import type { RegisterFormType } from "@/types/Auth";
import RegisterInputs from "./RegisterInputs";
import useUserStore from "@/stores/useUserStore";

export default function RegisterForm() {
    const t = useLanguageStore((state) => state.t);
    const setUser = useUserStore((state) => state.setUser);
    const navigate = useNavigate();
    const setToast = useToastStore((state) => state.setToast);

    const [error, setError] = useState("");
    const { request, loading } = useApi();

    const handleSubmit = async (e: React.FormEvent, form: RegisterFormType) => {
        e.preventDefault();

        const user = await request(() => registerUser(form), {
            onError: (message) => {
                const translation =
                    t.toast.error[message as keyof typeof t.toast.error] ??
                    t.toast.error.generic;
                setError(translation);
            }
        });

        if (user) {
            setUser(user);
            setToast({ msg: t.toast.success.register, type: "success" });
            navigate("/");
        }


    };

    return (
        <div className="bg-white p-6 md:p-8 rounded-xl shadow-2xl w-full mx-4 max-w-md text-gray-800">
            <h2 className="text-2xl md:text3xl font-bold mb-6 text-center">
                {t.auth.titleRegister }
            </h2>
            {error && (
                    <div className="bg-red-100 border border-red-400 text-red-700 py-1 rounded text-sm text-center ">
                        {error}
                    </div>
                )}
            <RegisterInputs handleSubmit={handleSubmit} />
            <div className="mt-4 text-center text-sm">
                <p>
                    {t.auth.questionLogin}
                    <button
                        onClick={() =>  navigate("/login" )}
                        className="text-blue-600 ml-4 font-bold hover:underline"
                    >
                        {t.auth.actionLogin}
                    </button>
                </p>
            </div>
        </div>
    );
}
