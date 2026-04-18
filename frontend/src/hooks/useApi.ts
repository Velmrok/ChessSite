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
            useToast?: boolean;                   
        }
    ): Promise<T | undefined> => {
        setLoading(true);
        try {
            return await fn();
        } catch (err: any) {
            const message =
                t.toast.error[err.message as keyof typeof t.toast.error] ??
                t.toast.error.generic;

            if (options?.useToast) setToast({ msg: message, type: "error" });
            if (options?.onError) options.onError(message, err.status);

            return undefined;
        } finally {
            setLoading(false);
        }
    };

    return { request, loading };
}