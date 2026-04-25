import type { GameSearchParams } from '@/types/game';
import { create } from 'zustand';


type GameSearchStore = {
    params: GameSearchParams;
    setQuery: (query: string) => void;
    setFilters: (filters: Partial<GameSearchParams>) => void;
    resetFilters: () => void;
    getParamsLink: () => string;
};

const useGameSearchStore = create<GameSearchStore>((set, get) => ({
    params: {
        query: '',
        limit: 10,
        gameType: null,
        status: null,
        page: 1,
        sortBy: 'date',
        SortDescending: false,
    },
    setQuery: (query) => set((state) => ({ params: { ...state.params, query, page: 1 } })),
    setFilters: (filters) => set((state) => ({ params: { ...state.params, ...filters} })),
    resetFilters: () => set({
        params: {
            query: '',
            limit: 10,
            gameType: null,
            status: null,
            page: 1,
            sortBy: 'date',
            SortDescending: false,
        }
    }),
    getParamsLink: () => {

        const params = useGameSearchStore.getState().params;

        const query = new URLSearchParams();

        const append = (key: string, value: unknown, defaultValue?: unknown) => {
            if (value === undefined || value === null) return;
            if (value === "") return;
            if (defaultValue !== undefined && value === defaultValue) return;
            query.append(key, String(value));
        };

        append("query", params.query);
        append("limit", params.limit, 10);
        append("gameType", params.gameType, null);
        append("status", params.status, null);
        append("page", params.page, 1);
        append("sortBy", params.sortBy, 'date');
        append("SortDescending", params.SortDescending, false);

        const queryString = query.toString();
        return queryString ? `?${queryString}` : '';
       
    },
}));

export default useGameSearchStore;