import { create } from 'zustand';

export type SearchParams = {
    query: string;
    limit: number;
    minRating: number ;
    maxRating: number;
    onlyActive: boolean;
    sortBy: 'rating' | 'nickname' | 'onlineStatus' | 'lastActive';
    sortOrder: boolean ; // 1 asc, 0 desc
    ratingType: 'ratingRapid' | 'ratingBlitz' | 'ratingBullet';
    page: number;
};

type SearchStore = {
    params: SearchParams;
    setQuery: (query: string) => void;
    setFilters: (filters: Partial<SearchParams>) => void;
    resetFilters: () => void;
    getParamsLink: () => string;
   
    setOrder: (sortBy: 'rating' | 'nickname' | 'onlineStatus' | 'lastActive') => void;
    
};

const useSearchStore = create<SearchStore>((set) => ({
    params: {
        query: '',
        limit: 10,  
        minRating: 0,
        maxRating: 3000,
        ratingType: 'ratingRapid',
        onlyActive: false,
        sortBy: 'rating',
        sortOrder: false,
        page: 1,
    },
    setQuery: (query) => set((state) => {
    if (state.params.query === query) return state; 
    return { params: { ...state.params, query, page: 1 } };
}),
    setFilters: (filters) => set((state) => ({ params: { ...state.params, ...filters} })),
    resetFilters: () => set({
        params: {
            query: '',
            limit: 10,
            minRating: 0,
            maxRating: 3000,
            ratingType: 'ratingRapid',
            onlyActive: false,
            sortBy: 'rating',
            sortOrder: false,
            page: 1,
        }
    }),
    getParamsLink: ():string => {
        const sortOrder = useSearchStore.getState().params.sortOrder ? 'asc' : 'desc';
        return "?query=" + encodeURIComponent(useSearchStore.getState().params.query) +
            "&limit=" + useSearchStore.getState().params.limit +
            "&minRating=" + useSearchStore.getState().params.minRating +
            "&maxRating=" + useSearchStore.getState().params.maxRating +
            "&onlyActive=" + useSearchStore.getState().params.onlyActive +
            "&sortBy=" + useSearchStore.getState().params.sortBy +
            "&sortOrder=" + sortOrder +
            "&page=" + useSearchStore.getState().params.page +
            "&ratingType=" + useSearchStore.getState().params.ratingType;
    },
    setOrder: (sortBy) => set((state) => {
        if(state.params.sortBy === sortBy){
        
            return { params: { ...state.params, sortOrder: !state.params.sortOrder, page:1 } };
        }
        return { params: { ...state.params, sortBy,sortOrder: false, page: 1 } };
    })
    ,
}));

export default useSearchStore;