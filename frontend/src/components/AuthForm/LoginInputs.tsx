import React, { useEffect, useState } from "react";
import { useContext } from "react";
import useLanguageStore from "@/stores/useLanguageStore";
import type { LoginFormType } from "@/types/Auth";



type Props = {
    handleSubmit: (e: React.FormEvent,
        form: LoginFormType) => void;
}
export default function LoginInputs({ handleSubmit }: Props) {

    const [form, setForm] = useState({
        login: "",
        password: ""
    });
 const t = useLanguageStore((state) => state.t);


    return (
        <>
            <form onSubmit={(e) => handleSubmit(e, form)} className="flex flex-col gap-4">
                <div className="flex flex-col">
                    <span>{t.auth.login}</span>
                    <input
                        type="text"
                        placeholder={t.auth.login}
                        value={form.login}
                        onChange={(e) => setForm({ ...form, login: e.target.value })}
                        className="border border-gray-300 p-2 rounded focus:outline-none focus:border-blue-500"
                    /></div>


                <div className="flex flex-col">
                    <span>{t.auth.password}</span>
                    <input
                        type="password"
                        placeholder={t.auth.password}
                        value={form.password}
                        onChange={(e) => setForm({ ...form, password: e.target.value })}
                        className="border border-gray-300 p-2 rounded focus:outline-none focus:border-blue-500"
                    /></div>

                <button
                    type="submit"
                    className="bg-blue-600 text-white py-2 rounded hover:bg-blue-700 transition-colors font-bold"
                >
                    {t.auth.login}
                </button>
            </form>

        </>
    )
}