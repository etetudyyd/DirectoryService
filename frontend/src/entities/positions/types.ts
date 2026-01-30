export type Position = {
    id: string;
    name: string;
    description: string;
    isActive: boolean;
    createdAt: Date;
    updatedAt: Date;
    deletedAt: Date | null;
    departmentsIds: string[];
}