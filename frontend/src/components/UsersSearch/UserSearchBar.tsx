import {  useEffect, useState } from "react";
import { useDebounce } from "@/hooks/useDebounce";
import useSearchStore from "../../stores/useUsersSearchStore";
import { FaSearch } from "react-icons/fa";
import { useTranslation } from "react-i18next";


export default function UserSearchBar() {
   const [inputValue, setInputValue] = useState("");
   const setQuery = useSearchStore((state) => state.setQuery);
   const debouncedValue = useDebounce(inputValue, 500);
    const {t} = useTranslation("search");
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
                placeholder={t('placeholder')}
                onChange={(e:any) => setInputValue(e.target.value)}
            ></input>
        </div>
    );
}