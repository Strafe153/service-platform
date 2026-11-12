import { useState } from "react";
import type { GridPaginationModel } from "@mui/x-data-grid";
import { pageSizeOptions } from "../utils/layout.utils";

export default function usePaginationModel() {
  return useState<GridPaginationModel>({
    page: 0,
    pageSize: pageSizeOptions[0],
  });
}