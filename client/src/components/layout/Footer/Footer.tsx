import { AppBar, Box, Toolbar, Typography } from '@mui/material';

export default function Footer() {
  return (
    <footer>
      <Box sx={{ flexGrow: 1 }}>
        <AppBar position="static">
          <Toolbar />
          <Typography>Text</Typography>
        </AppBar>
      </Box>
    </footer>
  );
}