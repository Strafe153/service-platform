import { create } from "zustand";
import type { ParameterlessAction } from "../types/common.types";

type AlertStoreState = {
  error: string | undefined;
  onClose?: ParameterlessAction;
};

type AlertStoreActions = {
  setError: (value: AlertStoreState["error"]) => void;
  setOnClose: (value: AlertStoreState["onClose"]) => void;
};

const useAlertStore = create<AlertStoreState & AlertStoreActions>((set) => ({
  error: undefined,
  setError: (newError) => set(() => ({ error: newError })),
  setOnClose: (onClose) => set(() => ({ onClose })),
}));

export default useAlertStore;