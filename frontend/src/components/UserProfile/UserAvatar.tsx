import { useState} from 'react'
import { uploadUserAvatar } from '../../services/userService';
import { useParams } from 'react-router-dom';
import useToastStore from '@/stores/useToastStore';
import useUserStore from '@/stores/useUserStore';
import { useTranslation } from 'react-i18next';


type Props = {
    avatar: string;
    className?: string;
    onAvatarUpdate: (newAvatarUrl: string) => void;
}
export default function UserAvatar({avatar, className, onAvatarUpdate}:Props) {
    const user = useUserStore((state) => state.user);
    const setUser = useUserStore((state) => state.setUser);
    const {nickname} = useParams<{nickname: string}>();
    const {t} = useTranslation("profile");
    const {t: toastT} = useTranslation("toast");
    const [file, setFile] = useState<File | null>(null);
    const setToast = useToastStore((state) => state.setToast);
    const handleFileChange = async (e: React.ChangeEvent<HTMLInputElement>)=>{
        const file = e.target.files ? e.target.files[0] : null;
        setFile(file);
        if(file)handleAvatarUpload(file);
    }
    const handleAvatarUpload = async (file: File)=>{
        if(file){
            try{
                const response = await uploadUserAvatar(file);
                setToast({ msg: toastT('success.changedAvatar'), type: "success" });
                const newAvatarUrl = response.avatar;
                onAvatarUpdate!(newAvatarUrl);
                setFile(null);
                const uniqueUrl = `${newAvatarUrl}?t=${Date.now()}`;
                setUser({ ...user!, avatar: uniqueUrl });
            }
            catch(error){
                console.error("Error uploading avatar:", error);
                setToast({ msg: toastT('error.changedAvatar'), type: "error" });
            }
        }else{
            setToast({ msg: toastT('error.avatarNoFile'), type: "error" });
        }
    }

    const defaultClass = "flex flex-col items-center  w-65   gap-4 md:ml-4";
    return(
        <div className={className || defaultClass}>
        
           <img src={`${avatar == "" ? "/default-avatar.png" : avatar}`} alt="Profile" 
           className="w-30 h-30 md:w-40 md:h-40  rounded-full border-2 border-black outline-12 outline-cyan-500"/>
           { user && user.nickname === nickname &&
           <div className="flex flex-row items-center gap-4 ">
           <label className="px-6 py-1 md:px-4 md:py-3 bg-blue-500 text-white text-[12px] rounded hover:scale-105
            font-MyFancyFont transition-transform duration-300 shadow-lg shadow-black/40 cursor-pointer">
                   <span>{file ? file.name : "📂 " + t('changeAvatar')}</span>
                   <input  type="file" accept="image/*" onChange={handleFileChange} className="hidden" />
               </label>
           </div>
        }
       
        </div>
    )
}