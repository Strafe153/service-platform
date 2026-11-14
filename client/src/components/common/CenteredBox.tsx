import { Box, type SxProps } from "@mui/material";
import type { PropsWithChildren } from "react";

type CenteredBoxProps = PropsWithChildren<{ sx?: SxProps }>;

export default function CenteredBox({ children, sx }: CenteredBoxProps) {
  return (
    <Box
      sx={{
        display: "flex",
        justifyContent: "center",
        alignItems: "center",
        flexGrow: 1,
        ...sx,
      }}
    >
      {children}
    </Box>
  );
}