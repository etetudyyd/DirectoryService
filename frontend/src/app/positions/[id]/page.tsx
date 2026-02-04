"use client";

import { useParams, useRouter } from "next/navigation";
import {
  ArrowLeft,
  Calendar,
  Clock,
  Edit,
  FileText,
  MoreVertical,
  Trash2,
  CheckCircle,
  XCircle,
  AlertCircle,
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
import { Skeleton } from "@/shared/components/ui/skeleton";
import {
  DropdownMenu,
  DropdownMenuContent,
  DropdownMenuItem,
  DropdownMenuLabel,
  DropdownMenuSeparator,
  DropdownMenuTrigger,
} from "@/shared/components/ui/dropdown-menu";
import {
  Alert,
  AlertDescription,
  AlertTitle,
} from "@/shared/components/ui/alert";
import { Avatar, AvatarFallback } from "@/shared/components/ui/avatar";
import { useGetPosition } from "@/features/positions/model/use-get-position";

function PositionLoadingSkeleton() {
  return (
    <div className="container mx-auto py-6 space-y-6">
      <div className="flex items-center justify-between">
        <div className="flex items-center gap-4">
          <Skeleton className="h-10 w-10 rounded-full" />
          <div>
            <Skeleton className="h-7 w-48 mb-2" />
            <Skeleton className="h-4 w-32" />
          </div>
        </div>
        <Skeleton className="h-10 w-24" />
      </div>

      <Card>
        <CardHeader>
          <Skeleton className="h-6 w-64 mb-2" />
          <Skeleton className="h-4 w-full" />
        </CardHeader>
        <CardContent className="space-y-4">
          <div className="grid grid-cols-1 md:grid-cols-2 gap-6">
            {Array.from({ length: 4 }).map((_, i) => (
              <div key={i} className="space-y-2">
                <Skeleton className="h-4 w-24" />
                <Skeleton className="h-6 w-full" />
              </div>
            ))}
          </div>
        </CardContent>
      </Card>
    </div>
  );
}

// Error state
function PositionError({
  error,
  onRetry,
}: {
  error: Error;
  onRetry: () => void;
}) {
  return (
    <div className="container mx-auto py-6">
      <Alert variant="destructive">
        <AlertCircle className="h-4 w-4" />
        <AlertTitle>Error loading position</AlertTitle>
        <AlertDescription className="space-y-4">
          <p>{error.message}</p>
          <div className="flex gap-2">
            <Button variant="outline" size="sm" onClick={onRetry}>
              Try Again
            </Button>
            <Button
              variant="ghost"
              size="sm"
              onClick={() => window.history.back()}
            >
              Go Back
            </Button>
          </div>
        </AlertDescription>
      </Alert>
    </div>
  );
}

// Main component
export default function PositionDetailPage() {
  const params = useParams();
  const router = useRouter();
  const positionId = params.id as string;

  const { position, isPending, error, isError, refetch } =
    useGetPosition(positionId);

  if (isPending) {
    return <PositionLoadingSkeleton />;
  }

  if (isError && error) {
    return <PositionError error={error} onRetry={refetch} />;
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

  const formatDateOnly = (date: Date) => {
    try {
      return new Intl.DateTimeFormat("en-US", {
        year: "numeric",
        month: "long",
        day: "numeric",
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

  // Handle delete
  const handleDelete = () => {
    if (
      confirm(`Are you sure you want to delete position "${position.name}"?`)
    ) {
      // deletePosition(position.id);
      router.push("/positions");
    }
  };

  // Handle edit
  const handleEdit = () => {
    router.push(`/positions/${position.id}/edit`);
  };

  return (
    <div className="container mx-auto py-6 space-y-6">
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
                                    : "bg-amber-500 hover:bg-amber-600"
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
              <p className="text-sm text-muted-foreground mt-1">
                ID: {position.id}
              </p>
            </div>
          </div>
        </div>

        <div className="flex items-center gap-2">
          <Button variant="default" onClick={handleEdit}>
            <Edit className="h-4 w-4 mr-2" />
            Edit
          </Button>

          <DropdownMenu>
            <DropdownMenuTrigger asChild>
              <Button variant="ghost" size="icon">
                <MoreVertical className="h-5 w-5" />
              </Button>
            </DropdownMenuTrigger>
            <DropdownMenuContent align="end">
              <DropdownMenuLabel>Actions</DropdownMenuLabel>
              <DropdownMenuItem
                onClick={() => navigator.clipboard.writeText(position.id)}
              >
                Copy Position ID
              </DropdownMenuItem>
              <DropdownMenuItem
                onClick={() => navigator.clipboard.writeText(position.name)}
              >
                Copy Position Name
              </DropdownMenuItem>
              <DropdownMenuSeparator />
              <DropdownMenuItem
                className="text-destructive focus:text-destructive"
                onClick={handleDelete}
              >
                <Trash2 className="h-4 w-4 mr-2" />
                Delete Position
              </DropdownMenuItem>
            </DropdownMenuContent>
          </DropdownMenu>
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

      {/* Main content */}
      <div className="grid grid-cols-1 lg:grid-cols-3 gap-6">
        {/* Left column - Description and Details */}
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
                      {position.isActive ? "Active" : "Inactive"}
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

                {/* Departments */}
                <div className="md:col-span-2 space-y-2">
                  <div className="text-sm font-medium text-muted-foreground">
                    Department IDs
                  </div>
                  <div className="p-3 bg-muted/50 rounded-md">
                    {position.departmentsIds &&
                    position.departmentsIds.length > 0 ? (
                      <div className="space-y-2">
                        <div className="flex items-center gap-2">
                          <Badge variant="outline" className="font-normal">
                            {position.departmentsIds.length} department
                            {position.departmentsIds.length !== 1 ? "s" : ""}
                          </Badge>
                        </div>
                        {/* <div className="flex flex-wrap gap-2">
                          {position.departmentsIds.map((deptId) => (
                            <div 
                              key={deptId} 
                              className="px-2 py-1 bg-background border rounded text-xs font-mono"
                            >
                              {deptId}
                            </div>
                          ))}
                        </div> */}
                      </div>
                    ) : (
                      <div className="text-center py-4 text-muted-foreground">
                        <p>No departments assigned</p>
                      </div>
                    )}
                  </div>
                </div>
              </div>
            </CardContent>
          </Card>
        </div>

        {/* Right column - Info and Actions */}
        <div className="space-y-6">
          {/* Info Card */}
          <Card>
            <CardHeader>
              <CardTitle className="text-lg">Information</CardTitle>
            </CardHeader>
            <CardContent className="space-y-4">
              <div className="space-y-2">
                <p className="text-sm text-muted-foreground">Position ID</p>
                <div className="p-2 bg-muted rounded text-xs font-mono break-all">
                  {position.id}
                </div>
              </div>

              <Separator />

              <div className="space-y-2">
                <p className="text-sm text-muted-foreground">Active Status</p>
                <div className="flex items-center gap-2">
                  {position.isActive ? (
                    <div className="flex items-center gap-2 text-emerald-600 dark:text-emerald-400 animate-pulse">
                      <CheckCircle className="h-4 w-4" />
                      <span className="font-medium">Active</span>
                    </div>
                  ) : (
                    <div className="flex items-center gap-2 text-amber-600 dark:text-amber-400 animate-pulse">
                      <XCircle className="h-4 w-4" />
                      <span className="font-medium">Inactive</span>
                    </div>
                  )}
                </div>
              </div>

              <Separator />

              <div className="space-y-2">
                <p className="text-sm text-muted-foreground">Created</p>
                <div className="flex items-center gap-2 text-sm">
                  <Calendar className="h-4 w-4 text-muted-foreground" />
                  <span>{formatDate(new Date(position.createdAt))}</span>
                </div>
              </div>

              <div className="space-y-2">
                <p className="text-sm text-muted-foreground">Last Updated</p>
                <div className="flex items-center gap-2 text-sm">
                  <Clock className="h-4 w-4 text-muted-foreground" />
                  <span>{formatDate(new Date(position.updatedAt))}</span>
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
                onClick={handleEdit}
              >
                <Edit className="h-4 w-4 mr-2" />
                Edit Position
              </Button>

              <Button
                variant="outline"
                className="w-full justify-start"
                onClick={() => {
                  // Toggle active status
                  // togglePositionStatus(position.id, !position.isActive);
                }}
              >
                {position.isActive ? (
                  <>
                    <XCircle className="h-4 w-4 mr-2" />
                    Deactivate
                  </>
                ) : (
                  <>
                    <CheckCircle className="h-4 w-4 mr-2" />
                    Activate
                  </>
                )}
              </Button>

              <Separator />

              <Button
                variant="destructive"
                className="w-full justify-start"
                onClick={handleDelete}
              >
                <Trash2 className="h-4 w-4 mr-2" />
                Delete Position
              </Button>
            </CardContent>
          </Card>
        </div>
      </div>
    </div>
  );
}
