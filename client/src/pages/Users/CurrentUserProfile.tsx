import { useQuery } from "@tanstack/react-query";
import ErrorMessage from "../../components/common/ErrorMessage";
import UserView from "../../features/user-profile/UserView";
import { getCurrentUser } from "../../services/usersService";
import { getSubId } from "../../services/authService";

export default function CurrentUserProfile() {
  const subId = getSubId();

  const { data: user, isError, error } = useQuery({
    queryKey: ["users", subId],
    queryFn: getCurrentUser,
  });

  if (isError) {
    return <ErrorMessage value={error.message} />;
  }

  return (
    <UserView
      user={user}
      isError={isError}
      error={error}
      checkId={false}
    />
  );
}