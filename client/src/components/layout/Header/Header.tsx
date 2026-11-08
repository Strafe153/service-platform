import { useState, type MouseEvent } from "react";
import { AppBar, Box, Button, ToggleButton, ToggleButtonGroup, Toolbar, Typography  } from "@mui/material";
import HomeRepairServiceIcon from "@mui/icons-material/HomeRepairService";
import { Link } from "react-router";
import { useTranslation } from "react-i18next";
import { isAuthenticated, login, logOut } from "../../../services/authService";
import { Languages } from "../../../i18n/language";
import { getLanguage, storeLanguage } from "../../../services/i18nStorageService";

export default function Header() {
  const { t, i18n } = useTranslation();
  const [lang, setLang] = useState(getLanguage());
  
  const authButtons = isAuthenticated()
    ? <Button color="inherit" onClick={logOut}>{t("header.logOut")}</Button>
    : <>
      <Button color="inherit">
        <Link to="/sign-up">{t("header.signUp")}</Link>
      </Button>
      <Button color="inherit" onClick={login}>{t("header.logIn")}</Button>
    </>;

  function changeLanguage(_: MouseEvent<HTMLElement>, value: string) {
    if (!Languages.includes(value)) {
      return;
    }

    i18n.changeLanguage(value);
    setLang(value);
    storeLanguage(value);
  }

  return (
    <Box component="header">
      <AppBar position="static">
        <Toolbar>
          <HomeRepairServiceIcon fontSize="large" sx={{ mr: 2 }} />
          <Typography
            variant="h5"
            sx={{
              flexGrow: 1,
              paddingTop: 1
            }}
          >
            <Link to="/">{t("header.servicePlatform")}</Link>
          </Typography>

          <Box
            sx={{
              display: "flex",
              gap: 1,
            }}
          >
            <ToggleButtonGroup
              value={lang}
              exclusive
              onChange={changeLanguage}
            >
              {Languages.map((l) => <ToggleButton value={l}>{l}</ToggleButton>)}
            </ToggleButtonGroup>
            {authButtons}
          </Box>
        </Toolbar>
      </AppBar>
    </Box>
  );
}