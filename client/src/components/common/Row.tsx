import Box from "@mui/material/Box";
import type { PropsWithChildren } from "react";

export default function Row({ children, gap = 2 }: PropsWithChildren<{ gap?: number }>) {
  return (
    <Box
      sx={{
        display: "flex",
        flexDirection: "row",
        gap,
      }}
    >
      {children}
    </Box>
  );
}