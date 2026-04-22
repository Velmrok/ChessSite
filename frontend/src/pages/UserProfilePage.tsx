import { useEffect, useState } from "react";
import UserAvatar from "../components/UserProfile/UserAvatar";
import UserBio from "../components/UserProfile/UserBio";
import UserGameHistory from "../components/UserProfile/UserGameHistory";
import { useParams } from "react-router-dom";
import UserInfo from "../components/UserProfile/UserInfo";
import NotFound from "./NotFound";
import useUserStore from '@/stores/useUserStore';
import AddFriendButton from "../components/global/AddFriendButton";
import FriendList from "@/components/UserProfile/FriendList";
import { useUserProfileStore } from "@/stores/useUserProfileStore";
import Loading from "../components/global/Loading";
import DeleteAccountButton from "../components/global/DeleteAccountButton";
import EloChart from "@/components/UserProfile/EloChart";
import ChangePasswordForm from "@/components/UserProfile/ChangePasswordForm";
import { useTranslation } from "react-i18next";
import { fetchUserProfile } from "@/services/userService";
import { useApi } from "@/hooks/useApi";


export default function UserProfilePage() {
    const nickname = useParams<{ nickname: string }>().nickname;
    const profile = useUserProfileStore(s => s.profile);
    const setProfile = useUserProfileStore(s => s.setProfile);
    //const fetchProfile = useUserProfileStore(s => s.fetchProfile);
    const [showPasswordForm, setShowPasswordForm] = useState(false);
    const [isLoading, setIsLoading] = useState(true);
    const [activeTab, setActiveTab] = useState<'history' | 'friends'|'elo'>('history');
    const user = useUserStore((state) => state.user);
    const { t } = useTranslation('profile');
    const loggedUserFriendList = useUserStore((state) => state.friendList);
    const [isDeleted, setIsDeleted] = useState(false);
    const {request} = useApi();

    
    useEffect( () => {
        const fetch = async () => {
        setIsLoading(true);
        var profile = await request(()=>fetchUserProfile(nickname!))
        if(profile)
            setProfile(profile);
        setIsLoading(false);
       
    };
        fetch();
    }, [nickname]);

    const handleUserAvatarUpdate = (newAvatarUrl: string) => {
        if (profile) {
            const uniqueUrl = `${newAvatarUrl}?t=${Date.now()}`;
            setProfile({ ...profile, avatar: uniqueUrl });
        }
    };
    if (isLoading) {
        return (
            <div className="flex justify-center items-center min-h-screen bg-cyan-800 ">
                <Loading />
            </div>
        )
    }
    if(isDeleted){
        return(
        <div className="flex justify-center items-center text-white font-MyFancyFont text-4xl
         min-h-screen bg-cyan-800 pt-10 pb-10 ">
            {t('delete')}
        </div>
        )
    }

    return (
        <>
        
            {!profile &&
                <NotFound whatIsMissing="user" />
            }
            {profile && (
                <div className="flex justify-center items-start min-h-screen bg-cyan-800 pt-10 pb-10">
                    <div className="flex flex-col items-center gap-6 mb-10 min-h-screen w-[90%]  md:w-[80%] 
             max-w-[1000px]  bg-cyan-900 rounded-lg shadow-lg p-6 relative">
                        {user&&user.nickname===profile.nickname&&(
                            <button className="absolute top-4 left-4 bg-blue-500 text-white shadow-md z-10
                        px-4 py-2 rounded hover:scale-105 transition-transform duration-200 font-MyFancyFont text-sm" onClick={() => setShowPasswordForm(true)}>
                            {t('changePassword')}
                            </button>
                        )}
                        {(!user || profile.nickname !== user?.nickname) && (
                            profile.onlineStatus === 'online' ? (
                                <span className="text-green-500  text-sm sm:text-2xl font-MyFancyFont absolute top-2 left-3">● {t('online')}</span>
                            ) : (
                                <span className="text-red-500 text-sm sm:text-2xl font-MyFancyFont absolute top-2 left-3">● {t('offline')}</span>
                            ))}
                        <div className="flex flex-col lg:flex-row relative items-center  w-full gap-5 ">

                            <UserAvatar avatar={profile.avatar} onAvatarUpdate={handleUserAvatarUpdate} />
                            <div className="flex flex-col  w-full max-w-full gap-4 items-center lg:items-start">
                                <div className="flex gap-5 text-white text-2xl mdtext-2xl lg:text-4xl  font-MyFancyFont lg:ml-4 pt-6 md:pt-0 ">
                                    {profile.nickname}

                                    {user && user?.nickname !== profile.nickname &&
                                        <div className="text-[6px] md:text-[8px] flex flex-col items-center justify-center
                                ">
                                            <AddFriendButton userNickname={profile.nickname} className="text-2xl" />
                                            {loggedUserFriendList.includes(profile.nickname) ? t('deleteFriend') : t('addFriend')}
                                        </div>
                                        }
                                </div>
                                {user && user?.nickname !== profile.nickname && user.role === 'admin' &&
                                <div className="absolute right-0 top-0   flex justify-center items-center gap-2" >
                                    <span className="text-sm text-white">{t('deleteAccount')}</span>
                                <DeleteAccountButton nickname={profile.nickname} cb={() => { setIsDeleted(true)}}
                                 className="text-2xl"/>
                                 </div>
                                }
                                <div className="grid w-full gap-6 lg:gap-6 lg:grid-cols-10 justify-items-center w-full ">
                                    <UserBio key={profile.nickname}
                                        bio={profile.bio} nickname={profile.nickname} />
                                    <UserInfo userInfo={profile.userInfo} />
                                </div>
                            </div>
                        </div>
                        <div className="w-full bg-gray-900/40 text-white font-MyFancyFont p-2 flex rounded-md xl:hidden">
                            <div className="w-[50%] flex justify-center">
                                <button className="bg-gray-900/60 p-2 text-sm md:text-lg rounded-md"
                                    onClick={() => setActiveTab('history')}
                                >{t('gameHistory')}</button>
                            </div>
                            <div className="w-[50%] flex justify-center">
                                <button className="bg-gray-900/60 p-2 text-sm md:text-lg rounded-md"
                                    onClick={() => setActiveTab('friends')}
                                >{t('friendList')}</button>
                            </div>
                            <div className="w-[50%] flex justify-center">
                                <button className="bg-gray-900/60 p-2 text-sm md:text-lg rounded-md"
                                    onClick={() => setActiveTab('elo')}
                                >{t('eloChart')}</button>
                            </div>
                        </div>
                         <div className={`${activeTab === 'elo' ? 'flex' : 'hidden'} xl:flex w-full justify-center mb-10`}>
                            <EloChart />
                            </div>
                        <div className={`w-full justify-center  mb-10
                ${activeTab === 'history' ? 'flex' : 'hidden'} xl:flex`}>

                            <UserGameHistory />
                        </div>
                        <div className={`w-full  ${activeTab === 'friends' ? 'block' : 'hidden'} xl:hidden`}>

                            <FriendList />
                        </div>
                    </div>
                    <div className="hidden xl:flex ml-5 min-w-[250px] mb-10  min-h-screen ">
                        <div className="w-full ">

                            <FriendList />
                        </div>
                    </div>
                </div>
            )}
            {showPasswordForm && <ChangePasswordForm onClose={() => setShowPasswordForm(false)} />}
        </>
    )
}