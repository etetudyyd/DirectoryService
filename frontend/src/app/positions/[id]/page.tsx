"use client";

import { useParams, useRouter } from "next/navigation";
import { useState } from "react";
import {
  ArrowLeft,
  Calendar,
  Clock,
  Edit,
  FileText,
  CheckCircle,
  XCircle,
  AlertCircle,
  Copy,
  X,
} from "lucide-react";
import { Button } from "@/shared/components/ui/button";
import {
  Card,
  CardContent,
  CardDescription,
  CardHeader,
  CardTitle,
} from "@/shared/components/ui/card";
import { Badge } from "@/shared/components/ui/badge";
import { Separator } from "@/shared/components/ui/separator";
import {
  Alert,
  AlertDescription,
  AlertTitle,
} from "@/shared/components/ui/alert";
import { DetailsErrorPage } from "@/pages/details-error-page";
import { Avatar, AvatarFallback } from "@/shared/components/ui/avatar";
import { useGetPosition } from "@/features/positions/model/use-get-position";
import { DetailsLoadingSkeleton } from "@/widgets/details-loading-skeleton";
import { UpdatePositionDialog } from "@/features/positions/update-position-dialog";
import { toast } from "sonner";
import DepartmentsSelectItem from "@/features/departments/departments-select-filter";
import { setFilterPositionsDepartmentIds } from "@/features/positions/model/position-filters-store";
import { useUpdatePositionDepartments } from "@/features/positions/model/update-position-departments";
import { DeleteConfirmationDialog } from "@/features/delete-confirmation-dialog";
import { useDeletePosition } from "@/features/positions/model/use-delete-position";

// Main component
export default function PositionDetailsPage() {
  const params = useParams();
  const router = useRouter();
  const [updateOpen, setUpdateOpen] = useState(false);
  const [isUpdateDepts, setIsUpdateDepts] = useState(false);
  const [selectedDeptIds, setSelectedDeptIds] = useState<string[]>([]);

  const { deletePosition, isPending: isDeletePending } = useDeletePosition();
  const [deleteOpen, setDeleteOpen] = useState(false);
  const [loading, setLoading] = useState(false);

  const positionId = params!.id as string;

  const { position, isPending, error, isError, refetch } =
    useGetPosition(positionId);

  const {
    updatePositionDepartments,
    isError: isUpdateError,
    error: updateError,
    isPending: isUpdatePending,
  } = useUpdatePositionDepartments();

  const handleDelete = async () => {
    setLoading(true);
    try {
      await deletePosition(position!.id);
    } finally {
      setLoading(false);
      setDeleteOpen(false);
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

  if (!position) {
    return (
      <div className="container mx-auto py-6">
        <Alert>
          <AlertTitle>Position not found</AlertTitle>
          <AlertDescription>
            The position you are looking for does not exist.
            <Button
              variant="outline"
              className="mt-2"
              onClick={() => router.push("/positions")}
            >
              View all positions
            </Button>
          </AlertDescription>
        </Alert>
      </div>
    );
  }

  // Инициализируем selectedDeptIds при первом рендере и когда position загружен
  const currentDeptIds = position.departments?.map((dept) => dept.id) || [];

  // Инициализируем selectedDeptIds только один раз, когда position загружен и selectedDeptIds пустой
  if (selectedDeptIds.length === 0 && currentDeptIds.length > 0) {
    setSelectedDeptIds(currentDeptIds);
  }

  const handleEditClick = () => {
    // При начале редактирования устанавливаем текущие значения
    setSelectedDeptIds(currentDeptIds);
    setIsUpdateDepts(true);
  };

  const handleSave = async () => {
    try {
      await updatePositionDepartments({
        positionId: positionId,
        departmentsIds: selectedDeptIds, // Отправляем выбранные ID
      });

      setIsUpdateDepts(false);
      refetch(); // Обновляем данные позиции
    } catch (error) {
      // Ошибка обрабатывается в хуке
    }
  };

  const handleCancel = () => {
    // Сбрасываем к исходным значениям
    setSelectedDeptIds(currentDeptIds);
    setIsUpdateDepts(false);
  };

  // Обработчик изменений в селекторе
  const handleDepartmentChange = (departmentIds: string[]) => {
    setSelectedDeptIds(departmentIds);
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
    navigator.clipboard.writeText(position.id);
    toast.success("Copied");
  };

  return (
    <main className="container mx-auto py-6 space-y-6">
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
                {getInitials(position.name)}
              </AvatarFallback>
            </Avatar>

            <div>
              <div className="flex items-center gap-2">
                <h1 className="text-3xl font-bold tracking-tight">
                  {position.name}
                </h1>
                <Badge
                  className={`flex items-center gap-1 text-white
                                ${
                                  position.isActive
                                    ? "bg-emerald-500 hover:bg-emerald-600"
                                    : "bg-red-400 hover:bg-red-400"
                                }`}
                >
                  {position.isActive ? (
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
                  {position.id}
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

      {/* Alert if position is deleted */}
      {position.deletedAt && (
        <Alert variant="destructive">
          <AlertCircle className="h-4 w-4" />
          <AlertTitle>Position Deleted</AlertTitle>
          <AlertDescription>
            This position was deleted on {formatDate(position.deletedAt)}. It
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
          {/* Description Card */}
          <Card>
            <CardHeader>
              <CardTitle className="flex items-center gap-2">
                <FileText className="h-5 w-5" />
                Description
              </CardTitle>
            </CardHeader>
            <CardContent>
              {position.description ? (
                <div className="prose prose-sm max-w-none">
                  <p className="text-muted-foreground whitespace-pre-line">
                    {position.description}
                  </p>
                </div>
              ) : (
                <div className="text-center py-8 text-muted-foreground">
                  <FileText className="h-12 w-12 mx-auto mb-4 opacity-30" />
                  <p>No description provided</p>
                </div>
              )}
            </CardContent>
          </Card>

          {/* Details Card */}
          <Card>
            <CardHeader>
              <CardTitle>Position Details</CardTitle>
              <CardDescription>
                Basic information about this position
              </CardDescription>
            </CardHeader>
            <CardContent>
              <div className="grid grid-cols-1 md:grid-cols-2 gap-6">
                {/* Name */}
                <div className="space-y-2">
                  <div className="text-sm font-medium text-muted-foreground">
                    Position Name
                  </div>
                  <div className="p-3 bg-muted/50 rounded-md">
                    <p className="font-medium">{position.name}</p>
                  </div>
                </div>

                {/* Status */}
                <div className="space-y-2">
                  <div className="text-sm font-medium text-muted-foreground">
                    Status
                  </div>
                  <div className="p-3 bg-muted/50 rounded-md flex items-center gap-2">
                    <span className={`font-medium`}>
                      {position.isActive ? (
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

                {/* Created At */}
                <div className="space-y-2">
                  <div className="text-sm font-medium text-muted-foreground flex items-center gap-2">
                    <Calendar className="h-4 w-4" />
                    Created At
                  </div>
                  <div className="p-3 bg-muted/50 rounded-md">
                    <p className="font-medium">
                      {formatDate(position.createdAt)}
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
                      {formatDate(position.updatedAt)}
                    </p>
                  </div>
                </div>
              </div>
            </CardContent>
          </Card>
        </div>

        {/* Right column - Info and Actions */}
        <div className="space-y-6">
          {/* Departments Card */}
          <Card>
            <CardHeader>
              <div className="flex justify-between items-center gap-2">
                <CardTitle className="text-lg">Departments</CardTitle>
                {isUpdateDepts ? (
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
              {isUpdateDepts ? (
                <>
                  <DepartmentsSelectItem
                    selectedDepartmentIds={selectedDeptIds}
                    onDepartmentChange={handleDepartmentChange}
                  />
                </>
              ) : (
                <>
                  <Separator />
                </>
              )}
              <div className="space-y-4">
                <div className="p-3 bg-muted/50 rounded-md">
                  {position.departments && position.departments.length > 0 ? (
                    <div className="space-y-2">
                      <div className="flex items-center gap-2">
                        <Badge variant="outline" className="font-normal">
                          {position.departments.length} department
                          {position.departments.length !== 1 ? "s" : ""}
                        </Badge>
                      </div>
                      <div className="grid grid-cols-1 gap-3">
                        {position.departments.map((department) => (
                          <div
                            key={department.id}
                            className="p-3 bg-card border rounded-lg hover:bg-accent/50 transition-colors"
                          >
                            <div className="font-medium text-sm truncate">
                              {department.name}
                            </div>
                            <div className="text-xs text-muted-foreground font-mono truncate mt-1 opacity-70">
                              {department.id}
                            </div>
                          </div>
                        ))}
                      </div>
                    </div>
                  ) : (
                    <div className="text-center py-4 text-muted-foreground">
                      <p>No departments assigned</p>
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
                Edit Position
              </Button>

              {position.isActive ? (
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
                  onClick={() => {}}
                >
                  <CheckCircle className="h-4 w-4 mr-2" />
                  Activate
                </Button>
              )}
            </CardContent>
          </Card>
        </div>
      </div>

      <UpdatePositionDialog
        key={positionId}
        position={position}
        open={updateOpen}
        onOpenChange={setUpdateOpen}
      />

      <DeleteConfirmationDialog
        open={deleteOpen}
        onOpenChange={setDeleteOpen}
        onConfirm={handleDelete}
        loading={loading}
        title={`Delete "${position.name}"?`}
        description="Are you sure you want to delete this position? This action cannot be undone."
      />
    </main>
  );
}
