import { Paper } from "@mui/material";
import { DataGrid, type GridColDef, type GridPaginationModel } from "@mui/x-data-grid";
import { type Dispatch, type SetStateAction } from "react";
import { pageSizeOptions } from "../../utils/layout.utils";
import { useTranslation } from "react-i18next";

type DataTableProps<R> = {
  columns: readonly GridColDef[];
  rows: readonly R[] | undefined;
  rowCount: number | undefined;
  paginationModel: GridPaginationModel;
  setPaginationModel: Dispatch<SetStateAction<GridPaginationModel>>;
};

export default function DataTable<R>({
  columns,
  rows,
  rowCount,
  paginationModel,
  setPaginationModel
}: DataTableProps<R>) {
  const { t } = useTranslation();

  return (
    <Paper>
      <DataGrid
        columns={columns}
        rows={rows}
        rowCount={rowCount ?? 0}
        paginationMode="server"
        pageSizeOptions={pageSizeOptions}
        paginationModel={paginationModel}
        onPaginationModelChange={setPaginationModel}
        localeText={{
          paginationRowsPerPage: t("table.rowsPerPage"),
        }}
        sx={{
          "& .MuiDataGrid-columnHeaderTitle": {
            fontWeight: "bold",
          },
        }}
      />
    </Paper>
  );
}