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
    getParamsLink: ():string => {
     
        return "?Search=" + encodeURIComponent(useUsersSearchStore.getState().params.search) +
            "&Limit=" + useUsersSearchStore.getState().params.limit +
            "&MinRating=" + useUsersSearchStore.getState().params.minRating +
            "&MaxRating=" + useUsersSearchStore.getState().params.maxRating +
            "&Online=" + useUsersSearchStore.getState().params.online +
            "&SortBy=" + useUsersSearchStore.getState().params.sortBy +
            "&SortDescending=" + useUsersSearchStore.getState().params.sortDescending +
            "&Page=" + useUsersSearchStore.getState().params.page +
            "&RatingType=" + useUsersSearchStore.getState().params.ratingType;
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