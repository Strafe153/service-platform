import { useQuery } from "@tanstack/react-query";
import { getUsers } from "../services/usersService";
import { type GridColDef } from "@mui/x-data-grid";
import { Link } from "react-router";
import { useTranslation } from "react-i18next";
import ErrorMessage from "../components/common/ErrorMessage";
import { buildFlexGridColumn } from "../utils/layout.utils";
import type { UserReadDto } from "../types/user.types";
import type { Page } from "../types/common.types";
import DataTable from "../components/common/DataTable";
import usePaginationModel from "../hooks/usePaginationModel";

export default function Users() {
  const [paginationModel, setPaginationModel] = usePaginationModel();

  const { data: users, isError, error } = useQuery<Page<UserReadDto>>({
    queryKey: ["users", paginationModel.page, paginationModel.pageSize],
    queryFn: () => getUsers({
      pageSize: paginationModel.pageSize,
      pageNumber: paginationModel.page + 1,
    }),
    placeholderData: (previousData) => previousData,
  });

  const { t } = useTranslation();

  const columns: GridColDef[] = [
    buildFlexGridColumn({
      field: "id",
      headerName: t("user.table.id"),
      renderCell: (params) => {
        return <Link to={`/users/${params.id}`}>{params.id}</Link>;
      },
    }),
    buildFlexGridColumn({
      field: "email",
      headerName: t("user.table.email"),
    }),
    buildFlexGridColumn({
      field: "firstName",
      headerName: t("user.table.firstName"),
    }),
    buildFlexGridColumn({
      field: "lastName",
      headerName: t("user.table.lastName"),
    }),
    buildFlexGridColumn({
      field: "phoneNumber",
      headerName: t("user.table.phoneNumber"),
    }),
    buildFlexGridColumn({
      field: "birthDate",
      headerName: t("user.table.birthDate"),
    }),
  ];

  if (isError) {
    return <ErrorMessage value={error.message} />;
  }

  return (
    <DataTable
      columns={columns}
      rows={users?.items}
      rowCount={users?.totalItems}
      paginationModel={paginationModel}
      setPaginationModel={setPaginationModel}
    />
  );
}