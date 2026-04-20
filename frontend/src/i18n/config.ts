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

import plAuth from './locales/pl/auth.json';
import plErrors from './locales/pl/errors.json';
import plNavbar from './locales/pl/navbar.json';
import plProfile from './locales/pl/profile.json';
import plToast from './locales/pl/toast.json';
import plHome from './locales/pl/home.json'
import plSearch from './locales/pl/search.json'
import plGlobal from './locales/pl/global.json';
import plGame from './locales/pl/game.json';
i18n
  .use(LanguageDetector)
  .use(initReactI18next)
  .init({
    resources: {
      en: { auth: enAuth, errors: enErrors, home: enHome, navbar: enNavbar, profile: enProfile, toast: enToast, search: enSearch ,
     global : enGlobal, game: enGame},
      pl: { auth: plAuth, errors: plErrors, home: plHome, navbar: plNavbar, profile: plProfile, toast: plToast, search: plSearch ,
     global : plGlobal, game: plGame}
    },
    fallbackLng: 'en',
    interpolation: { escapeValue: false },
  });

export default i18n;