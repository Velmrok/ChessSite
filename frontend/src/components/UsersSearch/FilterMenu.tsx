import { useDebounce } from "@/hooks/useDebounce";
import useLanguageStore from "@/stores/useLanguageStore";
import useSearchStore from "../../stores/useUsersSearchStore";
import useUserStore from "@/stores/useUserStore";
import { useFormik } from "formik";
import { useEffect } from "react";
import { MdAccessTime } from "react-icons/md";
import { SiPushbullet, SiStackblitz } from "react-icons/si";
import { useNavigate } from "react-router";

export default function FilterMenu() {
  const user = useUserStore((state) => state.user);
    const params = useSearchStore((state) => state.params);
    const setFilters = useSearchStore((state) => state.setFilters);
    const resetFilters = useSearchStore((state) => state.resetFilters);
    const t = useLanguageStore((state) => state.t);
    const navigate = useNavigate();
    const formik = useFormik({
        initialValues: {
            minRating:  0,
            maxRating:  3000,
            ratingType: 'Rapid',
            onlyActive:  false,
            limit: 10,
        },
        onSubmit: () => {}, 
    });

    
    const debouncedMin = useDebounce(formik.values.minRating, 500);
    const debouncedMax = useDebounce(formik.values.maxRating, 500);

   
    useEffect(() => {
        setFilters({
            ...params,
            limit: Number(formik.values.limit),
            online: formik.values.onlyActive,
            ratingType: formik.values.ratingType as 'Rapid' | 'Blitz' | 'Bullet',
            minRating: debouncedMin ?? 0,
            maxRating: debouncedMax ?? 3000,
        });
    }, [
        formik.values.limit, 
        formik.values.onlyActive, 
        formik.values.ratingType, 
        debouncedMin, 
        debouncedMax
    ] );




    const handleResetFilters = (e: any) => {
        e.preventDefault();
        resetFilters();
        formik.resetForm();
    }
    const handleGoToUsersCreate = (e: any) => { 
      e.preventDefault();
      navigate("/users/create"); }

  return (
    <form className="bg-gray-900/80 p-4 rounded-lg shadow-md">
      <div className="grid grid-cols-1 md:grid-cols-2 lg:grid-cols-4 gap-4 items-end">

        {/* LIMIT//////////////////////////////////////////////////// */}
        <div className="flex flex-col gap-1">
          <label htmlFor="limit-select" className="text-xs text-gray-400 uppercase font-bold">
            {t.search.entriesPerPage}
          </label>
          <select
            id="limit-select"
            name="limit"
            className="bg-gray-800 text-white p-2 rounded border border-gray-700"
            value={formik.values.limit}
            onChange={formik.handleChange}
          >
            <option value={5}>5</option>
            <option value={10}>10</option>
            <option value={20}>20</option>
          </select>
        </div>

        {/* RATING MIN/MAX //////////////////////////////////////////////////// */}
        <div className="flex flex-col gap-1">
          <span className="text-xs text-gray-400 uppercase font-bold">{t.search.rankingMinMax}</span>
          <div className="flex gap-2">
            <input
              type="number"
              name="minRating"
              placeholder={t.search.min}
              className="w-full bg-gray-800 text-white p-2 rounded border border-gray-700"
              value={formik.values.minRating}
              onChange={formik.handleChange}
            />
            <input
              type="number"
              name="maxRating"
              placeholder={t.search.max}
              className="w-full bg-gray-800 text-white p-2 rounded border border-gray-700"
              value={formik.values.maxRating}
              onChange={formik.handleChange}
            />
          </div>
        </div>

        {/* ACTIVE / TYPE //////////////////////////////////////////////////// */}
        <div className="flex lg:flex-col justify-center gap-4 h-[60px]">
          <div className="flex items-center gap-2">
            <input
              id="active-check"
              name="onlyActive"
              type="checkbox"
              className="w-5 h-5 accent-cyan-600"
              checked={formik.values.onlyActive}
              onChange={formik.handleChange}
            />
            <label htmlFor="active-check" className="text-sm text-white cursor-pointer select-none">
              {t.search.onlyActive}
            </label>
          </div>
          <div className="flex gap-2 md:gap-3">
            <button
              type="button"
              className={`hover:scale-110 flex items-center ${formik.values.ratingType === 'Rapid' ? ' border-b-3 border-green-500' : ''}`}
              onClick={() => formik.setFieldValue('ratingType', 'Rapid')}
            >
              <MdAccessTime className="text-green-500 text-base md:text-xl inline" />
            </button> 
            <button
              type="button"
              className={`hover:scale-110 flex items-center ${formik.values.ratingType === 'Blitz' ? ' border-b-3 border-yellow-300' : ''}`}
              onClick={() => formik.setFieldValue('ratingType', 'Blitz')}
            >
              <SiStackblitz className="text-yellow-300 text-base md:text-xl inline" />
            </button>
            <button
              type="button"
              className={`hover:scale-110 flex items-center ${formik.values.ratingType === 'Bullet' ? ' border-b-3 border-red-500' : ''}`}
              onClick={() => formik.setFieldValue('ratingType', 'Bullet')}
            >
              <SiPushbullet className="text-red-500 text-base md:text-xl" />
            </button>
          </div>
        </div>

        {/* RESET ////////////////////////////////////////////////////////////*/}
        <div className="w-full gap-2 flex flex-col">
          {user&&user.role=="admin" && <button
          className="bg-green-600 hover:bg-green-700 text-white p-1 rounded transition font-MyFancyFont w-full"
          onClick={(e) => handleGoToUsersCreate(e)}
        >
          {t.search.createUser}
        </button>}
        <button
          className="bg-red-600 hover:bg-red-700 text-white p-1 rounded transition font-MyFancyFont w-full"
          onClick={(e) => handleResetFilters(e)}
        >
          {t.search.resetFilters}
        </button>
        </div>

      </div>
    </form>
  );
}