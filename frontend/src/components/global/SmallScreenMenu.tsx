import useLanguageStore from "@/stores/useLanguageStore";
import { CiMenuBurger } from "react-icons/ci";
import { useNavigate } from "react-router-dom";

type Props={
    isOpen: boolean;
    setIsOpen: (isOpen:boolean)=>void;
}
export default function SmallScreenMenu({isOpen,setIsOpen}:Props){
    const t = useLanguageStore((state) => state.t);

     const navigate = useNavigate();
     const handleClick = (e:any, path:string) => {
        e.preventDefault();
        navigate(path);
        setIsOpen(false);
     }
    return(
        <div className={`fixed top-0 left-0 w-[75%] md:w-[35%] h-screen bg-gray-900/80 backdrop-blur-xs shadow-inner
         h-16 justify-around items-center z-50 ${isOpen ? "flex translate-x-0 xl:-translate-x-full" : " -translate-x-full "}
         transform transition-transform duration-300 ease-out`} >
            <button onClick={()=>setIsOpen(false)}
                        className="absolute left-2 top-5  xl:hidden scale-125 ml-3"><CiMenuBurger /></button>
            <div className="grid grid-cols-1 w-[80%] place-items gap-2 place-items-center grid-rows-10 h-screen mt-35">

            <button onClick={(e)=>handleClick(e, '/')} >ChessSite</button>
            <button onClick={(e)=>handleClick(e, '/search')}
                         className="bg-cyan-500/60  w-full px-1 py-4 rounded hover:bg-cyan-500 text-xs
                         hover:scale-105 transition-all">{t.smallMenu.findSomeone}</button>
            <button onClick={(e)=>handleClick(e, '/games')}
                         className="bg-cyan-500/60  w-full px-1 py-4 rounded hover:bg-cyan-500 text-xs
                         hover:scale-105 transition-all">{t.smallMenu.findGame}</button>
                <button onClick={(e)=>handleClick(e, '/find-game')}
                         className="bg-cyan-500/60  w-full px-1 py-4 rounded hover:bg-cyan-500 text-xs
                         hover:scale-105 transition-all">{t.smallMenu.play}</button>
            </div>
             </div>
    )
}