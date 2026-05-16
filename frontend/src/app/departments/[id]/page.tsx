"use client";

import { useParams, useRouter } from "next/navigation";
import { useDeleteDepartment } from "@/features/departments/model/use-delete-department";
import { useGetDepartment } from "@/features/departments/model/use-get-department";
import { useState } from "react";
import { DetailsLoadingSkeleton } from "@/widgets/details-loading-skeleton";
import { DetailsErrorPage } from "@/pages/details-error-page";
import { Alert, AlertDescription, AlertTitle } from "@/shared/components/ui/alert";
import { Button } from "@/shared/components/ui/button";
import { toast } from "sonner";
import { AlertCircle, ArrowLeft, Calendar, CheckCircle, Clock, Copy, Edit, FileText, IdCardIcon, X, XCircle } from "lucide-react";
import { Avatar, AvatarFallback } from "@/shared/components/ui/avatar";
import { Card, CardContent, CardDescription, CardHeader, CardTitle } from "@/shared/components/ui/card";
import { UpdateDepartmentDialog } from "@/features/departments/update-department-dialog";
import { DeleteConfirmationDialog } from "@/features/delete-confirmation-dialog";
import { Badge } from "@/shared/components/ui/badge";
import { useGetChildrenDepartment } from "@/features/departments/model/use-get-children-departments";
import { Breadcrumb, BreadcrumbItem, BreadcrumbLink, BreadcrumbList, BreadcrumbPage, BreadcrumbSeparator } from "@/shared/components/ui/breadcrumb";
import { buildBreadcrumbs } from "@/shared/lib/breadcrumbs/buildBreadcrumbs";
import React from "react";
import Link from "next/link";
import { useUpdateDepartmentLocations } from "@/features/departments/model/use-department-locations";
import LocationItemSelector from "@/widgets/locations/locations-item-selector";
import { Separator } from "@/shared/components/ui/separator";
import { useActivateDepartment } from "@/features/departments/model/use-activate-department";

export default function DepartmentDetailsPage() {

    const params = useParams();
    const router = useRouter();
    const [updateOpen, setUpdateOpen] = useState(false);
    const [isUpdateLocs, setIsUpdateLocs] = useState(false);
    const [selectedLocIds, setSelectedLocIds] = useState<string[]>([]);

    const { deleteDepartment, isPending: isDeletePending } = useDeleteDepartment();
    const { activateDepartment, isPending: isActivatePending } = useActivateDepartment();
    const [deleteOpen, setDeleteOpen] = useState(false);
    const [loading, setLoading] = useState(false);

    const departmentId = params!.id as string;

    const { department, isPending, error, isError, refetch } =
        useGetDepartment(departmentId);

    const { departments: childDepartments,
        isPending: isChildPending,
        totalItems: childTotalItems,
        totalPages: childTotalPages,
        error: childError,
        isError: isChildError,
        cursorRef: childCursorRef,
        isFetchingNextPage: isChildFetchingNextPage
    } =
        useGetChildrenDepartment(departmentId);


    const {
        updateDepartmentLocations,
        isError: isUpdateError,
        error: updateError,
        isPending: isUpdatePending,
    } = useUpdateDepartmentLocations();

    const handleDelete = async () => {
        setLoading(true);
        try {
            await deleteDepartment(department!.id);
        } finally {
            setLoading(false);
            setDeleteOpen(false);
        }
    };

      const handleActivate = async () => {
        setLoading(true);
        try {
            await activateDepartment(department!.id);
        } finally {
            setLoading(false);
        }
    };

    const handleDeleteClick = (e: React.MouseEvent) => {
        e.preventDefault();
        e.stopPropagation();
        setDeleteOpen(true);
    };

    if (isPending) {
        return <DetailsLoadingSkeleton />;
    }

    if (isError && error) {
        return <DetailsErrorPage error={error} onRetry={refetch} />;
    }

    if (!department) {
        return (
            <div className="container mx-auto py-6">
                <Alert>
                    <AlertTitle>Department not found</AlertTitle>
                    <AlertDescription>
                        The department you are looking for does not exist.
                        <Button
                            variant="outline"
                            className="mt-2"
                            onClick={() => router.push("/departments")}
                        >
                            View all departments
                        </Button>
                    </AlertDescription>
                </Alert>
            </div>
        );
    }

    // Инициализируем selectedLocationIds при первом рендере и когда department загружен
    const currentLocIds = department.locations?.map((location) => location.id) || [];

    // Инициализируем selectedLocIds только один раз, когда position загружен и selectedLocIds пустой
    if (selectedLocIds.length === 0 && currentLocIds.length > 0) {
        setSelectedLocIds(currentLocIds);
    }

    const handleEditClick = () => {
        // При начале редактирования устанавливаем текущие значения
        setSelectedLocIds(currentLocIds);
        setIsUpdateLocs(true);
    };

    const handleSave = async () => {
        try {
            await updateDepartmentLocations({
                departmentId: department!.id,
                locationsIds: selectedLocIds, // Отправляем выбранные ID
            });

            setIsUpdateLocs(false);
            refetch(); // Обновляем данные позиции
        } catch (error) {
            // Ошибка обрабатывается в хуке
        }
    };

    const handleCancel = () => {
        // Сбрасываем к исходным значениям
        setSelectedLocIds(currentLocIds);
        setIsUpdateLocs(false);
    };

    // Обработчик изменений в селекторе
    const handleLocationChange = (locationIds: string[]) => {
        setSelectedLocIds(locationIds);
    };

    // Format dates
    const formatDate = (date: Date) => {
        try {
            return new Intl.DateTimeFormat("en-US", {
                year: "numeric",
                month: "long",
                day: "numeric",
                hour: "2-digit",
                minute: "2-digit",
            }).format(new Date(date));
        } catch {
            return "Invalid date";
        }
    };

    // Get initials for avatar
    const getInitials = (name: string) => {
        if (!name) return "??";
        return name
            .split(" ")
            .map((word) => word[0])
            .join("")
            .toUpperCase()
            .slice(0, 2);
    };

    const handleCopy = () => {
        navigator.clipboard.writeText(department.id);
        toast.success("Copied");
    };

    const breadcrumbs = buildBreadcrumbs(department.path);

    return (
        <main className="container mx-auto py-6 space-y-6">

            <Breadcrumb>
                <BreadcrumbList>
                    {breadcrumbs.map((item, index) => (
                        <React.Fragment key={item.href}>
                            <BreadcrumbItem>
                                <BreadcrumbPage>
                                    {item.label}
                                </BreadcrumbPage>
                            </BreadcrumbItem>

                            {index !== breadcrumbs.length - 1 && (
                                <BreadcrumbSeparator />
                            )}
                        </React.Fragment>
                    ))}
                </BreadcrumbList>
            </Breadcrumb>

            {/* Header */}
            <div className="flex flex-col sm:flex-row sm:items-center justify-between gap-4">
                <div className="flex items-start gap-4">
                    <Button
                        variant="ghost"
                        size="icon"
                        onClick={() => router.back()}
                        className="rounded-full"
                    >
                        <ArrowLeft className="h-5 w-5" />
                    </Button>

                    <div className="flex items-start gap-4">
                        <Avatar className="h-12 w-12 border">
                            <AvatarFallback className="bg-primary/10 text-primary font-semibold">
                                {getInitials(department.name)}
                            </AvatarFallback>
                        </Avatar>

                        <div>
                            <div className="flex items-center gap-2">
                                <h1 className="text-3xl font-bold tracking-tight">
                                    {department.name}
                                </h1>
                                <Badge
                                    className={`flex items-center gap-1 text-white
                                ${department.isActive
                                            ? "bg-emerald-500 hover:bg-emerald-600"
                                            : "bg-red-400 hover:bg-red-400"
                                        }`}
                                >
                                    {department.isActive ? (
                                        <>
                                            <CheckCircle className="h-3 w-3" />
                                            Active
                                        </>
                                    ) : (
                                        <>
                                            <XCircle className="h-3 w-3" />
                                            Inactive
                                        </>
                                    )}
                                </Badge>
                            </div>
                            <div className="flex items-center gap-1">
                                <p className="text-sm text-muted-foreground mt-1">
                                    {department.id}
                                </p>
                                <Button
                                    onClick={handleCopy}
                                    className="h-6 w-6 text-white bg-transparent hover:bg-transparent hover:text-gray-500"
                                >
                                    <Copy className="h-3 w-3" />
                                </Button>
                            </div>
                        </div>
                    </div>
                </div>
            </div>

            {/* Alert if department is deleted */}
            {department.deletedAt && (
                <Alert variant="destructive">
                    <AlertCircle className="h-4 w-4" />
                    <AlertTitle>Department Deleted</AlertTitle>
                    <AlertDescription>
                        This department was deleted on {formatDate(department.deletedAt)}. It
                        may not be available for all operations.
                    </AlertDescription>
                </Alert>
            )}

            {/* Alert if update error */}
            {isUpdateError && (
                <Alert variant="destructive">
                    <AlertCircle className="h-4 w-4" />
                    <AlertTitle>Update Failed</AlertTitle>
                    <AlertDescription>
                        {updateError?.message || "Failed to update departments"}
                    </AlertDescription>
                </Alert>
            )}

            {/* Main content */}
            <div className="grid grid-cols-1 lg:grid-cols-3 gap-6">
                <div className="lg:col-span-2 space-y-6">

                    {/* Details Card */}
                    <Card>
                        <CardHeader>
                            <CardTitle>Department Details</CardTitle>
                            <CardDescription>
                                Basic information about this department
                            </CardDescription>
                        </CardHeader>
                        <CardContent>
                            <div className="grid grid-cols-1 md:grid-cols-2 gap-6">
                                {/* Name */}
                                <div className="space-y-2">
                                    <div className="text-sm font-medium text-muted-foreground">
                                        Department Name
                                    </div>
                                    <div className="p-3 bg-muted/50 rounded-md">
                                        <p className="font-medium">{department.name}</p>
                                    </div>
                                </div>

                                <div className="space-y-2">
                                    <div className="text-sm font-medium text-muted-foreground">
                                        Identifier
                                    </div>
                                    <div className="p-3 bg-muted/50 rounded-md">
                                        <p className="font-medium">{department.identifier}</p>
                                    </div>
                                </div>

                                {/* Status */}
                                <div className="space-y-2">
                                    <div className="text-sm font-medium text-muted-foreground">
                                        Status
                                    </div>
                                    <div className="p-3 bg-muted/50 rounded-md flex items-center gap-2">
                                        <span className={`font-medium`}>
                                            {department.isActive ? (
                                                <div className="flex items-center gap-2 text-emerald-600 dark:text-emerald-400 animate-pulse">
                                                    <CheckCircle className="h-4 w-4" />
                                                    <span className="font-medium">Active</span>
                                                </div>
                                            ) : (
                                                <div className="flex items-center gap-2 text-red-400 animate-pulse">
                                                    <XCircle className="h-4 w-4" />
                                                    <span className="font-medium">Inactive</span>
                                                </div>
                                            )}
                                        </span>
                                    </div>
                                </div>

                                <div className="space-y-2">
                                    <div className="text-sm font-medium text-muted-foreground">
                                        Parent Department
                                    </div>
                                    <div className="p-3 bg-muted/50 rounded-md">
                                        <p className="font-medium">{department.parentId || "None"}</p>
                                    </div>
                                </div>

                                {/* Created At */}
                                <div className="space-y-2">
                                    <div className="text-sm font-medium text-muted-foreground flex items-center gap-2">
                                        <Calendar className="h-4 w-4" />
                                        Created At
                                    </div>
                                    <div className="p-3 bg-muted/50 rounded-md">
                                        <p className="font-medium">
                                            {formatDate(department.createdAt)}
                                        </p>
                                    </div>
                                </div>

                                {/* Updated At */}
                                <div className="space-y-2">
                                    <div className="text-sm font-medium text-muted-foreground flex items-center gap-2">
                                        <Clock className="h-4 w-4" />
                                        Updated At
                                    </div>
                                    <div className="p-3 bg-muted/50 rounded-md">
                                        <p className="font-medium">
                                            {formatDate(department.updatedAt)}
                                        </p>
                                    </div>
                                </div>
                            </div>
                        </CardContent>
                    </Card>

                    <Card>
                        <CardHeader>
                            <div className="flex justify-between items-center gap-2">
                                <CardTitle className="text-lg">
                                    Children Departments
                                </CardTitle>

                                <Badge
                                    variant="outline"
                                    className="font-normal"
                                >
                                    {childTotalItems} total
                                </Badge>
                            </div>
                        </CardHeader>

                        <CardContent className="space-y-4">
                            <div className="p-3 bg-muted/50 rounded-md">
                                {childDepartments.length > 0 ? (
                                    <div className="space-y-3">
                                        <div className="grid grid-cols-1 gap-3">
                                            {childDepartments.map((child) => (
                                                <div
                                                    key={child.id}
                                                    className="
                                                                p-3
                                                                bg-card
                                                                border
                                                                rounded-lg
                                                                hover:bg-accent/50
                                                                transition-colors
                                                                "
                                                >
                                                    <div className="font-medium text-sm truncate">
                                                        {child.name}
                                                    </div>

                                                    <div
                                                        className="
                                                        text-xs
                                                                    text-muted-foreground
                                                                    font-mono
                                                                    truncate
                                                                    mt-1
                                                                    opacity-70
                                                                "
                                                    >
                                                        {formatDate(child.createdAt)}
                                                    </div>
                                                </div>
                                            ))}
                                        </div>

                                        <div
                                            ref={childCursorRef}
                                        />

                                        {isChildFetchingNextPage && (
                                            <div className="text-center text-sm text-muted-foreground">
                                                Loading more...
                                            </div>
                                        )}
                                    </div>
                                ) : (
                                    <div className="text-center py-4 text-muted-foreground">
                                        <p>No children assigned</p>
                                    </div>
                                )}
                            </div>
                        </CardContent>
                    </Card>
                </div>

                {/* Right column - Info and Actions */}
                <div className="space-y-6">

                    {/* Locations Card */}
                    
                    
                    <Card>
                        <CardHeader>
                            <div className="flex justify-between items-center gap-2">
                                <CardTitle className="text-lg">Locations</CardTitle>
                                {isUpdateLocs ? (
                                    <div className="flex gap-2">
                                        <Button
                                            variant="outline"
                                            size="sm"
                                            onClick={handleCancel}
                                            disabled={isUpdatePending}
                                        >
                                            <X className="h-4 w-4 mr-2" />
                                            Cancel
                                        </Button>
                                        <Button
                                            variant="default"
                                            size="sm"
                                            onClick={handleSave}
                                            disabled={isUpdatePending}
                                        >
                                            {isUpdatePending ? "Saving..." : "Save"}
                                        </Button>
                                    </div>
                                ) : (
                                    <Button variant="default" size="sm" onClick={handleEditClick}>
                                        <Edit className="h-4 w-4 mr-2" />
                                        Edit
                                    </Button>
                                )}
                            </div>
                        </CardHeader>
                        <CardContent className="space-y-4">
                            {isUpdateLocs ? (
                                <>
                                    <LocationItemSelector
                                        selectedItemsIds={selectedLocIds}
                                        onLocationChange={handleLocationChange}
                                    />
                                </>
                            ) : (
                                <>
                                    <Separator />
                                </>
                            )}
                            <div className="space-y-4">
                                <div className="p-3 bg-muted/50 rounded-md">
                                    {department.locations && department.locations.length > 0 ? (
                                        <div className="space-y-2">
                                            <div className="flex items-center gap-2">
                                                <Badge variant="outline" className="font-normal">
                                                    {department.locations.length} location
                                                    {department.locations.length !== 1 ? "s" : ""}
                                                </Badge>
                                            </div>
                                            <div className="grid grid-cols-1 gap-3">
                                                {department.locations.map((location) => (
                                                    <div
                                                        key={location.id}
                                                        className="p-3 bg-card border rounded-lg hover:bg-accent/50 transition-colors"
                                                    >
                                                        <div className="font-medium text-sm truncate">
                                                            {location.name}
                                                        </div>
                                                        <div className="text-xs text-muted-foreground font-mono truncate mt-1 opacity-70">
                                                            {location.id}
                                                        </div>
                                                    </div>
                                                ))}
                                            </div>
                                        </div>
                                    ) : (
                                        <div className="text-center py-4 text-muted-foreground">
                                            <p>No locations assigned</p>
                                        </div>
                                    )}
                                </div>
                            </div>
                        </CardContent>
                    </Card>

                    {/* Positions Card */}
                    <Card>
                        <CardHeader>
                            <div className="flex justify-between items-center gap-2">
                                <CardTitle className="text-lg">Positions</CardTitle>
                            </div>
                        </CardHeader>
                        <CardContent className="space-y-4">
                            <div className="space-y-4">
                                <div className="p-3 bg-muted/50 rounded-md">
                                    {department.positions && department.positions.length > 0 ? (
                                        <div className="space-y-2">
                                            <div className="flex items-center gap-2">
                                                <Badge variant="outline" className="font-normal">
                                                    {department.positions.length} position
                                                    {department.positions.length !== 1 ? "s" : ""}
                                                </Badge>
                                            </div>
                                            <div className="grid grid-cols-1 gap-3">
                                                {department.positions.map((position) => (
                                                    <div
                                                        key={position.id}
                                                        className="p-3 bg-card border rounded-lg hover:bg-accent/50 transition-colors"
                                                    >
                                                        <div className="font-medium text-sm truncate">
                                                            {position.name}
                                                        </div>
                                                        <div className="text-xs text-muted-foreground font-mono truncate mt-1 opacity-70">
                                                            {formatDate(position.createdAt)}
                                                        </div>
                                                    </div>
                                                ))}
                                            </div>
                                        </div>
                                    ) : (
                                        <div className="text-center py-4 text-muted-foreground">
                                            <p>No positions assigned</p>
                                        </div>
                                    )}
                                </div>
                            </div>
                        </CardContent>
                    </Card>

                    {/* Actions Card */}
                    <Card>
                        <CardHeader>
                            <CardTitle className="text-lg">Actions</CardTitle>
                        </CardHeader>
                        <CardContent className="space-y-3">
                            <Button
                                variant="default"
                                className="w-full justify-start"
                                onClick={() => setUpdateOpen(true)}
                            >
                                <Edit className="h-4 w-4 mr-2" />
                                Edit Department
                            </Button>

                            {department.isActive ? (
                                <Button
                                    className="w-full justify-start bg-red-400 hover:bg-red-600 text-white transition-colors"
                                    onClick={handleDeleteClick}
                                    disabled={isDeletePending}
                                >
                                    <XCircle className="h-4 w-4 mr-2" />
                                    Deactivate
                                </Button>
                            ) : (
                                <Button
                                    className="w-full justify-start bg-green-600 hover:bg-green-700 text-white transition-colors"
                                    onClick={handleActivate}
                                    disabled={isActivatePending}
                                >
                                    <CheckCircle className="h-4 w-4 mr-2" />
                                    Activate
                                </Button>
                            )}
                        </CardContent>
                    </Card>
                </div>
            </div>

            <UpdateDepartmentDialog
                key={departmentId}
                department={department}
                open={updateOpen}
                onOpenChange={setUpdateOpen}
            />

            <DeleteConfirmationDialog
                open={deleteOpen}
                onOpenChange={setDeleteOpen}
                onConfirm={handleDelete}
                loading={loading}
                title={`Delete "${department.name}"?`}
                description="Are you sure you want to delete this department? This action cannot be undone."
            />
        </main>
    );
}
