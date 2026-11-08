import { Language } from "../i18n/language";

const key = "lang";

export function getLanguage(): string {
  return localStorage.getItem(key) || Language.En;
}

export function storeLanguage(language: string) {
  localStorage.setItem(key, language);
}