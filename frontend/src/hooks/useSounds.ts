import { useRef } from "react";

export function useSounds() {
  const moveSound = useRef<HTMLAudioElement | null>(null);
  const captureSound = useRef<HTMLAudioElement | null>(null);
    const notifySound = useRef<HTMLAudioElement | null>(null);
    const castleSound = useRef<HTMLAudioElement | null>(null);
    const checkSound = useRef<HTMLAudioElement | null>(null);
  if (!moveSound.current) {
    moveSound.current = new Audio("/sounds/move-self.mp3");
    captureSound.current = new Audio("/sounds/capture.mp3");
    notifySound.current = new Audio("/sounds/notify.mp3");
    castleSound.current = new Audio("/sounds/castle.mp3");
    checkSound.current = new Audio("/sounds/move-check.mp3");
    
  }

  const playSound = (sound: typeof moveSound) => {
    if(!sound.current)return
    
    sound.current.currentTime = 0;
    sound.current.play();
  };
  const playMove = () => playSound(moveSound);
  const playCapture = () => playSound(captureSound);
  const playNotify = () => playSound(notifySound);
  const playCastle = () => playSound(castleSound);
  const playCheck = () => playSound(checkSound);
  return { playMove, playCapture, playNotify, playCastle, playCheck };
}
