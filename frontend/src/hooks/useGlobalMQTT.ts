import useLanguageStore from "@/stores/useLanguageStore";
import useToastStore from "@/stores/useToastStore";
import useUserStore from "@/stores/useUserStore";
import { useEffect } from "react";

export default function useGlobalMQTT() {
    const setUser = useUserStore((state) => state.setUser);
    const friends = useUserStore((state) => state.user?.friendList);
    const setToast = useToastStore((state) => state.setToast);
    const mqttClient = useUserStore((state) => state.mqttClient);
    const t = useLanguageStore((state) => state.t);

    const handleMessage = (topicReceived: string, messageBuffer: Buffer) => {
            if (topicReceived === "SYSTEM_ALERT") {
                const data = JSON.parse(messageBuffer.toString());
                setToast({
                    msg: `ADMIN: ${data.content}`,
                    type: "info"
                });
            }
            for (const friend of friends || []) {
                if (topicReceived === `USER_STATUS_${friend}`) {
                    const data = JSON.parse(messageBuffer.toString());
                    setToast({
                        msg: data.content=="online"?
                        t.toast.info.isOnline.replace("{nickname}", friend):
                        t.toast.info.isOffline.replace("{nickname}", friend),
                        type: "info"
                    });
                }
              
            }
            if(topicReceived === "SYSTEM_LOGOUT_ALL"){
                setToast({
                    msg: t.toast.info.logoutBySystem,
                    type: "info"
                });
                setUser(null);
                }
        };
        
    useEffect(() => {

        if (!mqttClient) return;
        mqttClient.subscribe("SYSTEM_ALERT", { qos: 1 });
         mqttClient.subscribe("SYSTEM_LOGOUT_ALL", { qos: 1 });
        
        for (const friend of friends || []) {

            mqttClient.subscribe(`USER_STATUS_${friend}`, { qos: 1 });
        }
        mqttClient.on('message', handleMessage);
        return () => {
            mqttClient.off('message', handleMessage);
        };
    }, [mqttClient]);
}

