import { useApi } from "@/hooks/useApi";
import { getEloHistory } from "@/services/userService";
import { useEffect, useState } from "react";
import { useTranslation } from "react-i18next";
import { MdAccessTime } from "react-icons/md";
import { SiPushbullet, SiStackblitz } from "react-icons/si";
import { useParams } from "react-router-dom";
import { Line, LineChart, Tooltip, XAxis, YAxis } from "recharts";

export default function EloChart() {

    const [eloHistory, setEloHistory] = useState<Array<{ date: string; rating: number }>>([]);
    const nickname = useParams<{ nickname: string }>().nickname;
    const [gameType, setGameType] = useState<'ratingRapid' | 'ratingBlitz' | 'ratingBullet'>('ratingRapid');
    const { t } = useTranslation("profile");

    const { request } = useApi();
    useEffect(() => {
        const fetch = async () => {
            const data = await request(() => getEloHistory(nickname!, gameType));
            if (data)
            setEloHistory(data.map((x, i) => ({ i, date: x.date.slice(0, 10), rating: x.rating })));
           
        };
        fetch();
    }, [nickname, gameType]);





    return (
        <div className="font-MyFancyFont flex flex-col  bg-gray-800/30 p-4 rounded-lg
         w-full max-w-3xl mb-6 gap-5 overflow-x-auto ">
            <div className="flex gap-2 md:gap-3 ">
                <button
                    type="button"
                    className={`hover:scale-110 flex items-center ${gameType === 'ratingRapid' ? ' border-b-3 border-green-500' : ''}`}
                    onClick={() => setGameType('ratingRapid')}
                >
                    <MdAccessTime className="text-green-500 text-base md:text-xl inline" />
                </button>
                <button
                    type="button"
                    className={`hover:scale-110 flex items-center ${gameType === 'ratingBlitz' ? ' border-b-3 border-yellow-300' : ''}`}
                    onClick={() => setGameType('ratingBlitz')}
                >
                    <SiStackblitz className="text-yellow-300 text-base md:text-xl inline" />
                </button>
                <button
                    type="button"
                    className={`hover:scale-110 flex items-center ${gameType === 'ratingBullet' ? ' border-b-3 border-red-500' : ''}`}
                    onClick={() => setGameType('ratingBullet')}
                >
                    <SiPushbullet className="text-red-500 text-base md:text-xl" />
                </button>

            </div>
            <LineChart width={600} height={300} data={eloHistory}>
                <XAxis dataKey="i" stroke="#ffffffff" />
                <YAxis stroke="#ffffffff" />
                <Tooltip />
                <Line stroke="#8884d8" dataKey="rating" name={t('rating')} />
            </LineChart>
        </div>
    )
}

