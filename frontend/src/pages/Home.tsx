
import HomeUser from "@/components/Home/HomeUser";
import useUserStore from "@/stores/useUserStore";


export default function Home(){
    const user = useUserStore((state) => state.user);
    
        
    
    return(
        <div className="bg-cyan-800 w-full min-h-screen flex justify-center items-center"> 
         
          {user && <HomeUser />}
        </div>
    )
}