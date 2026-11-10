import { Divider, Drawer, IconButton } from "@mui/material";
import type { ParameterlessAction } from "../../types/common.types";
import { ChevronLeft } from "@mui/icons-material";

const drawerWidth = "20%";
const drawerMinWidth = "240px";

type SidebarProps = {
  isOpen: boolean;
  onClose: ParameterlessAction;
};

export default function Sidebar({ isOpen, onClose }: SidebarProps) {
  return (
    <Drawer
      open={isOpen}
      onClose={onClose}
      slotProps={{
        paper: {
          sx: {
            width: drawerWidth,
            minWidth: drawerMinWidth,
            boxSizing: "border-box",
          },
        },
      }}
    >
      <IconButton
        size="large"
        onClick={onClose}
      >
        <ChevronLeft />
      </IconButton>

      <Divider />
    </Drawer>
  );
}