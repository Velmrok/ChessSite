import {  useEffect, useRef, useState } from "react";
import { useDebounce } from "@/hooks/useDebounce";
import useSearchStore from "@/stores/useSearchStore";
import { FaSearch } from "react-icons/fa";
import useLanguageStore from "@/stores/useLanguageStore";

export default function UserSearchBar() {
   const [inputValue, setInputValue] = useState("");
   const setQuery = useSearchStore((state) => state.setQuery);
   const debouncedValue = useDebounce(inputValue, 500);
    const t = useLanguageStore((state) => state.t);
   useEffect(() => {
   
       setQuery(debouncedValue);
   }, [debouncedValue, setQuery]);
    return (
        <div className=" flex items-center gap-2 bg-gray-900/[50%]
         text-white p-3 rounded-2xl shadow-xl relative">
            <FaSearch />
            <input
                type="text"
                className="w-full p-2 rounded-md  text-white font-MyFancyFont border-none 
                  outline-none"
                placeholder={t.search.placeholder}
                onChange={(e:any) => setInputValue(e.target.value)}
            ></input>
        </div>
    );
}