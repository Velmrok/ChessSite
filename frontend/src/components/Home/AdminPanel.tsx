import { publishSystemAlert,publishLogoutEveryone } from "@/services/socket/mqttService";
export default function AdminPanel() {

    return (
        <div className="fixed flex gap-5 bottom-0 right-0 bg-red-600/70 text-white px-4 py-2 z-50 w-full cursor-pointer">
            <div>ADMIN PANEL</div>
            <button onClick={()=>publishSystemAlert("This is a test MQTT alert")}
            className=" bg-blue-500 px-4 py-2 rounded hover:scale-110 transition-transform">TEST MQTT ALERT</button>
            <button onClick={()=>publishLogoutEveryone()}
            className=" bg-green-500 px-4 py-2 rounded hover:scale-110 transition-transform">LOGOUT EVERYONE</button>
        </div>
    )
}