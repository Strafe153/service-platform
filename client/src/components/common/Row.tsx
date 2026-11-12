import Stack from "@mui/material/Stack";
import type { PropsWithChildren } from "react";

type RowProps = PropsWithChildren<{ spacing?: number }>;

export default function Row({ children, spacing = 2 }: RowProps) {
  return (
    <Stack direction="row" spacing={spacing}>
      {children}
    </Stack>
  );
}