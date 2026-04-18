import useUserStore from "@/stores/useUserStore";
import LoginForm from "../components/AuthForm/LoginForm";
import {  useNavigate } from "react-router-dom";
import { useEffect, useState } from "react";

export default function LoginPage() {
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
           <LoginForm setUser={setUser} />
        </div>
    );
}