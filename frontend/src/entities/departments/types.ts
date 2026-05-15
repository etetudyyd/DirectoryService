import { Location } from "../locations/types";
import { Position } from "../positions/types";

export type Department = {
    id: string;
    name: string;
    identifier: string;
    path: string;
    parentId?: string | null;
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
    id: string;
    name: string;
    identifier: string;
    path: string;
    parentId?: string | null;
    depth: number;
    isActive: boolean;
    positionsCount: number; 
    createdAt: Date;
    updatedAt: Date;
    deletedAt: Date | null;
    positions: Position[];
    locations: Location[];
}


