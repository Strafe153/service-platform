import { useQuery } from "@tanstack/react-query";
import { getUser } from "../../services/usersService";
import { useParams } from "react-router";
import type { Guid } from "../../types/common.types";
import { isGuid } from "../../utils/common.utils";
import ErrorMessage from "../../components/common/ErrorMessage";
import UserView from "../../features/user-profile/UserView";

export default function UserProfile() {
  const params = useParams();

  const { data: user, isError, error } = useQuery({
    queryKey: ["users", params.id],
    queryFn: () => getUser(params.id as Guid)
  });

  if (!isGuid(params.id)) {
    return <ErrorMessage value="user.errors.incorrectId" />;
  }
  
  if (isError) {
    return <ErrorMessage value={error.message} />;
  }

  return (
    <UserView
      user={user}
      isError={isError}
      error={error}
      checkId={true}
    />
  );
}