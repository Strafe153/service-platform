import { Divider, Drawer, IconButton, List, ListItem, ListItemButton, ListItemText } from "@mui/material";
import type { ParameterlessAction } from "../../types/common.types";
import { ChevronLeft } from "@mui/icons-material";
import { useNavigate } from "react-router";
import type { MenuItem } from "../../types/layout.types";
import { useTranslation } from "react-i18next";

const drawerWidth = "20%";
const drawerMinWidth = "240px";

type SidebarProps = {
  isOpen: boolean;
  onClose: ParameterlessAction;
  items: MenuItem[];
};

export default function Sidebar({ isOpen, onClose, items }: SidebarProps) {
  const navigate = useNavigate();
  const { t } = useTranslation();

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
      <IconButton onClick={onClose}>
        <ChevronLeft />
      </IconButton>

      <Divider />

      <List>
        {items.map((i) => {
          return (
            <ListItem key={i.path}>
              <ListItemButton onClick={() => {
                onClose();
                navigate(i.path);
              }}>
                <ListItemText primary={t(i.name)} />
              </ListItemButton>
            </ListItem>
          );
        })}
      </List>
    </Drawer>
  );
}