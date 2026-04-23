import { useRef, useState } from "react";
import { updateUserBio } from "../../services/userService";
import useToastStore from "@/stores/useToastStore";
import useUserStore from "@/stores/useUserStore";
import { useTranslation } from "react-i18next";
import { useApi } from "@/hooks/useApi";



type Props = {
    bio: string;
    nickname?: string;
};

export default function UserBio({ bio, nickname }: Props) {

    const user = useUserStore((state) => state.user);
    const { t } = useTranslation("profile");
    const { t: toastT } = useTranslation("toast");
    const [editing, setEditing] = useState(false);
    const [bioValue, setBioValue] = useState(bio);
    const setToast = useToastStore((state) => state.setToast);
    const textAreaRef = useRef<HTMLTextAreaElement>(null);

    const { request } = useApi();

    const handleEditClick = async () => {
        const newValue = textAreaRef.current?.value;

        const updatedBio = await request(() => updateUserBio(nickname!, newValue || ""), {
            onError: (message) => {
               
                const errorMessage = toastT("error." + message) + ( message=="bioTooLong" ? ` ${textAreaRef.current?.value.length}/200` : "" );
                setToast({ msg: errorMessage, type: "error" });
                return;

            }
        });
        if (updatedBio) {
            setBioValue(updatedBio.bio);
            setEditing(false);

            setToast({ msg: toastT('success.changedBio'), type: "success" });
        }
    };
    return (

        <div className="flex flex-col gap-4 items-center lg:items-start h-auto w-full md:w-[80%]
        lg:max-w-[95%] lg:w-[35vw] lg:w-full xl:col-span-6">


            <div className={`min-w-full min-h-50 flex flex-col bg-gray-900/[30%] p-4 rounded-lg
         shadow-md text-white font-MyFancyFont text-sm  
         ${editing ? 'outline outline-2 outline-gray-300' : ''}`}>
                {editing ? (
                    <>
                        <textarea
                            className="w-full flex-1 bg-transparent resize-none 
               outline-none border-none focus:ring-0 focus:outline-none"
                            defaultValue={bioValue}
                            ref={textAreaRef}
                        />
                    </>
                ) : (<>

                    <h3 className="whitespace-pre-wrap break-all flex-1" >{bioValue}</h3>
                    {user && user.nickname == nickname &&
                        <div className="flex justify-end mt-2">
                            <button className="px-4 py-2 bg-blue-500 text-white rounded hover:scale-105 shadow-lg shadow-black/40" onClick={() => setEditing(true)}>
                                {t('editBio')}</button>
                        </div>}
                </>
                )
                }

            </div>
            {editing && <div className="flex  w-full justify-end gap-4 font-MyFancyFont">
                <button className="px-4 py-2 bg-blue-500 text-white rounded hover:scale-105 shadow-lg shadow-black/40"
                    onClick={() => setEditing(false)}>
                    {t('cancel')}
                </button>
                <button className="px-4 py-2 bg-blue-500 text-white rounded hover:scale-105 shadow-lg shadow-black/40" onClick={handleEditClick}>
                    {t('save')}
                </button>
            </div>}
        </div>
    )
}