import useToastStore from "../../stores/useToastStore";
import Toast from "./Toast";
export default function ToastContainer() {
    const setToast = useToastStore((state) => state.setToast);
    const toast = useToastStore((state) => state.toastData);
    return (
           <>
               {toast && (
                   <Toast 
                       message={toast.msg} 
                       type={toast.type} 
                       onClose={() => setToast(null)} 
                   />
               )}
         </>
       );
}