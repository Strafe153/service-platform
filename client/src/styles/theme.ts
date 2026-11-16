import { createTheme } from "@mui/material";
import "@mui/x-data-grid/themeAugmentation";

const theme = createTheme({
  palette: {
    primary: {
      main: "#1a73e8",
      light: "#4a9eff",
      dark: "#0d47a1",
      contrastText: "#ffffff",
    },
    secondary: {
      main: "#5f6368",
      light: "#8d9196",
      dark: "#373d42",
      contrastText: "#ffffff",
    },
    error: {
      main: "#d32f2f",
    },
    background: {
      default: "#f5f7fa",
      paper: "#ffffff",
    },
    text: {
      primary: "#202124",
      secondary: "#5f6368",
    },
    divider: "#e0e0e0",
  },

  typography: {
    fontFamily: [
      "Inter",
      "-apple-system",
      "BlinkMacSystemFont",
      '"Segoe UI"',
      "Roboto",
      "sans-serif",
    ].join(","),
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
      styleOverrides: {
        colorPrimary: {
          backgroundColor: "#1a73e8",
        },
      },
    },

    MuiButton: {
      defaultProps: {
        variant: "contained",
        disableElevation: true,
      },
      styleOverrides: {
        root: {
          borderRadius: 6,
          paddingTop: 8,
          paddingBottom: 8,
          "&.MuiButton-containedPrimary:hover": {
            backgroundColor: "#1557b0",
          },
          "&.MuiButton-containedError:hover": {
            backgroundColor: "#b71c1c",
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
          borderRight: "1px solid #e0e0e0",
        },
      },
    },

    MuiListItemButton: {
      styleOverrides: {
        root: {
          borderRadius: 6,
          "&:hover": {
            backgroundColor: "#e8f0fe",
          },
          "&.Mui-selected": {
            backgroundColor: "#e8f0fe",
            color: "#1a73e8",
          },
        },
      },
    },

    MuiToggleButtonGroup: {
      styleOverrides: {
        root: {
          borderRadius: 6,
        },
      },
    },

    MuiToggleButton: {
      styleOverrides: {
        root: {
          textTransform: "none",
          fontWeight: 500,
          color: "rgba(255,255,255,0.7)",
          borderColor: "rgba(255,255,255,0.4)",
          "&.Mui-selected": {
            backgroundColor: "#ffffff",
            color: "#1a73e8",
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

    MuiDivider: {
      styleOverrides: {
        root: {
          borderColor: "#e0e0e0",
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
            backgroundColor: "#f5f7fa",
            borderBottom: "1px solid #e0e0e0",
          },
          "& .MuiDataGrid-row:hover": {
            backgroundColor: "#f0f4ff",
          },
          "& .MuiDataGrid-footerContainer": {
            borderTop: "1px solid #e0e0e0",
          },
        },
      },
    },
  },
});

export default theme;
