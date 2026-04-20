import { useEffect, useState } from "react";

type Props = {
    onLeft: () => void;
    onRight: () => void;
    onfKey: () => void;
}

export default function useArrowKeys({ onLeft, onRight, onfKey }: Props) {
    const [onleftTimeoutId, setOnLeftTimeoutId] = useState<number | null>(null);
    const [onRightTimeoutId, setOnRightTimeoutId] = useState<number | null>(null);
    useEffect(() => {
        const handleKeyDown = (event: KeyboardEvent) => {
            if (
                event.target instanceof HTMLInputElement ||
                event.target instanceof HTMLTextAreaElement

            ) {
                return;
            }
            if (event.key === "ArrowLeft") {
                if (onleftTimeoutId) return;
                setOnLeftTimeoutId(setTimeout(() => {
                    onLeft();
                    setOnLeftTimeoutId(null);
                }, 5)
                )
            } else if (event.key === "ArrowRight") {
                if (onRightTimeoutId) return;
                setOnRightTimeoutId(setTimeout(() => {
                    onRight();
                    setOnRightTimeoutId(null);
                }, 5)
                )
            }
            else if (event.key === "f") {
                onfKey();
            }
        };
        window.addEventListener("keydown", handleKeyDown);
        return () => {
            window.removeEventListener("keydown", handleKeyDown);
        };
    }, [onLeft, onRight, onfKey]);
}