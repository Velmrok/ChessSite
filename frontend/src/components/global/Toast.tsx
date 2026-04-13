import { useSounds } from "@/hooks/useSounds";
import { useEffect, useState } from "react";
import { createPortal } from "react-dom";

type ToastProps = {
    message: string;
    type?: Toast;
    onClose: () => void;
};

export default function Toast({ message, type = "info", onClose }: ToastProps) {
    const [isVisible, setIsVisible] = useState(true);
    const {playNotify} = useSounds();
    useEffect(() => {
        setIsVisible(true);
        if(type==='info')playNotify();
        
        const fadeOutTimer = setTimeout(() => setIsVisible(false), 2500);
        const timer = setTimeout(onClose, 3000);
        return () => {
            clearTimeout(fadeOutTimer);
            clearTimeout(timer);
        };
    }, [onClose]);

    const colors = {
        success: "bg-green-600",
        error: "bg-red-600",
        info: "bg-blue-600"
    };

    const content = (
        <div className={`fixed bottom-5 right-5 z-50 px-6 py-3 rounded shadow-lg
         text-white font-MyFancyFont ${colors[type]}
          ${isVisible ? 'opacity-100' : 'opacity-0 ease-out transition-opacity duration-500'}`}>
            <div className="flex items-center gap-3">
                <span>{message}</span>
                <button onClick={onClose} className="font-bold ml-2 hover:text-gray-200">✕</button>
            </div>
        </div>
    );

    
    const portalRoot = document.getElementById("toast-root") || document.body;
    return createPortal(content, portalRoot);
}