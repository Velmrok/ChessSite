import useLanguageStore from "@/stores/useLanguageStore";
import useToastStore from "@/stores/useToastStore";
import { useState } from "react";

export function useApi() {
    const [loading, setLoading] = useState(false);
    const t = useLanguageStore((state) => state.t);
    const setToast = useToastStore((state) => state.setToast);

    const request = async <T>(
        fn: () => Promise<T>,
        options?: {
            onError?: (message: string, status?: number) => void;                 
        }
    ): Promise<T | undefined> => {
        setLoading(true);
        try {
            return await fn();
        } catch (err: any) {
           
            if (options?.onError) options.onError(err.message, err.status);

            return undefined;
        } finally {
            setLoading(false);
        }
    };

    return { request, loading };
}