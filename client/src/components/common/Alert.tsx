import { IconButton, Snackbar } from "@mui/material";
import CloseIcon from "@mui/icons-material/Close";
import { useShallow } from "zustand/react/shallow";
import useAlertStore from "../../store/alertStore";

export default function Alert() {
  const { error, setError, closeAlert } = useAlertStore(useShallow((state) => ({
    error: state.error,
    setError: state.setError,
    closeAlert: state.onClose,
  })));

  function close() {
    closeAlert?.();
    setError(undefined);
  }

  return (
    <Snackbar
      open={!!error}
      message={error}
      autoHideDuration={3000}
      anchorOrigin={{
        vertical: "top",
        horizontal: "right"
      }}
      onClose={close}
      action={
        <IconButton
          sx={{ p: .5 }}
          onClick={close}
        >
          <CloseIcon />
        </IconButton>
      }
    />
  );
}