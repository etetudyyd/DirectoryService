export type Department = {
    id: string;
    name: string;
    identifier: string;
    path: string;
    parentId?: string;
    depth: number;
    childrenCount: number;
    isActive: boolean;
    createdAt: Date;
    updatedAt: Date;
    deletedAt: Date | null;
}

// пока что тут, потом перенесу в стор, когда будет готов
export type DepartmentDictionaryState = {
  search?: string;
  departmentsIds?: string[];
  pageSize: number;
};


