
export type SignalRRequest<TPayload = unknown> = {
    type: string;
    correlationId: string;
    payload?: TPayload;
}

export type SignalRResponse<TData = unknown> = {
    type: string;
    correlationId: string;
    data: TData | null;
    error: SignalRError | null;
}
export type SignalRError = {
    title: string;
    message: string;
    details?: unknown;
} 