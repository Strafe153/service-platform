import Stack from "@mui/material/Stack";
import type { PropsWithChildren } from "react";

export default function Row({ children, spacing = 2 }: PropsWithChildren<{ spacing?: number }>) {
  return (
    <Stack direction="row" spacing={spacing}>
      {children}
    </Stack>
  );
}