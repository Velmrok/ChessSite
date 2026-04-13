import { useEffect, useState } from "react";
import { useParams, useNavigate } from "react-router-dom";
import  UserActionForm  from "@/components/UserAction/UserActionForm";
import { createAccount, editAccount, updateUserBio } from "@/services/userService"; 
import { useUserProfileStore } from "@/stores/useUserProfileStore"; 
import useLanguageStore from "@/stores/useLanguageStore";
import { profile } from "console";

export default function UserActionPage({ mode }: { mode: 'add' | 'edit' }) {
    const { nickname } = useParams(); 
    const navigate = useNavigate();
    const fetchProfile = useUserProfileStore(s => s.fetchProfile);
    const t = useLanguageStore((state) => state.t);
     const setProfile = useUserProfileStore(s => s.setProfile);
    

   
    useEffect(() => {
        if (mode === 'edit' && nickname) {
            const fetchData = async () => {
            await fetchProfile(nickname);
        }
        fetchData();
        }else setProfile(null);
        
    }, [mode, nickname]);
     
    const handleSubmit = async (values: {login:string; nickname: string; password: string; bio: string;rapid: number; blitz: number; bullet: number}) => {
        const data = {
                    ...values,
                    ratings: {
                        rapid: values.rapid,
                        blitz: values.blitz,
                        bullet: values.bullet
                    }
                };
        console.log('Submitting data:', data);
        try {
            if (mode === 'add') {
    
                await createAccount(data);
            } else {
                await editAccount(nickname!, data);
            }
            navigate('/search'); 
        } catch (error) {
            console.error(error);
        }
    };

    return (
        <div className="flex justify-center items-center min-h-screen bg-cyan-800">
            <div className="bg-white p-8 rounded shadow-lg w-full max-w-md flex flex-col items-center w-full">
                <h1 className="text-2xl mb-4 text-black font-bold">
                    {mode === 'add' ? t.createAccount.addAccount : t.createAccount.editAccount}
                </h1>
                <UserActionForm 
                    onSubmit={handleSubmit} 
                    mode={mode} 
                />
            </div>
        </div>
    );
}