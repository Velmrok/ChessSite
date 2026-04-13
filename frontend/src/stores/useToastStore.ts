import {create} from "zustand";
type ToastData = {
    msg: string;
    type: Toast;
} | null;

type ToastType = {
    toastData: ToastData | null;
    setToast: (toast: ToastData) => void;

}

const useToastStore = create<ToastType>(set => ({
    toastData: null,
    setToast: (toast: ToastData) => set({toastData: toast}),
}));
export default useToastStore;