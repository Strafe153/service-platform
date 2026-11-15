import { useQuery } from "@tanstack/react-query";
import ErrorMessage from "../../components/common/ErrorMessage";
import { getCurrentUser, updateCurrentUser } from "../../services/usersService";
import { getSubId } from "../../services/authService";
import UserDetails from "../../features/user-profile/UserDetails";

export default function CurrentUserProfile() {
  const { data: user, isError, error } = useQuery({
    queryKey: ["users", getSubId()],
    queryFn: getCurrentUser,
  });

  if (isError) {
    return <ErrorMessage value={error.message} />;
  }

  return (
    <UserDetails
      user={user}
      checkId={false}
      isError={isError}
      error={error}
      updateUserFn={(_, dto) => updateCurrentUser(dto)}
    />
  );
}