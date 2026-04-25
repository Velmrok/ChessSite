import { useEffect, useState } from "react";
import useGameSearchStore from "@/stores/useGameSearchStore";
import FilterMenu from "@/components/SearchGame/FilterMenu";
import { getAllGames } from "@/services/gameService";
import GameList from "@/components/SearchGame/GameList";
import { useTranslation } from "react-i18next";
import type { GameSummary } from "@/types/game";
import { useApi } from "@/hooks/useApi";


export default function GamesSearchPage() {
    const params = useGameSearchStore((state) => state.params);
    const resetFilters = useGameSearchStore((state) => state.resetFilters);
    const getParamsLink = useGameSearchStore((state) => state.getParamsLink);
    const [games, setGames] = useState<Array<GameSummary>>([]);
    const [isLoading, setIsLoading] = useState(false);
    const { t } = useTranslation('search');
    const [totalPages, setTotalPages] = useState(1);
    const {request } = useApi();
    useEffect(() => {
        resetFilters();
    }, []);


    useEffect(() => {
        const fetch = async () => {
            
            const timeoutId = setTimeout(() => setIsLoading(true), 300);

            const response = await request(() => getAllGames(getParamsLink()));
            if (response) {
                setGames(response.games);
                setTotalPages(response.totalPages);
                setIsLoading(false);
                clearTimeout(timeoutId);
            }
        };
        fetch();
    }, [params]);
   
    return (
        <div className="flex flex-col items-center w-full min-h-screen bg-cyan-900 text-white p-4 overflow-x-auto gap-6">
            <h1 className="text-3xl font-bold mb-6">{t('findGame')}</h1>

            <FilterMenu />
            
            <GameList isLoading={isLoading} games={games} totalPages={totalPages} />
            
        </div>
    );
}