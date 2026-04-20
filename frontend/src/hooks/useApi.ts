import { useState } from "react";

export function useApi() {
   

  

    const request = async <T>(
        fn: () => Promise<T>,
        options?: {
            onError?: (message: string, status?: number) => void;                 
        }
    ): Promise<T | undefined> => {

        try {
            return await fn();
        } catch (err: any) {
           
            if (options?.onError) options.onError(err.message, err.status);
            else console.error(err);
            return undefined;
        } 
    };

    return { request};
}