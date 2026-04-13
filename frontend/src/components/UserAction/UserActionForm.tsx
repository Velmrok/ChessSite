import { useFormik } from 'formik';
import * as Yup from 'yup';
import useLanguageStore from '@/stores/useLanguageStore';
import { use, useEffect } from 'react';
import { useUserProfileStore } from '@/stores/useUserProfileStore';

export default function UserActionForm({  onSubmit,  mode }: any) {
    const t = useLanguageStore((state) => state.t);
    const profile = useUserProfileStore(s => s.profile);
   
    const validationSchema = Yup.object({
        nickname: mode === 'add' ? Yup.string().required(t.createAccount.required.nickname) : Yup.string(),
        login: mode === 'add' ? Yup.string().required(t.createAccount.required.login) : Yup.string(),
        
        password: mode === 'add' 
            ? Yup.string().required(t.createAccount.required.password) 
            : Yup.string(),
        bio: Yup.string().max(200),
        rapid: Yup.number().min(0).max(3000).required(t.createAccount.required.rating),
        blitz: Yup.number().min(0).max(3000).required(t.createAccount.required.rating),
        bullet: Yup.number().min(0).max(3000).required(t.createAccount.required.rating),
       
    });

    const initialValues = {
        login: '',
        nickname: profile?.nickname || '',
        password: '',
        bio: profile?.bio || '',
        rapid: profile?.userInfo.rating.rapid || 1200,
        blitz: profile?.userInfo.rating.blitz || 1200,
        bullet: profile?.userInfo.rating.bullet || 1200,
    };

    const formik = useFormik({
        initialValues,
        validationSchema,
        enableReinitialize: true, 
        onSubmit: onSubmit
    });

    return (
        <form onSubmit={formik.handleSubmit} className="flex flex-col gap-2 text-black w-full">
             
            {mode==='add' && <><input 
                name="login" 
                value={formik.values.login} 
                onChange={formik.handleChange}
                onBlur={formik.handleBlur}
                placeholder="Login" 
                disabled={mode === 'edit'} 
                className={"p-2 rounded border"}
            />
            {formik.touched.login && formik.errors.login && (
                <div className="text-red-500 text-sm">{formik.errors.login}</div>
            )}
            
             <input 
                name="nickname" 
                value={formik.values.nickname} 
                onChange={formik.handleChange}
                onBlur={formik.handleBlur}
                placeholder={t.createAccount.nickname} 
                className={"p-2 rounded border"}
            /></>}
            {formik.touched.nickname && formik.errors.nickname && (
                <div className="text-red-500 text-sm">{formik.errors.nickname}</div>
            )}
            <input 
                name="password" 
                type="password"
                value={formik.values.password} 
                onChange={formik.handleChange}
                onBlur={formik.handleBlur}
                placeholder={t.createAccount.password} 
                className={"p-2 rounded border"}
            />
            {formik.touched.password && formik.errors.password && (
                <div className="text-red-500 text-sm">{formik.errors.password}</div>
            )}
            
            <input 
                name="bio" 
                value={formik.values.bio} 
                onChange={formik.handleChange}
                onBlur={formik.handleBlur}
                placeholder={t.createAccount.bioPlaceholder}  
                className={"p-2 rounded border"}
            />
            {formik.touched.bio && formik.errors.bio && (
                <div className="text-red-500 text-sm">{formik.errors.bio}</div>
            )}
            <input 
                name="rapid" 
                value={formik.values.rapid} 
                onChange={formik.handleChange}
                onBlur={formik.handleBlur}
                placeholder={t.createAccount.rapidRating} 
                className={"p-2 rounded border"}
            />
            {formik.touched.rapid && formik.errors.rapid && (
                <div className="text-red-500 text-sm">{formik.errors.rapid}</div>
            )}
            <input 
                name="blitz" 
                value={formik.values.blitz} 
                onChange={formik.handleChange}
                onBlur={formik.handleBlur}
                placeholder={t.createAccount.blitzRating} 
                className={"p-2 rounded border"}
            />
            {formik.touched.blitz && formik.errors.blitz && (
                <div className="text-red-500 text-sm">{formik.errors.blitz}</div>
            )}
            <input 
                name="bullet" 
                value={formik.values.bullet} 
                onChange={formik.handleChange}
                onBlur={formik.handleBlur}
                placeholder={t.createAccount.bulletRating} 
                className={"p-2 rounded border"}
            />
            {formik.touched.bullet && formik.errors.bullet && (
                <div className="text-red-500 text-sm">{formik.errors.bullet}</div>
            )}
            
            <button type="submit" className="bg-blue-500 hover:bg-blue-700 transition-colors duration-300 text-white p-2 rounded">
                {t.createAccount.submit}
            </button>
        </form>
    );
}