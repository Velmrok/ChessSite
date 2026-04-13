import { translations, type Language } from '../translations';
import {create} from 'zustand';
type LanguageStoreType = {
  language: Language;
  setLanguage: (lang: Language) => void;
  t: typeof translations['en']; 
};

const useLanguageStore = create<LanguageStoreType>(set=>({
    language : 'en',
    setLanguage: (lang: Language) => set({language: lang, t: translations[lang]}),
    t: translations['en']
}));

export default useLanguageStore;