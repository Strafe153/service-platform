import { AppBar, Box, Button, Toolbar } from "@mui/material";
import { logOut } from "../../../services/authService";

export default function Header() {
  return (
    <header>
      <Box sx={{ flexGrow: 1 }}>
        <AppBar position="static">
          <Toolbar>
            <Button color="inherit" onClick={logOut}>Log out</Button>
          </Toolbar>
        </AppBar>
      </Box>
    </header>
  );
}