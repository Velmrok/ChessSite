import { useChat } from "@/hooks/useChat";
import useUserStore from "@/stores/useUserStore";
import { useParams } from "react-router-dom";
import { sendMessage } from '@/services/socket/socketGameService';
import { useEffect, useRef } from "react";
import { useTranslation } from "react-i18next";

type Props = {
    previousMessages: Array<Message>;
}
export default function Chat({ previousMessages }: Props) {
    const gameId = useParams().gameId!;
    const { t } = useTranslation("game");
    const user = useUserStore((state) => state.user);
    const { messages } = useChat({ gameId, previousMessages });
    const chatRef = useRef<HTMLDivElement>(null);

    useEffect(() => {
        if (!chatRef.current) return;
        chatRef.current.scrollTop = chatRef.current?.scrollHeight;
    }, [messages]);
    const handleSendMessage = (e: any) => {
        if (e.key === 'Enter') {
            e.preventDefault();
            const text = e.target.value.trim();
            if (text.length > 0) {
                sendMessage(text, gameId);
                e.target.value = '';
            }
        }
    }
    return (
        <div className="w-full h-[50%] bg-black/40 rounded-lg font-MyFancyFont p-2 overflow-y-auto flex flex-col justify-between">
            <h2 className="text-white font-MyFancyFont text-lg mb-2">{t('chatTitle')}</h2>
            <div
                className="w-full flex-1 overflow-y-auto space-y-2 pr-1 "
                ref={chatRef}
            >
                {messages && messages.map((msg, index) => {
                    const isMine = user != null && msg.nickname === user.nickname;
                    const isSameOwnerPrev = index > 0 && messages[index - 1].nickname === messages[index].nickname;
                    return (
                        <div className={`flex flex-col ${isMine ? 'items-end' : 'items-start'}`}>
                            {!isSameOwnerPrev && <span className="text-xs">{msg.nickname}</span>}
                            <div className="inline-flex max-w-[75%] ">

                                <div
                                    className={` p-2  rounded-md text-sm break-words text-left
                            ${isMine ? 'bg-blue-600 text-white' : 'bg-gray-700 text-white'}
                                                                    `}
                                >
                                    {msg.text}
                                </div>
                            </div>
                        </div>
                    );
                })}

            </div>
            <div className="w-full max-h-[50px] bg-black/20 rounded-md p-2  overflow-y-auto flex-1">
                <input onKeyDown={handleSendMessage}
                    type="text" className="w-full h-full p-2 focus:outline-2 focus:outline-white rounded-sm" placeholder={t('chatPlaceholder')}></input>
            </div>

        </div>
    );
}