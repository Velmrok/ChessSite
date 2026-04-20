import { useDebounce } from "@/hooks/useDebounce";
import useGameSearchStore from "@/stores/useGameSearchStore";
import { useFormik } from "formik";
import { useEffect } from "react";
import { useTranslation } from "react-i18next";
import { IoIosInfinite } from "react-icons/io";
import { MdAccessTime } from "react-icons/md";
import { SiPushbullet, SiStackblitz } from "react-icons/si";


export default function FilterMenu() {

    const params = useGameSearchStore((state) => state.params);
    const setFilters = useGameSearchStore((state) => state.setFilters);
    const {t} = useTranslation("search");
    const formik = useFormik({
        initialValues: {
            query: '',
            gameType: 'all',
            status: 'all',
        },
        onSubmit: () => {}, 
    });

    const debouncedQuery = useDebounce(formik.values.query, 300);
    useEffect(() => {
        setFilters({
            ...params,
            query: debouncedQuery,
            gameType: formik.values.gameType as 'rapid' | 'blitz' | 'bullet' | 'all',
            status: formik.values.status as 'active' | 'finished' | 'all',
        });
    }, [
        debouncedQuery,
        formik.values.gameType,
        formik.values.status,
    ]);
    




    return(
        
            <div className="bg-cyan-800 p-4 rounded-lg shadow-md w-full max-w-4xl mb-6 flex flex-wrap gap-4 items-center">
                <input
                    type="text"
                    name="query"
                    placeholder={t('placeholder')}
                    className="p-2 rounded bg-cyan-700 text-white placeholder-cyan-300 border border-cyan-600 focus:outline-none focus:border-cyan-400 flex-grow"
                    value={formik.values.query}
                    onChange={formik.handleChange}
                />
                
            <div className="flex gap-2 md:gap-3">
                <button
                    type="button"
                    className={`hover:scale-110 flex items-center ${formik.values.gameType === 'rapid' ? ' border-b-3 border-green-500' : ''}`}
                    onClick={() => formik.setFieldValue('gameType', 'rapid')}
                >
                    <MdAccessTime className="text-green-500 text-base md:text-xl inline" />
                </button>
                <button
                    type="button"
                    className={`hover:scale-110 flex items-center ${formik.values.gameType === 'blitz' ? ' border-b-3 border-yellow-300' : ''}`}
                    onClick={() => formik.setFieldValue('gameType', 'blitz')}
                >
                    <SiStackblitz className="text-yellow-300 text-base md:text-xl inline" />
                </button>
                <button
                    type="button"
                    className={`hover:scale-110 flex items-center ${formik.values.gameType === 'bullet' ? ' border-b-3 border-red-500' : ''}`}
                    onClick={() => formik.setFieldValue('gameType', 'bullet')}
                >
                    <SiPushbullet className="text-red-500 text-base md:text-xl" />
                </button>
                <button
                    type="button"
                    className={`hover:scale-110 flex items-center ${formik.values.gameType === 'all' ? ' border-b-3 border-white' : ''}`}
                    onClick={() => formik.setFieldValue('gameType', 'all')}
                >
                    <IoIosInfinite className="text-white text-base md:text-xl inline" />
                </button>
               
            </div>

            <select
                className="p-2 rounded bg-cyan-700 border border-cyan-600"
                    name="status"
                    value={formik.values.status}
                    onChange={formik.handleChange}
                >
                    <option value="all">{t('all')}</option>
                    <option value="active">{t('active')}</option>
                    <option value="finished">{t('finished')}</option>
                </select>
            </div>
    )
}