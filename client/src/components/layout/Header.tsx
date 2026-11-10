import { useState, type MouseEvent } from "react";
import { Link } from "react-router";
import { useTranslation } from "react-i18next";
import { AppBar, Box, Button, IconButton, ToggleButton, ToggleButtonGroup, Toolbar, Typography  } from "@mui/material";
import MenuIcon from "@mui/icons-material/Menu";
import Row from "../common/Row";
import { isAuthenticated, login, logOut } from "../../services/authService";
import { getLanguage, storeLanguage } from "../../services/i18nStorageService";
import { Languages } from "../../i18n/language";
import type { ParameterlessAction } from "../../types/common.types";

export default function Header({ openSidebar }: { openSidebar: ParameterlessAction }) {
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
          <Box
            sx={{
              display: "flex",
              justifyContent: "space-between",
              alignItems: "center",
              width: "100%"
            }}
          >
            <Row spacing={1}>
              <IconButton size="large" onClick={openSidebar}>
                <MenuIcon />
              </IconButton>
              
              <Typography
                variant="h5"
                sx={{
                  flexGrow: 1,
                  paddingTop: 1
                }}
              >
                <Link to="/">{t("header.servicePlatform")}</Link>
              </Typography>
            </Row>

            <Row spacing={1}>
              <ToggleButtonGroup
                value={lang}
                exclusive
                onChange={changeLanguage}
              >
                {Languages.map((l) => <ToggleButton value={l}>{l}</ToggleButton>)}
              </ToggleButtonGroup>
              {authButtons}
            </Row>
          </Box>
        </Toolbar>
      </AppBar>
    </Box>
  );
}