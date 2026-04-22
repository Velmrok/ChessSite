import type { SearchParams, SortByOption } from '@/types/usersSearch';
import { create } from 'zustand';


type SearchStore = {
    params: SearchParams;
    setQuery: (query: string) => void;
    setFilters: (filters: Partial<SearchParams>) => void;
    resetFilters: () => void;
    getParamsLink: () => string;

    setOrder: (sortBy: SortByOption) => void;

};

const useUsersSearchStore = create<SearchStore>((set) => ({
    params: {
        search: '',
        limit: 10,
        minRating: 0,
        maxRating: 3000,
        ratingType: 'Rapid',
        online: false,
        sortBy: 'Rating',
        sortDescending: false,
        page: 1,
    },
    setQuery: (search) => set((state) => {
        if (state.params.search === search) return state;
        return { params: { ...state.params, search: search, page: 1 } };
    }),
    setFilters: (filters) => set((state) => ({ params: { ...state.params, ...filters } })),
    resetFilters: () => set({
        params: {
            search: '',
            limit: 10,
            minRating: 0,
            maxRating: 3000,
            ratingType: 'Rapid',
            online: false,
            sortBy  : 'Rating',
            sortDescending: false,
            page: 1,
        }
    }),
    getParamsLink: (): string => {
        const params = useUsersSearchStore.getState().params;

        const query = new URLSearchParams();

        const append = (key: string, value: unknown, defaultValue?: unknown) => {
            if (value === undefined || value === null) return;
            if (value === "") return;
            if (defaultValue !== undefined && value === defaultValue) return;
            query.append(key, String(value));
        };

        append("Search", params.search);
        append("Limit", params.limit, 10);
        append("MinRating", params.minRating, 0);
        append("MaxRating", params.maxRating, 3000);
        append("JustOnline", params.online, false);
        append("SortBy", params.sortBy, "CreatedAt");
        append("SortDescending", params.sortDescending, true);
        append("Page", params.page, 1);
        append("RatingType", params.ratingType, "Rapid");

        const queryString = query.toString();
        return queryString ? `?${queryString}` : "";
    },
    setOrder: (sortBy) => set((state) => {
        if(state.params.sortBy === sortBy){
        
            return { params: { ...state.params, sortDescending: !state.params.sortDescending, page:1 } };
        }
        return { params: { ...state.params, sortBy: sortBy, sortDescending: false, page: 1 } };
    })
    ,
}));

export default useUsersSearchStore;