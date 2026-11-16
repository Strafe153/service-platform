import { createTheme } from "@mui/material";
import "@mui/x-data-grid/themeAugmentation";

const colors = {
  primary: "#1a73e8",
  primaryDark: "#1557b0",
  primaryLight: "#4a9eff",
  primaryDeep: "#0d47a1",
  primaryTint: "#e8f0fe",

  secondary: "#5f6368",
  secondaryLight: "#8d9196",
  secondaryDark: "#373d42",

  error: "#d32f2f",
  errorDark: "#b71c1c",

  white: "#ffffff",
  backgroundDefault: "#f5f7fa",
  backgroundHover: "#f0f4ff",

  textPrimary: "#202124",
  divider: "#e0e0e0",
} as const;

const border = `1px solid ${colors.divider}`;
const smallRadius = 6;

const theme = createTheme({
  palette: {
    primary: {
      main: colors.primary,
      light: colors.primaryLight,
      dark: colors.primaryDeep,
      contrastText: colors.white,
    },
    secondary: {
      main: colors.secondary,
      light: colors.secondaryLight,
      dark: colors.secondaryDark,
      contrastText: colors.white,
    },
    error: {
      main: colors.error,
    },
    background: {
      default: colors.backgroundDefault,
      paper: colors.white,
    },
    text: {
      primary: colors.textPrimary,
      secondary: colors.secondary,
    },
    divider: colors.divider,
  },

  typography: {
    fontFamily: "Inter, -apple-system, BlinkMacSystemFont, \"Segoe UI\", Roboto, sans-serif",
    h5: {
      fontWeight: 600,
      letterSpacing: "-0.01em",
    },
    body1: {
      fontSize: "0.9375rem",
    },
    button: {
      textTransform: "none",
      fontWeight: 500,
    },
  },

  shape: {
    borderRadius: 8,
  },

  components: {
    MuiAppBar: {
      defaultProps: {
        elevation: 0,
      },
    },

    MuiButton: {
      defaultProps: {
        variant: "contained",
        disableElevation: true,
      },
      styleOverrides: {
        root: {
          borderRadius: smallRadius,
          paddingTop: 8,
          paddingBottom: 8,
          "&.MuiButton-containedPrimary:hover": {
            backgroundColor: colors.primaryDark,
          },
          "&.MuiButton-containedError:hover": {
            backgroundColor: colors.errorDark,
          },
        },
        colorInherit: {
          backgroundColor: "transparent",
          color: "inherit",
          boxShadow: "none",
          "&:hover": {
            backgroundColor: "rgba(255,255,255,0.15)",
            boxShadow: "none",
          },
        },
      },
    },

    MuiTextField: {
      defaultProps: {
        size: "small",
      },
    },

    MuiPaper: {
      defaultProps: {
        elevation: 0,
      },
    },

    MuiDrawer: {
      styleOverrides: {
        paper: {
          borderRight: border,
        },
      },
    },

    MuiListItemButton: {
      styleOverrides: {
        root: {
          borderRadius: smallRadius,
          "&:hover": {
            backgroundColor: colors.primaryTint,
          },
          "&.Mui-selected": {
            backgroundColor: colors.primaryTint,
            color: colors.primary,
          },
        },
      },
    },

    MuiToggleButtonGroup: {
      styleOverrides: {
        root: {
          borderRadius: smallRadius,
        },
      },
    },

    MuiToggleButton: {
      styleOverrides: {
        root: {
          color: "rgba(255,255,255,0.7)",
          borderColor: "rgba(255,255,255,0.4)",
          "&.Mui-selected": {
            backgroundColor: colors.white,
            color: colors.primary,
            "&:hover": {
              backgroundColor: "rgba(255,255,255,0.9)",
            },
          },
          "&:hover": {
            backgroundColor: "rgba(255,255,255,0.1)",
          },
        },
      },
    },

    MuiDataGrid: {
      defaultProps: {
        disableColumnMenu: true,
      },
      styleOverrides: {
        root: {
          border: "none",
          "& .MuiDataGrid-columnHeaders": {
            backgroundColor: colors.backgroundDefault,
            borderBottom: border,
          },
          "& .MuiDataGrid-row:hover": {
            backgroundColor: colors.backgroundHover,
          },
          "& .MuiDataGrid-footerContainer": {
            borderTop: border,
          },
        },
      },
    },
  },
});

export default theme;
