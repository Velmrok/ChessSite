
import HomeUser from "@/components/Home/HomeUser";
import { useHomeSignalR } from "@/hooks/useHomeSignalR";
import useUserStore from "@/stores/useUserStore";
import { useEffect } from "react";


export default function Home() {
    const user = useUserStore((state) => state.user);

    useHomeSignalR();
  


    return (
        <div className="bg-cyan-800 w-full min-h-screen flex justify-center items-center">

            {user && <HomeUser />}
        </div>
    )
}