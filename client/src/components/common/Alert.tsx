import { IconButton, Snackbar } from "@mui/material";
import CloseIcon from "@mui/icons-material/Close";

type AlertProps = {
  open: boolean;
  message: string | undefined;
  onClose: () => void;
};

export default function Alert({ open, message, onClose }: AlertProps) {
  return (
    <Snackbar
      open={open}
      message={message}
      autoHideDuration={3000}
      anchorOrigin={{ vertical: "top", horizontal: "right" }}
      onClose={onClose}
      action={
        <IconButton
          sx={{ p: .5 }}
          onClick={onClose}
        >
          <CloseIcon />
        </IconButton>
      }
    />
  );
}