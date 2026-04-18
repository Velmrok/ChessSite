import { useState } from "react";
import { useNavigate } from "react-router-dom";
import { getMe, loginUser } from "../../services/authService";
import useToastStore from "@/stores/useToastStore";
import useLanguageStore from "@/stores/useLanguageStore";
import { useApi } from "@/hooks/useApi";

import type { LoginFormType } from "@/types/Auth";
import LoginInputs from "./LoginInputs";

type Props = {
    setUser: (user: User) => void;
};
export default function LoginForm({ setUser }: Props) {
    const t = useLanguageStore((state) => state.t);

    const navigate = useNavigate();
    const setToast = useToastStore((state) => state.setToast);

    const [error,setError] = useState("");
    const { request, loading } = useApi();

    const handleSubmit = async (e: React.FormEvent, form: LoginFormType) => {
        e.preventDefault();

        const user = await request(() => loginUser(form), { useToast: false, onError: (message) => setError(message) });
        if (user) {

            setUser(user);
            setToast({ msg: t.toast.success.login, type: "success" });
            navigate("/");
        }


    };

    return (
        <div className="bg-white p-6 md:p-8 rounded-xl shadow-2xl w-full mx-4 max-w-md text-gray-800">
            <h2 className="text-2xl md:text3xl font-bold mb-6 text-center">
                {t.auth.titleLogin }
            </h2>
            {error && (
                    <div className="bg-red-100 border border-red-400 text-red-700 py-1 rounded text-sm text-center ">
                        {error}
                    </div>
                )}
            <LoginInputs handleSubmit={handleSubmit} />
            <div className="mt-4 text-center text-sm">
                <p>
                    {t.auth.questionRegister}
                    <button
                        onClick={() => navigate("/register")}
                        className="text-blue-600 ml-4 font-bold hover:underline"
                    >
                        {t.auth.actionRegister}
                    </button>
                </p>
            </div>
        </div>
    );
}
