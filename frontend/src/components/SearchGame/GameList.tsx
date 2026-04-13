import useLanguageStore from "@/stores/useLanguageStore";
import Loading from "../global/Loading";
import { MdAccessTime } from "react-icons/md";
import { SiPushbullet, SiStackblitz } from "react-icons/si";
import { Format } from "@/utils/format";
import type { JSX } from "react";
import { Link } from "react-router-dom";
import PaginationButtons from "../global/Pagination_buttons";
import useGameSearchStore from "@/stores/useGameSearchStore";
import SortArrows from "./SortArrows";

const API_URL = import.meta.env.VITE_API_URL;

type Props = {
    isLoading: boolean;
    games: Array<GameSummary>;
    totalPages: number;
}

export default function GameList({ isLoading, games, totalPages }: Props) {
    const t = useLanguageStore((state) => state.t);
    const page = useGameSearchStore((state) => state.params.page);
    const setFilters = useGameSearchStore((state) => state.setFilters);
    const params = useGameSearchStore((state) => state.params);
    
    const iconMap: { [key: string]: JSX.Element } = {
        rapid: <MdAccessTime className="text-green-500 text-base md:text-xl inline" />,
        blitz: <SiStackblitz className="text-yellow-300 text-base md:text-xl inline" />,
        bullet: <SiPushbullet className="text-red-500 text-base md:text-xl" />,
    };

    const handleChangePage = (newPage: number) => {
        if (newPage < 1 || newPage > totalPages) return;
        setFilters({ page: newPage });
    }


    const handleSort = (field: string) => {
        const isSameField = params.sortBy === field;
        const newOrder = isSameField ? !params.sortOrder : true; 
        setFilters({ sortBy: field as "date"|"time"|"nicknames", sortOrder: newOrder });
    };

  
    const renderSortArrow = (field: string) => {
        if (params.sortBy !== field) return <SortArrows sortOrder={null} />; 
        return <SortArrows sortOrder={params.sortOrder ? 'asc' : 'desc'} />;
    };

    return (
        <div className="w-full max-w-5xl">
            {isLoading ? (
                <Loading />
            ) : (
                <div className="flex flex-col gap-4">
                    <div className="w-full grid grid-cols-3 place-items-center ">
                        <PaginationButtons handleChangePage={handleChangePage} currentPage={page} totalPages={totalPages} />
                    </div>
                    <div className="w-full overflow-x-auto pb-2">
                        <div className="min-w-[800px] grid gap-4">

    
                            <div className="bg-cyan-800 p-2 h-10 rounded grid grid-cols-10 place-items-center text-xs md:text-lg font-MyFancyFont text-white">
                                

                                <div className="col-span-2 flex items-center justify-center gap-2 cursor-pointer hover:text-cyan-300 transition-colors" onClick={() => handleSort("nicknames")}>
                                    {t.search.players}
                                    {renderSortArrow("nicknames")}
                                </div>

                                  <div className="col-span-3 text-center">{t.search.moves}</div>


                                <div className="col-span-1 flex items-center justify-center gap-2 cursor-pointer hover:text-cyan-300 transition-colors" onClick={() => handleSort("time")}>
                                    {t.search.time}
                                    {renderSortArrow("time")}
                                </div>
                                <div className="col-span-2 flex items-center justify-center gap-2 cursor-pointer hover:text-cyan-300 transition-colors" onClick={() => handleSort("date")}>
                                    {t.search.date}
                                    {renderSortArrow("date")}
                                </div>


                                <div className="col-span-1 text-center">{t.search.type}</div>

                                <div className="col-span-1 text-center">{t.search.status}</div>
                            </div>

                            {games.length > 0 ? games.map((game) => (
                                <Link to={`/game/${game.id}`} key={game.id} className="block">
                                    <div className="bg-cyan-800 p-2 h-20 rounded grid grid-cols-10 place-items-center hover:bg-cyan-700 transition cursor-pointer text-white">


                                        <div className="flex items-center justify-center gap-2 md:gap-4 col-span-2 w-full">
                                            <div className="text-xs bg-black/50 rounded-md p-1 w-14 h-14 md:w-16 md:h-16 font-MyFancyFont text-white flex flex-col items-center justify-center shrink-0">
                                                <img src={`${API_URL}${game.whitePlayer.avatar}`} alt="White" className="w-8 h-8 md:w-10 md:h-10 rounded-full border-2 border-white object-cover" />
                                                <span className="truncate max-w-[50px] md:max-w-[60px]">{game.whitePlayer.nickname}</span>
                                            </div>
                                            <span className="font-bold text-sm">vs</span>
                                            <div className="text-xs bg-black/50 rounded-md p-1 w-14 h-14 md:w-16 md:h-16 font-MyFancyFont text-white flex flex-col items-center justify-center shrink-0">
                                                <img src={`${API_URL}${game.blackPlayer.avatar}`} alt="Black" className="w-8 h-8 md:w-10 md:h-10 rounded-full border-2 border-white object-cover" />
                                                <span className="truncate max-w-[50px] md:max-w-[60px]">{game.blackPlayer.nickname}</span>
                                            </div>
                                        </div>

                                        <div className="col-span-3 flex gap-2 text-xs md:text-sm overflow-hidden w-full justify-center">
                                            {game.moves && game.moves.length > 0 ? (
                                                <div className="flex gap-2">
                                                    {Format.groupMoves(game.moves).slice(0, 5).map((moveSet, index) => (
                                                        <span key={index} className="whitespace-nowrap">
                                                            {index + 1}. {moveSet.whiteMove} {moveSet.blackMove ? moveSet.blackMove : ''}
                                                        </span>
                                                    ))}
                                                </div>
                                            ) : (
                                                <div>{t.search.noMoves}</div>
                                            )}
                                        </div>

                                        <div className="col-span-1 text-xs md:text-sm whitespace-nowrap">
                                            {game.time} + {game.increment}s
                                        </div>

                                        <div className="col-span-2 text-xs md:text-sm whitespace-nowrap">{game.date}</div>
                                        

                                        <div className="col-span-1 text-sm text-cyan-300 flex items-center gap-1">
                                            {iconMap[game.gameType]}
                                        </div>

                                        <div className="text-xs md:text-sm">
                                            {game.status === 'finished' ? t.search.finished : t.search.live}
                                        </div>
                                    </div>
                                </Link>
                            )) : (
                                <div className="text-center text-cyan-300 mt-10">
                                    {t.search.noGamesFound}
                                </div>
                            )}
                        </div>
                    </div>
                </div>
            )}
        </div>
    );
}