import { DictionaryItemResponse } from "@/shared/api/types";

export type Position = {
    id: string;
    name: string;
    description: string;
    isActive: boolean;
    departmentCount: number;
    createdAt: Date;
    updatedAt: Date;
    deletedAt: Date | null;
    departmentsIds: string[];
}

export type PositionDetails = {
    id: string;
    name: string;
    description: string;
    isActive: boolean;
    departmentCount: number;
    createdAt: Date;
    updatedAt: Date;
    deletedAt: Date | null;
    departments: DictionaryItemResponse[];
}