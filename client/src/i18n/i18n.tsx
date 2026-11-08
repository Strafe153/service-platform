import i18n from "i18next";
import { initReactI18next } from "react-i18next";
import { Languages } from "./language";
import { getLanguage } from "../services/i18nStorageService";
import en from "./locales/en/translations.json";
import uk from "./locales/uk/translations.json";

const language = getLanguage();

export default i18n
  .use(initReactI18next)
  .init({
    resources: {
      en: { translation: en },
      uk: { translation: uk },
    },
    lng: language,
    fallbackLng: language,
    supportedLngs: Languages,
    interpolation: {
      escapeValue: false,
    },
  });