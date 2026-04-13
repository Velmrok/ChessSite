import useLanguageStore from "@/stores/useLanguageStore";


export default function Loading() {
    const t = useLanguageStore((state) => state.t);
    return (
        <div className="w-full h-70 flex flex-col justify-center items-center gap-4 ">
            
            <div className="w-12 h-12 border-4 border-cyan-600 border-t-transparent
             rounded-full animate-spin"></div>
            
            <div className="text-white text-2xl font-MyFancyFont animate-pulse">
                {t.loading.message}
            </div>
        </div>
    );
}