import { useEffect, useState } from "react"

import { FaChessPawn } from "react-icons/fa";
import { Link, useParams } from "react-router-dom";
import Loading from "../global/Loading";
import { useUserProfileStore } from "@/stores/useUserProfileStore";
import PaginationButtons from "../global/Pagination_buttons";
import { useTranslation } from "react-i18next";
import { fetchUserGameHistory } from "@/services/userService";
import { useApi } from "@/hooks/useApi";




export default function UserGameHistory() {
  const nickname = useParams<{ nickname: string }>().nickname;
  const { t } = useTranslation("profile");
  const games = useUserProfileStore(state => state.games);
  const [loading, setLoading] = useState(false);
  //const fetchGames = useUserProfileStore(state => state.fetchGames);
  const gamesPage = useUserProfileStore(state => state.gamesPage);
  const totalGamesPages = useUserProfileStore(state => state.totalGamesPages);
  const setGames = useUserProfileStore(state => state.setGames);
  const setGamesPage = useUserProfileStore(state => state.setGamesPage);
  const { request } = useApi();

  useEffect(() => {
    setGamesPage(1);
  }, [nickname]);

  useEffect(() => {
    setLoading(true);
    const fetch = async () => {
      var response = await request(() => fetchUserGameHistory(nickname!, gamesPage));
      if (response) {
        setGames(response.gameHistory);
      }
      setLoading(false);

    }
    fetch();

  }, [gamesPage, nickname]);

  const statusColors = {
    win: "bg-green-700/[80%] ",
    loss: "bg-red-700/[80%] ",
    draw: "bg-gray-700 "
  };

  const handleChangePage = (newPage: number) => {
    if (newPage < 1 || newPage > totalGamesPages) return;
    setGamesPage(newPage);
  }


  return (

    <div className="w-full overflow-x-auto">
      <div className="w-full text-white grid grid-cols-3 place-items-center mb-2 px-4">
        <PaginationButtons
          handleChangePage={handleChangePage}
          currentPage={gamesPage}
          totalPages={totalGamesPages}
        />
      </div>

      {loading ? (
        <Loading />
      ) : (
        <table className="min-w-[600px] w-full text-white font-MyFancyFont bg-gray-800/30 rounded-lg">
          <thead className="bg-gray-900/50">
            <tr>
              <th className="py-2 px-4">{t('players')}</th>
              <th className="py-2 px-4">{t('result')}</th>
              <th className="py-2 px-4">{t('analysis')}</th>
              <th className="py-2 px-4">{t('date')}</th>
            </tr>
          </thead>
          <tbody>
            {games.length === 0 ? (
              <tr className="h-[240px] ">
                <td colSpan={4} className="text-white font-MyFancyFont text-lg">
                  <div className="flex justify-center items-center w-full mt-4">
                    {t('noGames')}
                  </div>
                </td>
              </tr>
            ) :
              (
                games.map((game) => {
                  const status = game.scoreStatus;
                  return (
                    <tr
                      key={game.id}
                      className={`text-white h-[64px] ${statusColors[status]
                        } `}
                    >
                      <td className="py-2 px-4">
                        <div className="flex flex-col space-y-1 flex justify-between">
                          <div className="flex flex-col space-x-2 items-center">

                            <div>
                              <div className="flex items-center">
                                <FaChessPawn className="text-white " />
                                <Link
                                  to={`/users/${game.whiteNickname}/profile`}
                                  className="hover:text-amber-200 transition"
                                >
                                  {game.whiteNickname} ({game.whiteRating})
                                </Link>
                              </div>


                              <div className="flex items-center">
                                <FaChessPawn className="text-black" />
                                <Link
                                  to={`/users/${game.blackNickname}/profile`}
                                  className="hover:text-amber-200 transition"
                                >
                                  {game.blackNickname} ({game.blackRating})
                                </Link>
                              </div>
                            </div>

                          </div>
                        </div>
                      </td>
                      <td className="h-[64px] font-bold text-sm md:text-lg flex justify-center items-center">
                        {status === 'draw' ? '½ - ½' : status === 'win' ? '1 - 0' : '0 - 1'}
                      </td>
                      <td className="h-[64px] ">
                        <Link
                          to={`/game/${game.id}`}
                          className="text-yellow-300 hover:underline text-sm md:text-lg flex justify-center items-center"
                        >
                          {t('review')}
                        </Link>
                      </td>
                      <td className="h-[64px] ">
                        <div className="flex justify-center items-center">
                          {new Date(game.date).toLocaleDateString()}
                        </div>
                      </td>
                    </tr>
                  );
                })
              )}
          </tbody>
        </table>
      )}
    </div>

  )
}