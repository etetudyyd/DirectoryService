import { DictionaryItemResponse } from "@/shared/api/types";

export type Department = {
    id: string;
    name: string;
    identifier: string;
    path: string;
    parentId?: string;
    depth: number;
    childrenCount: number;
    locationCount: number;
    positionCount: number;
    isActive: boolean;
    createdAt: Date;
    updatedAt: Date;
    deletedAt: Date | null;
    locationsIds: string[];
    positionsIds:string[];
}

export type DepartmentDetails = {
    departmentId: string;
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
    locations: DictionaryItemResponse[];
    positions: DictionaryItemResponse[];
}


