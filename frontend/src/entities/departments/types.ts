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



