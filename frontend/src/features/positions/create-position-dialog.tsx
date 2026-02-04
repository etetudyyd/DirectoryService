import { zodResolver } from "@hookform/resolvers/zod";
import { useForm } from "react-hook-form";
import { z } from "zod";
import { useCreatePosition } from "./model/use-create-position";
import {
  Dialog,
  DialogContent,
  DialogDescription,
  DialogFooter,
  DialogHeader,
  DialogTitle,
} from "@/shared/components/ui/dialog";
import { AlertCircleIcon, BuildingIcon, PlusIcon } from "lucide-react";
import { Label } from "@/shared/components/ui/label";
import { Input } from "@/shared/components/ui/input";
import { Button } from "@/shared/components/ui/button";
import { DepartmentsMultiSelectItem } from "@/widgets/departments/multi-select/departments-multi-select-item";

type Props = {
  open: boolean;
  onOpenChange: (open: boolean) => void;
};

const createPositionSchema = z.object({
  name: z
    .string()
    .nonempty("Name is cannot be empty")
    .min(3, "Name is has to be at least 3 characters long")
    .max(100, "Name must be at most 100 characters"),
  description: z
    .string()
    .max(1000, "Description must be at most 1000 characters"),
  departmentsIds: z.array(
    z.string().nonempty("Department has to be not empty"),
  ),
});

type CreatePositionData = z.infer<typeof createPositionSchema>;

export function CreatePositionDialog({ open, onOpenChange }: Props) {
  const initalData: CreatePositionData = {
    name: "",
    description: "",
    departmentsIds: [],
  };

  const {
    register,
    handleSubmit,
    reset,
    setValue,
    watch,
    formState: { errors, isValid },
  } = useForm<CreatePositionData>({
    defaultValues: initalData,
    resolver: zodResolver(createPositionSchema),
  });

  const { createPosition, isPending, error, isError } = useCreatePosition();
  
  const departmentIds = watch("departmentsIds") || [];

  const onSubmit = async (data: CreatePositionData) => {
    createPosition(
      {
        ...data,
        departmentsIds: departmentIds,
      },
      {
        onSuccess: () => {
          reset(initalData);
          onOpenChange(false);
        },
      },
    );
  };

  return (
    <Dialog open={open} onOpenChange={onOpenChange}>
      <DialogContent className="max-w-2xl max-h-[90vh] overflow-y-auto p-0 bg-linear-to-b from-gray-900 to-black border-gray-800">
        {/* Header */}
        <DialogHeader className="p-6 pb-4 border-b border-gray-800 bg-gray-900/50">
          <div className="flex items-center gap-3">
            <div className="h-10 w-10 rounded-lg bg-linear-to-br from-green-600 to-emerald-800 flex items-center justify-center">
              <PlusIcon className="h-5 w-5 text-white" />
            </div>
            <div>
              <DialogTitle className="text-xl font-bold text-white">
                Create New Position
              </DialogTitle>
              <DialogDescription className="text-gray-400">
                Add a new position to the system
              </DialogDescription>
            </div>
          </div>
        </DialogHeader>

        <form
          id="createPosition"
          className="p-6 space-y-6"
          onSubmit={handleSubmit(onSubmit)}
        >
          {/* Error message */}
          {isError && (
            <div className="p-4 bg-red-900/20 border border-red-800 rounded-lg">
              <div className="flex items-center gap-3">
                <AlertCircleIcon className="h-5 w-5 text-red-400" />
                <div>
                  <p className="text-red-300 font-medium">Creation failed</p>
                  <p className="text-red-400 text-sm mt-1">
                    {error?.message || "An error occurred. Please try again."}
                  </p>
                </div>
              </div>
            </div>
          )}

          {/* Basic Information */}
          <div className="space-y-4">
            <div className="flex items-center gap-2">
              <BuildingIcon className="h-5 w-5 text-blue-400" />
              <h3 className="text-lg font-semibold text-white">
                Basic Information
              </h3>
            </div>

            <div className="grid gap-4">
              <div className="space-y-2">
                <Label htmlFor="name" className="text-gray-300">
                  Position Name *
                </Label>
                <Input
                  id="name"
                  type="text"
                  placeholder="Enter position name (e.g., Developer, HR)"
                  className={`bg-gray-900 border-gray-700 text-white placeholder-gray-500 focus:border-blue-500 focus:ring-blue-500/20 ${
                    errors.name
                      ? "border-red-500 focus:border-red-500 focus:ring-red-500/20"
                      : ""
                  }`}
                  {...register("name")}
                />
                {errors.name && (
                  <p className="text-red-400 text-sm flex items-center gap-1">
                    <AlertCircleIcon className="h-4 w-4" />
                    {errors.name.message}
                  </p>
                )}
              </div>

              <div className="space-y-2">
                <Label
                  htmlFor="description"
                  className="text-gray-300 flex items-center gap-2"
                >
                  Description
                </Label>
                <textarea
                  id="description"
                  placeholder="Enter position description (optional)"
                  className={`min-h-25 w-full px-3 py-2 bg-gray-900 border border-gray-700 text-white placeholder-gray-500 focus:border-blue-500 focus:ring-blue-500/20 rounded-md ${
                    errors.description
                      ? "border-red-500 focus:border-red-500 focus:ring-red-500/20"
                      : ""
                  }`}
                  {...register("description")}
                />
                {errors.description && (
                  <p className="text-red-400 text-sm flex items-center gap-1">
                    <AlertCircleIcon className="h-4 w-4" />
                    {errors.description.message}
                  </p>
                )}
                <p className="text-gray-500 text-sm">
                  Optional description of the position's responsibilities and
                  requirements
                </p>
              </div>
            </div>
          </div>

          {/* Departments */}
          {/* <DepartmentsMultiSelectItem
            departmentIds={departmentIds}
            onDepartmentChange={(ids) => {
              setValue("departmentsIds", ids);
            }}
            errors={{ departmentsIds: errors.departmentsIds }}
            placeholder="Choose departments..."
            className="my-4"
          /> */}

          {/* Footer */}
          <DialogFooter className="pt-6 border-t border-gray-800">
            <Button
              type="button"
              variant="outline"
              onClick={() => onOpenChange(false)}
              className="border-gray-700 text-gray-300 hover:bg-gray-800 hover:text-white"
            >
              Cancel
            </Button>
            <Button
              type="submit"
              disabled={isPending || !isValid}
              className="bg-linear-to-r from-green-600 to-emerald-700 hover:from-green-700 hover:to-emerald-800 text-white shadow-lg hover:shadow-xl transition-all duration-200 disabled:opacity-50 disabled:cursor-not-allowed"
            >
              {isPending ? (
                <>
                  <div className="h-4 w-4 border-2 border-white/30 border-t-white rounded-full animate-spin mr-2" />
                  Creating...
                </>
              ) : (
                <>
                  <PlusIcon className="h-4 w-4 mr-2" />
                  Create Position
                </>
              )}
            </Button>
          </DialogFooter>
        </form>
      </DialogContent>
    </Dialog>
  );
}