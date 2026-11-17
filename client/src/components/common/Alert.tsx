import { IconButton, Snackbar } from "@mui/material";
import CloseIcon from "@mui/icons-material/Close";
import useAlertStore from "../../store/alertStore";

export default function Alert() {
  const error = useAlertStore((state) => state.error);
  const setError = useAlertStore((state) => state.setError);
  const closeAlert = useAlertStore((state) => state.onClose);

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