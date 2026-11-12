import { useQuery } from "@tanstack/react-query";
import { getUser } from "../services/usersService";
import { Box, Typography } from "@mui/material";
import { useParams } from "react-router";
import type { Guid } from "../types/common.types";
import Row from "../components/common/Row";
import { isGuid } from "../utils/common.utils";
import ErrorMessage from "../components/common/ErrorMessage";

export default function User() {
  const params = useParams();

  const { data, isError, error } = useQuery({
    queryKey: ["user", params.id],
    queryFn: () => getUser(params.id as Guid)
  });

  if (!isGuid(params.id)) {
    return <ErrorMessage value="user.errors.incorrectId" />;
  }
  
  if (isError) {
    return <ErrorMessage value={error.message} />;
  }

  return (
    <Box
      sx={{
        display: "flex",
        justifyContent: "center",
        alignItems: "center",
        flexDirection: "column",
        minHeight: "calc(100vh - 18.5vh)",
      }}
    >
      <Row>
        <Typography>{data?.firstName}</Typography>
        <Typography>{data?.lastName}</Typography>
      </Row>
      <Row>
        <Typography>{data?.email}</Typography>
        <Typography>{data?.phoneNumber}</Typography>
      </Row>
      <Row>
        <Typography>{data?.birthDate.toString()}</Typography>
        <Typography>{data?.address.country}</Typography>
      </Row>
      <Row>
        <Typography>{data?.address.state}</Typography>
        <Typography>{data?.address.city}</Typography>
      </Row>
      <Row>
        <Typography>{data?.address.zipCode}</Typography>
        <Typography>{data?.address.street}</Typography>
      </Row>
    </Box>
  );
}