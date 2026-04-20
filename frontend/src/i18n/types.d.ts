import 'i18next';
import enAuth from './locales/en/auth.json';
import enErrors from './locales/en/errors.json';
import enNavbar from './locales/en/navbar.json';
import enProfile from './locales/en/profile.json';
import enToast from './locales/en/toast.json';
import enHome from './locales/en/home.json'
import enSearch from './locales/en/search.json'
import enGlobal from './locales/en/global.json';
import enGame from './locales/en/game.json';
declare module 'i18next' {
    interface CustomTypeOptions {
        resources: {
            auth: typeof enAuth;
            errors: typeof enErrors;
            navbar: typeof enNavbar;
            profile: typeof enProfile;
            toast: typeof enToast;
            home: typeof enHome;
            search: typeof enSearch;
            global: typeof enGlobal;
            game: typeof enGame;

        };
    }
}