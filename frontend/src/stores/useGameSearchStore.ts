import { create } from 'zustand';

export type GameSearchParams = {
    query: string; 
    limit: number;
    gameType: 'rapid' | 'blitz' | 'bullet' | 'all';
    status: 'active' | 'finished' | 'all';
    page: number;
    sortBy : 'date' | 'time' | 'nicknames' ;
    sortOrder: boolean; // 1 for ASC,0 for DESC
};

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
        gameType: 'all',
        status: 'all',
        page: 1,
        sortBy: 'date',
        sortOrder: false,
    },
    setQuery: (query) => set((state) => ({ params: { ...state.params, query, page: 1 } })),
    setFilters: (filters) => set((state) => ({ params: { ...state.params, ...filters} })),
    resetFilters: () => set({
        params: {
            query: '',
            limit: 10,
            gameType: 'all',
            status: 'all',
            page: 1,
            sortBy: 'date',
            sortOrder: false,
        }
    }),
    getParamsLink: () => {
        const p = get().params;
        return `?query=${encodeURIComponent(p.query)}
        &limit=${p.limit}
        &gameType=${p.gameType}
        &status=${p.status}
        &page=${p.page}
        &sortBy=${p.sortBy}
        &sortOrder=${p.sortOrder?'ASC':'DESC'}
        `;
    },
}));

export default useGameSearchStore;