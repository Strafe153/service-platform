import { useState } from "react";
import ErrorMessage from "../../components/common/ErrorMessage";
import EditUser from "./EditUser";
import UserView from "./UserView";
import type { UpdateUserFn, UserReadDto } from "../../types/user.types";

type UserDetailsProps = {
  user: UserReadDto | undefined;
  checkId: boolean;
  isError: boolean;
  error: Error | null;
  updateUserFn: UpdateUserFn;
};

export default function UserDetails({
  user,
  checkId,
  isError,
  error,
  updateUserFn
}: UserDetailsProps) {
  const [isEdit, setIsEdit] = useState<boolean>(false);

  if (isError) {
    return <ErrorMessage value={error!.message} />;
  }

  const userView = !isEdit
    ? (
      <UserView
        user={user}
        isError={isError}
        error={error}
        checkId={checkId}
        setEditMode={() => setIsEdit(true)}
      />
    ) : (
      <EditUser
        user={user}
        checkId={checkId}
        updateUserFn={updateUserFn}
        setViewMode={() => setIsEdit(false)}
      />
    );

  return userView;
}