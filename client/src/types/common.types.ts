export type Guid = `${string}-${string}-${string}-${string}-${string}`;

export type ParameterlessAction = () => void;

export interface Page<T> {
  pageNumber: number;
  pageSize: number;
  items: T[];
  totalItems: number;
};
