import type { GridColDef } from "@mui/x-data-grid";

export const pageSizeOptions = [5, 10, 25];

export function buildFlexGridColumn(definition: GridColDef): GridColDef {
  return {
    headerAlign: "center",
    flex: 1,
    ...definition,
  };
}