import useLanguageStore from "@/stores/useLanguageStore";
import { useState } from "react";

export function useApi() {
    const [loading, setLoading] = useState(false);
    const t = useLanguageStore((state) => state.t);
    const request = async <T>(
        fn: () => Promise<T>,
        options?: {
            onError?: (message: string) => void;
            showToast?: (message: string) => void;
        }
    ): Promise<T | undefined> => {
        setLoading(true);

        try {
            const result = await fn();
            return result;
        } catch (err: any) {
            const message = t.toast.error[err.message as keyof typeof t.toast.error] ?? t.toast.error.generic;

            options?.onError?.(message);

            if (options?.showToast) {
                options.showToast(message);
            }

            throw err;
        } finally {
            setLoading(false);
        }
    };

    return { request, loading };
}