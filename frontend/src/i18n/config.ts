import i18n from 'i18next';
import { initReactI18next } from 'react-i18next';
import LanguageDetector from 'i18next-browser-languagedetector';

import enAuth from './locales/en/auth.json';
import enErrors from './locales/en/errors.json';
import enNavbar from './locales/en/navbar.json';
import enProfile from './locales/en/profile.json';
import enToast from './locales/en/toast.json';
import enHome from './locales/en/home.json'
import enSearch from './locales/en/search.json'
import enGlobal from './locales/en/global.json';
import enGame from './locales/en/game.json';
i18n
  .use(LanguageDetector)
  .use(initReactI18next)
  .init({
    resources: {
      en: { auth: enAuth, errors: enErrors, home: enHome, navbar: enNavbar, profile: enProfile, toast: enToast, search: enSearch ,
     global : enGlobal, game: enGame},
    },
    fallbackLng: 'en',
    interpolation: { escapeValue: false },
  });

export default i18n;