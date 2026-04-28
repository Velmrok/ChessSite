
import { useState } from "react";
import { useTranslation } from "react-i18next";
import { MdAccessTime } from "react-icons/md";
import { SiPushbullet, SiStackblitz } from "react-icons/si";
import { TfiCup } from "react-icons/tfi";
import { Link } from "react-router-dom";
import Avatar from "../global/Avatar";
import type { LeaderboardResponse} from "@/types/home";


type Props = {
  leaderboard: LeaderboardResponse | undefined;
};

export default function LeaderBoard({ leaderboard }: Props) {
  const { t } = useTranslation("home");
  const [gameType, setGameType] = useState<"topRapidPlayers" | "topBlitzPlayers" | "topBulletPlayers">("topRapidPlayers");


  const handleChangeGameType = (e: any, type: "topRapidPlayers" | "topBlitzPlayers" | "topBulletPlayers") => {
    e.preventDefault();
    setGameType(type);
  };

  if (!leaderboard || leaderboard[gameType].length === 0) {
    return (
      <div className="bg-gray-900/20 shadow-md text-white font-MyFancyFont p-4 rounded-md w-full flex 
      items-center justify-center flex-col">
        <div className="text-2xl ">{t('leaderboardTitle')}</div>
        <div className="mt-4">{t('noData')}</div>
      </div>
    );
  }
  const gameTypeIcon = {
    topRapidPlayers: <MdAccessTime className="text-green-500 text-base md:text-xl inline" />,
    topBlitzPlayers: <SiStackblitz className="text-yellow-300 text-base md:text-xl inline" />,
    topBulletPlayers: <SiPushbullet className="text-red-500 text-base md:text-xl inline" />,
  }


  return (
    <div className="bg-gray-900/20 shadow-md text-white  font-MyFancyFont p-4 rounded-md w-full
     flex flex-col items-center">
      <div className="flex flex-col gap-3 items-center justify-between items-center mb-4">
        <div className="text-2xl">{t('leaderboardTitle')}</div>
        <div className="flex gap-2 md:gap-3">
          <button className={`hover:scale-110 ${gameType === 'topRapidPlayers' ? 'scale-80 border-b-3 border-green-500' : ''}`}
            onClick={(e) => handleChangeGameType(e, 'topRapidPlayers')}>
            {gameTypeIcon.topRapidPlayers}</button>
          <button className={`hover:scale-110 ${gameType === 'topBlitzPlayers' ? 'scale-80 border-b-3 border-yellow-300' : ''}`}
            onClick={(e) => handleChangeGameType(e, 'topBlitzPlayers')}>
            {gameTypeIcon.topBlitzPlayers}</button>
          <button className={`hover:scale-110 ${gameType === 'topBulletPlayers' ? 'scale-80 border-b-3 border-red-500' : ''}`}
            onClick={(e) => handleChangeGameType(e, 'topBulletPlayers')}>
            {gameTypeIcon.topBulletPlayers}</button>


        </div>
      </div>

      <div className="w-full overflow-x-auto">
        <table className="min-w-full text-left divide-y divide-gray-700">
          <thead>
            <tr>
              <th className="px-4 py-2 w-12">#</th>
              <th className="px-4 py-2">{t('player')}</th>
              <th className="px-4 py-2 w-28">{t('rating')}</th>
            </tr>
          </thead>

          <tbody className="divide-y divide-gray-800">
            {leaderboard[gameType].map((user, idx) => {
              if (typeof user.rating !== "number") {

                throw new Error("Invalid rating type in leaderboard user");
              }
              const rank = idx + 1;
              const key = idx
              const trophy =
                rank === 1 ? (
                  <TfiCup className="inline text-yellow-400" />
                ) : rank === 2 ? (
                  <TfiCup className="inline text-gray-400" />
                ) : rank === 3 ? (
                  <TfiCup className="inline text-yellow-700" />
                ) : null;

              return (
                <tr
                  key={key}
                  className={idx == 0 ? "bg-yellow-600/25 " : idx == 1 ? "bg-zinc-700/40" :
                    idx == 2 ? "bg-yellow-800/30" : "even:bg-white/2"}
                >
                  <td className="px-4 py-3 align-middle">
                    <div className="flex items-center gap-2">
                      <span className={`w-6 ${trophy ? "text-lg" : "text-sm"} `}>{trophy || rank}</span>
                    </div>
                  </td>

                  <td className="px-4 py-3 align-middle">
                    <div >
                      <Link to={`/users/${user.nickname}/profile`} className="flex items-center gap-2 hover:text-amber-200
                       transition-color duration-200">
                        <Avatar avatarUrl={user.profilePictureUrl} className="w-8 h-8 rounded-full mb-1 outline-2" />
                        <span className="font-MyFancyFont">{user.nickname}</span>
                      </Link>
                    </div>

                  </td>

                  <td className="px-4 py-3 align-middle font-MyFancyFont">
                    <span>{gameTypeIcon[gameType]} {user.rating} </span>
                  </td>
                </tr>
              );
            })}
          </tbody>
        </table>
      </div>
    </div>
  );
}