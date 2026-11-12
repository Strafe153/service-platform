import { useTranslation } from "react-i18next";
import ErrorMessage from "../components/common/ErrorMessage";

export default function Forbidden() {
  const { t } = useTranslation();

  return <ErrorMessage value={t("forbidden")} />;
}